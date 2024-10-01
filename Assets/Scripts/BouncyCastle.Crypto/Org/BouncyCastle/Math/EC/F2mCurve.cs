using System;
using Org.BouncyCastle.Math.EC.Multiplier;

namespace Org.BouncyCastle.Math.EC
{
	public class F2mCurve : AbstractF2mCurve
	{
		private const int F2M_DEFAULT_COORDS = 6;

		private readonly int m;

		private readonly int k1;

		private readonly int k2;

		private readonly int k3;

		protected readonly F2mPoint m_infinity;

		public override int FieldSize
		{
			get
			{
				return m;
			}
		}

		public override ECPoint Infinity
		{
			get
			{
				return m_infinity;
			}
		}

		public int M
		{
			get
			{
				return m;
			}
		}

		public int K1
		{
			get
			{
				return k1;
			}
		}

		public int K2
		{
			get
			{
				return k2;
			}
		}

		public int K3
		{
			get
			{
				return k3;
			}
		}

		[Obsolete("Use 'Order' property instead")]
		public BigInteger N
		{
			get
			{
				return m_order;
			}
		}

		[Obsolete("Use 'Cofactor' property instead")]
		public BigInteger H
		{
			get
			{
				return m_cofactor;
			}
		}

		public F2mCurve(int m, int k, BigInteger a, BigInteger b)
			: this(m, k, 0, 0, a, b, null, null)
		{
		}

		public F2mCurve(int m, int k, BigInteger a, BigInteger b, BigInteger order, BigInteger cofactor)
			: this(m, k, 0, 0, a, b, order, cofactor)
		{
		}

		public F2mCurve(int m, int k1, int k2, int k3, BigInteger a, BigInteger b)
			: this(m, k1, k2, k3, a, b, null, null)
		{
		}

		public F2mCurve(int m, int k1, int k2, int k3, BigInteger a, BigInteger b, BigInteger order, BigInteger cofactor)
			: base(m, k1, k2, k3)
		{
			this.m = m;
			this.k1 = k1;
			this.k2 = k2;
			this.k3 = k3;
			m_order = order;
			m_cofactor = cofactor;
			m_infinity = new F2mPoint(this, null, null);
			if (k1 == 0)
			{
				throw new ArgumentException("k1 must be > 0");
			}
			if (k2 == 0)
			{
				if (k3 != 0)
				{
					throw new ArgumentException("k3 must be 0 if k2 == 0");
				}
			}
			else
			{
				if (k2 <= k1)
				{
					throw new ArgumentException("k2 must be > k1");
				}
				if (k3 <= k2)
				{
					throw new ArgumentException("k3 must be > k2");
				}
			}
			m_a = FromBigInteger(a);
			m_b = FromBigInteger(b);
			m_coord = 6;
		}

		protected F2mCurve(int m, int k1, int k2, int k3, ECFieldElement a, ECFieldElement b, BigInteger order, BigInteger cofactor)
			: base(m, k1, k2, k3)
		{
			this.m = m;
			this.k1 = k1;
			this.k2 = k2;
			this.k3 = k3;
			m_order = order;
			m_cofactor = cofactor;
			m_infinity = new F2mPoint(this, null, null);
			m_a = a;
			m_b = b;
			m_coord = 6;
		}

		protected override ECCurve CloneCurve()
		{
			return new F2mCurve(m, k1, k2, k3, m_a, m_b, m_order, m_cofactor);
		}

		public override bool SupportsCoordinateSystem(int coord)
		{
			switch (coord)
			{
			case 0:
			case 1:
			case 6:
				return true;
			default:
				return false;
			}
		}

		protected override ECMultiplier CreateDefaultMultiplier()
		{
			if (IsKoblitz)
			{
				return new WTauNafMultiplier();
			}
			return base.CreateDefaultMultiplier();
		}

		public override ECFieldElement FromBigInteger(BigInteger x)
		{
			return new F2mFieldElement(m, k1, k2, k3, x);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
		{
			return new F2mPoint(this, x, y, withCompression);
		}

		protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
		{
			return new F2mPoint(this, x, y, zs, withCompression);
		}

		public bool IsTrinomial()
		{
			if (k2 == 0)
			{
				return k3 == 0;
			}
			return false;
		}
	}
}
