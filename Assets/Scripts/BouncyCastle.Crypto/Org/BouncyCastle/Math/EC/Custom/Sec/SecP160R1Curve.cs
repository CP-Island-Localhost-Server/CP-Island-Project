using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP160R1Curve : AbstractFpCurve
	{
		private const int SecP160R1_DEFAULT_COORDS = 2;

		public static readonly BigInteger q = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7FFFFFFF"));

		protected readonly SecP160R1Point m_infinity;

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

		public SecP160R1Curve()
			: base(q)
		{
			m_infinity = new SecP160R1Point(this, null, null);
			m_a = FromBigInteger(new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7FFFFFFC")));
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("1C97BEFC54BD7A8B65ACF89F81D4D4ADC565FA45")));
			m_order = new BigInteger(1, Hex.Decode("0100000000000000000001F4C8F927AED3CA752257"));
			m_cofactor = BigInteger.One;
			m_coord = 2;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecP160R1Curve();
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
			return new SecP160R1FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecP160R1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecP160R1Point(this, x, y, zs, withCompression);
		}
	}
}
