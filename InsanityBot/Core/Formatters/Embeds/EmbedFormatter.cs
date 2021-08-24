using System;
using System.Runtime.CompilerServices;
using System.Text;

using DSharpPlus.Entities;

using InsanityBot.Core.Formatters.Abstractions;

using Newtonsoft.Json;

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
            if(String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Null or whitespace-only embed string submitted.");
            }

            if(value.StartsWith('{')) // if our string starts with a curly bracket, we can safely assume it is Json.
            {
                return JsonConvert.DeserializeObject<DiscordEmbed>(value);
            }

            value += "\u0003"; // append EOT

            Char currentCharacter, lastCharacter = '\u0000';
            StringBuilder builder = new();
            DiscordEmbedBuilder embedBuilder = new();
            Boolean activeObject = false, activeArray = false, skipContinue = false, endedOnObject = false;
            String rootObject = "", currentObject = "";
            FieldObjectBuilder fieldBuilder = new();
            FooterObjectBuilder footerBuilder = new();
            AuthorObjectBuilder authorBuilder = new();

            foreach(Char c in value)
            {
                currentCharacter = c;

                #region Character Checks

                if(currentCharacter == '\u0003') // EOT character, our string ends here
                {
                    if(!endedOnObject)
                    {
                        goto FINALIZE_OBJECT_CREATION;
                    }
                    break;
                }

                if(currentCharacter == ':' && lastCharacter != '\\') // colon means an object transition, but not finalization
                {
                    currentObject = builder.ToString();
                    builder.Clear();
                    lastCharacter = currentCharacter;
                    continue;
                }

                if(currentCharacter == '|' && lastCharacter == ' ') // end of object, finalize the object
                {
                    lastCharacter = currentCharacter;
                    goto FINALIZE_OBJECT_CREATION;
                }

                if(currentCharacter == '{' && (lastCharacter == ':' || lastCharacter == '[')) // opens sub-level object
                {
                    rootObject = currentObject;
                    activeObject = true;
                    lastCharacter = currentCharacter;
                    continue;
                }

                if(currentCharacter == '[' && lastCharacter == ':')
                {
                    activeArray = true;
                    lastCharacter = currentCharacter;
                    continue;
                }

                if(currentCharacter == '}' && lastCharacter != '\\')
                {
                    endedOnObject = true;
                    lastCharacter = currentCharacter;
                    goto CLOSE_ACTIVE_OBJECTS;
                }

                if(currentCharacter == ']' && lastCharacter == '}')
                {
                    endedOnObject = true;
                    continue;
                }

                // we can leave handling backslashes escaping :, | et al to discord, thankfully

                #endregion

                endedOnObject = false;
                builder.Append(currentCharacter);
                lastCharacter = currentCharacter;

                continue;

            FINALIZE_OBJECT_CREATION:

                #region Object Creation

                String objectName = currentObject.Trim();
                String objectValue = builder.ToString().Trim();
                builder.Clear();

                if(!activeArray && !activeObject) // top level object
                {
                    switch(objectName)
                    {
                        case "title":
                            embedBuilder.WithTitle(objectValue);
                            break;
                        case "description":
                            embedBuilder.WithDescription(objectValue);
                            break;
                        case "url":
                            embedBuilder.WithImageUrl(objectValue);
                            break;
                        case "thumbnail":
                            embedBuilder.WithThumbnail(objectValue);
                            break;
                        case "color":
                            embedBuilder.WithColor(new DiscordColor(objectValue));
                            break;
                        default:
                            throw new FormatException($"Couldn't find embed top-level property {objectName}");
                    }
                }
                else if(activeArray) // we can infer activeObject to be true too
                {
                    if(rootObject != "fields")
                    {
                        throw new FormatException("Only fields can be an array.");
                    }

                    switch(objectName)
                    {
                        case "title":
                            fieldBuilder.WithTitle(objectValue);
                            break;
                        case "value":
                            fieldBuilder.WithValue(objectValue);
                            break;
                        case "inline":
                            fieldBuilder.WithInline(Convert.ToBoolean(objectValue));
                            break;
                        default:
                            throw new FormatException($"{objectName} is not a valid field sub-identifier.");
                    }
                }
                else
                {
                    switch(rootObject)
                    {
                        case "author":
                            switch(objectName)
                            {
                                case "name":
                                    authorBuilder.WithName(objectValue);
                                    break;
                                case "icon":
                                    authorBuilder.WithIcon(new Uri(objectValue));
                                    break;
                                case "url":
                                    authorBuilder.WithUrl(new Uri(objectValue));
                                    break;
                                default:
                                    throw new FormatException($"author.{objectName} is not a valid field identifier.");
                            }
                            break;
                        case "footer":
                            switch(objectName)
                            {
                                case "text":
                                    footerBuilder.WithText(objectValue);
                                    break;
                                case "url":
                                    footerBuilder.WithUrl(new Uri(objectValue));
                                    break;
                                default:
                                    throw new FormatException($"footer.{objectName} is not a valid field identifier.");
                            }
                            break;
                        default:
                            throw new FormatException($"{rootObject} is not a valid root object.");
                    }
                }

                #endregion

                if(!skipContinue)
                {
                    continue;
                }

            CLOSE_ACTIVE_OBJECTS:

                if(activeObject)
                {
                    if(!skipContinue)
                    {
                        skipContinue = true;
                        goto FINALIZE_OBJECT_CREATION;
                    }

                    switch(rootObject)
                    {
                        case "author":
                            embedBuilder = authorBuilder.Build(embedBuilder);
                            break;
                        case "footer":
                            embedBuilder = footerBuilder.Build(embedBuilder);
                            break;
                        case "fields":
                            embedBuilder = fieldBuilder.Build(embedBuilder);
                            break;
                    }

                    skipContinue = false;
                    activeObject = false;
                }

                continue;
            }

            return embedBuilder.Build();
        }
    }
}
