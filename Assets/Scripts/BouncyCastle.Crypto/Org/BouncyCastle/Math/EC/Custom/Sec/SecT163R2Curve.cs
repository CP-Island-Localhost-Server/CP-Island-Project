using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT163R2Curve : AbstractF2mCurve
	{
		private const int SecT163R2_DEFAULT_COORDS = 6;

		protected readonly SecT163R2Point m_infinity;

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
				return 163;
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
				return 163;
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
				return 3;
			}
		}

		public virtual int K2
		{
			get
			{
				return 6;
			}
		}

		public virtual int K3
		{
			get
			{
				return 7;
			}
		}

		public SecT163R2Curve()
			: base(163, 3, 6, 7)
		{
			m_infinity = new SecT163R2Point(this, null, null);
			m_a = FromBigInteger(BigInteger.One);
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("020A601907B8C953CA1481EB10512F78744A3205FD")));
			m_order = new BigInteger(1, Hex.Decode("040000000000000000000292FE77E70C12A4234C33"));
			m_cofactor = BigInteger.Two;
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT163R2Curve();
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
			return new SecT163FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecT163R2Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT163R2Point(this, x, y, zs, withCompression);
		}
	}
}
