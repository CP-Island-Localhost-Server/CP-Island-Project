namespace Org.BouncyCastle.Math.Raw
{
	internal abstract class Interleave
	{
		private const ulong M32 = 1431655765uL;

		private const ulong M64 = 6148914691236517205uL;

		internal static uint Expand8to16(uint x)
		{
			x &= 0xFFu;
			x = (x | (x << 4)) & 0xF0Fu;
			x = (x | (x << 2)) & 0x3333u;
			x = (x | (x << 1)) & 0x5555u;
			return x;
		}

		internal static uint Expand16to32(uint x)
		{
			x &= 0xFFFFu;
			x = (x | (x << 8)) & 0xFF00FFu;
			x = (x | (x << 4)) & 0xF0F0F0Fu;
			x = (x | (x << 2)) & 0x33333333u;
			x = (x | (x << 1)) & 0x55555555u;
			return x;
		}

		internal static ulong Expand32to64(uint x)
		{
			uint num = (x ^ (x >> 8)) & 0xFF00u;
			x ^= num ^ (num << 8);
			num = (x ^ (x >> 4)) & 0xF000F0u;
			x ^= num ^ (num << 4);
			num = (x ^ (x >> 2)) & 0xC0C0C0Cu;
			x ^= num ^ (num << 2);
			num = (x ^ (x >> 1)) & 0x22222222u;
			x ^= num ^ (num << 1);
			return (((ulong)(x >> 1) & 0x55555555uL) << 32) | ((ulong)x & 0x55555555uL);
		}

		internal static void Expand64To128(ulong x, ulong[] z, int zOff)
		{
			ulong num = (x ^ (x >> 16)) & 0xFFFF0000u;
			x ^= num ^ (num << 16);
			num = (x ^ (x >> 8)) & 0xFF000000FF00uL;
			x ^= num ^ (num << 8);
			num = (x ^ (x >> 4)) & 0xF000F000F000F0uL;
			x ^= num ^ (num << 4);
			num = (x ^ (x >> 2)) & 0xC0C0C0C0C0C0C0CuL;
			x ^= num ^ (num << 2);
			num = (x ^ (x >> 1)) & 0x2222222222222222uL;
			x ^= num ^ (num << 1);
			z[zOff] = x & 0x5555555555555555uL;
			z[zOff + 1] = (x >> 1) & 0x5555555555555555uL;
		}

		internal static ulong Unshuffle(ulong x)
		{
			ulong num = (x ^ (x >> 1)) & 0x2222222222222222uL;
			x ^= num ^ (num << 1);
			num = (x ^ (x >> 2)) & 0xC0C0C0C0C0C0C0CuL;
			x ^= num ^ (num << 2);
			num = (x ^ (x >> 4)) & 0xF000F000F000F0uL;
			x ^= num ^ (num << 4);
			num = (x ^ (x >> 8)) & 0xFF000000FF00uL;
			x ^= num ^ (num << 8);
			num = (x ^ (x >> 16)) & 0xFFFF0000u;
			x ^= num ^ (num << 16);
			return x;
		}
	}
}
