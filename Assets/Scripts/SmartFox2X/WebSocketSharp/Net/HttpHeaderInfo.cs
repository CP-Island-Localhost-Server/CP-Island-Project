namespace WebSocketSharp.Net
{
	internal class HttpHeaderInfo
	{
		private string _name;

		private HttpHeaderType _type;

		internal bool IsMultiValueInRequest
		{
			get
			{
				return (_type & HttpHeaderType.MultiValueInRequest) == HttpHeaderType.MultiValueInRequest;
			}
		}

		internal bool IsMultiValueInResponse
		{
			get
			{
				return (_type & HttpHeaderType.MultiValueInResponse) == HttpHeaderType.MultiValueInResponse;
			}
		}

		public bool IsRequest
		{
			get
			{
				return (_type & HttpHeaderType.Request) == HttpHeaderType.Request;
			}
		}

		public bool IsResponse
		{
			get
			{
				return (_type & HttpHeaderType.Response) == HttpHeaderType.Response;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public HttpHeaderType Type
		{
			get
			{
				return _type;
			}
		}

		internal HttpHeaderInfo(string name, HttpHeaderType type)
		{
			_name = name;
			_type = type;
		}

		public bool IsMultiValue(bool response)
		{
			return ((_type & HttpHeaderType.MultiValue) == HttpHeaderType.MultiValue) ? ((!response) ? IsRequest : IsResponse) : ((!response) ? IsMultiValueInRequest : IsMultiValueInResponse);
		}

		public bool IsRestricted(bool response)
		{
			return (_type & HttpHeaderType.Restricted) == HttpHeaderType.Restricted && ((!response) ? IsRequest : IsResponse);
		}
	}
}
