namespace InsanityBot.Core.Formatters.Embeds;
using System;

using DSharpPlus.Entities;

public class AuthorObjectBuilder
{
	private String Name { get; set; } = null;
	private String Icon { get; set; } = null;
	private String Url { get; set; } = null;

	public AuthorObjectBuilder WithName(String name)
	{
		this.Name = name;
		return this;
	}

	public AuthorObjectBuilder WithIcon(Uri iconUri)
	{
		this.Icon = iconUri.ToString();
		return this;
	}

	public AuthorObjectBuilder WithUrl(Uri url)
	{
		this.Url = url.ToString();
		return this;
	}

	public DiscordEmbedBuilder Build(DiscordEmbedBuilder builder)
	{
		builder.WithAuthor(this.Name, this.Url, this.Icon);
		return builder;
	}
}