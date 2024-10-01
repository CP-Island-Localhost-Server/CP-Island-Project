using System;
using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	internal abstract class GcmUtilities
	{
		private const uint E1 = 3774873600u;

		private const ulong E1L = 16212958658533785600uL;

		private static readonly uint[] LOOKUP = GenerateLookup();

		private static uint[] GenerateLookup()
		{
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = 0u;
				for (int num2 = 7; num2 >= 0; num2--)
				{
					if ((i & (1 << num2)) != 0)
					{
						num ^= 3774873600u >> 7 - num2;
					}
				}
				array[i] = num;
			}
			return array;
		}

		internal static byte[] OneAsBytes()
		{
			return new byte[16]
			{
				128, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0
			};
		}

		internal static uint[] OneAsUints()
		{
			return new uint[4] { 2147483648u, 0u, 0u, 0u };
		}

		internal static ulong[] OneAsUlongs()
		{
			return new ulong[2] { 9223372036854775808uL, 0uL };
		}

		internal static byte[] AsBytes(uint[] x)
		{
			return Pack.UInt32_To_BE(x);
		}

		internal static void AsBytes(uint[] x, byte[] z)
		{
			Pack.UInt32_To_BE(x, z, 0);
		}

		internal static byte[] AsBytes(ulong[] x)
		{
			byte[] array = new byte[16];
			Pack.UInt64_To_BE(x, array, 0);
			return array;
		}

		internal static void AsBytes(ulong[] x, byte[] z)
		{
			Pack.UInt64_To_BE(x, z, 0);
		}

		internal static uint[] AsUints(byte[] bs)
		{
			uint[] array = new uint[4];
			Pack.BE_To_UInt32(bs, 0, array);
			return array;
		}

		internal static void AsUints(byte[] bs, uint[] output)
		{
			Pack.BE_To_UInt32(bs, 0, output);
		}

		internal static ulong[] AsUlongs(byte[] x)
		{
			ulong[] array = new ulong[2];
			Pack.BE_To_UInt64(x, 0, array);
			return array;
		}

		public static void AsUlongs(byte[] x, ulong[] z)
		{
			Pack.BE_To_UInt64(x, 0, z);
		}

		internal static void Multiply(byte[] x, byte[] y)
		{
			uint[] x2 = AsUints(x);
			uint[] y2 = AsUints(y);
			Multiply(x2, y2);
			AsBytes(x2, x);
		}

		internal static void Multiply(uint[] x, uint[] y)
		{
			uint num = x[0];
			uint num2 = x[1];
			uint num3 = x[2];
			uint num4 = x[3];
			uint num5 = 0u;
			uint num6 = 0u;
			uint num7 = 0u;
			uint num8 = 0u;
			for (int i = 0; i < 4; i++)
			{
				int num9 = (int)y[i];
				for (int j = 0; j < 32; j++)
				{
					uint num10 = (uint)(num9 >> 31);
					num9 <<= 1;
					num5 ^= num & num10;
					num6 ^= num2 & num10;
					num7 ^= num3 & num10;
					num8 ^= num4 & num10;
					uint num11 = (uint)((int)(num4 << 31) >> 8);
					num4 = (num4 >> 1) | (num3 << 31);
					num3 = (num3 >> 1) | (num2 << 31);
					num2 = (num2 >> 1) | (num << 31);
					num = (num >> 1) ^ (num11 & 0xE1000000u);
				}
			}
			x[0] = num5;
			x[1] = num6;
			x[2] = num7;
			x[3] = num8;
		}

		internal static void Multiply(ulong[] x, ulong[] y)
		{
			ulong num = x[0];
			ulong num2 = x[1];
			ulong num3 = 0uL;
			ulong num4 = 0uL;
			for (int i = 0; i < 2; i++)
			{
				long num5 = (long)y[i];
				for (int j = 0; j < 64; j++)
				{
					ulong num6 = (ulong)(num5 >> 63);
					num5 <<= 1;
					num3 ^= num & num6;
					num4 ^= num2 & num6;
					ulong num7 = (ulong)((long)(num2 << 63) >> 8);
					num2 = (num2 >> 1) | (num << 63);
					num = (num >> 1) ^ (num7 & 0xE100000000000000uL);
				}
			}
			x[0] = num3;
			x[1] = num4;
		}

		internal static void MultiplyP(uint[] x)
		{
			uint num = (uint)((int)ShiftRight(x) >> 8);
			uint[] array;
			(array = x)[0] = array[0] ^ (num & 0xE1000000u);
		}

		internal static void MultiplyP(uint[] x, uint[] z)
		{
			uint num = (uint)((int)ShiftRight(x, z) >> 8);
			uint[] array;
			(array = z)[0] = array[0] ^ (num & 0xE1000000u);
		}

		internal static void MultiplyP8(uint[] x)
		{
			uint num = ShiftRightN(x, 8);
			uint[] array;
			(array = x)[0] = array[0] ^ LOOKUP[num >> 24];
		}

		internal static void MultiplyP8(uint[] x, uint[] y)
		{
			uint num = ShiftRightN(x, 8, y);
			uint[] array;
			(array = y)[0] = array[0] ^ LOOKUP[num >> 24];
		}

		internal static uint ShiftRight(uint[] x)
		{
			uint num = x[0];
			x[0] = num >> 1;
			uint num2 = num << 31;
			num = x[1];
			x[1] = (num >> 1) | num2;
			num2 = num << 31;
			num = x[2];
			x[2] = (num >> 1) | num2;
			num2 = num << 31;
			num = x[3];
			x[3] = (num >> 1) | num2;
			return num << 31;
		}

		internal static uint ShiftRight(uint[] x, uint[] z)
		{
			uint num = x[0];
			z[0] = num >> 1;
			uint num2 = num << 31;
			num = x[1];
			z[1] = (num >> 1) | num2;
			num2 = num << 31;
			num = x[2];
			z[2] = (num >> 1) | num2;
			num2 = num << 31;
			num = x[3];
			z[3] = (num >> 1) | num2;
			return num << 31;
		}

		internal static uint ShiftRightN(uint[] x, int n)
		{
			uint num = x[0];
			int num2 = 32 - n;
			x[0] = num >> n;
			uint num3 = num << num2;
			num = x[1];
			x[1] = (num >> n) | num3;
			num3 = num << num2;
			num = x[2];
			x[2] = (num >> n) | num3;
			num3 = num << num2;
			num = x[3];
			x[3] = (num >> n) | num3;
			return num << num2;
		}

		internal static uint ShiftRightN(uint[] x, int n, uint[] z)
		{
			uint num = x[0];
			int num2 = 32 - n;
			z[0] = num >> n;
			uint num3 = num << num2;
			num = x[1];
			z[1] = (num >> n) | num3;
			num3 = num << num2;
			num = x[2];
			z[2] = (num >> n) | num3;
			num3 = num << num2;
			num = x[3];
			z[3] = (num >> n) | num3;
			return num << num2;
		}

		internal static void Xor(byte[] x, byte[] y)
		{
			int num = 0;
			do
			{
				byte[] array;
				byte[] array2 = (array = x);
				int num2 = num;
				IntPtr intPtr = (IntPtr)num2;
				array2[num2] = (byte)(array[(long)intPtr] ^ y[num]);
				num++;
				byte[] array3 = (array = x);
				int num3 = num;
				intPtr = (IntPtr)num3;
				array3[num3] = (byte)(array[(long)intPtr] ^ y[num]);
				num++;
				byte[] array4 = (array = x);
				int num4 = num;
				intPtr = (IntPtr)num4;
				array4[num4] = (byte)(array[(long)intPtr] ^ y[num]);
				num++;
				byte[] array5 = (array = x);
				int num5 = num;
				intPtr = (IntPtr)num5;
				array5[num5] = (byte)(array[(long)intPtr] ^ y[num]);
				num++;
			}
			while (num < 16);
		}

		internal static void Xor(byte[] x, byte[] y, int yOff, int yLen)
		{
			while (--yLen >= 0)
			{
				byte[] array;
				byte[] array2 = (array = x);
				int num = yLen;
				IntPtr intPtr = (IntPtr)num;
				array2[num] = (byte)(array[(long)intPtr] ^ y[yOff + yLen]);
			}
		}

		internal static void Xor(byte[] x, byte[] y, byte[] z)
		{
			int num = 0;
			do
			{
				z[num] = (byte)(x[num] ^ y[num]);
				num++;
				z[num] = (byte)(x[num] ^ y[num]);
				num++;
				z[num] = (byte)(x[num] ^ y[num]);
				num++;
				z[num] = (byte)(x[num] ^ y[num]);
				num++;
			}
			while (num < 16);
		}

		internal static void Xor(uint[] x, uint[] y)
		{
			uint[] array;
			(array = x)[0] = array[0] ^ y[0];
			(array = x)[1] = array[1] ^ y[1];
			(array = x)[2] = array[2] ^ y[2];
			(array = x)[3] = array[3] ^ y[3];
		}

		internal static void Xor(uint[] x, uint[] y, uint[] z)
		{
			z[0] = x[0] ^ y[0];
			z[1] = x[1] ^ y[1];
			z[2] = x[2] ^ y[2];
			z[3] = x[3] ^ y[3];
		}

		internal static void Xor(ulong[] x, ulong[] y)
		{
			ulong[] array;
			(array = x)[0] = array[0] ^ y[0];
			(array = x)[1] = array[1] ^ y[1];
		}

		internal static void Xor(ulong[] x, ulong[] y, ulong[] z)
		{
			z[0] = x[0] ^ y[0];
			z[1] = x[1] ^ y[1];
		}
	}
}
