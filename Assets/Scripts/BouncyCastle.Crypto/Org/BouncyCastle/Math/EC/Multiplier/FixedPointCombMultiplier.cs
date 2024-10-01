using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class FixedPointCombMultiplier : AbstractECMultiplier
	{
		protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
		{
			ECCurve curve = p.Curve;
			int combSize = FixedPointUtilities.GetCombSize(curve);
			if (k.BitLength > combSize)
			{
				throw new InvalidOperationException("fixed-point comb doesn't support scalars larger than the curve order");
			}
			int widthForCombSize = GetWidthForCombSize(combSize);
			FixedPointPreCompInfo fixedPointPreCompInfo = FixedPointUtilities.Precompute(p, widthForCombSize);
			ECPoint[] preComp = fixedPointPreCompInfo.PreComp;
			int width = fixedPointPreCompInfo.Width;
			int num = (combSize + width - 1) / width;
			ECPoint eCPoint = curve.Infinity;
			int num2 = num * width - 1;
			for (int i = 0; i < num; i++)
			{
				int num3 = 0;
				for (int num4 = num2 - i; num4 >= 0; num4 -= num)
				{
					num3 <<= 1;
					if (k.TestBit(num4))
					{
						num3 |= 1;
					}
				}
				eCPoint = eCPoint.TwicePlus(preComp[num3]);
			}
			return eCPoint;
		}

		protected virtual int GetWidthForCombSize(int combSize)
		{
			if (combSize <= 257)
			{
				return 5;
			}
			return 6;
		}
	}
}
