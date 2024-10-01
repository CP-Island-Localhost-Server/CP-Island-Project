using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP160R2Curve : AbstractFpCurve
	{
		private const int SecP160R2_DEFAULT_COORDS = 2;

		public static readonly BigInteger q = new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFAC73"));

		protected readonly SecP160R2Point m_infinity;

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

		public SecP160R2Curve()
			: base(q)
		{
			m_infinity = new SecP160R2Point(this, null, null);
			m_a = FromBigInteger(new BigInteger(1, Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFAC70")));
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("B4E134D3FB59EB8BAB57274904664D5AF50388BA")));
			m_order = new BigInteger(1, Hex.Decode("0100000000000000000000351EE786A818F3A1A16B"));
			m_cofactor = BigInteger.One;
			m_coord = 2;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecP160R2Curve();
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
			return new SecP160R2FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecP160R2Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecP160R2Point(this, x, y, zs, withCompression);
		}
	}
}
