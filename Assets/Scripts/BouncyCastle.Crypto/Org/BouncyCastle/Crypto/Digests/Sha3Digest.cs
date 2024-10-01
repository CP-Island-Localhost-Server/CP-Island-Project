using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class Sha3Digest : KeccakDigest
	{
		public override string AlgorithmName
		{
			get
			{
				return "SHA3-" + fixedOutputLength;
			}
		}

		private static int CheckBitLength(int bitLength)
		{
			switch (bitLength)
			{
			case 224:
			case 256:
			case 384:
			case 512:
				return bitLength;
			default:
				throw new ArgumentException(bitLength + " not supported for SHA-3", "bitLength");
			}
		}

		public Sha3Digest()
			: this(256)
		{
		}

		public Sha3Digest(int bitLength)
			: base(CheckBitLength(bitLength))
		{
		}

		public Sha3Digest(Sha3Digest source)
			: base(source)
		{
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			Absorb(new byte[1] { 2 }, 0, 2L);
			return base.DoFinal(output, outOff);
		}

		protected override int DoFinal(byte[] output, int outOff, byte partialByte, int partialBits)
		{
			if (partialBits < 0 || partialBits > 7)
			{
				throw new ArgumentException("must be in the range [0,7]", "partialBits");
			}
			int num = (partialByte & ((1 << partialBits) - 1)) | (2 << partialBits);
			int num2 = partialBits + 2;
			if (num2 >= 8)
			{
				oneByte[0] = (byte)num;
				Absorb(oneByte, 0, 8L);
				num2 -= 8;
				num >>= 8;
			}
			return base.DoFinal(output, outOff, (byte)num, num2);
		}

		public override IMemoable Copy()
		{
			return new Sha3Digest(this);
		}
	}
}
