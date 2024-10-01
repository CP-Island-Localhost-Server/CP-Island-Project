using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT163K1Curve : AbstractF2mCurve
	{
		private const int SecT163K1_DEFAULT_COORDS = 6;

		protected readonly SecT163K1Point m_infinity;

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
				return true;
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

		public SecT163K1Curve()
			: base(163, 3, 6, 7)
		{
			m_infinity = new SecT163K1Point(this, null, null);
			m_a = FromBigInteger(BigInteger.One);
			m_b = m_a;
			m_order = new BigInteger(1, Hex.Decode("04000000000000000000020108A2E0CC0D99F8A5EF"));
			m_cofactor = BigInteger.Two;
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT163K1Curve();
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			if (coord == 6)
			{
				return true;
			}
			return false;
		}

		protected override ECMultiplier CreateDefaultMultiplier()
		{
			return new WTauNafMultiplier();
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new SecT163FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecT163K1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT163K1Point(this, x, y, zs, withCompression);
		}
	}
}
