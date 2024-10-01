using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class DerBmpString : DerStringBase
	{
		private readonly string str;

		public static DerBmpString GetInstance(object obj)
		{
			if (obj == null || obj is DerBmpString)
			{
				return (DerBmpString)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
		}

		public static DerBmpString GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerBmpString)
			{
				return GetInstance(@object);
			}
			return new DerBmpString(Asn1OctetString.GetInstance(@object).GetOctets());
		}

		public DerBmpString(byte[] str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			char[] array = new char[str.Length / 2];
			for (int i = 0; i != array.Length; i++)
			{
				array[i] = (char)((uint)(str[2 * i] << 8) | (str[2 * i + 1] & 0xFFu));
			}
			this.str = new string(array);
		}

		public DerBmpString(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.str = str;
		}

		public override string GetString()
		{
			return str;
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerBmpString derBmpString = asn1Object as DerBmpString;
			if (derBmpString == null)
			{
				return false;
			}
			return str.Equals(derBmpString.str);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			char[] array = str.ToCharArray();
			byte[] array2 = new byte[array.Length * 2];
			for (int i = 0; i != array.Length; i++)
			{
				array2[2 * i] = (byte)((int)array[i] >> 8);
				array2[2 * i + 1] = (byte)array[i];
			}
			derOut.WriteEncoded(30, array2);
		}
	}
}
