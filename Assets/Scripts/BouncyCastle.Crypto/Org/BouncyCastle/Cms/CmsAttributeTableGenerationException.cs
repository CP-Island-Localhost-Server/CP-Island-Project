using System;

namespace Org.BouncyCastle.Cms
{
	[Serializable]
	public class CmsAttributeTableGenerationException : CmsException
	{
		public CmsAttributeTableGenerationException()
		{
		}

		public CmsAttributeTableGenerationException(string name)
			: base(name)
		{
		}

		public CmsAttributeTableGenerationException(string name, Exception e)
			: base(name, e)
		{
		}
	}
}
