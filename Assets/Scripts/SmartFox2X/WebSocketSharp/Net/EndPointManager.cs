using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace WebSocketSharp.Net
{
	internal sealed class EndPointManager
	{
		private static Dictionary<IPAddress, Dictionary<int, EndPointListener>> _addressToEndpoints;

		private EndPointManager()
		{
		}

		static EndPointManager()
		{
			_addressToEndpoints = new Dictionary<IPAddress, Dictionary<int, EndPointListener>>();
		}

		private static void addPrefix(string uriPrefix, HttpListener listener)
		{
			HttpListenerPrefix httpListenerPrefix = new HttpListenerPrefix(uriPrefix);
			if (httpListenerPrefix.Path.IndexOf('%') != -1)
			{
				throw new HttpListenerException(400, "Invalid path.");
			}
			if (httpListenerPrefix.Path.IndexOf("//", StringComparison.Ordinal) != -1)
			{
				throw new HttpListenerException(400, "Invalid path.");
			}
			EndPointListener endPointListener = getEndPointListener(httpListenerPrefix.Host, httpListenerPrefix.Port, listener, httpListenerPrefix.IsSecure);
			endPointListener.AddPrefix(httpListenerPrefix, listener);
		}

		private static IPAddress convertToAddress(string hostname)
		{
			if (hostname == "*" || hostname == "+")
			{
				return IPAddress.Any;
			}
			IPAddress address;
			if (IPAddress.TryParse(hostname, out address))
			{
				return address;
			}
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
				return (hostEntry == null) ? IPAddress.Any : hostEntry.AddressList[0];
			}
			catch
			{
				return IPAddress.Any;
			}
		}

		private static EndPointListener getEndPointListener(string host, int port, HttpListener listener, bool secure)
		{
			IPAddress iPAddress = convertToAddress(host);
			Dictionary<int, EndPointListener> dictionary = null;
			if (_addressToEndpoints.ContainsKey(iPAddress))
			{
				dictionary = _addressToEndpoints[iPAddress];
			}
			else
			{
				dictionary = new Dictionary<int, EndPointListener>();
				_addressToEndpoints[iPAddress] = dictionary;
			}
			EndPointListener endPointListener = null;
			return dictionary.ContainsKey(port) ? dictionary[port] : (dictionary[port] = new EndPointListener(iPAddress, port, secure, listener.CertificateFolderPath, listener.SslConfiguration, listener.ReuseAddress));
		}

		private static void removePrefix(string uriPrefix, HttpListener listener)
		{
			HttpListenerPrefix httpListenerPrefix = new HttpListenerPrefix(uriPrefix);
			if (httpListenerPrefix.Path.IndexOf('%') == -1 && httpListenerPrefix.Path.IndexOf("//", StringComparison.Ordinal) == -1)
			{
				EndPointListener endPointListener = getEndPointListener(httpListenerPrefix.Host, httpListenerPrefix.Port, listener, httpListenerPrefix.IsSecure);
				endPointListener.RemovePrefix(httpListenerPrefix, listener);
			}
		}

		internal static void RemoveEndPoint(EndPointListener listener)
		{
			lock (((ICollection)_addressToEndpoints).SyncRoot)
			{
				IPAddress address = listener.Address;
				Dictionary<int, EndPointListener> dictionary = _addressToEndpoints[address];
				dictionary.Remove(listener.Port);
				if (dictionary.Count == 0)
				{
					_addressToEndpoints.Remove(address);
				}
				listener.Close();
			}
		}

		public static void AddListener(HttpListener listener)
		{
			List<string> list = new List<string>();
			lock (((ICollection)_addressToEndpoints).SyncRoot)
			{
				try
				{
					foreach (string prefix in listener.Prefixes)
					{
						addPrefix(prefix, listener);
						list.Add(prefix);
					}
				}
				catch
				{
					foreach (string item in list)
					{
						removePrefix(item, listener);
					}
					throw;
				}
			}
		}

		public static void AddPrefix(string uriPrefix, HttpListener listener)
		{
			lock (((ICollection)_addressToEndpoints).SyncRoot)
			{
				addPrefix(uriPrefix, listener);
			}
		}

		public static void RemoveListener(HttpListener listener)
		{
			lock (((ICollection)_addressToEndpoints).SyncRoot)
			{
				foreach (string prefix in listener.Prefixes)
				{
					removePrefix(prefix, listener);
				}
			}
		}

		public static void RemovePrefix(string uriPrefix, HttpListener listener)
		{
			lock (((ICollection)_addressToEndpoints).SyncRoot)
			{
				removePrefix(uriPrefix, listener);
			}
		}
	}
}
