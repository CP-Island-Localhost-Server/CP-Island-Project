using System;

namespace Sfs2X.Exceptions
{
	public class SFSError : Exception
	{
		public SFSError(string message)
			: base(message)
		{
		}
	}
}
