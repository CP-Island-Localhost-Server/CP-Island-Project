using System;
using System.Collections.Generic;
using System.Net;

namespace Disney.Manimal.Http
{
	public interface IHttpResponse
	{
		string ContentType
		{
			get;
			set;
		}

		long ContentLength
		{
			get;
			set;
		}

		string ContentEncoding
		{
			get;
			set;
		}

		string Content
		{
			get;
		}

		HttpStatusCode StatusCode
		{
			get;
			set;
		}

		string StatusDescription
		{
			get;
			set;
		}

		byte[] RawBytes
		{
			get;
			set;
		}

		Uri ResponseUri
		{
			get;
			set;
		}

		string Server
		{
			get;
			set;
		}

		IList<HttpHeader> Headers
		{
			get;
		}

		IList<HttpCookie> Cookies
		{
			get;
		}

		ResponseStatus ResponseStatus
		{
			get;
			set;
		}

		string ErrorMessage
		{
			get;
			set;
		}

		Exception ErrorException
		{
			get;
			set;
		}
	}
}
