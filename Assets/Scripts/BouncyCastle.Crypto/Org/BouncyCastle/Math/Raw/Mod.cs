using System;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Math.Raw
{
	internal abstract class Mod
	{
		private static readonly SecureRandom RandomSource = new SecureRandom();

		public static void Invert(uint[] p, uint[] x, uint[] z)
		{
			int num = p.Length;
			if (Nat.IsZero(num, x))
			{
				throw new ArgumentException("cannot be 0", "x");
			}
			if (Nat.IsOne(num, x))
			{
				Array.Copy(x, 0, z, 0, num);
				return;
			}
			uint[] array = Nat.Copy(num, x);
			uint[] array2 = Nat.Create(num);
			array2[0] = 1u;
			int xc = 0;
			if ((array[0] & 1) == 0)
			{
				InversionStep(p, array, num, array2, ref xc);
			}
			if (Nat.IsOne(num, array))
			{
				InversionResult(p, xc, array2, z);
				return;
			}
			uint[] array3 = Nat.Copy(num, p);
			uint[] array4 = Nat.Create(num);
			int xc2 = 0;
			int num2 = num;
			while (true)
			{
				if (array[num2 - 1] == 0 && array3[num2 - 1] == 0)
				{
					num2--;
				}
				else if (Nat.Gte(num, array, array3))
				{
					Nat.SubFrom(num, array3, array);
					xc += Nat.SubFrom(num, array4, array2) - xc2;
					InversionStep(p, array, num2, array2, ref xc);
					if (Nat.IsOne(num, array))
					{
						InversionResult(p, xc, array2, z);
						return;
					}
				}
				else
				{
					Nat.SubFrom(num, array, array3);
					xc2 += Nat.SubFrom(num, array2, array4) - xc;
					InversionStep(p, array3, num2, array4, ref xc2);
					if (Nat.IsOne(num, array3))
					{
						break;
					}
				}
			}
			InversionResult(p, xc2, array4, z);
		}

		public static uint[] Random(uint[] p)
		{
			int num = p.Length;
			uint[] array = Nat.Create(num);
			uint num2 = p[num - 1];
			num2 |= num2 >> 1;
			num2 |= num2 >> 2;
			num2 |= num2 >> 4;
			num2 |= num2 >> 8;
			num2 |= num2 >> 16;
			do
			{
				byte[] array2 = new byte[num << 2];
				RandomSource.NextBytes(array2);
				Pack.BE_To_UInt32(array2, 0, array);
				uint[] array3;
				uint[] array4 = (array3 = array);
				int num3 = num - 1;
				IntPtr intPtr = (IntPtr)num3;
				array4[num3] = array3[(long)intPtr] & num2;
			}
			while (Nat.Gte(num, array, p));
			return array;
		}

		public static void Add(uint[] p, uint[] x, uint[] y, uint[] z)
		{
			int len = p.Length;
			if (Nat.Add(len, x, y, z) != 0)
			{
				Nat.SubFrom(len, p, z);
			}
		}

		public static void Subtract(uint[] p, uint[] x, uint[] y, uint[] z)
		{
			int len = p.Length;
			if (Nat.Sub(len, x, y, z) != 0)
			{
				Nat.AddTo(len, p, z);
			}
		}

		private static void InversionResult(uint[] p, int ac, uint[] a, uint[] z)
		{
			if (ac < 0)
			{
				Nat.Add(p.Length, a, p, z);
			}
			else
			{
				Array.Copy(a, 0, z, 0, p.Length);
			}
		}

		private static void InversionStep(uint[] p, uint[] u, int uLen, uint[] x, ref int xc)
		{
			int len = p.Length;
			int num = 0;
			while (u[0] == 0)
			{
				Nat.ShiftDownWord(uLen, u, 0u);
				num += 32;
			}
			int trailingZeroes = GetTrailingZeroes(u[0]);
			if (trailingZeroes > 0)
			{
				Nat.ShiftDownBits(uLen, u, trailingZeroes, 0u);
				num += trailingZeroes;
			}
			for (int i = 0; i < num; i++)
			{
				if ((x[0] & (true ? 1u : 0u)) != 0)
				{
					if (xc < 0)
					{
						xc += (int)Nat.AddTo(len, p, x);
					}
					else
					{
						xc += Nat.SubFrom(len, p, x);
					}
				}
				Nat.ShiftDownBit(len, x, (uint)xc);
			}
		}

		private static int GetTrailingZeroes(uint x)
		{
			int num = 0;
			while ((x & 1) == 0)
			{
				x >>= 1;
				num++;
			}
			return num;
		}
	}
}
