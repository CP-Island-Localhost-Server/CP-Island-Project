using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	internal abstract class HttpBase
	{
		private const int _headersMaxLength = 8192;

		protected const string CrLf = "\r\n";

		private NameValueCollection _headers;

		private Version _version;

		internal byte[] EntityBodyData;

		public string EntityBody
		{
			get
			{
				if (EntityBodyData == null || EntityBodyData.LongLength == 0)
				{
					return string.Empty;
				}
				Encoding encoding = null;
				string text = _headers["Content-Type"];
				if (text != null && text.Length > 0)
				{
					encoding = HttpUtility.GetEncoding(text);
				}
				return (encoding ?? Encoding.UTF8).GetString(EntityBodyData);
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return _headers;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return _version;
			}
		}

		protected HttpBase(Version version, NameValueCollection headers)
		{
			_version = version;
			_headers = headers;
		}

		private static byte[] readEntityBody(Stream stream, string length)
		{
			long result;
			if (!long.TryParse(length, out result))
			{
				throw new ArgumentException("Cannot be parsed.", "length");
			}
			if (result < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Less than zero.");
			}
			return (result > 1024) ? stream.ReadBytes(result, 1024) : ((result <= 0) ? null : stream.ReadBytes((int)result));
		}

		private static string[] readHeaders(Stream stream, int maxLength)
		{
			List<byte> buff = new List<byte>();
			int cnt = 0;
			Action<int> action = delegate(int i)
			{
				if (i == -1)
				{
					throw new EndOfStreamException("The header cannot be read from the data source.");
				}
				buff.Add((byte)i);
				cnt++;
			};
			bool flag = false;
			while (cnt < maxLength)
			{
				if (stream.ReadByte().EqualsWith('\r', action) && stream.ReadByte().EqualsWith('\n', action) && stream.ReadByte().EqualsWith('\r', action) && stream.ReadByte().EqualsWith('\n', action))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new WebSocketException("The length of header part is greater than the max length.");
			}
			return Encoding.UTF8.GetString(buff.ToArray()).Replace("\r\n ", " ").Replace("\r\n\t", " ")
				.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		}

		protected static T Read<T>(Stream stream, Func<string[], T> parser, int millisecondsTimeout) where T : HttpBase
		{
			bool timeout = false;
			Timer timer = new Timer(delegate
			{
				timeout = true;
				stream.Close();
			}, null, millisecondsTimeout, -1);
			T val = (T)null;
			Exception ex = null;
			try
			{
				val = parser(readHeaders(stream, 8192));
				string text = val.Headers["Content-Length"];
				if (text != null && text.Length > 0)
				{
					val.EntityBodyData = readEntityBody(stream, text);
				}
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			finally
			{
				timer.Change(-1, -1);
				timer.Dispose();
			}
			string text2 = (timeout ? "A timeout has occurred while reading an HTTP request/response." : ((ex == null) ? null : "An exception has occurred while reading an HTTP request/response."));
			if (text2 != null)
			{
				throw new WebSocketException(text2, ex);
			}
			return val;
		}

		public byte[] ToByteArray()
		{
			return Encoding.UTF8.GetBytes(ToString());
		}
	}
}
