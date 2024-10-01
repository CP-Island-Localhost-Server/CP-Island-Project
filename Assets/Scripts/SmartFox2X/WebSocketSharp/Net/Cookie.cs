using System;
using System.Globalization;
using System.Text;

namespace WebSocketSharp.Net
{
	[Serializable]
	public sealed class Cookie
	{
		private string _comment;

		private Uri _commentUri;

		private bool _discard;

		private string _domain;

		private DateTime _expires;

		private bool _httpOnly;

		private string _name;

		private string _path;

		private string _port;

		private int[] _ports;

		private static readonly char[] _reservedCharsForName;

		private static readonly char[] _reservedCharsForValue;

		private bool _secure;

		private DateTime _timestamp;

		private string _value;

		private int _version;

		internal bool ExactDomain { get; set; }

		internal int MaxAge
		{
			get
			{
				if (_expires == DateTime.MinValue)
				{
					return 0;
				}
				DateTime dateTime = ((_expires.Kind == DateTimeKind.Local) ? _expires : _expires.ToLocalTime());
				TimeSpan timeSpan = dateTime - DateTime.Now;
				return (timeSpan > TimeSpan.Zero) ? ((int)timeSpan.TotalSeconds) : 0;
			}
		}

		internal int[] Ports
		{
			get
			{
				return _ports;
			}
		}

		public string Comment
		{
			get
			{
				return _comment;
			}
			set
			{
				_comment = value ?? string.Empty;
			}
		}

		public Uri CommentUri
		{
			get
			{
				return _commentUri;
			}
			set
			{
				_commentUri = value;
			}
		}

		public bool Discard
		{
			get
			{
				return _discard;
			}
			set
			{
				_discard = value;
			}
		}

		public string Domain
		{
			get
			{
				return _domain;
			}
			set
			{
				if (value.IsNullOrEmpty())
				{
					_domain = string.Empty;
					ExactDomain = true;
				}
				else
				{
					_domain = value;
					ExactDomain = value[0] != '.';
				}
			}
		}

		public bool Expired
		{
			get
			{
				return _expires != DateTime.MinValue && _expires <= DateTime.Now;
			}
			set
			{
				_expires = ((!value) ? DateTime.MinValue : DateTime.Now);
			}
		}

		public DateTime Expires
		{
			get
			{
				return _expires;
			}
			set
			{
				_expires = value;
			}
		}

