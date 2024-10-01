using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	public class ExtensionRequest : BaseRequest
	{
		public static readonly string KEY_CMD = "c";

		public static readonly string KEY_PARAMS = "p";

		public static readonly string KEY_ROOM = "r";

		private string extCmd;

		private ISFSObject parameters;

		private Room room;

		private bool useUDP;

		public bool UseUDP
		{
			get
			{
				return useUDP;
			}
		}

		public ExtensionRequest(string extCmd, ISFSObject parameters, Room room, bool useUDP)
			: base(RequestType.CallExtension)
		{
			Init(extCmd, parameters, room, useUDP);
		}

		public ExtensionRequest(string extCmd, ISFSObject parameters, Room room)
			: base(RequestType.CallExtension)
		{
			Init(extCmd, parameters, room, false);
		}

		public ExtensionRequest(string extCmd, ISFSObject parameters)
			: base(RequestType.CallExtension)
		{
			Init(extCmd, parameters, null, false);
		}

		private void Init(string extCmd, ISFSObject parameters, Room room, bool useUDP)
		{
			targetController = 1;
			this.extCmd = extCmd;
			this.parameters = parameters;
			this.room = room;
			this.useUDP = useUDP;
			if (parameters == null)
			{
				parameters = new SFSObject();
			}
		}

		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (extCmd == null || extCmd.Length == 0)
			{
				list.Add("Missing extension command");
			}
			if (parameters == null)
			{
				list.Add("Missing extension parameters");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ExtensionCall request error", list);
			}
		}

		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_CMD, extCmd);
			sfso.PutInt(KEY_ROOM, (room != null) ? room.Id : (-1));
			sfso.PutSFSObject(KEY_PARAMS, parameters);
		}
	}
}
