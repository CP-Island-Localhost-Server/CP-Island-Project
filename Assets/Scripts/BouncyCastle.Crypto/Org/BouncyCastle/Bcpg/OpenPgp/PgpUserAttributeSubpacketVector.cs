using Org.BouncyCastle.Bcpg.Attr;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	public class PgpUserAttributeSubpacketVector
	{
		private readonly UserAttributeSubpacket[] packets;

		internal PgpUserAttributeSubpacketVector(UserAttributeSubpacket[] packets)
		{
			this.packets = packets;
		}

		public UserAttributeSubpacket GetSubpacket(UserAttributeSubpacketTag type)
		{
			for (int i = 0; i != packets.Length; i++)
			{
				if (packets[i].SubpacketType == type)
				{
					return packets[i];
				}
			}
			return null;
		}

		public ImageAttrib GetImageAttribute()
		{
			UserAttributeSubpacket subpacket = GetSubpacket(UserAttributeSubpacketTag.ImageAttribute);
			if (subpacket != null)
			{
				return (ImageAttrib)subpacket;
			}
			return null;
		}

		internal UserAttributeSubpacket[] ToSubpacketArray()
		{
			return packets;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			PgpUserAttributeSubpacketVector pgpUserAttributeSubpacketVector = obj as PgpUserAttributeSubpacketVector;
			if (pgpUserAttributeSubpacketVector == null)
			{
				return false;
			}
			if (pgpUserAttributeSubpacketVector.packets.Length != packets.Length)
			{
				return false;
			}
			for (int i = 0; i != packets.Length; i++)
			{
				if (!pgpUserAttributeSubpacketVector.packets[i].Equals(packets[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			UserAttributeSubpacket[] array = packets;
			foreach (object obj in array)
			{
				num ^= obj.GetHashCode();
			}
			return num;
		}
	}
}
