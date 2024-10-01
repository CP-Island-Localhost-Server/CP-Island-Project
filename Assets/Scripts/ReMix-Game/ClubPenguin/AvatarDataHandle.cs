using Disney.Kelowna.Common.DataModel;
using UnityEngine;

namespace ClubPenguin
{
	public class AvatarDataHandle : MonoBehaviour
	{
		[SerializeField]
		private GameObject Entity;

		public bool IsLocalPlayer
		{
			get;
			private set;
		}

		public DataEntityHandle Handle
		{
			get;
			private set;
		}

		public static bool TryGetPlayerHandle(GameObject owner, out DataEntityHandle handle)
		{
			if (!owner.IsDestroyed())
			{
				AvatarDataHandle component = owner.GetComponent<AvatarDataHandle>();
				if (component != null)
				{
					handle = component.Handle;
					return !handle.IsNull;
				}
			}
			handle = DataEntityHandle.NullHandle;
			return false;
		}

		public void SetHandle(DataEntityHandle handle, bool isLocalPlayer = false)
		{
			Handle = handle;
			IsLocalPlayer = isLocalPlayer;
		}
	}
}
