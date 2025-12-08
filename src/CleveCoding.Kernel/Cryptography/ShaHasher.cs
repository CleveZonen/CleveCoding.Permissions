using System.Security.Cryptography;
using System.Text;

namespace CleveCoding.Kernel.Cryptography;

public static class ShaHasher
{
	/// <summary>
	/// Compute the checksum of the data.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public static string ComputeSha256(byte[] data)
	{
		ArgumentNullException.ThrowIfNull(data);

		var bytes = SHA256.HashData(data);
		var builder = new StringBuilder();
		for (var i = 0; i < bytes.Length; i++)
		{
			builder.Append(bytes[i].ToString("x2"));
		}

		return builder.ToString();
	}

	/// <summary>
	/// Compute the checksum of the data.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public static string ComputeSha256(string data)
		=> ComputeSha256(Encoding.UTF8.GetBytes(data));

	/// <summary>
	/// Compare left and right if they contain the same data.
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	/// <returns></returns>
	public static bool CompareSha256(byte[] left, byte[] right)
		=> ComputeSha256(left) == ComputeSha256(right);
}
