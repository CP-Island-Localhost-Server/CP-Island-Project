namespace Sfs2X.Requests
{
	public class ManualDisconnectionRequest : BaseRequest
	{
		public ManualDisconnectionRequest()
			: base(RequestType.ManualDisconnection)
		{
		}

		public override void Validate(SmartFox sfs)
		{
		}

		public override void Execute(SmartFox sfs)
		{
		}
	}
}
