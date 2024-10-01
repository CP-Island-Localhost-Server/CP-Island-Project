using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class DerInteger : Asn1Object
	{
		private readonly byte[] bytes;

		public BigInteger Value
		{
			get
			{
				return new BigInteger(bytes);
			}
		}

		public BigInteger PositiveValue
		{
			get
			{
				return new BigInteger(1, bytes);
			}
		}

		public static DerInteger GetInstance(object obj)
		{
			if (obj == null || obj is DerInteger)
			{
				return (DerInteger)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj));
		}

		public static DerInteger GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerInteger)
			{
				return GetInstance(@object);
			}
			return new DerInteger(Asn1OctetString.GetInstance(@object).GetOctets());
		}

		public DerInteger(int value)
		{
			bytes = BigInteger.ValueOf(value).ToByteArray();
		}

		public DerInteger(BigInteger value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			bytes = value.ToByteArray();
		}

		public DerInteger(byte[] bytes)
		{
			this.bytes = bytes;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(2, bytes);
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(bytes);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerInteger derInteger = asn1Object as DerInteger;
			if (derInteger == null)
			{
				return false;
			}
			return Arrays.AreEqual(bytes, derInteger.bytes);
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
