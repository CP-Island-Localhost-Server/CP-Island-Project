using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT131R1Curve : AbstractF2mCurve
	{
		private const int SecT131R1_DEFAULT_COORDS = 6;

		protected readonly SecT131R1Point m_infinity;

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
				return 131;
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

		public SecT131R1Curve()
			: base(131, 2, 3, 8)
		{
			m_infinity = new SecT131R1Point(this, null, null);
			m_a = FromBigInteger(new BigInteger(1, Hex.Decode("07A11B09A76B562144418FF3FF8C2570B8")));
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("0217C05610884B63B9C6C7291678F9D341")));
			m_order = new BigInteger(1, Hex.Decode("0400000000000000023123953A9464B54D"));
			m_cofactor = BigInteger.Two;
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT131R1Curve();
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
			return new SecT131R1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT131R1Point(this, x, y, zs, withCompression);
		}
	}
}
