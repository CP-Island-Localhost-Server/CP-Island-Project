using System;
using Org.BouncyCastle.Math.EC.Endo;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class GlvMultiplier : AbstractECMultiplier
	{
		protected readonly ECCurve curve;

		protected readonly GlvEndomorphism glvEndomorphism;

		public GlvMultiplier(ECCurve curve, GlvEndomorphism glvEndomorphism)
		{
			if (curve == null || curve.Order == null)
			{
				throw new ArgumentException("Need curve with known group order", "curve");
			}
			this.curve = curve;
			this.glvEndomorphism = glvEndomorphism;
		}

		protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
		{
			if (!curve.Equals(p.Curve))
			{
				throw new InvalidOperationException();
			}
			BigInteger order = p.Curve.Order;
			BigInteger[] array = glvEndomorphism.DecomposeScalar(k.Mod(order));
			BigInteger k2 = array[0];
			BigInteger l = array[1];
			ECPointMap pointMap = glvEndomorphism.PointMap;
			if (glvEndomorphism.HasEfficientPointMap)
			{
				return ECAlgorithms.ImplShamirsTrickWNaf(p, k2, pointMap, l);
			}
			return ECAlgorithms.ImplShamirsTrickWNaf(p, k2, pointMap.Map(p), l);
		}
	}
}
