using DSharpPlus.Entities;

using InsanityBot.Core.Formatters.Abstractions;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace InsanityBot.Core.Formatters.Embeds
{
	public class EmbedFormatter : IFormatter<DiscordEmbed>
	{
		// Intended. This is supposed to throw NotImplementedException, we never actually use it.
		[Obsolete("This is not in use in any way or shape.")]
		public String Format(DiscordEmbed value) => throw new NotImplementedException();

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public DiscordEmbed Read(String value)
		{
			if(value.StartsWith('{')) // if our string starts with a curly bracket, we can safely assume it is Json.
			{
				return JsonConvert.DeserializeObject<DiscordEmbed>(value);
			}

			String[] components = value.Split(" | ", StringSplitOptions.TrimEntries);
			DiscordEmbedBuilder embedBuilder = new();
			Boolean activeObject = false, activeArray = false; // we know we do not have any complex data structures, don't worry
			String activeObjectName = null;

			AuthorObjectBuilder authorBuilder = null;
			FieldObjectBuilder fieldBuilder = null;
			FooterObjectBuilder footerBuilder = null;

			foreach(var v in components)
			{
				#region Validity checks
				if (!v.Contains(':') && !activeArray)
				{
					throw new FormatException($"Invalid embed format string: segment {v} does not contain a valid identifier:value pair.");
				}

				//update array and object states
				if(v.Contains('{'))
				{
					if(activeObject)
					{
						throw new FormatException($"Invalid embed format string: segment {v} attempts to open a " +
							$"new object although there is already an opened object, multi-layers are not supported.");
					}
					else
					{
						activeObject = true;
						activeObjectName = v.Split(':')[0];

						if(activeObjectName != "author" && activeObjectName != "fields" && activeObjectName != "footer")
						{
							throw new FormatException($"Invalid embed format string: {activeObjectName} is not a valid object name.");
						}
					}
				}
				
				if(v.Contains('}'))
				{
					if(activeObject)
					{
						activeObject = false;
					}
					else
					{
						throw new FormatException($"Invalid embed format string: segment {v} attempts to close an " +
							$"object although none are opened.");
					}
				}

				if(v.Contains('['))
				{
					if(activeArray)
					{
						throw new FormatException($"Invalid embed format string: segment {v} attempts to open a " +
							$"new array although there is already an opened array, multi-layers are not supported.");
					}
					else
					{
						activeArray = true;
						if(v.Split(':')[0] != "fields")
						{
							throw new FormatException($"Invalid embed format string: segment {v} attempts to open a" +
								$"different array than \'fields\', no other arrays are supported.");
						}
					}
				}

				if(v.Contains(']'))
				{
					if(activeArray)
					{
						activeArray = false;
					}
					else
					{
						throw new FormatException($"Invalid embed format string: segment {v} attempts to close an " +
							$"array although none are opened.");
					}
				}

				String identifier = v.Split(':')[0];
				if (!AllowedIdentifiers.Contains(identifier))
				{
					throw new FormatException($"Invalid embed format string: segment {v} contains an invalid identifier.");
				}
				#endregion

				// parse simple top-level entries
				switch(identifier)
				{
					case "title":
						embedBuilder.WithTitle(String.Join(':', v.Split(':')[1..])); // ensure that any :s are perserved
						break;
					case "description":
						embedBuilder.WithDescription(String.Join(':', v.Split(':')[1..])); //ensure that any :s are preserved
						break;
					case "thumbnail":
						embedBuilder.WithThumbnail(v.Split(':')[1]);
						break;
					case "image":
						embedBuilder.WithImageUrl(v.Split(':')[1]);
						break;
					case "color":
						embedBuilder.WithColor(new(Int32.Parse(v.Split(':')[1].Replace("#", "0x"))));
						break;
				} // deliberately no 'default', we still have more values to parse through

				//parse complex top-level entries
				#region complex top-level
				String subObject = null;
				switch(identifier)
				{
					case "author":
						if(!activeObject)
						{
							throw new FormatException($"'author' object requires an object opening instruction, none was found.");
						}
						authorBuilder = new();
						subObject = v.Split('{')[1].Split(':')[0];

						switch(subObject)
						{
							case "name":
								authorBuilder.WithName(v.Split('{')[1].Split(':')[1]);
								break;
							case "icon":
								if(Uri.TryCreate(v.Split('{')[1].Split(':')[1], UriKind.Absolute, out Uri icon))
								{
									authorBuilder.WithIcon(icon);
								}
								else
								{
									throw new FormatException("Invalid argument passed as Uri.");
								}
								break;
							case "url":
								if (Uri.TryCreate(v.Split('{')[1].Split(':')[1], UriKind.Absolute, out Uri url))
								{
									authorBuilder.WithUrl(url);
								}
								else
								{
									throw new FormatException("Invalid argument passed as Uri.");
								}
								break;
							default:
								throw new FormatException($"Could not find field author.{v.Split('{')[1].Split(':')[0]}");
						}
						break;
					case "footer":
						if(!activeObject)
						{
							throw new FormatException($"'footer' object requires an object opening instruction, none was found.");
						}
						footerBuilder = new();
						subObject = v.Split('{')[1].Split(':')[0];

						switch(subObject)
						{
							case "icon":
								if (Uri.TryCreate(v.Split('{')[1].Split(':')[1], UriKind.Absolute, out Uri url))
								{
									footerBuilder.WithUrl(url);
								}
								else
								{
									throw new FormatException("Invalid argument passed as Uri.");
								}
								break;
							case "text":
								footerBuilder.WithText(v.Split('{')[1].Split(':')[1]);
								break;
							default:
								throw new FormatException($"Could not find field footer.{v.Split('{')[1].Split(':')[0]}");
						}
						break;
					case "fields":
						if(!(activeObject && activeArray))
						{
							throw new FormatException($"'fields' object-array pair requires an opening instruction, none was found.");
						}
						fieldBuilder = new();
						subObject = v.Split('{')[1].Split(':')[0];

						switch(subObject)
						{
							case "title":
								fieldBuilder.WithTitle(String.Join(':', v.Split('{')[1].Split(':')[1..]));
								break;
							case "value":
								fieldBuilder.WithValue(String.Join(':', v.Split('{')[1].Split(':')[1..]));
								break;
							case "inline":
								fieldBuilder.WithInline(Convert.ToBoolean(v.Split('{')[1].Split(':')[1]));
								break;
							default:
								throw new FormatException($"Could not find field fields.{v.Split('{')[1].Split(':')[0]}");
						}
						break;
				}
				#endregion

				#region second-level
				switch(identifier)
				{
					case "name":
						if(!activeObject || activeObjectName != "author")
						{
							throw new FormatException($"Field 'name' requires to be embedded in an 'author' object");
						}
						authorBuilder.WithName(String.Join(':', v.Split(':')[1..]));
						break;
					case "icon":
						if(!activeObject || activeObjectName != "author")
						{
							throw new FormatException($"Field 'name' requires to be embedded in an 'author' object");
						}

						if(Uri.TryCreate(v.Split(':')[1], UriKind.Absolute, out Uri uri))
						{
							authorBuilder.WithIcon(uri);
						}
						else
						{
							throw new FormatException($"Could not parse url from 'icon' field");
						}
						break;
					case "url":
						if(!activeObject || (activeObjectName != "author" && activeObjectName != "footer"))
                        {
							throw new FormatException($"Field 'url' requires to either be embedded in an 'author' or 'footer' object");
                        }

						if (Uri.TryCreate(v.Split(':')[1], UriKind.Absolute, out Uri url))
						{
							if (activeObjectName == "author")
							{
								authorBuilder.WithUrl(url);
							}
                            else
                            {
								footerBuilder.WithUrl(url);
                            }
						}
						else
						{
							throw new FormatException($"Could not parse url from 'url' field");
						}
						break;
					case "title":
						if(!activeObject || activeObjectName != "fields")
                        {
							throw new FormatException($"Field 'title' requires to be embedded in a 'fields' object");
                        }
						fieldBuilder.WithTitle(String.Join(':', v.Split(':')[1..]));
						break;
					case "value":
						if (!activeObject || activeObjectName != "fields")
						{
							throw new FormatException($"Field 'value' requires to be embedded in a 'fields' object");
						}
						fieldBuilder.WithValue(String.Join(':', v.Split(':')[1..]));
						break;
					case "inline":
						if (!activeObject || activeObjectName != "fields")
						{
							throw new FormatException($"Field 'inline' requires to be embedded in a 'fields' object");
						}
						fieldBuilder.WithInline(Convert.ToBoolean(v.Split(':')[1]));
						break;
					case "text":
						if (!activeObject || activeObjectName != "fields")
						{
							throw new FormatException($"Field 'text' requires to be embedded in a 'footer' object");
						}
						footerBuilder.WithText(String.Join(':', v.Split(':')[1..]));
						break;
				}
				#endregion

				//close everything as needed
				if(v.Contains('}'))
                {
					switch(activeObjectName)
                    {
						case "author":
							embedBuilder = authorBuilder.Build(embedBuilder);
							break;
						case "footer":
							embedBuilder = footerBuilder.Build(embedBuilder);
							break;
						case "fields":
							embedBuilder = fieldBuilder.Build(embedBuilder);
							fieldBuilder = new();
							break;
						default:
							throw new FormatException($"Attempting to close invalid object {activeObjectName}");
                    }
                }
			}

			return embedBuilder.Build();
		}

		private readonly List<String> AllowedIdentifiers = new()
		{
			"title",
			"description",
			"author",
			"name",
			"icon",
			"url",
			"thumbnail",
			"image",
			"fields",
			"title",
			"value",
			"inline",
			"footer",
			"text",
			"color"
		};
	}
}
