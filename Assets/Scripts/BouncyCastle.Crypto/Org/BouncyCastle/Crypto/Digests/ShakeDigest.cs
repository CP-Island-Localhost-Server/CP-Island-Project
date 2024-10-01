using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class ShakeDigest : KeccakDigest, IXof, IDigest
	{
		public override string AlgorithmName
		{
			get
			{
				return "SHAKE" + fixedOutputLength;
			}
		}

		private static int CheckBitLength(int bitLength)
		{
			if (bitLength == 128 || bitLength == 256)
			{
				return bitLength;
			}
			throw new ArgumentException(bitLength + " not supported for SHAKE", "bitLength");
		}

		public ShakeDigest()
			: this(128)
		{
		}

		public ShakeDigest(int bitLength)
			: base(CheckBitLength(bitLength))
		{
		}

		public ShakeDigest(ShakeDigest source)
			: base(source)
		{
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			return DoFinal(output, outOff, GetDigestSize());
		}

		public virtual int DoFinal(byte[] output, int outOff, int outLen)
		{
			Absorb(new byte[1] { 15 }, 0, 4L);
			Squeeze(output, outOff, (long)outLen * 8L);
			Reset();
			return outLen;
		}

		protected override int DoFinal(byte[] output, int outOff, byte partialByte, int partialBits)
		{
			return DoFinal(output, outOff, GetDigestSize(), partialByte, partialBits);
		}

		protected virtual int DoFinal(byte[] output, int outOff, int outLen, byte partialByte, int partialBits)
		{
			if (partialBits < 0 || partialBits > 7)
			{
				throw new ArgumentException("must be in the range [0,7]", "partialBits");
			}
			int num = (partialByte & ((1 << partialBits) - 1)) | (15 << partialBits);
			int num2 = partialBits + 4;
			if (num2 >= 8)
			{
				oneByte[0] = (byte)num;
				Absorb(oneByte, 0, 8L);
				num2 -= 8;
				num >>= 8;
			}
			if (num2 > 0)
			{
				oneByte[0] = (byte)num;
				Absorb(oneByte, 0, num2);
			}
			Squeeze(output, outOff, (long)outLen * 8L);
			Reset();
			return outLen;
		}

		public override IMemoable Copy()
		{
			return new ShakeDigest(this);
		}
	}
}
