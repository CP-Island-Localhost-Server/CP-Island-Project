using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WebSocketSharp.Net
{
	[Serializable]
	public class CookieCollection : ICollection, IEnumerable
	{
		private List<Cookie> _list;

		private object _sync;

		internal IList<Cookie> List
		{
			get
			{
				return _list;
			}
		}

		internal IEnumerable<Cookie> Sorted
		{
			get
			{
				List<Cookie> list = new List<Cookie>(_list);
				if (list.Count > 1)
				{
					list.Sort(compareCookieWithinSorted);
				}
				return list;
			}
		}

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public Cookie this[int index]
		{
			get
			{
				if (index < 0 || index >= _list.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return _list[index];
			}
		}

		public Cookie this[string name]
		{
			get
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				foreach (Cookie item in Sorted)
				{
					if (item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					{
						return item;
					}
				}
				return null;
			}
		}

		public object SyncRoot
		{
			get
			{
				return _sync ?? (_sync = ((ICollection)_list).SyncRoot);
			}
		}

		public CookieCollection()
		{
			_list = new List<Cookie>();
		}

		private static int compareCookieWithinSort(Cookie x, Cookie y)
		{
			return x.Name.Length + x.Value.Length - (y.Name.Length + y.Value.Length);
		}

		private static int compareCookieWithinSorted(Cookie x, Cookie y)
		{
			int num = 0;
			return ((num = x.Version - y.Version) != 0) ? num : (((num = x.Name.CompareTo(y.Name)) == 0) ? (y.Path.Length - x.Path.Length) : num);
		}

		private static CookieCollection parseRequest(string value)
		{
			CookieCollection cookieCollection = new CookieCollection();
			Cookie cookie = null;
			int num = 0;
			string[] array = splitCookieHeaderValue(value);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.Length == 0)
				{
					continue;
				}
				if (text.StartsWith("$version", StringComparison.InvariantCultureIgnoreCase))
				{
					num = int.Parse(text.GetValue('=', true));
					continue;
				}
				if (text.StartsWith("$path", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Path = text.GetValue('=');
					}
					continue;
				}
				if (text.StartsWith("$domain", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Domain = text.GetValue('=');
					}
					continue;
				}
				if (text.StartsWith("$port", StringComparison.InvariantCultureIgnoreCase))
				{
					string port = ((!text.Equals("$port", StringComparison.InvariantCultureIgnoreCase)) ? text.GetValue('=') : "\"\"");
					if (cookie != null)
					{
						cookie.Port = port;
					}
					continue;
				}
				if (cookie != null)
				{
					cookieCollection.Add(cookie);
				}
				string value2 = string.Empty;
				int num2 = text.IndexOf('=');
				string name;
				if (num2 == -1)
				{
					name = text;
				}
				else if (num2 == text.Length - 1)
				{
					name = text.Substring(0, num2).TrimEnd(' ');
				}
				else
				{
					name = text.Substring(0, num2).TrimEnd(' ');
					value2 = text.Substring(num2 + 1).TrimStart(' ');
				}
				cookie = new Cookie(name, value2);
				if (num != 0)
				{
					cookie.Version = num;
				}
			}
			if (cookie != null)
			{
				cookieCollection.Add(cookie);
			}
			return cookieCollection;
		}

		private static CookieCollection parseResponse(string value)
		{
			CookieCollection cookieCollection = new CookieCollection();
			Cookie cookie = null;
			string[] array = splitCookieHeaderValue(value);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.Length == 0)
				{
					continue;
				}
				if (text.StartsWith("version", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Version = int.Parse(text.GetValue('=', true));
					}
					continue;
				}
				if (text.StartsWith("expires", StringComparison.InvariantCultureIgnoreCase))
				{
					StringBuilder stringBuilder = new StringBuilder(text.GetValue('='), 32);
					if (i < array.Length - 1)
					{
						stringBuilder.AppendFormat(", {0}", array[++i].Trim());
					}
					DateTime result;
					if (!DateTime.TryParseExact(stringBuilder.ToString(), new string[2] { "ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", "r" }, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
					{
						result = DateTime.Now;
					}
					if (cookie != null && cookie.Expires == DateTime.MinValue)
					{
						cookie.Expires = result.ToLocalTime();
					}
					continue;
				}
				if (text.StartsWith("max-age", StringComparison.InvariantCultureIgnoreCase))
				{
					int num = int.Parse(text.GetValue('=', true));
					DateTime expires = DateTime.Now.AddSeconds(num);
					if (cookie != null)
					{
						cookie.Expires = expires;
					}
					continue;
				}
				if (text.StartsWith("path", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Path = text.GetValue('=');
					}
					continue;
				}
				if (text.StartsWith("domain", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Domain = text.GetValue('=');
					}
					continue;
				}
				if (text.StartsWith("port", StringComparison.InvariantCultureIgnoreCase))
				{
					string port = ((!text.Equals("port", StringComparison.InvariantCultureIgnoreCase)) ? text.GetValue('=') : "\"\"");
					if (cookie != null)
					{
						cookie.Port = port;
					}
					continue;
				}
				if (text.StartsWith("comment", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Comment = text.GetValue('=').UrlDecode();
					}
					continue;
				}
				if (text.StartsWith("commenturl", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.CommentUri = text.GetValue('=', true).ToUri();
					}
					continue;
				}
				if (text.StartsWith("discard", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Discard = true;
					}
					continue;
				}
				if (text.StartsWith("secure", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.Secure = true;
					}
					continue;
				}
				if (text.StartsWith("httponly", StringComparison.InvariantCultureIgnoreCase))
				{
					if (cookie != null)
					{
						cookie.HttpOnly = true;
					}
					continue;
				}
				if (cookie != null)
				{
					cookieCollection.Add(cookie);
				}
				string value2 = string.Empty;
				int num2 = text.IndexOf('=');
				string name;
				if (num2 == -1)
				{
					name = text;
				}
				else if (num2 == text.Length - 1)
				{
					name = text.Substring(0, num2).TrimEnd(' ');
				}
				else
				{
					name = text.Substring(0, num2).TrimEnd(' ');
					value2 = text.Substring(num2 + 1).TrimStart(' ');
				}
				cookie = new Cookie(name, value2);
			}
			if (cookie != null)
			{
				cookieCollection.Add(cookie);
			}
			return cookieCollection;
		}

		private int searchCookie(Cookie cookie)
		{
			string name = cookie.Name;
			string path = cookie.Path;
			string domain = cookie.Domain;
			int version = cookie.Version;
			for (int num = _list.Count - 1; num >= 0; num--)
			{
				Cookie cookie2 = _list[num];
				if (cookie2.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && cookie2.Path.Equals(path, StringComparison.InvariantCulture) && cookie2.Domain.Equals(domain, StringComparison.InvariantCultureIgnoreCase) && cookie2.Version == version)
				{
					return num;
				}
			}
			return -1;
		}

		private static string[] splitCookieHeaderValue(string value)
		{
			return new List<string>(value.SplitHeaderValue(',', ';')).ToArray();
		}

		internal static CookieCollection Parse(string value, bool response)
		{
			return (!response) ? parseRequest(value) : parseResponse(value);
		}

		internal void SetOrRemove(Cookie cookie)
		{
			int num = searchCookie(cookie);
			if (num == -1)
			{
				if (!cookie.Expired)
				{
					_list.Add(cookie);
				}
			}
			else if (!cookie.Expired)
			{
				_list[num] = cookie;
			}
			else
			{
				_list.RemoveAt(num);
			}
		}

		internal void SetOrRemove(CookieCollection cookies)
		{
			foreach (Cookie cookie in cookies)
			{
				SetOrRemove(cookie);
			}
		}

		internal void Sort()
		{
			if (_list.Count > 1)
			{
				_list.Sort(compareCookieWithinSort);
			}
		}

		public void Add(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			int num = searchCookie(cookie);
			if (num == -1)
			{
				_list.Add(cookie);
			}
			else
			{
				_list[num] = cookie;
			}
		}

		public void Add(CookieCollection cookies)
		{
			if (cookies == null)
			{
				throw new ArgumentNullException("cookies");
			}
			foreach (Cookie cookie in cookies)
			{
				Add(cookie);
			}
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Less than zero.");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("Multidimensional.", "array");
			}
			if (array.Length - index < _list.Count)
			{
				throw new ArgumentException("The number of elements in this collection is greater than the available space of the destination array.");
			}
			if (!array.GetType().GetElementType().IsAssignableFrom(typeof(Cookie)))
			{
				throw new InvalidCastException("The elements in this collection cannot be cast automatically to the type of the destination array.");
			}
			((ICollection)_list).CopyTo(array, index);
		}

		public void CopyTo(Cookie[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Less than zero.");
			}
			if (array.Length - index < _list.Count)
			{
				throw new ArgumentException("The number of elements in this collection is greater than the available space of the destination array.");
			}
			_list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}
	}
}
