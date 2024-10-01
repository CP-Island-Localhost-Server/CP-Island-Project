using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP192R1Curve : AbstractFpCurve
	{
		private const int SecP192R1_DEFAULT_COORDS = 2;

		public static readonly BigInteger q = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFF"));

		protected readonly SecP192R1Point m_infinity;

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

		public SecP192R1Curve()
			: base(q)
		{
			m_infinity = new SecP192R1Point(this, null, null);
			m_a = FromBigInteger(new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFC")));
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("64210519E59C80E70FA7E9AB72243049FEB8DEECC146B9B1")));
			m_order = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFF99DEF836146BC9B1B4D22831"));
			m_cofactor = BigInteger.One;
			m_coord = 2;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecP192R1Curve();
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
			return new SecP192R1FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecP192R1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecP192R1Point(this, x, y, zs, withCompression);
		}
	}
}
