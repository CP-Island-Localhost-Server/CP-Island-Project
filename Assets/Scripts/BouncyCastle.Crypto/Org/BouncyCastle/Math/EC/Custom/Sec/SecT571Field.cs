using System;
using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT571Field
	{
		private const ulong M59 = 576460752303423487uL;

		private const ulong RM = 17256631552825064414uL;

		private static readonly ulong[] ROOT_Z = new ulong[9] { 3161836309350906777uL, 10804290191530228771uL, 14625517132619890193uL, 7312758566309945096uL, 17890083061325672324uL, 8945041530681231562uL, 13695892802195391589uL, 6847946401097695794uL, 541669439031730457uL };

		public static void Add(ulong[] x, ulong[] y, ulong[] z)
		{
			for (int i = 0; i < 9; i++)
			{
				z[i] = x[i] ^ y[i];
			}
		}

		private static void Add(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z, int zOff)
		{
			for (int i = 0; i < 9; i++)
			{
				z[zOff + i] = x[xOff + i] ^ y[yOff + i];
			}
		}

		private static void AddBothTo(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z, int zOff)
		{
			for (int i = 0; i < 9; i++)
			{
				ulong[] array;
				ulong[] array2 = (array = z);
				int num = zOff + i;
				IntPtr intPtr = (IntPtr)num;
				array2[num] = array[(long)intPtr] ^ (x[xOff + i] ^ y[yOff + i]);
			}
		}

		public static void AddExt(ulong[] xx, ulong[] yy, ulong[] zz)
		{
			for (int i = 0; i < 18; i++)
			{
				zz[i] = xx[i] ^ yy[i];
			}
		}

		public static void AddOne(ulong[] x, ulong[] z)
		{
			z[0] = x[0] ^ 1;
			for (int i = 1; i < 9; i++)
			{
				z[i] = x[i];
			}
		}

		public static ulong[] FromBigInteger(BigInteger x)
		{
			ulong[] array = Nat576.FromBigInteger64(x);
			Reduce5(array, 0);
			return array;
		}

		public static void Invert(ulong[] x, ulong[] z)
		{
			if (Nat576.IsZero64(x))
			{
				throw new InvalidOperationException();
			}
			ulong[] array = Nat576.Create64();
			ulong[] array2 = Nat576.Create64();
			ulong[] array3 = Nat576.Create64();
			Square(x, array3);
			Square(array3, array);
			Square(array, array2);
			Multiply(array, array2, array);
			SquareN(array, 2, array2);
			Multiply(array, array2, array);
			Multiply(array, array3, array);
			SquareN(array, 5, array2);
			Multiply(array, array2, array);
			SquareN(array2, 5, array2);
			Multiply(array, array2, array);
			SquareN(array, 15, array2);
			Multiply(array, array2, array3);
			SquareN(array3, 30, array);
			SquareN(array, 30, array2);
			Multiply(array, array2, array);
			SquareN(array, 60, array2);
			Multiply(array, array2, array);
			SquareN(array2, 60, array2);
			Multiply(array, array2, array);
			SquareN(array, 180, array2);
			Multiply(array, array2, array);
			SquareN(array2, 180, array2);
			Multiply(array, array2, array);
			Multiply(array, array3, z);
		}

		public static void Multiply(ulong[] x, ulong[] y, ulong[] z)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplMultiply(x, y, array);
			Reduce(array, z);
		}

		public static void MultiplyAddToExt(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplMultiply(x, y, array);
			AddExt(zz, array, zz);
		}

		public static void Reduce(ulong[] xx, ulong[] z)
		{
			ulong num = xx[9];
			ulong num2 = xx[17];
			ulong num3 = num;
			num = num3 ^ (num2 >> 59) ^ (num2 >> 57) ^ (num2 >> 54) ^ (num2 >> 49);
			num3 = xx[8] ^ (num2 << 5) ^ (num2 << 7) ^ (num2 << 10) ^ (num2 << 15);
			for (int num4 = 16; num4 >= 10; num4--)
			{
				num2 = xx[num4];
				z[num4 - 8] = num3 ^ (num2 >> 59) ^ (num2 >> 57) ^ (num2 >> 54) ^ (num2 >> 49);
				num3 = xx[num4 - 9] ^ (num2 << 5) ^ (num2 << 7) ^ (num2 << 10) ^ (num2 << 15);
			}
			num2 = num;
			z[1] = num3 ^ (num2 >> 59) ^ (num2 >> 57) ^ (num2 >> 54) ^ (num2 >> 49);
			num3 = xx[0] ^ (num2 << 5) ^ (num2 << 7) ^ (num2 << 10) ^ (num2 << 15);
			ulong num5 = z[8];
			ulong num6 = num5 >> 59;
			z[0] = num3 ^ num6 ^ (num6 << 2) ^ (num6 << 5) ^ (num6 << 10);
			z[8] = num5 & 0x7FFFFFFFFFFFFFFuL;
		}

		public static void Reduce5(ulong[] z, int zOff)
		{
			ulong num = z[zOff + 8];
			ulong num2 = num >> 59;
			ulong[] array;
			ulong[] array2 = (array = z);
			IntPtr intPtr = (IntPtr)zOff;
			array2[zOff] = array[(long)intPtr] ^ (num2 ^ (num2 << 2) ^ (num2 << 5) ^ (num2 << 10));
			z[zOff + 8] = num & 0x7FFFFFFFFFFFFFFuL;
		}

		public static void Sqrt(ulong[] x, ulong[] z)
		{
			ulong[] array = Nat576.Create64();
			ulong[] array2 = Nat576.Create64();
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				ulong num2 = Interleave.Unshuffle(x[num++]);
				ulong num3 = Interleave.Unshuffle(x[num++]);
				array[i] = (num2 & 0xFFFFFFFFu) | (num3 << 32);
				array2[i] = (num2 >> 32) | (num3 & 0xFFFFFFFF00000000uL);
			}
			ulong num4 = Interleave.Unshuffle(x[num]);
			array[4] = num4 & 0xFFFFFFFFu;
			array2[4] = num4 >> 32;
			Multiply(array2, ROOT_Z, z);
			Add(z, array, z);
		}

		public static void Square(ulong[] x, ulong[] z)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplSquare(x, array);
			Reduce(array, z);
		}

		public static void SquareAddToExt(ulong[] x, ulong[] zz)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplSquare(x, array);
			AddExt(zz, array, zz);
		}

		public static void SquareN(ulong[] x, int n, ulong[] z)
		{
			ulong[] array = Nat576.CreateExt64();
			ImplSquare(x, array);
			Reduce(array, z);
			while (--n > 0)
			{
				ImplSquare(z, array);
				Reduce(array, z);
			}
		}

		public static uint Trace(ulong[] x)
		{
			return (uint)(int)(x[0] ^ (x[8] >> 49) ^ (x[8] >> 57)) & 1u;
		}

		protected static void ImplMultiply(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = new ulong[144];
			Array.Copy(y, 0, array, 9, 9);
			int num = 0;
			for (int num2 = 7; num2 > 0; num2--)
			{
				num += 18;
				Nat.ShiftUpBit64(9, array, num >> 1, 0uL, array, num);
				Reduce5(array, num);
				Add(array, 9, array, num, array, num + 9);
			}
			ulong[] array2 = new ulong[array.Length];
			Nat.ShiftUpBits64(array.Length, array, 0, 4, 0uL, array2, 0);
			uint num3 = 15u;
			for (int num4 = 56; num4 >= 0; num4 -= 8)
			{
				for (int i = 1; i < 9; i += 2)
				{
					uint num5 = (uint)(x[i] >> num4);
					uint num6 = num5 & num3;
					uint num7 = (num5 >> 4) & num3;
					AddBothTo(array, (int)(9 * num6), array2, (int)(9 * num7), zz, i - 1);
				}
				Nat.ShiftUpBits64(16, zz, 0, 8, 0uL);
			}
			for (int num8 = 56; num8 >= 0; num8 -= 8)
			{
				for (int j = 0; j < 9; j += 2)
				{
					uint num9 = (uint)(x[j] >> num8);
					uint num10 = num9 & num3;
					uint num11 = (num9 >> 4) & num3;
					AddBothTo(array, (int)(9 * num10), array2, (int)(9 * num11), zz, j);
				}
				if (num8 > 0)
				{
					Nat.ShiftUpBits64(18, zz, 0, 8, 0uL);
				}
			}
		}

		protected static void ImplMulwAcc(ulong[] xs, ulong y, ulong[] z, int zOff)
		{
			ulong[] array = new ulong[32];
			array[1] = y;
			for (int i = 2; i < 32; i += 2)
			{
				array[i] = array[i >> 1] << 1;
				array[i + 1] = array[i] ^ y;
			}
			ulong num = 0uL;
			ulong[] array2;
			IntPtr intPtr;
			for (int j = 0; j < 9; j++)
			{
				ulong num2 = xs[j];
				uint num3 = (uint)num2;
				num ^= array[num3 & 0x1F];
				ulong num4 = 0uL;
				int num5 = 60;
				do
				{
					num3 = (uint)(num2 >> num5);
					ulong num6 = array[num3 & 0x1F];
					num ^= num6 << num5;
					num4 ^= num6 >> -num5;
				}
				while ((num5 -= 5) > 0);
				for (int k = 0; k < 4; k++)
				{
					num2 = (num2 & 0xEF7BDEF7BDEF7BDEuL) >> 1;
					num4 ^= num2 & (ulong)((long)(y << k) >> 63);
				}
				ulong[] array3 = (array2 = z);
				int num7 = zOff + j;
				intPtr = (IntPtr)num7;
				array3[num7] = array2[(long)intPtr] ^ num;
				num = num4;
			}
			ulong[] array4 = (array2 = z);
			int num8 = zOff + 9;
			intPtr = (IntPtr)num8;
			array4[num8] = array2[(long)intPtr] ^ num;
		}

		protected static void ImplSquare(ulong[] x, ulong[] zz)
		{
			for (int i = 0; i < 9; i++)
			{
				Interleave.Expand64To128(x[i], zz, i << 1);
			}
		}
	}
}
