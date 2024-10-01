using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT131R2Curve : AbstractF2mCurve
	{
		private const int SecT131R2_DEFAULT_COORDS = 6;

		protected readonly SecT131R2Point m_infinity;

		public override int FieldSize
		{
			get
			{
				return 131;
			}
		}

		public override ECPoint Infinity
		{
			get
			{
				return m_infinity;
			}
		}

		public override bool IsKoblitz
		{
			get
			{
				return false;
			}
		}

		public virtual int M
		{
			get
			{
				return 131;
			}
		}

		public virtual bool IsTrinomial
		{
			get
			{
				return false;
			}
		}

		public virtual int K1
		{
			get
			{
				return 2;
			}
		}

		public virtual int K2
		{
			get
			{
				return 3;
			}
		}

		public virtual int K3
		{
			get
			{
				return 8;
			}
		}

		public SecT131R2Curve()
			: base(131, 2, 3, 8)
		{
			m_infinity = new SecT131R2Point(this, null, null);
			m_a = FromBigInteger(new BigInteger(1, Hex.Decode("03E5A88919D7CAFCBF415F07C2176573B2")));
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("04B8266A46C55657AC734CE38F018F2192")));
			m_order = new BigInteger(1, Hex.Decode("0400000000000000016954A233049BA98F"));
			m_cofactor = BigInteger.Two;
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT131R2Curve();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			if (coord == 6)
			{
				return true;
			}
			return false;
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new SecT131FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecT131R2Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT131R2Point(this, x, y, zs, withCompression);
		}
	}
}
