namespace InsanityBot.Utility.Exceptions;
using System;

public class DurationTooLongException : Exception
{
	public DurationTooLongException(String message) : base(message)
	{
	}
}