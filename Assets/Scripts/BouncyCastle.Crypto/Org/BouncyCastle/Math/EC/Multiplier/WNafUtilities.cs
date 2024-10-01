using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public abstract class WNafUtilities
	{
		public static readonly string PRECOMP_NAME = "bc_wnaf";

		private static readonly int[] DEFAULT_WINDOW_SIZE_CUTOFFS = new int[6] { 13, 41, 121, 337, 897, 2305 };

		private static readonly byte[] EMPTY_BYTES = new byte[0];

		private static readonly int[] EMPTY_INTS = new int[0];

		private static readonly ECPoint[] EMPTY_POINTS = new ECPoint[0];

		public static int[] GenerateCompactNaf(BigInteger k)
		{
			if (k.BitLength >> 16 != 0)
			{
				throw new ArgumentException("must have bitlength < 2^16", "k");
			}
			if (k.SignValue == 0)
			{
				return EMPTY_INTS;
			}
			BigInteger bigInteger = k.ShiftLeft(1).Add(k);
			int bitLength = bigInteger.BitLength;
			int[] array = new int[bitLength >> 1];
			BigInteger bigInteger2 = bigInteger.Xor(k);
			int num = bitLength - 1;
			int num2 = 0;
			int num3 = 0;
			for (int i = 1; i < num; i++)
			{
				if (!bigInteger2.TestBit(i))
				{
					num3++;
					continue;
				}
				int num4 = ((!k.TestBit(i)) ? 1 : (-1));
				array[num2++] = (num4 << 16) | num3;
				num3 = 1;
				i++;
			}
			array[num2++] = 0x10000 | num3;
			if (array.Length > num2)
			{
				array = Trim(array, num2);
			}
			return array;
		}

		public static int[] GenerateCompactWindowNaf(int width, BigInteger k)
		{
			switch (width)
			{
			case 2:
				return GenerateCompactNaf(k);
			default:
				throw new ArgumentException("must be in the range [2, 16]", "width");
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			{
				if (k.BitLength >> 16 != 0)
				{
					throw new ArgumentException("must have bitlength < 2^16", "k");
				}
				if (k.SignValue == 0)
				{
					return EMPTY_INTS;
				}
				int[] array = new int[k.BitLength / width + 1];
				int num = 1 << width;
				int num2 = num - 1;
				int num3 = num >> 1;
				bool flag = false;
				int num4 = 0;
				int num5 = 0;
				while (num5 <= k.BitLength)
				{
					if (k.TestBit(num5) == flag)
					{
						num5++;
						continue;
					}
					k = k.ShiftRight(num5);
					int num6 = k.IntValue & num2;
					if (flag)
					{
						num6++;
					}
					flag = (num6 & num3) != 0;
					if (flag)
					{
						num6 -= num;
					}
					int num7 = ((num4 > 0) ? (num5 - 1) : num5);
					array[num4++] = (num6 << 16) | num7;
					num5 = width;
				}
				if (array.Length > num4)
				{
					array = Trim(array, num4);
				}
				return array;
			}
			}
		}

		public static byte[] GenerateJsf(BigInteger g, BigInteger h)
		{
			int num = System.Math.Max(g.BitLength, h.BitLength) + 1;
			byte[] array = new byte[num];
			BigInteger bigInteger = g;
			BigInteger bigInteger2 = h;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			while ((num3 | num4) != 0 || bigInteger.BitLength > num5 || bigInteger2.BitLength > num5)
			{
				int num6 = ((int)((uint)bigInteger.IntValue >> num5) + num3) & 7;
				int num7 = ((int)((uint)bigInteger2.IntValue >> num5) + num4) & 7;
				int num8 = num6 & 1;
				if (num8 != 0)
				{
					num8 -= num6 & 2;
					if (num6 + num8 == 4 && (num7 & 3) == 2)
					{
						num8 = -num8;
					}
				}
				int num9 = num7 & 1;
				if (num9 != 0)
				{
					num9 -= num7 & 2;
					if (num7 + num9 == 4 && (num6 & 3) == 2)
					{
						num9 = -num9;
					}
				}
				if (num3 << 1 == 1 + num8)
				{
					num3 ^= 1;
				}
				if (num4 << 1 == 1 + num9)
				{
					num4 ^= 1;
				}
				if (++num5 == 30)
				{
					num5 = 0;
					bigInteger = bigInteger.ShiftRight(30);
					bigInteger2 = bigInteger2.ShiftRight(30);
				}
				array[num2++] = (byte)((uint)(num8 << 4) | ((uint)num9 & 0xFu));
			}
			if (array.Length > num2)
			{
				array = Trim(array, num2);
			}
			return array;
		}

		public static byte[] GenerateNaf(BigInteger k)
		{
			if (k.SignValue == 0)
			{
				return EMPTY_BYTES;
			}
			BigInteger bigInteger = k.ShiftLeft(1).Add(k);
			int num = bigInteger.BitLength - 1;
			byte[] array = new byte[num];
			BigInteger bigInteger2 = bigInteger.Xor(k);
			for (int i = 1; i < num; i++)
			{
				if (bigInteger2.TestBit(i))
				{
					array[i - 1] = (byte)((!k.TestBit(i)) ? 1u : uint.MaxValue);
					i++;
				}
			}
			array[num - 1] = 1;
			return array;
		}

		public static byte[] GenerateWindowNaf(int width, BigInteger k)
		{
			switch (width)
			{
			case 2:
				return GenerateNaf(k);
			default:
				throw new ArgumentException("must be in the range [2, 8]", "width");
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			{
				if (k.SignValue == 0)
				{
					return EMPTY_BYTES;
				}
				byte[] array = new byte[k.BitLength + 1];
				int num = 1 << width;
				int num2 = num - 1;
				int num3 = num >> 1;
				bool flag = false;
				int num4 = 0;
				int num5 = 0;
				while (num5 <= k.BitLength)
				{
					if (k.TestBit(num5) == flag)
					{
						num5++;
						continue;
					}
					k = k.ShiftRight(num5);
					int num6 = k.IntValue & num2;
					if (flag)
					{
						num6++;
					}
					flag = (num6 & num3) != 0;
					if (flag)
					{
						num6 -= num;
					}
					num4 += ((num4 > 0) ? (num5 - 1) : num5);
					array[num4++] = (byte)num6;
					num5 = width;
				}
				if (array.Length > num4)
				{
					array = Trim(array, num4);
				}
				return array;
			}
			}
		}

		public static int GetNafWeight(BigInteger k)
		{
			if (k.SignValue == 0)
			{
				return 0;
			}
			BigInteger bigInteger = k.ShiftLeft(1).Add(k);
			BigInteger bigInteger2 = bigInteger.Xor(k);
			return bigInteger2.BitCount;
		}

		public static WNafPreCompInfo GetWNafPreCompInfo(ECPoint p)
		{
			return GetWNafPreCompInfo(p.Curve.GetPreCompInfo(p, PRECOMP_NAME));
		}

		public static WNafPreCompInfo GetWNafPreCompInfo(PreCompInfo preCompInfo)
		{
			if (preCompInfo != null && preCompInfo is WNafPreCompInfo)
			{
				return (WNafPreCompInfo)preCompInfo;
			}
			return new WNafPreCompInfo();
		}

		public static int GetWindowSize(int bits)
		{
			return GetWindowSize(bits, DEFAULT_WINDOW_SIZE_CUTOFFS);
		}

		public static int GetWindowSize(int bits, int[] windowSizeCutoffs)
		{
			int i;
			for (i = 0; i < windowSizeCutoffs.Length && bits >= windowSizeCutoffs[i]; i++)
			{
			}
			return i + 2;
		}

		public static ECPoint MapPointWithPrecomp(ECPoint p, int width, bool includeNegated, ECPointMap pointMap)
		{
			ECCurve curve = p.Curve;
			WNafPreCompInfo wNafPreCompInfo = Precompute(p, width, includeNegated);
			ECPoint eCPoint = pointMap.Map(p);
			WNafPreCompInfo wNafPreCompInfo2 = GetWNafPreCompInfo(curve.GetPreCompInfo(eCPoint, PRECOMP_NAME));
			ECPoint twice = wNafPreCompInfo.Twice;
			if (twice != null)
			{
				ECPoint twice2 = pointMap.Map(twice);
				wNafPreCompInfo2.Twice = twice2;
			}
			ECPoint[] preComp = wNafPreCompInfo.PreComp;
			ECPoint[] array = new ECPoint[preComp.Length];
			for (int i = 0; i < preComp.Length; i++)
			{
				array[i] = pointMap.Map(preComp[i]);
			}
			wNafPreCompInfo2.PreComp = array;
			if (includeNegated)
			{
				ECPoint[] array2 = new ECPoint[array.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = array[j].Negate();
				}
				wNafPreCompInfo2.PreCompNeg = array2;
			}
			curve.SetPreCompInfo(eCPoint, PRECOMP_NAME, wNafPreCompInfo2);
			return eCPoint;
		}

		public static WNafPreCompInfo Precompute(ECPoint p, int width, bool includeNegated)
		{
			ECCurve curve = p.Curve;
			WNafPreCompInfo wNafPreCompInfo = GetWNafPreCompInfo(curve.GetPreCompInfo(p, PRECOMP_NAME));
			int num = 0;
			int num2 = 1 << System.Math.Max(0, width - 2);
			ECPoint[] array = wNafPreCompInfo.PreComp;
			if (array == null)
			{
				array = EMPTY_POINTS;
			}
			else
			{
				num = array.Length;
			}
			if (num < num2)
			{
				array = ResizeTable(array, num2);
				if (num2 == 1)
				{
					array[0] = p.Normalize();
				}
				else
				{
					int num3 = num;
					if (num3 == 0)
					{
						array[0] = p;
						num3 = 1;
					}
					ECFieldElement eCFieldElement = null;
					if (num2 == 2)
					{
						array[1] = p.ThreeTimes();
					}
					else
					{
						ECPoint eCPoint = wNafPreCompInfo.Twice;
						ECPoint eCPoint2 = array[num3 - 1];
						if (eCPoint == null)
						{
							eCPoint = (wNafPreCompInfo.Twice = array[0].Twice());
							if (ECAlgorithms.IsFpCurve(curve) && curve.FieldSize >= 64)
							{
								switch (curve.CoordinateSystem)
								{
								case 2:
								case 3:
								case 4:
								{
									eCFieldElement = eCPoint.GetZCoord(0);
									eCPoint = curve.CreatePoint(eCPoint.XCoord.ToBigInteger(), eCPoint.YCoord.ToBigInteger());
									ECFieldElement eCFieldElement2 = eCFieldElement.Square();
									ECFieldElement scale = eCFieldElement2.Multiply(eCFieldElement);
									eCPoint2 = eCPoint2.ScaleX(eCFieldElement2).ScaleY(scale);
									if (num == 0)
									{
										array[0] = eCPoint2;
									}
									break;
								}
								}
							}
						}
						while (num3 < num2)
						{
							eCPoint2 = (array[num3++] = eCPoint2.Add(eCPoint));
						}
					}
					curve.NormalizeAll(array, num, num2 - num, eCFieldElement);
				}
			}
			wNafPreCompInfo.PreComp = array;
			if (includeNegated)
			{
				ECPoint[] array2 = wNafPreCompInfo.PreCompNeg;
				int i;
				if (array2 == null)
				{
					i = 0;
					array2 = new ECPoint[num2];
				}
				else
				{
					i = array2.Length;
					if (i < num2)
					{
						array2 = ResizeTable(array2, num2);
					}
				}
				for (; i < num2; i++)
				{
					array2[i] = array[i].Negate();
				}
				wNafPreCompInfo.PreCompNeg = array2;
			}
			curve.SetPreCompInfo(p, PRECOMP_NAME, wNafPreCompInfo);
			return wNafPreCompInfo;
		}

		private static byte[] Trim(byte[] a, int length)
		{
			byte[] array = new byte[length];
			Array.Copy(a, 0, array, 0, array.Length);
			return array;
		}

		private static int[] Trim(int[] a, int length)
		{
			int[] array = new int[length];
			Array.Copy(a, 0, array, 0, array.Length);
			return array;
		}

		private static ECPoint[] ResizeTable(ECPoint[] a, int length)
		{
			ECPoint[] array = new ECPoint[length];
			Array.Copy(a, 0, array, 0, a.Length);
			return array;
		}
	}
}
