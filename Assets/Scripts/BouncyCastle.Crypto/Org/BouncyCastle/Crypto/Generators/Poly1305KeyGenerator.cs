using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class Poly1305KeyGenerator : CipherKeyGenerator
	{
		private const byte R_MASK_LOW_2 = 252;

		private const byte R_MASK_HIGH_4 = 15;

		protected override void engineInit(KeyGenerationParameters param)
		{
			random = param.Random;
			strength = 32;
		}

		protected override byte[] engineGenerateKey()
		{
			byte[] array = base.engineGenerateKey();
			Clamp(array);
			return array;
		}

		public static void Clamp(byte[] key)
		{
			if (key.Length != 32)
			{
				throw new ArgumentException("Poly1305 key must be 256 bits.");
			}
			byte[] array;
			(array = key)[19] = (byte)(array[19] & 0xFu);
			(array = key)[23] = (byte)(array[23] & 0xFu);
			(array = key)[27] = (byte)(array[27] & 0xFu);
			(array = key)[31] = (byte)(array[31] & 0xFu);
			(array = key)[20] = (byte)(array[20] & 0xFCu);
			(array = key)[24] = (byte)(array[24] & 0xFCu);
			(array = key)[28] = (byte)(array[28] & 0xFCu);
		}

		public static void CheckKey(byte[] key)
		{
			if (key.Length != 32)
			{
				throw new ArgumentException("Poly1305 key must be 256 bits.");
			}
			checkMask(key[19], 15);
			checkMask(key[23], 15);
			checkMask(key[27], 15);
			checkMask(key[31], 15);
			checkMask(key[20], 252);
			checkMask(key[24], 252);
			checkMask(key[28], 252);
		}

		private static void checkMask(byte b, byte mask)
		{
			if ((b & ~mask) != 0)
			{
				throw new ArgumentException("Invalid format for r portion of Poly1305 key.");
			}
		}
	}
}
