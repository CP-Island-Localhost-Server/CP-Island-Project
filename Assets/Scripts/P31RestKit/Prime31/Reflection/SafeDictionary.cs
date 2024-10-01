using System.Collections.Generic;

namespace Prime31.Reflection
{
	public class SafeDictionary<TKey, TValue>
	{
		private readonly object _padlock = new object();

		private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

		public TValue this[TKey key]
		{
			get
			{
				return _dictionary[key];
			}
		}

		public bool tryGetValue(TKey key, out TValue value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
		}

		public void add(TKey key, TValue value)
		{
			lock (_padlock)
			{
				if (!_dictionary.ContainsKey(key))
				{
					_dictionary.Add(key, value);
				}
			}
		}
	}
}
