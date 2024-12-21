using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class ElinEncoder
{
	private const string key = "123456789012345678901234";

	private const string iv = "12345678";

	public static string AesEncrypt(string srcStr)
	{
		TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
		byte[] bytes = Encoding.UTF8.GetBytes(srcStr);
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateEncryptor(Encoding.UTF8.GetBytes("123456789012345678901234"), Encoding.UTF8.GetBytes("12345678")), CryptoStreamMode.Write);
		cryptoStream.Write(bytes, 0, bytes.Length);
		cryptoStream.Close();
		byte[] inArray = memoryStream.ToArray();
		memoryStream.Close();
		return Convert.ToBase64String(inArray);
	}

	public static string AesDecrypt(string encStr)
	{
		TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
		byte[] array = Convert.FromBase64String(encStr);
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateDecryptor(Encoding.UTF8.GetBytes("123456789012345678901234"), Encoding.UTF8.GetBytes("12345678")), CryptoStreamMode.Write);
		cryptoStream.Write(array, 0, array.Length);
		cryptoStream.Close();
		byte[] bytes = memoryStream.ToArray();
		memoryStream.Close();
		return Encoding.UTF8.GetString(bytes);
	}

	public static string GetCode(int idBacker, int index)
	{
		return AesEncrypt(idBacker.ToString("0000") + index).TrimEnd('=');
	}

	public static string GetID(string code)
	{
		return AesDecrypt(code + "=");
	}

	public static bool IsValid(string code)
	{
		try
		{
			int result;
			return int.TryParse(AesDecrypt(code + "="), out result);
		}
		catch
		{
		}
		return false;
	}
}
