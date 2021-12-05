namespace InsanityBot.Utility;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class SHA512FileChecksumProvider
{
	public static String GetSHA512Checksum(this String filename)
	{
		Stream stream = new FileStream(filename, FileMode.Open);
		SHA512 provider = SHA512.Create();

		String checksum = Encoding.UTF8.GetString(provider.ComputeHash(stream));

		stream.Close();
		return checksum;
	}

	public static String GetSHA512Checksum(this String filename, Encoding encoding)
	{
		Stream stream = new FileStream(filename, FileMode.Open);
		SHA512 provider = SHA512.Create();

		String checksum = encoding.GetString(provider.ComputeHash(stream));

		stream.Close();
		return checksum;
	}

	public static String GetSHA512Checksum(this Stream stream)
	{
		SHA512 provider = SHA512.Create();
		return Encoding.UTF8.GetString(provider.ComputeHash(stream));
	}

	public static String GetSHA512Checksum(this Stream stream, Encoding encoding)
	{
		SHA512 provider = SHA512.Create();
		return encoding.GetString(provider.ComputeHash(stream));
	}
}