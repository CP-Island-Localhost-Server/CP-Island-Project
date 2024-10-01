using System;
using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT409Field
	{
		private const ulong M25 = 33554431uL;

		private const ulong M59 = 576460752303423487uL;

		public static void Add(ulong[] x, ulong[] y, ulong[] z)
		{
			z[0] = x[0] ^ y[0];
			z[1] = x[1] ^ y[1];
			z[2] = x[2] ^ y[2];
			z[3] = x[3] ^ y[3];
			z[4] = x[4] ^ y[4];
			z[5] = x[5] ^ y[5];
			z[6] = x[6] ^ y[6];
		}

		public static void AddExt(ulong[] xx, ulong[] yy, ulong[] zz)
		{
			for (int i = 0; i < 13; i++)
			{
				zz[i] = xx[i] ^ yy[i];
			}
		}

		public static void AddOne(ulong[] x, ulong[] z)
		{
			z[0] = x[0] ^ 1;
			z[1] = x[1];
			z[2] = x[2];
			z[3] = x[3];
			z[4] = x[4];
			z[5] = x[5];
			z[6] = x[6];
		}

		public static ulong[] FromBigInteger(BigInteger x)
		{
			ulong[] array = Nat448.FromBigInteger64(x);
			Reduce39(array, 0);
			return array;
		}

		public static void Invert(ulong[] x, ulong[] z)
		{
			if (Nat448.IsZero64(x))
			{
				throw new InvalidOperationException();
			}
			ulong[] array = Nat448.Create64();
			ulong[] array2 = Nat448.Create64();
			ulong[] array3 = Nat448.Create64();
			Square(x, array);
			SquareN(array, 1, array2);
			Multiply(array, array2, array);
			SquareN(array2, 1, array2);
			Multiply(array, array2, array);
			SquareN(array, 3, array2);
			Multiply(array, array2, array);
			SquareN(array, 6, array2);
			Multiply(array, array2, array);
			SquareN(array, 12, array2);
			Multiply(array, array2, array3);
			SquareN(array3, 24, array);
			SquareN(array, 24, array2);
			Multiply(array, array2, array);
			SquareN(array, 48, array2);
			Multiply(array, array2, array);
			SquareN(array, 96, array2);
			Multiply(array, array2, array);
			SquareN(array, 192, array2);
			Multiply(array, array2, array);
			Multiply(array, array3, z);
		}

		public static void Multiply(ulong[] x, ulong[] y, ulong[] z)
		{
			ulong[] array = Nat448.CreateExt64();
			ImplMultiply(x, y, array);
			Reduce(array, z);
		}

		public static void MultiplyAddToExt(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = Nat448.CreateExt64();
			ImplMultiply(x, y, array);
			AddExt(zz, array, zz);
		}

		public static void Reduce(ulong[] xx, ulong[] z)
		{
			ulong num = xx[0];
			ulong num2 = xx[1];
			ulong num3 = xx[2];
			ulong num4 = xx[3];
			ulong num5 = xx[4];
			ulong num6 = xx[5];
			ulong num7 = xx[6];
			ulong num8 = xx[7];
			ulong num9 = xx[12];
			num6 ^= num9 << 39;
			num7 ^= (num9 >> 25) ^ (num9 << 62);
			num8 ^= num9 >> 2;
			num9 = xx[11];
			num5 ^= num9 << 39;
			num6 ^= (num9 >> 25) ^ (num9 << 62);
			num7 ^= num9 >> 2;
			num9 = xx[10];
			num4 ^= num9 << 39;
			num5 ^= (num9 >> 25) ^ (num9 << 62);
			num6 ^= num9 >> 2;
			num9 = xx[9];
			num3 ^= num9 << 39;
			num4 ^= (num9 >> 25) ^ (num9 << 62);
			num5 ^= num9 >> 2;
			num9 = xx[8];
			num2 ^= num9 << 39;
			num3 ^= (num9 >> 25) ^ (num9 << 62);
			num4 ^= num9 >> 2;
			num9 = num8;
			num ^= num9 << 39;
			num2 ^= (num9 >> 25) ^ (num9 << 62);
			num3 ^= num9 >> 2;
			ulong num10 = num7 >> 25;
			z[0] = num ^ num10;
			z[1] = num2 ^ (num10 << 23);
			z[2] = num3;
			z[3] = num4;
			z[4] = num5;
			z[5] = num6;
			z[6] = num7 & 0x1FFFFFF;
		}

		public static void Reduce39(ulong[] z, int zOff)
		{
			ulong num = z[zOff + 6];
			ulong num2 = num >> 25;
			ulong[] array;
			ulong[] array2 = (array = z);
			IntPtr intPtr = (IntPtr)zOff;
			array2[zOff] = array[(long)intPtr] ^ num2;
			ulong[] array3 = (array = z);
			int num3 = zOff + 1;
			intPtr = (IntPtr)num3;
			array3[num3] = array[(long)intPtr] ^ (num2 << 23);
			z[zOff + 6] = num & 0x1FFFFFF;
		}

		public static void Sqrt(ulong[] x, ulong[] z)
		{
			ulong num = Interleave.Unshuffle(x[0]);
			ulong num2 = Interleave.Unshuffle(x[1]);
			ulong num3 = (num & 0xFFFFFFFFu) | (num2 << 32);
			ulong num4 = (num >> 32) | (num2 & 0xFFFFFFFF00000000uL);
			num = Interleave.Unshuffle(x[2]);
			num2 = Interleave.Unshuffle(x[3]);
			ulong num5 = (num & 0xFFFFFFFFu) | (num2 << 32);
			ulong num6 = (num >> 32) | (num2 & 0xFFFFFFFF00000000uL);
			num = Interleave.Unshuffle(x[4]);
			num2 = Interleave.Unshuffle(x[5]);
			ulong num7 = (num & 0xFFFFFFFFu) | (num2 << 32);
			ulong num8 = (num >> 32) | (num2 & 0xFFFFFFFF00000000uL);
			num = Interleave.Unshuffle(x[6]);
			ulong num9 = num & 0xFFFFFFFFu;
			ulong num10 = num >> 32;
			z[0] = num3 ^ (num4 << 44);
			z[1] = num5 ^ (num6 << 44) ^ (num4 >> 20);
			z[2] = num7 ^ (num8 << 44) ^ (num6 >> 20);
			z[3] = num9 ^ (num10 << 44) ^ (num8 >> 20) ^ (num4 << 13);
			z[4] = (num10 >> 20) ^ (num6 << 13) ^ (num4 >> 51);
			z[5] = (num8 << 13) ^ (num6 >> 51);
			z[6] = (num10 << 13) ^ (num8 >> 51);
		}

		public static void Square(ulong[] x, ulong[] z)
		{
			ulong[] array = Nat.Create64(13);
			ImplSquare(x, array);
			Reduce(array, z);
		}

		public static void SquareAddToExt(ulong[] x, ulong[] zz)
		{
			ulong[] array = Nat.Create64(13);
			ImplSquare(x, array);
			AddExt(zz, array, zz);
		}

		public static void SquareN(ulong[] x, int n, ulong[] z)
		{
			ulong[] array = Nat.Create64(13);
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
			return (uint)(int)x[0] & 1u;
		}

		protected static void ImplCompactExt(ulong[] zz)
		{
			ulong num = zz[0];
			ulong num2 = zz[1];
			ulong num3 = zz[2];
			ulong num4 = zz[3];
			ulong num5 = zz[4];
			ulong num6 = zz[5];
			ulong num7 = zz[6];
			ulong num8 = zz[7];
			ulong num9 = zz[8];
			ulong num10 = zz[9];
			ulong num11 = zz[10];
			ulong num12 = zz[11];
			ulong num13 = zz[12];
			ulong num14 = zz[13];
			zz[0] = num ^ (num2 << 59);
			zz[1] = (num2 >> 5) ^ (num3 << 54);
			zz[2] = (num3 >> 10) ^ (num4 << 49);
			zz[3] = (num4 >> 15) ^ (num5 << 44);
			zz[4] = (num5 >> 20) ^ (num6 << 39);
			zz[5] = (num6 >> 25) ^ (num7 << 34);
			zz[6] = (num7 >> 30) ^ (num8 << 29);
			zz[7] = (num8 >> 35) ^ (num9 << 24);
			zz[8] = (num9 >> 40) ^ (num10 << 19);
			zz[9] = (num10 >> 45) ^ (num11 << 14);
			zz[10] = (num11 >> 50) ^ (num12 << 9);
			zz[11] = (num12 >> 55) ^ (num13 << 4) ^ (num14 << 63);
			zz[12] = (num13 >> 60) ^ (num14 >> 1);
			zz[13] = 0uL;
		}

		protected static void ImplExpand(ulong[] x, ulong[] z)
		{
			ulong num = x[0];
			ulong num2 = x[1];
			ulong num3 = x[2];
			ulong num4 = x[3];
			ulong num5 = x[4];
			ulong num6 = x[5];
			ulong num7 = x[6];
			z[0] = num & 0x7FFFFFFFFFFFFFFuL;
			z[1] = ((num >> 59) ^ (num2 << 5)) & 0x7FFFFFFFFFFFFFFuL;
			z[2] = ((num2 >> 54) ^ (num3 << 10)) & 0x7FFFFFFFFFFFFFFuL;
			z[3] = ((num3 >> 49) ^ (num4 << 15)) & 0x7FFFFFFFFFFFFFFuL;
			z[4] = ((num4 >> 44) ^ (num5 << 20)) & 0x7FFFFFFFFFFFFFFuL;
			z[5] = ((num5 >> 39) ^ (num6 << 25)) & 0x7FFFFFFFFFFFFFFuL;
			z[6] = (num6 >> 34) ^ (num7 << 30);
		}

		protected static void ImplMultiply(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = new ulong[7];
			ulong[] array2 = new ulong[7];
			ImplExpand(x, array);
			ImplExpand(y, array2);
			for (int i = 0; i < 7; i++)
			{
				ImplMulwAcc(array, array2[i], zz, i);
			}
			ImplCompactExt(zz);
		}

		protected static void ImplMulwAcc(ulong[] xs, ulong y, ulong[] z, int zOff)
		{
			ulong[] array = new ulong[8];
			array[1] = y;
			array[2] = array[1] << 1;
			array[3] = array[2] ^ y;
			array[4] = array[2] << 1;
			array[5] = array[4] ^ y;
			array[6] = array[3] << 1;
			array[7] = array[6] ^ y;
			for (int i = 0; i < 7; i++)
			{
				ulong num = xs[i];
				uint num2 = (uint)num;
				ulong num3 = 0uL;
				ulong num4 = array[num2 & 7] ^ (array[(num2 >> 3) & 7] << 3);
				int num5 = 54;
				do
				{
					num2 = (uint)(num >> num5);
					ulong num6 = array[num2 & 7] ^ (array[(num2 >> 3) & 7] << 3);
					num4 ^= num6 << num5;
					num3 ^= num6 >> -num5;
				}
				while ((num5 -= 6) > 0);
				ulong[] array2;
				ulong[] array3 = (array2 = z);
				int num7 = zOff + i;
				IntPtr intPtr = (IntPtr)num7;
				array3[num7] = array2[(long)intPtr] ^ (num4 & 0x7FFFFFFFFFFFFFFuL);
				ulong[] array4 = (array2 = z);
				int num8 = zOff + i + 1;
				intPtr = (IntPtr)num8;
				array4[num8] = array2[(long)intPtr] ^ ((num4 >> 59) ^ (num3 << 5));
			}
		}

		protected static void ImplSquare(ulong[] x, ulong[] zz)
		{
			for (int i = 0; i < 6; i++)
			{
				Interleave.Expand64To128(x[i], zz, i << 1);
			}
			zz[12] = Interleave.Expand32to64((uint)x[6]);
		}
	}
}
