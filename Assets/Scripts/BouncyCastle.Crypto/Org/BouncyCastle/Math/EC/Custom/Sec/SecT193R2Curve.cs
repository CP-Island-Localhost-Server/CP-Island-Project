using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT193R2Curve : AbstractF2mCurve
	{
		private const int SecT193R2_DEFAULT_COORDS = 6;

		protected readonly SecT193R2Point m_infinity;

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
				return 193;
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
				return 193;
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
				return 15;
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

		public SecT193R2Curve()
			: base(193, 15, 0, 0)
		{
			m_infinity = new SecT193R2Point(this, null, null);
			m_a = FromBigInteger(new BigInteger(1, Hex.Decode("0163F35A5137C2CE3EA6ED8667190B0BC43ECD69977702709B")));
			m_b = FromBigInteger(new BigInteger(1, Hex.Decode("00C9BB9E8927D4D64C377E2AB2856A5B16E3EFB7F61D4316AE")));
			m_order = new BigInteger(1, Hex.Decode("010000000000000000000000015AAB561B005413CCD4EE99D5"));
			m_cofactor = BigInteger.Two;
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new SecT193R2Curve();
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
			return new SecT193FieldElement(x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new SecT193R2Point(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new SecT193R2Point(this, x, y, zs, withCompression);
		}
	}
}