		public bool HttpOnly
		{
			get
			{
				return _httpOnly;
			}
			set
			{
				_httpOnly = value;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				string message;
				if (!canSetName(value, out message))
				{
					throw new CookieException(message);
				}
				_name = value;
			}
		}

		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value ?? string.Empty;
			}
		}

		public string Port
		{
			get
			{
				return _port;
			}
			set
			{
				if (value.IsNullOrEmpty())
				{
					_port = string.Empty;
					_ports = new int[0];
					return;
				}
				if (!value.IsEnclosedIn('"'))
				{
					throw new CookieException("The value specified for the Port attribute isn't enclosed in double quotes.");
				}
				string parseError;
				if (!tryCreatePorts(value, out _ports, out parseError))
				{
					throw new CookieException(string.Format("The value specified for the Port attribute contains an invalid value: {0}", parseError));
				}
				_port = value;
			}
		}

		public bool Secure
		{
			get
			{
				return _secure;
			}
			set
			{
				_secure = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return _timestamp;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				string message;
				if (!canSetValue(value, out message))
				{
					throw new CookieException(message);
				}
				_value = ((value.Length <= 0) ? "\"\"" : value);
			}
		}

		public int Version
		{
			get
			{
				return _version;
			}
			set
			{
				if (value < 0 || value > 1)
				{
					throw new ArgumentOutOfRangeException("value", "Not 0 or 1.");
				}
				_version = value;
			}
		}

		public Cookie()
		{
			_comment = string.Empty;
			_domain = string.Empty;
			_expires = DateTime.MinValue;
			_name = string.Empty;
			_path = string.Empty;
			_port = string.Empty;
			_ports = new int[0];
			_timestamp = DateTime.Now;
			_value = string.Empty;
			_version = 0;
		}

		public Cookie(string name, string value)
			: this()
		{
			Name = name;
			Value = value;
		}

		public Cookie(string name, string value, string path)
			: this(name, value)
		{
			Path = path;
		}

		public Cookie(string name, string value, string path, string domain)
			: this(name, value, path)
		{
			Domain = domain;
		}

		static Cookie()
		{
			_reservedCharsForName = new char[7] { ' ', '=', ';', ',', '\n', '\r', '\t' };
			_reservedCharsForValue = new char[2] { ';', ',' };
		}

		private static bool canSetName(string name, out string message)
		{
			if (name.IsNullOrEmpty())
			{
				message = "The value specified for the Name is null or empty.";
				return false;
			}
			if (name[0] == '$' || name.Contains(_reservedCharsForName))
			{
				message = "The value specified for the Name contains an invalid character.";
				return false;
			}
			message = string.Empty;
			return true;
		}

		private static bool canSetValue(string value, out string message)
		{
			if (value == null)
			{
				message = "The value specified for the Value is null.";
				return false;
			}
			if (value.Contains(_reservedCharsForValue) && !value.IsEnclosedIn('"'))
			{
				message = "The value specified for the Value contains an invalid character.";
				return false;
			}
			message = string.Empty;
			return true;
		}

		private static int hash(int i, int j, int k, int l, int m)
		{
			return i ^ ((j << 13) | (j >> 19)) ^ ((k << 26) | (k >> 6)) ^ ((l << 7) | (l >> 25)) ^ ((m << 20) | (m >> 12));
		}

		private string toResponseStringVersion0()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}", _name, _value);
			if (_expires != DateTime.MinValue)
			{
				stringBuilder.AppendFormat("; Expires={0}", _expires.ToUniversalTime().ToString("ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", CultureInfo.CreateSpecificCulture("en-US")));
			}
			if (!_path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Path={0}", _path);
			}
			if (!_domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Domain={0}", _domain);
			}
			if (_secure)
			{
				stringBuilder.Append("; Secure");
			}
			if (_httpOnly)
			{
				stringBuilder.Append("; HttpOnly");
			}
			return stringBuilder.ToString();
		}

		private string toResponseStringVersion1()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}; Version={2}", _name, _value, _version);
			if (_expires != DateTime.MinValue)
			{
				stringBuilder.AppendFormat("; Max-Age={0}", MaxAge);
			}
			if (!_path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Path={0}", _path);
			}
			if (!_domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Domain={0}", _domain);
			}
			if (!_port.IsNullOrEmpty())
			{
				if (_port == "\"\"")
				{
					stringBuilder.Append("; Port");
				}
				else
				{
					stringBuilder.AppendFormat("; Port={0}", _port);
				}
			}
			if (!_comment.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Comment={0}", _comment.UrlEncode());
			}
			if (_commentUri != null)
			{
				string originalString = _commentUri.OriginalString;
				stringBuilder.AppendFormat("; CommentURL={0}", (!originalString.IsToken()) ? originalString.Quote() : originalString);
			}
			if (_discard)
			{
				stringBuilder.Append("; Discard");
			}
			if (_secure)
			{
				stringBuilder.Append("; Secure");
			}
			return stringBuilder.ToString();
		}

		private static bool tryCreatePorts(string value, out int[] result, out string parseError)
		{
			string[] array = value.Trim('"').Split(',');
			int num = array.Length;
			int[] array2 = new int[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = int.MinValue;
				string text = array[i].Trim();
				if (text.Length != 0 && !int.TryParse(text, out array2[i]))
				{
					result = new int[0];
					parseError = text;
					return false;
				}
			}
			result = array2;
			parseError = string.Empty;
			return true;
		}

		internal string ToRequestString(Uri uri)
		{
			if (_name.Length == 0)
			{
				return string.Empty;
			}
			if (_version == 0)
			{
				return string.Format("{0}={1}", _name, _value);
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("$Version={0}; {1}={2}", _version, _name, _value);
			if (!_path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; $Path={0}", _path);
			}
			else if (uri != null)
			{
				stringBuilder.AppendFormat("; $Path={0}", uri.GetAbsolutePath());
			}
			else
			{
				stringBuilder.Append("; $Path=/");
			}
			if ((uri == null || uri.Host != _domain) && !_domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; $Domain={0}", _domain);
			}
			if (!_port.IsNullOrEmpty())
			{
				if (_port == "\"\"")
				{
					stringBuilder.Append("; $Port");
				}
				else
				{
					stringBuilder.AppendFormat("; $Port={0}", _port);
				}
			}
			return stringBuilder.ToString();
		}

		internal string ToResponseString()
		{
			return (_name.Length <= 0) ? string.Empty : ((_version != 0) ? toResponseStringVersion1() : toResponseStringVersion0());
		}

		public override bool Equals(object comparand)
		{
			Cookie cookie = comparand as Cookie;
			return cookie != null && _name.Equals(cookie.Name, StringComparison.InvariantCultureIgnoreCase) && _value.Equals(cookie.Value, StringComparison.InvariantCulture) && _path.Equals(cookie.Path, StringComparison.InvariantCulture) && _domain.Equals(cookie.Domain, StringComparison.InvariantCultureIgnoreCase) && _version == cookie.Version;
		}

		public override int GetHashCode()
		{
			return hash(StringComparer.InvariantCultureIgnoreCase.GetHashCode(_name), _value.GetHashCode(), _path.GetHashCode(), StringComparer.InvariantCultureIgnoreCase.GetHashCode(_domain), _version);
		}

		public override string ToString()
		{
			return ToRequestString(null);
		}
	}
}
