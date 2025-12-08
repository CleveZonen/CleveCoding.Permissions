using System.Security.Cryptography;
using System.Text;

namespace CleveCoding.Kernel.Cryptography;

public static class Encryption
{
	private static string _key = null!;

	public static void SetKey(string key)
	{
		_key = key;
	}

	public static byte[] Encrypt(byte[]? data)
	{
		ArgumentNullException.ThrowIfNull(_key, nameof(_key));
		ArgumentNullException.ThrowIfNull(data, nameof(data));

		using var aesAlg = Aes.Create();
		aesAlg.Mode = CipherMode.ECB;
		aesAlg.Padding = PaddingMode.PKCS7;
		aesAlg.Key = Encoding.UTF8.GetBytes(_key);

		using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
		using var memoryStream = new MemoryStream();
		using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

		cryptoStream.Write(data, 0, data.Length);
		cryptoStream.FlushFinalBlock();

		return memoryStream.ToArray();
	}

	public static string? Encrypt(string? plaintext)
	{
		ArgumentNullException.ThrowIfNull(_key, nameof(_key));

		if (string.IsNullOrWhiteSpace(plaintext)) return null;

		byte[] encryptedBytes;
		using (var aesAlg = Aes.Create())
		{
			aesAlg.Key = Encoding.UTF8.GetBytes(_key);
			aesAlg.Mode = CipherMode.ECB;
			aesAlg.Padding = PaddingMode.PKCS7;

			using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

			using var memoryStream = new MemoryStream();
			using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
			using (var streamWriter = new StreamWriter(cryptoStream))
			{
				streamWriter.Write(plaintext);
			}

			encryptedBytes = memoryStream.ToArray();
		}

		return Convert.ToBase64String(encryptedBytes);
	}

	public static byte[] Decrypt(byte[]? cipherData)
	{
		ArgumentNullException.ThrowIfNull(_key, nameof(_key));
		ArgumentNullException.ThrowIfNull(cipherData, nameof(cipherData));

		using var aes = Aes.Create();
		aes.Key = Encoding.UTF8.GetBytes(_key);
		aes.Mode = CipherMode.ECB;
		aes.Padding = PaddingMode.PKCS7;

		using var descriptor = aes.CreateDecryptor(aes.Key, aes.IV);

		using var memoryStream = new MemoryStream(cipherData);
		using var cryptoStream = new CryptoStream(memoryStream, descriptor, CryptoStreamMode.Read);
		using var plainReader = new MemoryStream();
		cryptoStream.CopyTo(plainReader);

		return plainReader.ToArray();
	}

	public static string? Decrypt(string? cipherText)
	{
		ArgumentNullException.ThrowIfNull(_key, nameof(_key));

		if (string.IsNullOrWhiteSpace(cipherText)) return null;

		using var aes = Aes.Create();
		aes.Key = Encoding.UTF8.GetBytes(_key);
		aes.Mode = CipherMode.ECB;
		aes.Padding = PaddingMode.PKCS7;

		using var descriptor = aes.CreateDecryptor(aes.Key, aes.IV);

		var buffer = Convert.FromBase64String(cipherText);
		using var memoryStream = new MemoryStream(buffer);
		using var cryptoStream = new CryptoStream(memoryStream, descriptor, CryptoStreamMode.Read);
		using var streamReader = new StreamReader(cryptoStream);

		return streamReader.ReadToEnd();
	}
}