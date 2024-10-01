using System;
using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Math.Raw
{
	internal abstract class Nat320
	{
		public static void Copy64(ulong[] x, ulong[] z)
		{
			z[0] = x[0];
			z[1] = x[1];
			z[2] = x[2];
			z[3] = x[3];
			z[4] = x[4];
		}

		public static ulong[] Create64()
		{
			return new ulong[5];
		}

		public static ulong[] CreateExt64()
		{
			return new ulong[10];
		}

		public static bool Eq64(ulong[] x, ulong[] y)
		{
			for (int num = 4; num >= 0; num--)
			{
				if (x[num] != y[num])
				{
					return false;
				}
			}
			return true;
		}

		public static ulong[] FromBigInteger64(BigInteger x)
		{
			if (x.SignValue < 0 || x.BitLength > 320)
			{
				throw new ArgumentException();
			}
			ulong[] array = Create64();
			int num = 0;
			while (x.SignValue != 0)
			{
				array[num++] = (ulong)x.LongValue;
				x = x.ShiftRight(64);
			}
			return array;
		}

		public static bool IsOne64(ulong[] x)
		{
			if (x[0] != 1)
			{
				return false;
			}
			for (int i = 1; i < 5; i++)
			{
				if (x[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsZero64(ulong[] x)
		{
			for (int i = 0; i < 5; i++)
			{
				if (x[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static BigInteger ToBigInteger64(ulong[] x)
		{
			byte[] array = new byte[40];
			for (int i = 0; i < 5; i++)
			{
				ulong num = x[i];
				if (num != 0)
				{
					Pack.UInt64_To_BE(num, array, 4 - i << 3);
				}
			}
			return new BigInteger(1, array);
		}
	}
}
