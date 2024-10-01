using Sfs2X.Bitswarm;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests
{
	public class BaseRequest : IRequest
	{
		public static readonly string KEY_ERROR_CODE = "ec";

		public static readonly string KEY_ERROR_PARAMS = "ep";

		protected ISFSObject sfso;

		private int id;

		protected int targetController;

		private bool isEncrypted;

		public int TargetController
		{
			get
			{
				return targetController;
			}
			set
			{
				targetController = value;
			}
		}

		public bool IsEncrypted
		{
			get
			{
				return isEncrypted;
			}
			set
			{
				isEncrypted = value;
			}
		}

		public IMessage Message
		{
			get
			{
				IMessage message = new Message();
				message.Id = id;
				message.IsEncrypted = isEncrypted;
				message.TargetController = targetController;
				message.Content = sfso;
				if (this is ExtensionRequest)
				{
					message.IsUDP = (this as ExtensionRequest).UseUDP;
				}
				return message;
			}
		}

		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public RequestType Type
		{
			get
			{
				return (RequestType)id;
			}
			set
			{
				id = (int)value;
			}
		}

		public BaseRequest(RequestType tp)
		{
			sfso = SFSObject.NewInstance();
			targetController = 0;
			isEncrypted = false;
			id = (int)tp;
		}

		public BaseRequest(int id)
		{
			sfso = SFSObject.NewInstance();
			targetController = 0;
			isEncrypted = false;
			this.id = id;
		}

		public virtual void Validate(SmartFox sfs)
		{
		}

		public virtual void Execute(SmartFox sfs)
		{
		}
	}
}
