using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Djb
{
	internal class Curve25519 : AbstractFpCurve
	{
		private const int Curve25519_DEFAULT_COORDS = 4;

		public static readonly BigInteger q = Nat256.ToBigInteger(Curve25519Field.P);

		protected readonly Curve25519Point m_infinity;

		public virtual BigInteger Q
		{
			get
			{
				return q;
			}
		}

		public override ECPoint Infinity
		{
			get
			{
				return m_infinity;
			}
		}

		public override int FieldSize
		{
			get
			{
				return q.BitLength;
			}
		}

		public Curve25519()
			: base(q)
		{
			m_infinity = new Curve25519Point(this, null, null);
			m_a = FromBigInteger(new BigInteger(1, Hex.Decode("2AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA984914A144")));
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("7B425ED097B425ED097B425ED097B425ED097B425ED097B4260B5E9C7710C864")));
			m_order = new BigInteger(1, Hex.Decode("1000000000000000000000000000000014DEF9DEA2F79CD65812631A5CF5D3ED"));
			m_cofactor = BigInteger.ValueOf(8L);
			m_coord = 4;
		}

		protected override ECCurve CloneCurve()
		{
			return new Curve25519();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			if (coord == 4)
			{
				return true;
			}
			return false;
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new Curve25519FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new Curve25519Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new Curve25519Point(this, x, y, zs, withCompression);
		}
	}
}
