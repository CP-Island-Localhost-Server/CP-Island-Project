using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace WebSocketSharp
{
	public static class Ext
	{
		private const string _tspecials = "()<>@,;:\\\"/[]?={} \t";

		private static byte[] compress(this byte[] data)
		{
			if (data.LongLength == 0)
			{
				return data;
			}
			using (MemoryStream stream = new MemoryStream(data))
			{
				return stream.compressToArray();
			}
		}

		private static MemoryStream compress(this Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			if (stream.Length == 0)
			{
				return memoryStream;
			}
			stream.Position = 0L;
			using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
			{
				stream.CopyTo(deflateStream);
				deflateStream.Close();
				memoryStream.Position = 0L;
				return memoryStream;
			}
		}

		private static byte[] compressToArray(this Stream stream)
		{
			using (MemoryStream memoryStream = stream.compress())
			{
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		private static byte[] decompress(this byte[] data)
		{
			if (data.LongLength == 0)
			{
				return data;
			}
			using (MemoryStream stream = new MemoryStream(data))
			{
				return stream.decompressToArray();
			}
		}

		private static MemoryStream decompress(this Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			if (stream.Length == 0)
			{
				return memoryStream;
			}
			stream.Position = 0L;
			using (DeflateStream source = new DeflateStream(stream, CompressionMode.Decompress, true))
			{
				source.CopyTo(memoryStream);
				memoryStream.Position = 0L;
				return memoryStream;
			}
		}

		private static byte[] decompressToArray(this Stream stream)
		{
			using (MemoryStream memoryStream = stream.decompress())
			{
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		private static byte[] readBytes(this Stream stream, byte[] buffer, int offset, int length)
		{
			int i = 0;
			try
			{
				i = stream.Read(buffer, offset, length);
				if (i < 1)
				{
					return buffer.SubArray(0, offset);
				}
				int num;
				for (; i < length; i += num)
				{
					num = stream.Read(buffer, offset + i, length - i);
					if (num < 1)
					{
						break;
					}
				}
			}
			catch
			{
			}
			return (i >= length) ? buffer : buffer.SubArray(0, offset + i);
		}

		private static bool readBytes(this Stream stream, byte[] buffer, int offset, int length, Stream destination)
		{
			byte[] array = stream.readBytes(buffer, offset, length);
			int num = array.Length;
			destination.Write(array, 0, num);
			return num == offset + length;
		}

		private static void times(this ulong n, Action action)
		{
			for (ulong num = 0uL; num < n; num++)
			{
				action();
			}
		}

		internal static byte[] Append(this ushort code, string reason)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] buffer = code.InternalToByteArray(ByteOrder.Big);
				memoryStream.Write(buffer, 0, 2);
				if (reason != null && reason.Length > 0)
				{
					buffer = Encoding.UTF8.GetBytes(reason);
					memoryStream.Write(buffer, 0, buffer.Length);
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static string CheckIfCanRead(this Stream stream)
		{
			return (stream == null) ? "'stream' is null." : (stream.CanRead ? null : "'stream' cannot be read.");
		}

		internal static string CheckIfClosable(this WebSocketState state)
		{
			object result;
			switch (state)
			{
			case WebSocketState.Closing:
				result = "While closing the WebSocket connection.";
				break;
			case WebSocketState.Closed:
				result = "The WebSocket connection has already been closed.";
				break;
			default:
				result = null;
				break;
			}
			return (string)result;
		}

		internal static string CheckIfConnectable(this WebSocketState state)
		{
			return (state != WebSocketState.Open && state != WebSocketState.Closing) ? null : "A WebSocket connection has already been established.";
		}

		internal static string CheckIfOpen(this WebSocketState state)
		{
			object result;
			switch (state)
			{
			case WebSocketState.Connecting:
				result = "A WebSocket connection isn't established.";
				break;
			case WebSocketState.Closing:
				result = "While closing the WebSocket connection.";
				break;
			case WebSocketState.Closed:
				result = "The WebSocket connection has already been closed.";
				break;
			default:
				result = null;
				break;
			}
			return (string)result;
		}

		internal static string CheckIfStart(this ServerState state)
		{
			object result;
			switch (state)
			{
			case ServerState.Ready:
				result = "The server hasn't yet started.";
				break;
			case ServerState.ShuttingDown:
				result = "The server is shutting down.";
				break;
			case ServerState.Stop:
				result = "The server has already stopped.";
				break;
			default:
				result = null;
				break;
			}
			return (string)result;
		}

		internal static string CheckIfStartable(this ServerState state)
		{
			object result;
			switch (state)
			{
			case ServerState.Start:
				result = "The server has already started.";
				break;
			case ServerState.ShuttingDown:
				result = "The server is shutting down.";
				break;
			default:
				result = null;
				break;
			}
			return (string)result;
		}

		internal static string CheckIfValidCloseParameters(this ushort code, string reason)
		{
			return (!code.IsCloseStatusCode()) ? "An invalid close status code." : ((code.IsNoStatusCode() && !reason.IsNullOrEmpty()) ? "NoStatusCode cannot have a reason." : ((reason.IsNullOrEmpty() || Encoding.UTF8.GetBytes(reason).Length <= 123) ? null : "A reason has greater than the allowable max size."));
		}

		internal static string CheckIfValidCloseParameters(this CloseStatusCode code, string reason)
		{
			return (code.IsNoStatusCode() && !reason.IsNullOrEmpty()) ? "NoStatusCode cannot have a reason." : ((reason.IsNullOrEmpty() || Encoding.UTF8.GetBytes(reason).Length <= 123) ? null : "A reason has greater than the allowable max size.");
		}

		internal static string CheckIfValidCloseStatusCode(this ushort code)
		{
			return code.IsCloseStatusCode() ? null : "An invalid close status code.";
		}

		internal static string CheckIfValidControlData(this byte[] data, string paramName)
		{
			return (data.Length <= 125) ? null : string.Format("'{0}' has greater than the allowable max size.", paramName);
		}

		internal static string CheckIfValidProtocols(this string[] protocols)
		{
			return protocols.Contains((string protocol) => protocol == null || protocol.Length == 0 || !protocol.IsToken()) ? "Contains an invalid value." : ((!protocols.ContainsTwice()) ? null : "Contains a value twice.");
		}

		internal static string CheckIfValidSendData(this byte[] data)
		{
			return (data != null) ? null : "'data' is null.";
		}

		internal static string CheckIfValidSendData(this FileInfo file)
		{
			return (file != null) ? null : "'file' is null.";
		}

		internal static string CheckIfValidSendData(this string data)
		{
			return (data != null) ? null : "'data' is null.";
		}

		internal static string CheckIfValidServicePath(this string path)
		{
			return (path == null || path.Length == 0) ? "'path' is null or empty." : ((path[0] != '/') ? "'path' isn't an absolute path." : ((path.IndexOfAny(new char[2] { '?', '#' }) <= -1) ? null : "'path' includes either or both query and fragment components."));
		}

		internal static string CheckIfValidSessionID(this string id)
		{
			return (id != null && id.Length != 0) ? null : "'id' is null or empty.";
		}

		internal static string CheckIfValidWaitTime(this TimeSpan time)
		{
			return (!(time <= TimeSpan.Zero)) ? null : "A wait time is zero or less.";
		}

		internal static void Close(this WebSocketSharp.Net.HttpListenerResponse response, WebSocketSharp.Net.HttpStatusCode code)
		{
			response.StatusCode = (int)code;
			response.OutputStream.Close();
		}

		internal static void CloseWithAuthChallenge(this WebSocketSharp.Net.HttpListenerResponse response, string challenge)
		{
			response.Headers.InternalSet("WWW-Authenticate", challenge, true);
			response.Close(WebSocketSharp.Net.HttpStatusCode.Unauthorized);
		}

		internal static byte[] Compress(this byte[] data, CompressionMethod method)
		{
			return (method != CompressionMethod.Deflate) ? data : data.compress();
		}

		internal static Stream Compress(this Stream stream, CompressionMethod method)
		{
			return (method != CompressionMethod.Deflate) ? stream : stream.compress();
		}

		internal static byte[] CompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method != CompressionMethod.Deflate) ? stream.ToByteArray() : stream.compressToArray();
		}

		internal static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> condition)
		{
			foreach (T item in source)
			{
				if (condition(item))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool ContainsTwice(this string[] values)
		{
			int len = values.Length;
			Func<int, bool> contains = null;
			contains = delegate(int idx)
			{
				if (idx < len - 1)
				{
					for (int i = idx + 1; i < len; i++)
					{
						if (values[i] == values[idx])
						{
							return true;
						}
					}
					return contains(++idx);
				}
				return false;
			};
			return contains(0);
		}

		internal static T[] Copy<T>(this T[] source, long length)
		{
			T[] array = new T[length];
			Array.Copy(source, 0L, array, 0L, length);
			return array;
		}

		internal static void CopyTo(this Stream source, Stream destination)
		{
			int num = 256;
			byte[] buffer = new byte[num];
			int num2 = 0;
			while ((num2 = source.Read(buffer, 0, num)) > 0)
			{
				destination.Write(buffer, 0, num2);
			}
		}

		internal static byte[] Decompress(this byte[] data, CompressionMethod method)
		{
			return (method != CompressionMethod.Deflate) ? data : data.decompress();
		}

		internal static Stream Decompress(this Stream stream, CompressionMethod method)
		{
			return (method != CompressionMethod.Deflate) ? stream : stream.decompress();
		}

		internal static byte[] DecompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method != CompressionMethod.Deflate) ? stream.ToByteArray() : stream.decompressToArray();
		}

		internal static bool EqualsWith(this int value, char c, Action<int> action)
		{
			action(value);
			return value == c;
		}

		internal static string GetAbsolutePath(this Uri uri)
		{
			if (uri.IsAbsoluteUri)
			{
				return uri.AbsolutePath;
			}
			string originalString = uri.OriginalString;
			if (originalString[0] != '/')
			{
				return null;
			}
			int num = originalString.IndexOfAny(new char[2] { '?', '#' });
			return (num <= 0) ? originalString : originalString.Substring(0, num);
		}

		internal static string GetMessage(this CloseStatusCode code)
		{
			object result;
			switch (code)
			{
			case CloseStatusCode.ProtocolError:
				result = "A WebSocket protocol error has occurred.";
				break;
			case CloseStatusCode.IncorrectData:
				result = "An incorrect data has been received.";
				break;
			case CloseStatusCode.Abnormal:
				result = "An exception has occurred.";
				break;
			case CloseStatusCode.InconsistentData:
				result = "An inconsistent data has been received.";
				break;
			case CloseStatusCode.PolicyViolation:
				result = "A policy violation has occurred.";
				break;
			case CloseStatusCode.TooBig:
				result = "A too big data has been received.";
				break;
			case CloseStatusCode.IgnoreExtension:
				result = "WebSocket client didn't receive expected extension(s).";
				break;
			case CloseStatusCode.ServerError:
				result = "WebSocket server got an internal error.";
				break;
			case CloseStatusCode.TlsHandshakeFailure:
				result = "An error has occurred while handshaking.";
				break;
			default:
				result = string.Empty;
				break;
			}
			return (string)result;
		}

		internal static string GetName(this string nameAndValue, char separator)
		{
			int num = nameAndValue.IndexOf(separator);
			return (num <= 0) ? null : nameAndValue.Substring(0, num).Trim();
		}

		internal static string GetValue(this string nameAndValue, char separator)
		{
			int num = nameAndValue.IndexOf(separator);
			return (num <= -1 || num >= nameAndValue.Length - 1) ? null : nameAndValue.Substring(num + 1).Trim();
		}

		internal static string GetValue(this string nameAndValue, char separator, bool unquote)
		{
			int num = nameAndValue.IndexOf(separator);
			if (num < 0 || num == nameAndValue.Length - 1)
			{
				return null;
			}
			string text = nameAndValue.Substring(num + 1).Trim();
			return (!unquote) ? text : text.Unquote();
		}

		internal static TcpListenerWebSocketContext GetWebSocketContext(this TcpClient tcpClient, string protocol, bool secure, ServerSslConfiguration sslConfig, Logger logger)
		{
			return new TcpListenerWebSocketContext(tcpClient, protocol, secure, sslConfig, logger);
		}

		internal static byte[] InternalToByteArray(this ushort value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!order.IsHostOrder())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		internal static byte[] InternalToByteArray(this ulong value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (!order.IsHostOrder())
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		internal static bool IsCompressionExtension(this string value, CompressionMethod method)
		{
			return value.StartsWith(method.ToExtensionString());
		}

		internal static bool IsNoStatusCode(this ushort code)
		{
			return code == 1005;
		}

		internal static bool IsNoStatusCode(this CloseStatusCode code)
		{
			return code == CloseStatusCode.NoStatusCode;
		}

		internal static bool IsPortNumber(this int value)
		{
			return value > 0 && value < 65536;
		}

		internal static bool IsReserved(this ushort code)
		{
			return code == 1004 || code == 1005 || code == 1006 || code == 1015;
		}

		internal static bool IsReserved(this CloseStatusCode code)
		{
			return code == CloseStatusCode.Undefined || code == CloseStatusCode.NoStatusCode || code == CloseStatusCode.Abnormal || code == CloseStatusCode.TlsHandshakeFailure;
		}

		internal static bool IsText(this string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				if (c < ' ' && !"\r\n\t".Contains(c))
				{
					return false;
				}
				switch (c)
				{
				case '\u007f':
					return false;
				case '\n':
					if (++i < length)
					{
						c = value[i];
						if (!" \t".Contains(c))
						{
							return false;
						}
					}
					break;
				}
			}
			return true;
		}

		internal static bool IsToken(this string value)
		{
			foreach (char c in value)
			{
				if (c < ' ' || c >= '\u007f' || "()<>@,;:\\\"/[]?={} \t".Contains(c))
				{
					return false;
				}
			}
			return true;
		}

		internal static string Quote(this string value)
		{
			return string.Format("\"{0}\"", value.Replace("\"", "\\\""));
		}

		internal static byte[] ReadBytes(this Stream stream, int length)
		{
			return stream.readBytes(new byte[length], 0, length);
		}

		internal static byte[] ReadBytes(this Stream stream, long length, int bufferLength)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				long num = length / bufferLength;
				int num2 = (int)(length % bufferLength);
				byte[] buffer = new byte[bufferLength];
				bool flag = false;
				for (long num3 = 0L; num3 < num; num3++)
				{
					if (!stream.readBytes(buffer, 0, bufferLength, memoryStream))
					{
						flag = true;
						break;
					}
				}
				if (!flag && num2 > 0)
				{
					stream.readBytes(new byte[num2], 0, num2, memoryStream);
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static void ReadBytesAsync(this Stream stream, int length, Action<byte[]> completed, Action<Exception> error)
		{
			byte[] buff = new byte[length];
			stream.BeginRead(buff, 0, length, delegate(IAsyncResult ar)
			{
				try
				{
					byte[] array = null;
					try
					{
						int num = stream.EndRead(ar);
						array = ((num < 1) ? new byte[0] : ((num >= length) ? buff : stream.readBytes(buff, num, length - num)));
					}
					catch
					{
						array = new byte[0];
					}
					if (completed != null)
					{
						completed(array);
					}
				}
				catch (Exception obj2)
				{
					if (error != null)
					{
						error(obj2);
					}
				}
			}, null);
		}

		internal static string RemovePrefix(this string value, params string[] prefixes)
		{
			int num = 0;
			foreach (string text in prefixes)
			{
				if (value.StartsWith(text))
				{
					num = text.Length;
					break;
				}
			}
			return (num <= 0) ? value : value.Substring(num);
		}

		internal static T[] Reverse<T>(this T[] array)
		{
			int num = array.Length;
			T[] array2 = new T[num];
			int num2 = num - 1;
			for (int i = 0; i <= num2; i++)
			{
				array2[i] = array[num2 - i];
			}
			return array2;
		}

		internal static IEnumerable<string> SplitHeaderValue(this string value, params char[] separators)
		{
			int len = value.Length;
			string seps = new string(separators);
			StringBuilder buff = new StringBuilder(32);
			bool escaped = false;
			bool quoted = false;
			for (int i = 0; i < len; i++)
			{
				char c = value[i];
				switch (c)
				{
				case '"':
					if (escaped)
					{
						escaped = !escaped;
					}
					else
					{
						quoted = !quoted;
					}
					break;
				case '\\':
					if (i < len - 1 && value[i + 1] == '"')
					{
						escaped = true;
					}
					break;
				default:
					if (seps.Contains(c) && !quoted)
					{
						yield return buff.ToString();
						buff.Length = 0;
						continue;
					}
					break;
				}
				buff.Append(c);
			}
			if (buff.Length > 0)
			{
				yield return buff.ToString();
			}
		}

		internal static byte[] ToByteArray(this Stream stream)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.Position = 0L;
				stream.CopyTo(memoryStream);
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static CompressionMethod ToCompressionMethod(this string value)
		{
			foreach (CompressionMethod value2 in Enum.GetValues(typeof(CompressionMethod)))
			{
				if (value2.ToExtensionString() == value)
				{
					return value2;
				}
			}
			return CompressionMethod.None;
		}

		internal static string ToExtensionString(this CompressionMethod method, params string[] parameters)
		{
			if (method == CompressionMethod.None)
			{
				return string.Empty;
			}
			string text = string.Format("permessage-{0}", method.ToString().ToLower());
			if (parameters == null || parameters.Length == 0)
			{
				return text;
			}
			return string.Format("{0}; {1}", text, parameters.ToString("; "));
		}

		internal static IPAddress ToIPAddress(this string hostNameOrAddress)
		{
			try
			{
				IPAddress[] hostAddresses = Dns.GetHostAddresses(hostNameOrAddress);
				return hostAddresses[0];
			}
			catch
			{
				return null;
			}
		}

		internal static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			return new List<TSource>(source);
		}

		internal static ushort ToUInt16(this byte[] source, ByteOrder sourceOrder)
		{
			return BitConverter.ToUInt16(source.ToHostOrder(sourceOrder), 0);
		}

		internal static ulong ToUInt64(this byte[] source, ByteOrder sourceOrder)
		{
			return BitConverter.ToUInt64(source.ToHostOrder(sourceOrder), 0);
		}

		internal static string TrimEndSlash(this string value)
		{
			value = value.TrimEnd('/');
			return (value.Length <= 0) ? "/" : value;
		}

		internal static bool TryCreateWebSocketUri(this string uriString, out Uri result, out string message)
		{
			result = null;
			Uri uri = uriString.ToUri();
			if (!uri.IsAbsoluteUri)
			{
				message = "Not an absolute URI: " + uriString;
				return false;
			}
			string scheme = uri.Scheme;
			if (scheme != "ws" && scheme != "wss")
			{
				message = "The scheme part isn't 'ws' or 'wss': " + uriString;
				return false;
			}
			if (uri.Fragment.Length > 0)
			{
				message = "Includes the fragment component: " + uriString;
				return false;
			}
			int port = uri.Port;
			if (port > 0)
			{
				if (port > 65535)
				{
					message = "The port part is greater than 65535: " + uriString;
					return false;
				}
				if ((scheme == "ws" && port == 443) || (scheme == "wss" && port == 80))
				{
					message = "An invalid pair of scheme and port: " + uriString;
					return false;
				}
			}
			else
			{
				uri = new Uri(string.Format("{0}://{1}:{2}{3}", scheme, uri.Host, (!(scheme == "ws")) ? 443 : 80, uri.PathAndQuery));
			}
			result = uri;
			message = string.Empty;
			return true;
		}

		internal static string Unquote(this string value)
		{
			int num = value.IndexOf('"');
			if (num < 0)
			{
				return value;
			}
			int num2 = value.LastIndexOf('"');
			int num3 = num2 - num - 1;
			return (num3 < 0) ? value : ((num3 != 0) ? value.Substring(num + 1, num3).Replace("\\\"", "\"") : string.Empty);
		}

		internal static void WriteBytes(this Stream stream, byte[] bytes)
		{
			using (MemoryStream source = new MemoryStream(bytes))
			{
				source.CopyTo(stream);
			}
		}

		public static bool Contains(this string value, params char[] chars)
		{
			return chars == null || chars.Length == 0 || (value != null && value.Length != 0 && value.IndexOfAny(chars) > -1);
		}

		public static bool Contains(this NameValueCollection collection, string name)
		{
			return collection != null && collection.Count > 0 && collection[name] != null;
		}

		public static bool Contains(this NameValueCollection collection, string name, string value)
		{
			if (collection == null || collection.Count == 0)
			{
				return false;
			}
			string text = collection[name];
			if (text == null)
			{
				return false;
			}
			string[] array = text.Split(',');
			foreach (string text2 in array)
			{
				if (text2.Trim().Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public static void Emit(this EventHandler eventHandler, object sender, EventArgs e)
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		public static void Emit<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e) where TEventArgs : EventArgs
		{
			if (eventHandler != null)
			{
				eventHandler(sender, e);
			}
		}

		public static WebSocketSharp.Net.CookieCollection GetCookies(this NameValueCollection headers, bool response)
		{
			string name = ((!response) ? "Cookie" : "Set-Cookie");
			return (headers == null || !headers.Contains(name)) ? new WebSocketSharp.Net.CookieCollection() : WebSocketSharp.Net.CookieCollection.Parse(headers[name], response);
		}

		public static string GetDescription(this WebSocketSharp.Net.HttpStatusCode code)
		{
			return ((int)code).GetStatusDescription();
		}

		public static string GetStatusDescription(this int code)
		{
			switch (code)
			{
			case 100:
				return "Continue";
			case 101:
				return "Switching Protocols";
			case 102:
				return "Processing";
			case 200:
				return "OK";
			case 201:
				return "Created";
			case 202:
				return "Accepted";
			case 203:
				return "Non-Authoritative Information";
			case 204:
				return "No Content";
			case 205:
				return "Reset Content";
			case 206:
				return "Partial Content";
			case 207:
				return "Multi-Status";
			case 300:
				return "Multiple Choices";
			case 301:
				return "Moved Permanently";
			case 302:
				return "Found";
			case 303:
				return "See Other";
			case 304:
				return "Not Modified";
			case 305:
				return "Use Proxy";
			case 307:
				return "Temporary Redirect";
			case 400:
				return "Bad Request";
			case 401:
				return "Unauthorized";
			case 402:
				return "Payment Required";
			case 403:
				return "Forbidden";
			case 404:
				return "Not Found";
			case 405:
				return "Method Not Allowed";
			case 406:
				return "Not Acceptable";
			case 407:
				return "Proxy Authentication Required";
			case 408:
				return "Request Timeout";
			case 409:
				return "Conflict";
			case 410:
				return "Gone";
			case 411:
				return "Length Required";
			case 412:
				return "Precondition Failed";
			case 413:
				return "Request Entity Too Large";
			case 414:
				return "Request-Uri Too Long";
			case 415:
				return "Unsupported Media Type";
			case 416:
				return "Requested Range Not Satisfiable";
			case 417:
				return "Expectation Failed";
			case 422:
				return "Unprocessable Entity";
			case 423:
				return "Locked";
			case 424:
				return "Failed Dependency";
			case 500:
				return "Internal Server Error";
			case 501:
				return "Not Implemented";
			case 502:
				return "Bad Gateway";
			case 503:
				return "Service Unavailable";
			case 504:
				return "Gateway Timeout";
			case 505:
				return "Http Version Not Supported";
			case 507:
				return "Insufficient Storage";
			default:
				return string.Empty;
			}
		}

		public static bool IsCloseStatusCode(this ushort value)
		{
			return value > 999 && value < 5000;
		}

		public static bool IsEnclosedIn(this string value, char c)
		{
			return value != null && value.Length > 1 && value[0] == c && value[value.Length - 1] == c;
		}

		public static bool IsHostOrder(this ByteOrder order)
		{
			return !(BitConverter.IsLittleEndian ^ (order == ByteOrder.Little));
		}

		public static bool IsLocal(this IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Equals(IPAddress.Any) || IPAddress.IsLoopback(address))
			{
				return true;
			}
			string hostName = Dns.GetHostName();
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
			IPAddress[] array = hostAddresses;
			foreach (IPAddress obj in array)
			{
				if (address.Equals(obj))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNullOrEmpty(this string value)
		{
			return value == null || value.Length == 0;
		}

		public static bool IsPredefinedScheme(this string value)
		{
			if (value == null || value.Length < 2)
			{
				return false;
			}
			char c = value[0];
			if (c == 'h')
			{
				return value == "http" || value == "https";
			}
			if (c == 'w')
			{
				return value == "ws" || value == "wss";
			}
			if (c == 'f')
			{
				return value == "file" || value == "ftp";
			}
			if (c == 'n')
			{
				c = value[1];
				return (c != 'e') ? (value == "nntp") : (value == "news" || value == "net.pipe" || value == "net.tcp");
			}
			return (c == 'g' && value == "gopher") || (c == 'm' && value == "mailto");
		}

		public static bool IsUpgradeTo(this WebSocketSharp.Net.HttpListenerRequest request, string protocol)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (protocol == null)
			{
				throw new ArgumentNullException("protocol");
			}
			if (protocol.Length == 0)
			{
				throw new ArgumentException("An empty string.", "protocol");
			}
			return request.Headers.Contains("Upgrade", protocol) && request.Headers.Contains("Connection", "Upgrade");
		}

		public static bool MaybeUri(this string value)
		{
			if (value == null || value.Length == 0)
			{
				return false;
			}
			int num = value.IndexOf(':');
			if (num == -1)
			{
				return false;
			}
			if (num >= 10)
			{
				return false;
			}
			return value.Substring(0, num).IsPredefinedScheme();
		}

		public static T[] SubArray<T>(this T[] array, int startIndex, int length)
		{
			int num;
			if (array == null || (num = array.Length) == 0)
			{
				return new T[0];
			}
			if (startIndex < 0 || length <= 0 || startIndex + length > num)
			{
				return new T[0];
			}
			if (startIndex == 0 && length == num)
			{
				return array;
			}
			T[] array2 = new T[length];
			Array.Copy(array, startIndex, array2, 0, length);
			return array2;
		}

		public static T[] SubArray<T>(this T[] array, long startIndex, long length)
		{
			long longLength;
			if (array == null || (longLength = array.LongLength) == 0)
			{
				return new T[0];
			}
			if (startIndex < 0 || length <= 0 || startIndex + length > longLength)
			{
				return new T[0];
			}
			if (startIndex == 0 && length == longLength)
			{
				return array;
			}
			T[] array2 = new T[length];
			Array.Copy(array, startIndex, array2, 0L, length);
			return array2;
		}

		public static void Times(this int n, Action action)
		{
			if (n > 0 && action != null)
			{
				((ulong)n).times(action);
			}
		}

		public static void Times(this long n, Action action)
		{
			if (n > 0 && action != null)
			{
				((ulong)n).times(action);
			}
		}

		public static void Times(this uint n, Action action)
		{
			if (n != 0 && action != null)
			{
				times(n, action);
			}
		}

		public static void Times(this ulong n, Action action)
		{
			if (n != 0 && action != null)
			{
				n.times(action);
			}
		}

		public static void Times(this int n, Action<int> action)
		{
			if (n > 0 && action != null)
			{
				for (int i = 0; i < n; i++)
				{
					action(i);
				}
			}
		}

		public static void Times(this long n, Action<long> action)
		{
			if (n > 0 && action != null)
			{
				for (long num = 0L; num < n; num++)
				{
					action(num);
				}
			}
		}

		public static void Times(this uint n, Action<uint> action)
		{
			if (n != 0 && action != null)
			{
				for (uint num = 0u; num < n; num++)
				{
					action(num);
				}
			}
		}

		public static void Times(this ulong n, Action<ulong> action)
		{
			if (n != 0 && action != null)
			{
				for (ulong num = 0uL; num < n; num++)
				{
					action(num);
				}
			}
		}

		public static T To<T>(this byte[] source, ByteOrder sourceOrder) where T : struct
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Length == 0)
			{
				return default(T);
			}
			Type typeFromHandle = typeof(T);
			byte[] value = source.ToHostOrder(sourceOrder);
			return (typeFromHandle == typeof(bool)) ? ((T)(object)BitConverter.ToBoolean(value, 0)) : ((typeFromHandle == typeof(char)) ? ((T)(object)BitConverter.ToChar(value, 0)) : ((typeFromHandle == typeof(double)) ? ((T)(object)BitConverter.ToDouble(value, 0)) : ((typeFromHandle == typeof(short)) ? ((T)(object)BitConverter.ToInt16(value, 0)) : ((typeFromHandle == typeof(int)) ? ((T)(object)BitConverter.ToInt32(value, 0)) : ((typeFromHandle == typeof(long)) ? ((T)(object)BitConverter.ToInt64(value, 0)) : ((typeFromHandle == typeof(float)) ? ((T)(object)BitConverter.ToSingle(value, 0)) : ((typeFromHandle == typeof(ushort)) ? ((T)(object)BitConverter.ToUInt16(value, 0)) : ((typeFromHandle == typeof(uint)) ? ((T)(object)BitConverter.ToUInt32(value, 0)) : ((typeFromHandle != typeof(ulong)) ? default(T) : ((T)(object)BitConverter.ToUInt64(value, 0)))))))))));
		}

		public static byte[] ToByteArray<T>(this T value, ByteOrder order) where T : struct
		{
			Type typeFromHandle = typeof(T);
			byte[] array = ((typeFromHandle == typeof(bool)) ? BitConverter.GetBytes((bool)(object)value) : ((typeFromHandle == typeof(byte)) ? new byte[1] { (byte)(object)value } : ((typeFromHandle == typeof(char)) ? BitConverter.GetBytes((char)(object)value) : ((typeFromHandle == typeof(double)) ? BitConverter.GetBytes((double)(object)value) : ((typeFromHandle == typeof(short)) ? BitConverter.GetBytes((short)(object)value) : ((typeFromHandle == typeof(int)) ? BitConverter.GetBytes((int)(object)value) : ((typeFromHandle == typeof(long)) ? BitConverter.GetBytes((long)(object)value) : ((typeFromHandle == typeof(float)) ? BitConverter.GetBytes((float)(object)value) : ((typeFromHandle == typeof(ushort)) ? BitConverter.GetBytes((ushort)(object)value) : ((typeFromHandle == typeof(uint)) ? BitConverter.GetBytes((uint)(object)value) : ((typeFromHandle != typeof(ulong)) ? new byte[0] : BitConverter.GetBytes((ulong)(object)value))))))))))));
			if (array.Length > 1 && !order.IsHostOrder())
			{
				Array.Reverse(array);
			}
			return array;
		}

		public static byte[] ToHostOrder(this byte[] source, ByteOrder sourceOrder)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return (source.Length <= 1 || sourceOrder.IsHostOrder()) ? source : source.Reverse();
		}

		public static string ToString<T>(this T[] array, string separator)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = array.Length;
			if (num == 0)
			{
				return string.Empty;
			}
			if (separator == null)
			{
				separator = string.Empty;
			}
			StringBuilder buff = new StringBuilder(64);
			(num - 1).Times(delegate(int i)
			{
				buff.AppendFormat("{0}{1}", array[i].ToString(), separator);
			});
			buff.Append(array[num - 1].ToString());
			return buff.ToString();
		}

		public static Uri ToUri(this string uriString)
		{
			Uri result;
			return (!Uri.TryCreate(uriString, uriString.MaybeUri() ? UriKind.Absolute : UriKind.Relative, out result)) ? null : result;
		}

		public static string UrlDecode(this string value)
		{
			return (value == null || value.Length <= 0) ? value : HttpUtility.UrlDecode(value);
		}

		public static string UrlEncode(this string value)
		{
			return (value == null || value.Length <= 0) ? value : HttpUtility.UrlEncode(value);
		}

		public static void WriteContent(this WebSocketSharp.Net.HttpListenerResponse response, byte[] content)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			int num = 0;
			if (content != null && (num = content.Length) != 0)
			{
				Stream outputStream = response.OutputStream;
				response.ContentLength64 = num;
				outputStream.Write(content, 0, num);
				outputStream.Close();
			}
		}
	}
}
