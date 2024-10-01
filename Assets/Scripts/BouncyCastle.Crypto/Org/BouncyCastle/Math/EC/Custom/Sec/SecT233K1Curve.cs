using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT233K1Curve : AbstractF2mCurve
	{
		private const int SecT233K1_DEFAULT_COORDS = 6;

		protected readonly SecT233K1Point m_infinity;

		public override int FieldSize
		{
			get
			{
				return 233;
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
				return true;
			}
		}

		public virtual int M
		{
			get
			{
				return 233;
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
				return 74;
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

		public SecT233K1Curve()
			: base(233, 74, 0, 0)
		{
			m_infinity = new SecT233K1Point(this, null, null);
			m_a = FromBigInteger(BigInteger.Zero);
			m_b = FromBigInteger(BigInteger.One);
			m_order = new BigInteger(1, Hex.Decode("8000000000000000000000000000069D5BB915BCD46EFB1AD5F173ABDF"));
			m_cofactor = BigInteger.ValueOf(4L);
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT233K1Curve();
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
			return new SecT233FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecT233K1Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT233K1Point(this, x, y, zs, withCompression);
		}
	}
}
