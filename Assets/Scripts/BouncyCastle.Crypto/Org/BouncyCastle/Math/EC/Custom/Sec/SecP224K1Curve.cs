using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP224K1Curve : AbstractFpCurve
	{
		private const int SECP224K1_DEFAULT_COORDS = 2;

		public static readonly BigInteger q = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFE56D"));

		protected readonly SecP224K1Point m_infinity;

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

		public SecP224K1Curve()
			: base(q)
		{
			m_infinity = new SecP224K1Point(this, null, null);
			m_a = FromBigInteger(BigInteger.Zero);
			m_b = FromBigInteger(BigInteger.ValueOf(5L));
			m_order = new BigInteger(1, Hex.Decode("010000000000000000000000000001DCE8D2EC6184CAF0A971769FB1F7"));
			m_cofactor = BigInteger.One;
			m_coord = 2;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecP224K1Curve();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			if (coord == 2)
			{
				return true;
			}
			return false;
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new SecP224K1FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecP224K1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecP224K1Point(this, x, y, zs, withCompression);
		}
	}
}
