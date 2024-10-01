using System;

namespace Sfs2X.Exceptions
{
	public class SFSCodecError : Exception
	{
		public SFSCodecError(string message)
			: base(message)
		{
		}
	}
}
