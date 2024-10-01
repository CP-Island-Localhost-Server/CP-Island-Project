using System.Security.Cryptography;
using System.Text;

namespace Sfs2X.Util
{
	public class PasswordUtil
	{
		public static string MD5Password(string pass)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			byte[] array = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(pass));
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
