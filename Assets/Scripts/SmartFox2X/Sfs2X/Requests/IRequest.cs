using Sfs2X.Bitswarm;

namespace Sfs2X.Requests
{
	public interface IRequest
	{
		int TargetController { get; set; }

		bool IsEncrypted { get; set; }

		IMessage Message { get; }

		void Validate(SmartFox sfs);

		void Execute(SmartFox sfs);
	}
}
