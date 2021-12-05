namespace InsanityBot.Utility;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class SHA512HashMap<TValue> : Dictionary<String, TValue>
{
	protected TValue NullValue { get; set; } = default;

	public new TValue this[String key]
	{
		get
		{
			if(!this.ContainsKey(key))
			{
				return this.NullValue;
			}

			return base[key];
		}
		set
		{
			if(this.ContainsKey(key))
			{
				base[key] = value;
			}
			else
			{
				this.Add(key, value);
			}
		}
	}

	public void Add(TValue value)
	{
		SHA512 sha512Provider = SHA512.Create();
		this.Add(Encoding.UTF8.GetString(sha512Provider.ComputeHash(Encoding.UTF8.GetBytes(value.ToString()))), value);
	}
}