using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class DerEnumerated : Asn1Object
	{
		private readonly byte[] bytes;

		private static readonly DerEnumerated[] cache = new DerEnumerated[12];

		public BigInteger Value
		{
			get
			{
				return new BigInteger(bytes);
			}
		}

		public static DerEnumerated GetInstance(object obj)
		{
			if (obj == null || obj is DerEnumerated)
			{
				return (DerEnumerated)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
		}

		public static DerEnumerated GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerEnumerated)
			{
				return GetInstance(@object);
			}
			return FromOctetString(((Asn1OctetString)@object).GetOctets());
		}

		public DerEnumerated(int val)
		{
			bytes = BigInteger.ValueOf(val).ToByteArray();
		}

		public DerEnumerated(BigInteger val)
		{
			bytes = val.ToByteArray();
		}

		public DerEnumerated(byte[] bytes)
		{
			this.bytes = bytes;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(10, bytes);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerEnumerated derEnumerated = asn1Object as DerEnumerated;
			if (derEnumerated == null)
			{
				return false;
			}
			return Arrays.AreEqual(bytes, derEnumerated.bytes);
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(bytes);
		}

		internal static DerEnumerated FromOctetString(byte[] enc)
		{
			if (enc.Length == 0)
			{
				throw new ArgumentException("ENUMERATED has zero length", "enc");
			}
			if (enc.Length == 1)
			{
				int num = enc[0];
				if (num < cache.Length)
				{
					DerEnumerated derEnumerated = cache[num];
					if (derEnumerated != null)
					{
						return derEnumerated;
					}
					return cache[num] = new DerEnumerated(Arrays.Clone(enc));
				}
			}
			return new DerEnumerated(Arrays.Clone(enc));
		}
	}
}
