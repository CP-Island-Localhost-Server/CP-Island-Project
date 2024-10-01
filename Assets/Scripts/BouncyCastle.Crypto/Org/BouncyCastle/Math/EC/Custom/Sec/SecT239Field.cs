using System;
using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecT239Field
	{
		private const ulong M47 = 140737488355327uL;

		private const ulong M60 = 1152921504606846975uL;

		public static void Add(ulong[] x, ulong[] y, ulong[] z)
		{
			z[0] = x[0] ^ y[0];
			z[1] = x[1] ^ y[1];
			z[2] = x[2] ^ y[2];
			z[3] = x[3] ^ y[3];
		}

		public static void AddExt(ulong[] xx, ulong[] yy, ulong[] zz)
		{
			zz[0] = xx[0] ^ yy[0];
			zz[1] = xx[1] ^ yy[1];
			zz[2] = xx[2] ^ yy[2];
			zz[3] = xx[3] ^ yy[3];
			zz[4] = xx[4] ^ yy[4];
			zz[5] = xx[5] ^ yy[5];
			zz[6] = xx[6] ^ yy[6];
			zz[7] = xx[7] ^ yy[7];
		}

		public static void AddOne(ulong[] x, ulong[] z)
		{
			z[0] = x[0] ^ 1;
			z[1] = x[1];
			z[2] = x[2];
			z[3] = x[3];
		}

		public static ulong[] FromBigInteger(BigInteger x)
		{
			ulong[] array = Nat256.FromBigInteger64(x);
			Reduce17(array, 0);
			return array;
		}

		public static void Invert(ulong[] x, ulong[] z)
		{
			if (Nat256.IsZero64(x))
			{
				throw new InvalidOperationException();
			}
			ulong[] array = Nat256.Create64();
			ulong[] array2 = Nat256.Create64();
			Square(x, array);
			Multiply(array, x, array);
			Square(array, array);
			Multiply(array, x, array);
			SquareN(array, 3, array2);
			Multiply(array2, array, array2);
			Square(array2, array2);
			Multiply(array2, x, array2);
			SquareN(array2, 7, array);
			Multiply(array, array2, array);
			SquareN(array, 14, array2);
			Multiply(array2, array, array2);
			Square(array2, array2);
			Multiply(array2, x, array2);
			SquareN(array2, 29, array);
			Multiply(array, array2, array);
			Square(array, array);
			Multiply(array, x, array);
			SquareN(array, 59, array2);
			Multiply(array2, array, array2);
			Square(array2, array2);
			Multiply(array2, x, array2);
			SquareN(array2, 119, array);
			Multiply(array, array2, array);
			Square(array, z);
		}

		public static void Multiply(ulong[] x, ulong[] y, ulong[] z)
		{
			ulong[] array = Nat256.CreateExt64();
			ImplMultiply(x, y, array);
			Reduce(array, z);
		}

		public static void MultiplyAddToExt(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = Nat256.CreateExt64();
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
			num4 ^= num8 << 17;
			num5 ^= num8 >> 47;
			num6 ^= num8 << 47;
			num7 ^= num8 >> 17;
			num3 ^= num7 << 17;
			num4 ^= num7 >> 47;
			num5 ^= num7 << 47;
			num6 ^= num7 >> 17;
			num2 ^= num6 << 17;
			num3 ^= num6 >> 47;
			num4 ^= num6 << 47;
			num5 ^= num6 >> 17;
			num ^= num5 << 17;
			num2 ^= num5 >> 47;
			num3 ^= num5 << 47;
			num4 ^= num5 >> 17;
			ulong num9 = num4 >> 47;
			z[0] = num ^ num9;
			z[1] = num2;
			z[2] = num3 ^ (num9 << 30);
			z[3] = num4 & 0x7FFFFFFFFFFFuL;
		}

		public static void Reduce17(ulong[] z, int zOff)
		{
			ulong num = z[zOff + 3];
			ulong num2 = num >> 47;
			ulong[] array;
			ulong[] array2 = (array = z);
			IntPtr intPtr = (IntPtr)zOff;
			array2[zOff] = array[(long)intPtr] ^ num2;
			ulong[] array3 = (array = z);
			int num3 = zOff + 2;
			intPtr = (IntPtr)num3;
			array3[num3] = array[(long)intPtr] ^ (num2 << 30);
			z[zOff + 3] = num & 0x7FFFFFFFFFFFuL;
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
			ulong num7 = num6 >> 49;
			ulong num8 = (num4 >> 49) | (num6 << 15);
			num6 ^= num4 << 15;
			ulong[] array = Nat256.CreateExt64();
			int[] array2 = new int[2] { 39, 120 };
			ulong[] array3;
			for (int i = 0; i < array2.Length; i++)
			{
				int num9 = array2[i] >> 6;
				int num10 = array2[i] & 0x3F;
				ulong[] array4 = (array3 = array);
				IntPtr intPtr = (IntPtr)num9;
				array4[num9] = array3[(long)intPtr] ^ (num4 << num10);
				ulong[] array5 = (array3 = array);
				int num11 = num9 + 1;
				intPtr = (IntPtr)num11;
				array5[num11] = array3[(long)intPtr] ^ ((num6 << num10) | (num4 >> -num10));
				ulong[] array6 = (array3 = array);
				int num12 = num9 + 2;
				intPtr = (IntPtr)num12;
				array6[num12] = array3[(long)intPtr] ^ ((num8 << num10) | (num6 >> -num10));
				ulong[] array7 = (array3 = array);
				int num13 = num9 + 3;
				intPtr = (IntPtr)num13;
				array7[num13] = array3[(long)intPtr] ^ ((num7 << num10) | (num8 >> -num10));
				ulong[] array8 = (array3 = array);
				int num14 = num9 + 4;
				intPtr = (IntPtr)num14;
				array8[num14] = array3[(long)intPtr] ^ (num7 >> -num10);
			}
			Reduce(array, z);
			(array3 = z)[0] = array3[0] ^ num3;
			(array3 = z)[1] = array3[1] ^ num5;
		}

		public static void Square(ulong[] x, ulong[] z)
		{
			ulong[] array = Nat256.CreateExt64();
			ImplSquare(x, array);
			Reduce(array, z);
		}

		public static void SquareAddToExt(ulong[] x, ulong[] zz)
		{
			ulong[] array = Nat256.CreateExt64();
			ImplSquare(x, array);
			AddExt(zz, array, zz);
		}

		public static void SquareN(ulong[] x, int n, ulong[] z)
		{
			ulong[] array = Nat256.CreateExt64();
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
			return (uint)(int)(x[0] ^ (x[1] >> 17) ^ (x[2] >> 34)) & 1u;
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
			zz[0] = num ^ (num2 << 60);
			zz[1] = (num2 >> 4) ^ (num3 << 56);
			zz[2] = (num3 >> 8) ^ (num4 << 52);
			zz[3] = (num4 >> 12) ^ (num5 << 48);
			zz[4] = (num5 >> 16) ^ (num6 << 44);
			zz[5] = (num6 >> 20) ^ (num7 << 40);
			zz[6] = (num7 >> 24) ^ (num8 << 36);
			zz[7] = num8 >> 28;
		}

		protected static void ImplExpand(ulong[] x, ulong[] z)
		{
			ulong num = x[0];
			ulong num2 = x[1];
			ulong num3 = x[2];
			ulong num4 = x[3];
			z[0] = num & 0xFFFFFFFFFFFFFFFuL;
			z[1] = ((num >> 60) ^ (num2 << 4)) & 0xFFFFFFFFFFFFFFFuL;
			z[2] = ((num2 >> 56) ^ (num3 << 8)) & 0xFFFFFFFFFFFFFFFuL;
			z[3] = (num3 >> 52) ^ (num4 << 12);
		}

		protected static void ImplMultiply(ulong[] x, ulong[] y, ulong[] zz)
		{
			ulong[] array = new ulong[4];
			ulong[] array2 = new ulong[4];
			ImplExpand(x, array);
			ImplExpand(y, array2);
			ImplMulwAcc(array[0], array2[0], zz, 0);
			ImplMulwAcc(array[1], array2[1], zz, 1);
			ImplMulwAcc(array[2], array2[2], zz, 2);
			ImplMulwAcc(array[3], array2[3], zz, 3);
			ulong[] array3;
			for (int num = 5; num > 0; num--)
			{
				ulong[] array4 = (array3 = zz);
				int num2 = num;
				IntPtr intPtr = (IntPtr)num2;
				array4[num2] = array3[(long)intPtr] ^ zz[num - 1];
			}
			ImplMulwAcc(array[0] ^ array[1], array2[0] ^ array2[1], zz, 1);
			ImplMulwAcc(array[2] ^ array[3], array2[2] ^ array2[3], zz, 3);
			for (int num3 = 7; num3 > 1; num3--)
			{
				ulong[] array5 = (array3 = zz);
				int num4 = num3;
				IntPtr intPtr = (IntPtr)num4;
				array5[num4] = array3[(long)intPtr] ^ zz[num3 - 2];
			}
			ulong num5 = array[0] ^ array[2];
			ulong num6 = array[1] ^ array[3];
			ulong num7 = array2[0] ^ array2[2];
			ulong num8 = array2[1] ^ array2[3];
			ImplMulwAcc(num5 ^ num6, num7 ^ num8, zz, 3);
			ulong[] array6 = new ulong[3];
			ImplMulwAcc(num5, num7, array6, 0);
			ImplMulwAcc(num6, num8, array6, 1);
			ulong num9 = array6[0];
			ulong num10 = array6[1];
			ulong num11 = array6[2];
			(array3 = zz)[2] = array3[2] ^ num9;
			(array3 = zz)[3] = array3[3] ^ (num9 ^ num10);
			(array3 = zz)[4] = array3[4] ^ (num11 ^ num10);
			(array3 = zz)[5] = array3[5] ^ num11;
			ImplCompactExt(zz);
		}

		protected static void ImplMulwAcc(ulong x, ulong y, ulong[] z, int zOff)
		{
			ulong[] array = new ulong[8];
			array[1] = y;
			array[2] = array[1] << 1;
			array[3] = array[2] ^ y;
			array[4] = array[2] << 1;
			array[5] = array[4] ^ y;
			array[6] = array[3] << 1;
			array[7] = array[6] ^ y;
			uint num = (uint)x;
			ulong num2 = 0uL;
			ulong num3 = array[num & 7] ^ (array[(num >> 3) & 7] << 3);
			int num4 = 54;
			do
			{
				num = (uint)(x >> num4);
				ulong num5 = array[num & 7] ^ (array[(num >> 3) & 7] << 3);
				num3 ^= num5 << num4;
				num2 ^= num5 >> -num4;
			}
			while ((num4 -= 6) > 0);
			num2 ^= (x & 0x820820820820820L & (ulong)((long)(y << 4) >> 63)) >> 5;
			ulong[] array2;
			ulong[] array3 = (array2 = z);
			IntPtr intPtr = (IntPtr)zOff;
			array3[zOff] = array2[(long)intPtr] ^ (num3 & 0xFFFFFFFFFFFFFFFuL);
			ulong[] array4 = (array2 = z);
			int num6 = zOff + 1;
			intPtr = (IntPtr)num6;
			array4[num6] = array2[(long)intPtr] ^ ((num3 >> 60) ^ (num2 << 4));
		}

		protected static void ImplSquare(ulong[] x, ulong[] zz)
		{
			Interleave.Expand64To128(x[0], zz, 0);
			Interleave.Expand64To128(x[1], zz, 2);
			Interleave.Expand64To128(x[2], zz, 4);
			ulong num = x[3];
			zz[6] = Interleave.Expand32to64((uint)num);
			zz[7] = Interleave.Expand16to32((uint)(num >> 32));
		}
	}
}
