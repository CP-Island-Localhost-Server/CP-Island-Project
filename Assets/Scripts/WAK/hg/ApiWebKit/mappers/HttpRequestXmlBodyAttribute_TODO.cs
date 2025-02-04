using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.core.http;
using System;
using System.Reflection;

namespace hg.ApiWebKit.mappers
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class HttpRequestXmlBodyAttribute_TODO : HttpMappedValueAttribute
	{
		public HttpRequestXmlBodyAttribute_TODO()
			: base(MappingDirection.REQUEST, null)
		{
		}

		public override void OnRequestResolveModel(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
			model.SetStringBody((value != null) ? ((string)value) : "<xml></xml>");
		}
	}
}
