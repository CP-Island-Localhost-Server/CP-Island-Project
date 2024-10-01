namespace Org.BouncyCastle.Math.EC.Endo
{
	public class GlvTypeBEndomorphism : GlvEndomorphism, ECEndomorphism
	{
		protected readonly ECCurve m_curve;

		protected readonly GlvTypeBParameters m_parameters;

		protected readonly ECPointMap m_pointMap;

		public virtual ECPointMap PointMap
		{
			get
			{
				return m_pointMap;
			}
		}

		public virtual bool HasEfficientPointMap
		{
			get
			{
				return true;
			}
		}

		public GlvTypeBEndomorphism(ECCurve curve, GlvTypeBParameters parameters)
		{
			m_curve = curve;
			m_parameters = parameters;
			m_pointMap = new ScaleXPointMap(curve.FromBigInteger(parameters.Beta));
		}

		public virtual BigInteger[] DecomposeScalar(BigInteger k)
		{
			int bits = m_parameters.Bits;
			BigInteger bigInteger = CalculateB(k, m_parameters.G1, bits);
			BigInteger bigInteger2 = CalculateB(k, m_parameters.G2, bits);
			BigInteger[] v = m_parameters.V1;
			BigInteger[] v2 = m_parameters.V2;
			BigInteger bigInteger3 = k.Subtract(bigInteger.Multiply(v[0]).Add(bigInteger2.Multiply(v2[0])));
			BigInteger bigInteger4 = bigInteger.Multiply(v[1]).Add(bigInteger2.Multiply(v2[1])).Negate();
			return new BigInteger[2] { bigInteger3, bigInteger4 };
		}

		protected virtual BigInteger CalculateB(BigInteger k, BigInteger g, int t)
		{
			bool flag = g.SignValue < 0;
			BigInteger bigInteger = k.Multiply(g.Abs());
			bool flag2 = bigInteger.TestBit(t - 1);
			bigInteger = bigInteger.ShiftRight(t);
			if (flag2)
			{
				bigInteger = bigInteger.Add(BigInteger.One);
			}
			if (!flag)
			{
				return bigInteger;
			}
			return bigInteger.Negate();
		}
	}
}
