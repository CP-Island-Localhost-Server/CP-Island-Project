using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class WNafL2RMultiplier : AbstractECMultiplier
	{
		protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
		{
			int num = System.Math.Max(2, System.Math.Min(16, GetWindowSize(k.BitLength)));
			WNafPreCompInfo wNafPreCompInfo = WNafUtilities.Precompute(p, num, true);
			ECPoint[] preComp = wNafPreCompInfo.PreComp;
			ECPoint[] preCompNeg = wNafPreCompInfo.PreCompNeg;
			int[] array = WNafUtilities.GenerateCompactWindowNaf(num, k);
			ECPoint eCPoint = p.Curve.Infinity;
			int num2 = array.Length;
			if (num2 > 1)
			{
				int num3 = array[--num2];
				int num4 = num3 >> 16;
				int num5 = num3 & 0xFFFF;
				int num6 = System.Math.Abs(num4);
				ECPoint[] array2 = ((num4 < 0) ? preCompNeg : preComp);
				if (num6 << 2 < 1 << num)
				{
					int num7 = LongArray.BitLengths[num6];
					int num8 = num - num7;
					int num9 = num6 ^ (1 << num7 - 1);
					int num10 = (1 << num - 1) - 1;
					int num11 = (num9 << num8) + 1;
					eCPoint = array2[num10 >> 1].Add(array2[num11 >> 1]);
					num5 -= num8;
				}
				else
				{
					eCPoint = array2[num6 >> 1];
				}
				eCPoint = eCPoint.TimesPow2(num5);
			}
			while (num2 > 0)
			{
				int num12 = array[--num2];
				int num13 = num12 >> 16;
				int e = num12 & 0xFFFF;
				int num14 = System.Math.Abs(num13);
				ECPoint[] array3 = ((num13 < 0) ? preCompNeg : preComp);
				ECPoint b = array3[num14 >> 1];
				eCPoint = eCPoint.TwicePlus(b);
				eCPoint = eCPoint.TimesPow2(e);
			}
			return eCPoint;
		}

		protected virtual int GetWindowSize(int bits)
		{
			return WNafUtilities.GetWindowSize(bits);
		}
	}
}
