namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class FixedPointUtilities
	{
		public static readonly string PRECOMP_NAME = "bc_fixed_point";

		public static int GetCombSize(ECCurve c)
		{
			BigInteger order = c.Order;
			if (order != null)
			{
				return order.BitLength;
			}
			return c.FieldSize + 1;
		}

		public static FixedPointPreCompInfo GetFixedPointPreCompInfo(PreCompInfo preCompInfo)
		{
			if (preCompInfo != null && preCompInfo is FixedPointPreCompInfo)
			{
				return (FixedPointPreCompInfo)preCompInfo;
			}
			return new FixedPointPreCompInfo();
		}

		public static FixedPointPreCompInfo Precompute(ECPoint p, int minWidth)
		{
			ECCurve curve = p.Curve;
			int num = 1 << minWidth;
			FixedPointPreCompInfo fixedPointPreCompInfo = GetFixedPointPreCompInfo(curve.GetPreCompInfo(p, PRECOMP_NAME));
			ECPoint[] preComp = fixedPointPreCompInfo.PreComp;
			if (preComp == null || preComp.Length < num)
			{
				int combSize = GetCombSize(curve);
				int e = (combSize + minWidth - 1) / minWidth;
				ECPoint[] array = new ECPoint[minWidth];
				array[0] = p;
				for (int i = 1; i < minWidth; i++)
				{
					array[i] = array[i - 1].TimesPow2(e);
				}
				curve.NormalizeAll(array);
				preComp = new ECPoint[num];
				preComp[0] = curve.Infinity;
				for (int num2 = minWidth - 1; num2 >= 0; num2--)
				{
					ECPoint b = array[num2];
					int num3 = 1 << num2;
					for (int j = num3; j < num; j += num3 << 1)
					{
						preComp[j] = preComp[j - num3].Add(b);
					}
				}
				curve.NormalizeAll(preComp);
				fixedPointPreCompInfo.PreComp = preComp;
				fixedPointPreCompInfo.Width = minWidth;
				curve.SetPreCompInfo(p, PRECOMP_NAME, fixedPointPreCompInfo);
			}
			return fixedPointPreCompInfo;
		}
	}
}
