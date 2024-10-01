using System;
using WebSocketSharp.Net;

namespace WebSocketSharp.Server
{
	public class HttpRequestEventArgs : EventArgs
	{
		private HttpListenerRequest _request;

		private HttpListenerResponse _response;

		public HttpListenerRequest Request
		{
			get
			{
				return _request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return _response;
			}
		}

		internal HttpRequestEventArgs(HttpListenerContext context)
		{
			_request = context.Request;
			_response = context.Response;
		}
	}
}
