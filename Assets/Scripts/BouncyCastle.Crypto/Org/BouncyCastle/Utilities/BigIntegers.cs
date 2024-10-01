using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Utilities
{
	public abstract class BigIntegers
	{
		private const int MaxIterations = 1000;

		public static byte[] AsUnsignedByteArray(BigInteger n)
		{
			return n.ToByteArrayUnsigned();
		}

		public static byte[] AsUnsignedByteArray(int length, BigInteger n)
		{
			byte[] array = n.ToByteArrayUnsigned();
			if (array.Length > length)
			{
				throw new ArgumentException("standard length exceeded", "n");
			}
			if (array.Length == length)
			{
				return array;
			}
			byte[] array2 = new byte[length];
			Array.Copy(array, 0, array2, array2.Length - array.Length, array.Length);
			return array2;
		}

		public static BigInteger CreateRandomInRange(BigInteger min, BigInteger max, SecureRandom random)
		{
			int num = min.CompareTo(max);
			if (num >= 0)
			{
				if (num > 0)
				{
					throw new ArgumentException("'min' may not be greater than 'max'");
				}
				return min;
			}
			if (min.BitLength > max.BitLength / 2)
			{
				return CreateRandomInRange(BigInteger.Zero, max.Subtract(min), random).Add(min);
			}
			for (int i = 0; i < 1000; i++)
			{
				BigInteger bigInteger = new BigInteger(max.BitLength, random);
				if (bigInteger.CompareTo(min) >= 0 && bigInteger.CompareTo(max) <= 0)
				{
					return bigInteger;
				}
			}
			return new BigInteger(max.Subtract(min).BitLength - 1, random).Add(min);
		}
	}
}
