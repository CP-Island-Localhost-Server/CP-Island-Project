using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT113R2Curve : AbstractF2mCurve
	{
		private const int SecT113R2_DEFAULT_COORDS = 6;

		protected readonly SecT113R2Point m_infinity;

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
				return 113;
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
				return 113;
			}
		}

		public virtual bool IsTrinomial
		{
			get
			{
				return true;
			}
		}

		public virtual int K1
		{
			get
			{
				return 9;
			}
		}

		public virtual int K2
		{
			get
			{
				return 0;
			}
		}

		public virtual int K3
		{
			get
			{
				return 0;
			}
		}

		public SecT113R2Curve()
			: base(113, 9, 0, 0)
		{
			m_infinity = new SecT113R2Point(this, null, null);
			m_a = FromBigInteger(new BigInteger(1, Hex.Decode("00689918DBEC7E5A0DD6DFC0AA55C7")));
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("0095E9A9EC9B297BD4BF36E059184F")));
			m_order = new BigInteger(1, Hex.Decode("010000000000000108789B2496AF93"));
			m_cofactor = BigInteger.Two;
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT113R2Curve();
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
			return new SecT113FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecT113R2Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT113R2Point(this, x, y, zs, withCompression);
		}
	}
}
