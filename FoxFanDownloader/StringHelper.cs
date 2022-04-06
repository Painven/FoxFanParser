using System.Security.Cryptography;
using System.Text;

namespace FoxFanDownloader;

public static class StringHelper
{
	public static string ComputeMd5Hash(this string str)
	{
		using (MD5 md5 = MD5.Create())
		{
			byte[] input = Encoding.ASCII.GetBytes(str);
			byte[] hash = md5.ComputeHash(input);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}
	}
}
