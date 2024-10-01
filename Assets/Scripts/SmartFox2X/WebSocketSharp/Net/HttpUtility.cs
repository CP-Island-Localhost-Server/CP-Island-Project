using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace WebSocketSharp.Net
{
	internal sealed class HttpUtility
	{
		private static Dictionary<string, char> _entities;

		private static char[] _hexChars = "0123456789abcdef".ToCharArray();

		private static object _sync = new object();

		private static int getChar(byte[] bytes, int offset, int length)
		{
			int num = 0;
			int num2 = length + offset;
			for (int i = offset; i < num2; i++)
			{
				int @int = getInt(bytes[i]);
				if (@int == -1)
				{
					return -1;
				}
				num = (num << 4) + @int;
			}
			return num;
		}

		private static int getChar(string s, int offset, int length)
		{
			int num = 0;
			int num2 = length + offset;
			for (int i = offset; i < num2; i++)
			{
				char c = s[i];
				if (c > '\u007f')
				{
					return -1;
				}
				int @int = getInt((byte)c);
				if (@int == -1)
				{
					return -1;
				}
				num = (num << 4) + @int;
			}
			return num;
		}

		private static char[] getChars(MemoryStream buffer, Encoding encoding)
		{
			return encoding.GetChars(buffer.GetBuffer(), 0, (int)buffer.Length);
		}

		private static Dictionary<string, char> getEntities()
		{
			lock (_sync)
			{
				if (_entities == null)
				{
					initEntities();
				}
				return _entities;
			}
		}

		private static int getInt(byte b)
		{
			char c = (char)b;
			return (c >= '0' && c <= '9') ? (c - 48) : ((c >= 'a' && c <= 'f') ? (c - 97 + 10) : ((c < 'A' || c > 'F') ? (-1) : (c - 65 + 10)));
		}

		private static void initEntities()
		{
			_entities = new Dictionary<string, char>();
			_entities.Add("nbsp", '\u00a0');
			_entities.Add("iexcl", '¡');
			_entities.Add("cent", '¢');
			_entities.Add("pound", '£');
			_entities.Add("curren", '¤');
			_entities.Add("yen", '¥');
			_entities.Add("brvbar", '¦');
			_entities.Add("sect", '§');
			_entities.Add("uml", '\u00a8');
			_entities.Add("copy", '©');
			_entities.Add("ordf", 'ª');
			_entities.Add("laquo", '«');
			_entities.Add("not", '¬');
			_entities.Add("shy", '\u00ad');
			_entities.Add("reg", '®');
			_entities.Add("macr", '\u00af');
			_entities.Add("deg", '°');
			_entities.Add("plusmn", '±');
			_entities.Add("sup2", '²');
			_entities.Add("sup3", '³');
			_entities.Add("acute", '\u00b4');
			_entities.Add("micro", 'µ');
			_entities.Add("para", '¶');
			_entities.Add("middot", '·');
			_entities.Add("cedil", '\u00b8');
			_entities.Add("sup1", '¹');
			_entities.Add("ordm", 'º');
			_entities.Add("raquo", '»');
			_entities.Add("frac14", '¼');
			_entities.Add("frac12", '½');
			_entities.Add("frac34", '¾');
			_entities.Add("iquest", '¿');
			_entities.Add("Agrave", 'À');
			_entities.Add("Aacute", 'Á');
			_entities.Add("Acirc", 'Â');
			_entities.Add("Atilde", 'Ã');
			_entities.Add("Auml", 'Ä');
			_entities.Add("Aring", 'Å');
			_entities.Add("AElig", 'Æ');
			_entities.Add("Ccedil", 'Ç');
			_entities.Add("Egrave", 'È');
			_entities.Add("Eacute", 'É');
			_entities.Add("Ecirc", 'Ê');
			_entities.Add("Euml", 'Ë');
			_entities.Add("Igrave", 'Ì');
			_entities.Add("Iacute", 'Í');
			_entities.Add("Icirc", 'Î');
			_entities.Add("Iuml", 'Ï');
			_entities.Add("ETH", 'Ð');
			_entities.Add("Ntilde", 'Ñ');
			_entities.Add("Ograve", 'Ò');
			_entities.Add("Oacute", 'Ó');
			_entities.Add("Ocirc", 'Ô');
			_entities.Add("Otilde", 'Õ');
			_entities.Add("Ouml", 'Ö');
			_entities.Add("times", '×');
			_entities.Add("Oslash", 'Ø');
			_entities.Add("Ugrave", 'Ù');
			_entities.Add("Uacute", 'Ú');
			_entities.Add("Ucirc", 'Û');
			_entities.Add("Uuml", 'Ü');
			_entities.Add("Yacute", 'Ý');
			_entities.Add("THORN", 'Þ');
			_entities.Add("szlig", 'ß');
			_entities.Add("agrave", 'à');
			_entities.Add("aacute", 'á');
			_entities.Add("acirc", 'â');
			_entities.Add("atilde", 'ã');
			_entities.Add("auml", 'ä');
			_entities.Add("aring", 'å');
			_entities.Add("aelig", 'æ');
			_entities.Add("ccedil", 'ç');
			_entities.Add("egrave", 'è');
			_entities.Add("eacute", 'é');
			_entities.Add("ecirc", 'ê');
			_entities.Add("euml", 'ë');
			_entities.Add("igrave", 'ì');
			_entities.Add("iacute", 'í');
			_entities.Add("icirc", 'î');
			_entities.Add("iuml", 'ï');
			_entities.Add("eth", 'ð');
			_entities.Add("ntilde", 'ñ');
			_entities.Add("ograve", 'ò');
			_entities.Add("oacute", 'ó');
			_entities.Add("ocirc", 'ô');
			_entities.Add("otilde", 'õ');
			_entities.Add("ouml", 'ö');
			_entities.Add("divide", '÷');
			_entities.Add("oslash", 'ø');
			_entities.Add("ugrave", 'ù');
			_entities.Add("uacute", 'ú');
			_entities.Add("ucirc", 'û');
			_entities.Add("uuml", 'ü');
			_entities.Add("yacute", 'ý');
			_entities.Add("thorn", 'þ');
			_entities.Add("yuml", 'ÿ');
			_entities.Add("fnof", 'ƒ');
			_entities.Add("Alpha", 'Α');
			_entities.Add("Beta", 'Β');
			_entities.Add("Gamma", 'Γ');
			_entities.Add("Delta", 'Δ');
			_entities.Add("Epsilon", 'Ε');
			_entities.Add("Zeta", 'Ζ');
			_entities.Add("Eta", 'Η');
			_entities.Add("Theta", 'Θ');
			_entities.Add("Iota", 'Ι');
			_entities.Add("Kappa", 'Κ');
			_entities.Add("Lambda", 'Λ');
			_entities.Add("Mu", 'Μ');
			_entities.Add("Nu", 'Ν');
			_entities.Add("Xi", 'Ξ');
			_entities.Add("Omicron", 'Ο');
			_entities.Add("Pi", 'Π');
			_entities.Add("Rho", 'Ρ');
			_entities.Add("Sigma", 'Σ');
			_entities.Add("Tau", 'Τ');
			_entities.Add("Upsilon", 'Υ');
			_entities.Add("Phi", 'Φ');
			_entities.Add("Chi", 'Χ');
			_entities.Add("Psi", 'Ψ');
			_entities.Add("Omega", 'Ω');
			_entities.Add("alpha", 'α');
			_entities.Add("beta", 'β');
			_entities.Add("gamma", 'γ');
			_entities.Add("delta", 'δ');
			_entities.Add("epsilon", 'ε');
			_entities.Add("zeta", 'ζ');
			_entities.Add("eta", 'η');
			_entities.Add("theta", 'θ');
			_entities.Add("iota", 'ι');
			_entities.Add("kappa", 'κ');
			_entities.Add("lambda", 'λ');
			_entities.Add("mu", 'μ');
			_entities.Add("nu", 'ν');
			_entities.Add("xi", 'ξ');
			_entities.Add("omicron", 'ο');
			_entities.Add("pi", 'π');
			_entities.Add("rho", 'ρ');
			_entities.Add("sigmaf", 'ς');
			_entities.Add("sigma", 'σ');
			_entities.Add("tau", 'τ');
			_entities.Add("upsilon", 'υ');
			_entities.Add("phi", 'φ');
			_entities.Add("chi", 'χ');
			_entities.Add("psi", 'ψ');
			_entities.Add("omega", 'ω');
			_entities.Add("thetasym", 'ϑ');
			_entities.Add("upsih", 'ϒ');
			_entities.Add("piv", 'ϖ');
			_entities.Add("bull", '•');
			_entities.Add("hellip", '…');
			_entities.Add("prime", '′');
			_entities.Add("Prime", '″');
			_entities.Add("oline", '‾');
			_entities.Add("frasl", '⁄');
			_entities.Add("weierp", '℘');
			_entities.Add("image", 'ℑ');
			_entities.Add("real", 'ℜ');
			_entities.Add("trade", '™');
			_entities.Add("alefsym", 'ℵ');
			_entities.Add("larr", '←');
			_entities.Add("uarr", '↑');
			_entities.Add("rarr", '→');
			_entities.Add("darr", '↓');
			_entities.Add("harr", '↔');
			_entities.Add("crarr", '↵');
			_entities.Add("lArr", '⇐');
			_entities.Add("uArr", '⇑');
			_entities.Add("rArr", '⇒');
			_entities.Add("dArr", '⇓');
			_entities.Add("hArr", '⇔');
			_entities.Add("forall", '∀');
			_entities.Add("part", '∂');
			_entities.Add("exist", '∃');
			_entities.Add("empty", '∅');
			_entities.Add("nabla", '∇');
			_entities.Add("isin", '∈');
			_entities.Add("notin", '∉');
			_entities.Add("ni", '∋');
			_entities.Add("prod", '∏');
			_entities.Add("sum", '∑');
			_entities.Add("minus", '−');
			_entities.Add("lowast", '∗');
			_entities.Add("radic", '√');
			_entities.Add("prop", '∝');
			_entities.Add("infin", '∞');
			_entities.Add("ang", '∠');
			_entities.Add("and", '∧');
			_entities.Add("or", '∨');
			_entities.Add("cap", '∩');
			_entities.Add("cup", '∪');
			_entities.Add("int", '∫');
			_entities.Add("there4", '∴');
			_entities.Add("sim", '∼');
			_entities.Add("cong", '≅');
			_entities.Add("asymp", '≈');
			_entities.Add("ne", '≠');
			_entities.Add("equiv", '≡');
			_entities.Add("le", '≤');
			_entities.Add("ge", '≥');
			_entities.Add("sub", '⊂');
			_entities.Add("sup", '⊃');
			_entities.Add("nsub", '⊄');
			_entities.Add("sube", '⊆');
			_entities.Add("supe", '⊇');
			_entities.Add("oplus", '⊕');
			_entities.Add("otimes", '⊗');
			_entities.Add("perp", '⊥');
			_entities.Add("sdot", '⋅');
			_entities.Add("lceil", '⌈');
			_entities.Add("rceil", '⌉');
			_entities.Add("lfloor", '⌊');
			_entities.Add("rfloor", '⌋');
			_entities.Add("lang", '〈');
			_entities.Add("rang", '〉');
			_entities.Add("loz", '◊');
			_entities.Add("spades", '♠');
			_entities.Add("clubs", '♣');
			_entities.Add("hearts", '♥');
			_entities.Add("diams", '♦');
			_entities.Add("quot", '"');
			_entities.Add("amp", '&');
			_entities.Add("lt", '<');
			_entities.Add("gt", '>');
			_entities.Add("OElig", 'Œ');
			_entities.Add("oelig", 'œ');
			_entities.Add("Scaron", 'Š');
			_entities.Add("scaron", 'š');
			_entities.Add("Yuml", 'Ÿ');
			_entities.Add("circ", 'ˆ');
			_entities.Add("tilde", '\u02dc');
			_entities.Add("ensp", '\u2002');
			_entities.Add("emsp", '\u2003');
			_entities.Add("thinsp", '\u2009');
			_entities.Add("zwnj", '\u200c');
			_entities.Add("zwj", '\u200d');
			_entities.Add("lrm", '\u200e');
			_entities.Add("rlm", '\u200f');
			_entities.Add("ndash", '–');
			_entities.Add("mdash", '—');
			_entities.Add("lsquo", '‘');
			_entities.Add("rsquo", '’');
			_entities.Add("sbquo", '‚');
			_entities.Add("ldquo", '“');
			_entities.Add("rdquo", '”');
			_entities.Add("bdquo", '„');
			_entities.Add("dagger", '†');
			_entities.Add("Dagger", '‡');
			_entities.Add("permil", '‰');
			_entities.Add("lsaquo", '‹');
			_entities.Add("rsaquo", '›');
			_entities.Add("euro", '€');
		}

		private static bool notEncoded(char c)
		{
			return c == '!' || c == '\'' || c == '(' || c == ')' || c == '*' || c == '-' || c == '.' || c == '_';
		}

		private static void urlEncode(char c, Stream result, bool unicode)
		{
			if (c > 'ÿ')
			{
				result.WriteByte(37);
				result.WriteByte(117);
				int num = (int)c >> 12;
				result.WriteByte((byte)_hexChars[num]);
				num = ((int)c >> 8) & 0xF;
				result.WriteByte((byte)_hexChars[num]);
				num = ((int)c >> 4) & 0xF;
				result.WriteByte((byte)_hexChars[num]);
				num = c & 0xF;
				result.WriteByte((byte)_hexChars[num]);
			}
			else if (c > ' ' && notEncoded(c))
			{
				result.WriteByte((byte)c);
			}
			else if (c == ' ')
			{
				result.WriteByte(43);
			}
			else if (c < '0' || (c < 'A' && c > '9') || (c > 'Z' && c < 'a') || c > 'z')
			{
				if (unicode && c > '\u007f')
				{
					result.WriteByte(37);
					result.WriteByte(117);
					result.WriteByte(48);
					result.WriteByte(48);
				}
				else
				{
					result.WriteByte(37);
				}
				int num2 = (int)c >> 4;
				result.WriteByte((byte)_hexChars[num2]);
				num2 = c & 0xF;
				result.WriteByte((byte)_hexChars[num2]);
			}
			else
			{
				result.WriteByte((byte)c);
			}
		}

		private static void urlPathEncode(char c, Stream result)
		{
			if (c < '!' || c > '~')
			{
				byte[] bytes = Encoding.UTF8.GetBytes(c.ToString());
				byte[] array = bytes;
				foreach (byte b in array)
				{
					result.WriteByte(37);
					int num = b;
					int num2 = num >> 4;
					result.WriteByte((byte)_hexChars[num2]);
					num2 = num & 0xF;
					result.WriteByte((byte)_hexChars[num2]);
				}
			}
			else if (c == ' ')
			{
				result.WriteByte(37);
				result.WriteByte(50);
				result.WriteByte(48);
			}
			else
			{
				result.WriteByte((byte)c);
			}
		}

		private static void writeCharBytes(char c, IList buffer, Encoding encoding)
		{
			if (c > 'ÿ')
			{
				byte[] bytes = encoding.GetBytes(new char[1] { c });
				foreach (byte b in bytes)
				{
					buffer.Add(b);
				}
			}
			else
			{
				buffer.Add((byte)c);
			}
		}

		internal static Uri CreateRequestUrl(string requestUri, string host, bool websocketRequest, bool secure)
		{
			if (requestUri == null || requestUri.Length == 0 || host == null || host.Length == 0)
			{
				return null;
			}
			string text = null;
			string arg = null;
			if (requestUri.StartsWith("/"))
			{
				arg = requestUri;
			}
			else if (requestUri.MaybeUri())
			{
				Uri result;
				if (!Uri.TryCreate(requestUri, UriKind.Absolute, out result) || ((!(text = result.Scheme).StartsWith("http") || websocketRequest) && (!text.StartsWith("ws") || !websocketRequest)))
				{
					return null;
				}
				host = result.Authority;
				arg = result.PathAndQuery;
			}
			else if (!(requestUri == "*"))
			{
				host = requestUri;
			}
			if (text == null)
			{
				text = ((!websocketRequest) ? "http" : "ws") + ((!secure) ? string.Empty : "s");
			}
			int num = host.IndexOf(':');
			if (num == -1)
			{
				host = string.Format("{0}:{1}", host, (!(text == "http") && !(text == "ws")) ? 443 : 80);
			}
			string uriString = string.Format("{0}://{1}{2}", text, host, arg);
			Uri result2;
			if (!Uri.TryCreate(uriString, UriKind.Absolute, out result2))
			{
				return null;
			}
			return result2;
		}

		internal static IPrincipal CreateUser(string response, AuthenticationSchemes scheme, string realm, string method, Func<IIdentity, NetworkCredential> credentialsFinder)
		{
			if (response == null || !response.StartsWith(scheme.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			AuthenticationResponse authenticationResponse = AuthenticationResponse.Parse(response);
			if (authenticationResponse == null)
			{
				return null;
			}
			IIdentity identity = authenticationResponse.ToIdentity();
			if (identity == null)
			{
				return null;
			}
			NetworkCredential networkCredential = null;
			try
			{
				networkCredential = credentialsFinder(identity);
			}
			catch
			{
			}
			if (networkCredential == null)
			{
				return null;
			}
			bool num;
			if (scheme == AuthenticationSchemes.Basic)
			{
				num = ((HttpBasicIdentity)identity).Password == networkCredential.Password;
			}
			else
			{
				if (scheme != AuthenticationSchemes.Digest)
				{
					goto IL_00b6;
				}
				num = ((HttpDigestIdentity)identity).IsValid(networkCredential.Password, realm, method, null);
			}
			if (!num)
			{
				goto IL_00b6;
			}
			object result = new GenericPrincipal(identity, networkCredential.Roles);
			goto IL_00b7;
			IL_00b6:
			result = null;
			goto IL_00b7;
			IL_00b7:
			return (IPrincipal)result;
		}

		internal static Encoding GetEncoding(string contentType)
		{
			string[] array = contentType.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim();
				if (text2.StartsWith("charset", StringComparison.OrdinalIgnoreCase))
				{
					return Encoding.GetEncoding(text2.GetValue('=', true));
				}
			}
			return null;
		}

		internal static NameValueCollection InternalParseQueryString(string query, Encoding encoding)
		{
			int length;
			if (query == null || (length = query.Length) == 0 || (length == 1 && query[0] == '?'))
			{
				return new NameValueCollection(1);
			}
			if (query[0] == '?')
			{
				query = query.Substring(1);
			}
			QueryStringCollection queryStringCollection = new QueryStringCollection();
			string[] array = query.Split('&');
			string[] array2 = array;
			foreach (string text in array2)
			{
				int num = text.IndexOf('=');
				if (num > -1)
				{
					string name = UrlDecode(text.Substring(0, num), encoding);
					string val = ((text.Length <= num + 1) ? string.Empty : UrlDecode(text.Substring(num + 1), encoding));
					queryStringCollection.Add(name, val);
				}
				else
				{
					queryStringCollection.Add(null, UrlDecode(text, encoding));
				}
			}
			return queryStringCollection;
		}

		internal static string InternalUrlDecode(byte[] bytes, int offset, int count, Encoding encoding)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = count + offset;
				for (int i = offset; i < num; i++)
				{
					if (bytes[i] == 37 && i + 2 < count && bytes[i + 1] != 37)
					{
						int @char;
						if (bytes[i + 1] == 117 && i + 5 < num)
						{
							if (memoryStream.Length > 0)
							{
								stringBuilder.Append(getChars(memoryStream, encoding));
								memoryStream.SetLength(0L);
							}
							@char = getChar(bytes, i + 2, 4);
							if (@char != -1)
							{
								stringBuilder.Append((char)@char);
								i += 5;
								continue;
							}
						}
						else if ((@char = getChar(bytes, i + 1, 2)) != -1)
						{
							memoryStream.WriteByte((byte)@char);
							i += 2;
							continue;
						}
					}
					if (memoryStream.Length > 0)
					{
						stringBuilder.Append(getChars(memoryStream, encoding));
						memoryStream.SetLength(0L);
					}
					if (bytes[i] == 43)
					{
						stringBuilder.Append(' ');
					}
					else
					{
						stringBuilder.Append((char)bytes[i]);
					}
				}
				if (memoryStream.Length > 0)
				{
					stringBuilder.Append(getChars(memoryStream, encoding));
				}
			}
			return stringBuilder.ToString();
		}

		internal static byte[] InternalUrlDecodeToBytes(byte[] bytes, int offset, int count)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = offset + count;
				for (int i = offset; i < num; i++)
				{
					char c = (char)bytes[i];
					switch (c)
					{
					case '+':
						c = ' ';
						break;
					case '%':
						if (i < num - 2)
						{
							int @char = getChar(bytes, i + 1, 2);
							if (@char != -1)
							{
								c = (char)@char;
								i += 2;
							}
						}
						break;
					}
					memoryStream.WriteByte((byte)c);
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static byte[] InternalUrlEncodeToBytes(byte[] bytes, int offset, int count)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = offset + count;
				for (int i = offset; i < num; i++)
				{
					urlEncode((char)bytes[i], memoryStream, false);
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		internal static byte[] InternalUrlEncodeUnicodeToBytes(string s)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				foreach (char c in s)
				{
					urlEncode(c, memoryStream, true);
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		public static string HtmlAttributeEncode(string s)
		{
			if (s == null || s.Length == 0 || !s.Contains('&', '"', '<', '>'))
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				object value;
				switch (c)
				{
				case '&':
					value = "&amp;";
					break;
				case '"':
					value = "&quot;";
					break;
				case '<':
					value = "&lt;";
					break;
				case '>':
					value = "&gt;";
					break;
				default:
					value = c.ToString();
					break;
				}
				stringBuilder.Append((string)value);
			}
			return stringBuilder.ToString();
		}

		public static void HtmlAttributeEncode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(HtmlAttributeEncode(s));
		}

		public static string HtmlDecode(string s)
		{
			if (s == null || s.Length == 0 || !s.Contains('&'))
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			int num = 0;
			int num2 = 0;
			bool flag = false;
			foreach (char c in s)
			{
				if (num == 0)
				{
					if (c == '&')
					{
						stringBuilder.Append(c);
						num = 1;
					}
					else
					{
						stringBuilder2.Append(c);
					}
					continue;
				}
				if (c == '&')
				{
					num = 1;
					if (flag)
					{
						stringBuilder.Append(num2.ToString(CultureInfo.InvariantCulture));
						flag = false;
					}
					stringBuilder2.Append(stringBuilder.ToString());
					stringBuilder.Length = 0;
					stringBuilder.Append('&');
					continue;
				}
				switch (num)
				{
				case 1:
					if (c == ';')
					{
						num = 0;
						stringBuilder2.Append(stringBuilder.ToString());
						stringBuilder2.Append(c);
						stringBuilder.Length = 0;
					}
					else
					{
						num2 = 0;
						num = ((c == '#') ? 3 : 2);
						stringBuilder.Append(c);
					}
					break;
				case 2:
					stringBuilder.Append(c);
					if (c == ';')
					{
						string text = stringBuilder.ToString();
						Dictionary<string, char> entities = getEntities();
						if (text.Length > 1 && entities.ContainsKey(text.Substring(1, text.Length - 2)))
						{
							text = entities[text.Substring(1, text.Length - 2)].ToString();
						}
						stringBuilder2.Append(text);
						num = 0;
						stringBuilder.Length = 0;
					}
					break;
				case 3:
					if (c == ';')
					{
						if (num2 > 65535)
						{
							stringBuilder2.Append("&#");
							stringBuilder2.Append(num2.ToString(CultureInfo.InvariantCulture));
							stringBuilder2.Append(";");
						}
						else
						{
							stringBuilder2.Append((char)num2);
						}
						num = 0;
						stringBuilder.Length = 0;
						flag = false;
					}
					else if (char.IsDigit(c))
					{
						num2 = num2 * 10 + (c - 48);
						flag = true;
					}
					else
					{
						num = 2;
						if (flag)
						{
							stringBuilder.Append(num2.ToString(CultureInfo.InvariantCulture));
							flag = false;
						}
						stringBuilder.Append(c);
					}
					break;
				}
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder2.Append(stringBuilder.ToString());
			}
			else if (flag)
			{
				stringBuilder2.Append(num2.ToString(CultureInfo.InvariantCulture));
			}
			return stringBuilder2.ToString();
		}

		public static void HtmlDecode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(HtmlDecode(s));
		}

		public static string HtmlEncode(string s)
		{
			if (s == null || s.Length == 0)
			{
				return s;
			}
			bool flag = false;
			foreach (char c in s)
			{
				if (c == '&' || c == '"' || c == '<' || c == '>' || c > '\u009f')
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c2 in s)
			{
				if (c2 == '&')
				{
					stringBuilder.Append("&amp;");
				}
				else if (c2 == '"')
				{
					stringBuilder.Append("&quot;");
				}
				else if (c2 == '<')
				{
					stringBuilder.Append("&lt;");
				}
				else if (c2 == '>')
				{
					stringBuilder.Append("&gt;");
				}
				else if (c2 > '\u009f')
				{
					stringBuilder.Append("&#");
					int num = c2;
					stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append(";");
				}
				else
				{
					stringBuilder.Append(c2);
				}
			}
			return stringBuilder.ToString();
		}

		public static void HtmlEncode(string s, TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write(HtmlEncode(s));
		}

		public static NameValueCollection ParseQueryString(string query)
		{
			return ParseQueryString(query, Encoding.UTF8);
		}

		public static NameValueCollection ParseQueryString(string query, Encoding encoding)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}
			return InternalParseQueryString(query, encoding ?? Encoding.UTF8);
		}

		public static string UrlDecode(string s)
		{
			return UrlDecode(s, Encoding.UTF8);
		}

		public static string UrlDecode(string s, Encoding encoding)
		{
			if (s == null || s.Length == 0 || !s.Contains('%', '+'))
			{
				return s;
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			List<byte> list = new List<byte>();
			int length = s.Length;
			for (int i = 0; i < length; i++)
			{
				char c = s[i];
				if (c == '%' && i + 2 < length && s[i + 1] != '%')
				{
					int @char;
					if (s[i + 1] == 'u' && i + 5 < length)
					{
						@char = getChar(s, i + 2, 4);
						if (@char != -1)
						{
							writeCharBytes((char)@char, list, encoding);
							i += 5;
						}
						else
						{
							writeCharBytes('%', list, encoding);
						}
					}
					else if ((@char = getChar(s, i + 1, 2)) != -1)
					{
						writeCharBytes((char)@char, list, encoding);
						i += 2;
					}
					else
					{
						writeCharBytes('%', list, encoding);
					}
				}
				else if (c == '+')
				{
					writeCharBytes(' ', list, encoding);
				}
				else
				{
					writeCharBytes(c, list, encoding);
				}
			}
			return encoding.GetString(list.ToArray());
		}

		public static string UrlDecode(byte[] bytes, Encoding encoding)
		{
			int count;
			return (bytes == null) ? null : (((count = bytes.Length) != 0) ? InternalUrlDecode(bytes, 0, count, encoding ?? Encoding.UTF8) : string.Empty);
		}

		public static string UrlDecode(byte[] bytes, int offset, int count, Encoding encoding)
		{
			if (bytes == null)
			{
				return null;
			}
			int num = bytes.Length;
			if (num == 0 || count == 0)
			{
				return string.Empty;
			}
			if (offset < 0 || offset >= num)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > num - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return InternalUrlDecode(bytes, offset, count, encoding ?? Encoding.UTF8);
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes)
		{
			int count;
			return (bytes == null || (count = bytes.Length) <= 0) ? bytes : InternalUrlDecodeToBytes(bytes, 0, count);
		}

		public static byte[] UrlDecodeToBytes(string s)
		{
			return UrlDecodeToBytes(s, Encoding.UTF8);
		}

		public static byte[] UrlDecodeToBytes(string s, Encoding encoding)
		{
			if (s == null)
			{
				return null;
			}
			if (s.Length == 0)
			{
				return new byte[0];
			}
			byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(s);
			return InternalUrlDecodeToBytes(bytes, 0, bytes.Length);
		}

		public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
		{
			int num;
			if (bytes == null || (num = bytes.Length) == 0)
			{
				return bytes;
			}
			if (count == 0)
			{
				return new byte[0];
			}
			if (offset < 0 || offset >= num)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > num - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return InternalUrlDecodeToBytes(bytes, offset, count);
		}

		public static string UrlEncode(byte[] bytes)
		{
			int count;
			return (bytes == null) ? null : (((count = bytes.Length) != 0) ? Encoding.ASCII.GetString(InternalUrlEncodeToBytes(bytes, 0, count)) : string.Empty);
		}

		public static string UrlEncode(string s)
		{
			return UrlEncode(s, Encoding.UTF8);
		}

		public static string UrlEncode(string s, Encoding encoding)
		{
			int length;
			if (s == null || (length = s.Length) == 0)
			{
				return s;
			}
			bool flag = false;
			foreach (char c in s)
			{
				if ((c < '0' || (c < 'A' && c > '9') || (c > 'Z' && c < 'a') || c > 'z') && !notEncoded(c))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return s;
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			byte[] bytes = new byte[encoding.GetMaxByteCount(length)];
			int bytes2 = encoding.GetBytes(s, 0, length, bytes, 0);
			return Encoding.ASCII.GetString(InternalUrlEncodeToBytes(bytes, 0, bytes2));
		}

		public static string UrlEncode(byte[] bytes, int offset, int count)
		{
			byte[] array = UrlEncodeToBytes(bytes, offset, count);
			return (array == null) ? null : ((array.Length != 0) ? Encoding.ASCII.GetString(array) : string.Empty);
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes)
		{
			int count;
			return (bytes == null || (count = bytes.Length) <= 0) ? bytes : InternalUrlEncodeToBytes(bytes, 0, count);
		}

		public static byte[] UrlEncodeToBytes(string s)
		{
			return UrlEncodeToBytes(s, Encoding.UTF8);
		}

		public static byte[] UrlEncodeToBytes(string s, Encoding encoding)
		{
			if (s == null)
			{
				return null;
			}
			if (s.Length == 0)
			{
				return new byte[0];
			}
			byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(s);
			return InternalUrlEncodeToBytes(bytes, 0, bytes.Length);
		}

		public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
		{
			int num;
			if (bytes == null || (num = bytes.Length) == 0)
			{
				return bytes;
			}
			if (count == 0)
			{
				return new byte[0];
			}
			if (offset < 0 || offset >= num)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > num - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return InternalUrlEncodeToBytes(bytes, offset, count);
		}

		public static string UrlEncodeUnicode(string s)
		{
			return (s == null || s.Length <= 0) ? s : Encoding.ASCII.GetString(InternalUrlEncodeUnicodeToBytes(s));
		}

		public static byte[] UrlEncodeUnicodeToBytes(string s)
		{
			return (s == null) ? null : ((s.Length != 0) ? InternalUrlEncodeUnicodeToBytes(s) : new byte[0]);
		}

		public static string UrlPathEncode(string s)
		{
			if (s == null || s.Length == 0)
			{
				return s;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				foreach (char c in s)
				{
					urlPathEncode(c, memoryStream);
				}
				memoryStream.Close();
				return Encoding.ASCII.GetString(memoryStream.ToArray());
			}
		}
	}
}
