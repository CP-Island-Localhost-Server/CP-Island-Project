using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class KeccakDigest : IDigest, IMemoable
	{
		private static readonly ulong[] KeccakRoundConstants = KeccakInitializeRoundConstants();

		private static readonly int[] KeccakRhoOffsets = KeccakInitializeRhoOffsets();

		protected byte[] state = new byte[200];

		protected byte[] dataQueue = new byte[192];

		protected int rate;

		protected int bitsInQueue;

		protected int fixedOutputLength;

		protected bool squeezing;

		protected int bitsAvailableForSqueezing;

		protected byte[] chunk;

		protected byte[] oneByte;

		private ulong[] C = new ulong[5];

		private ulong[] tempA = new ulong[25];

		private ulong[] chiC = new ulong[5];

		public virtual string AlgorithmName
		{
			get
			{
				return "Keccak-" + fixedOutputLength;
			}
		}

		private static ulong[] KeccakInitializeRoundConstants()
		{
			ulong[] array = new ulong[24];
			byte b = 1;
			for (int i = 0; i < 24; i++)
			{
				array[i] = 0uL;
				for (int j = 0; j < 7; j++)
				{
					int num = (1 << j) - 1;
					if (((uint)b & (true ? 1u : 0u)) != 0)
					{
						ulong[] array2;
						ulong[] array3 = (array2 = array);
						int num2 = i;
						IntPtr intPtr = (IntPtr)num2;
						array3[num2] = array2[(long)intPtr] ^ (ulong)(1L << num);
					}
					bool flag = (b & 0x80) != 0;
					b <<= 1;
					if (flag)
					{
						b = (byte)(b ^ 0x71u);
					}
				}
			}
			return array;
		}

		private static int[] KeccakInitializeRhoOffsets()
		{
			int[] array = new int[25];
			int num = (array[0] = 0);
			int num2 = 1;
			int num3 = 0;
			for (int i = 1; i < 25; i++)
			{
				num = (num + i) & 0x3F;
				array[num2 % 5 + 5 * (num3 % 5)] = num;
				int num4 = num3 % 5;
				int num5 = (2 * num2 + 3 * num3) % 5;
				num2 = num4;
				num3 = num5;
			}
			return array;
		}

		private void ClearDataQueueSection(int off, int len)
		{
			for (int i = off; i != off + len; i++)
			{
				dataQueue[i] = 0;
			}
		}

		public KeccakDigest()
			: this(288)
		{
		}

		public KeccakDigest(int bitLength)
		{
			Init(bitLength);
		}

		public KeccakDigest(KeccakDigest source)
		{
			CopyIn(source);
		}

		private void CopyIn(KeccakDigest source)
		{
			Array.Copy(source.state, 0, state, 0, source.state.Length);
			Array.Copy(source.dataQueue, 0, dataQueue, 0, source.dataQueue.Length);
			rate = source.rate;
			bitsInQueue = source.bitsInQueue;
			fixedOutputLength = source.fixedOutputLength;
			squeezing = source.squeezing;
			bitsAvailableForSqueezing = source.bitsAvailableForSqueezing;
			chunk = Arrays.Clone(source.chunk);
			oneByte = Arrays.Clone(source.oneByte);
		}

		public virtual int GetDigestSize()
		{
			return fixedOutputLength / 8;
		}

		public virtual void Update(byte input)
		{
			oneByte[0] = input;
			Absorb(oneByte, 0, 8L);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int len)
		{
			Absorb(input, inOff, (long)len * 8L);
		}

		public virtual int DoFinal(byte[] output, int outOff)
		{
			Squeeze(output, outOff, fixedOutputLength);
			Reset();
			return GetDigestSize();
		}

		protected virtual int DoFinal(byte[] output, int outOff, byte partialByte, int partialBits)
		{
			if (partialBits > 0)
			{
				oneByte[0] = partialByte;
				Absorb(oneByte, 0, partialBits);
			}
			Squeeze(output, outOff, fixedOutputLength);
			Reset();
			return GetDigestSize();
		}

		public virtual void Reset()
		{
			Init(fixedOutputLength);
		}

		public virtual int GetByteLength()
		{
			return rate / 8;
		}

		private void Init(int bitLength)
		{
			switch (bitLength)
			{
			case 128:
				InitSponge(1344, 256);
				break;
			case 224:
				InitSponge(1152, 448);
				break;
			case 256:
				InitSponge(1088, 512);
				break;
			case 288:
				InitSponge(1024, 576);
				break;
			case 384:
				InitSponge(832, 768);
				break;
			case 512:
				InitSponge(576, 1024);
				break;
			default:
				throw new ArgumentException("must be one of 128, 224, 256, 288, 384, or 512.", "bitLength");
			}
		}

		private void InitSponge(int rate, int capacity)
		{
			if (rate + capacity != 1600)
			{
				throw new InvalidOperationException("rate + capacity != 1600");
			}
			if (rate <= 0 || rate >= 1600 || rate % 64 != 0)
			{
				throw new InvalidOperationException("invalid rate value");
			}
			this.rate = rate;
			fixedOutputLength = 0;
			Arrays.Fill(state, 0);
			Arrays.Fill(dataQueue, 0);
			bitsInQueue = 0;
			squeezing = false;
			bitsAvailableForSqueezing = 0;
			fixedOutputLength = capacity / 2;
			chunk = new byte[rate / 8];
			oneByte = new byte[1];
		}

		private void AbsorbQueue()
		{
			KeccakAbsorb(state, dataQueue, rate / 8);
			bitsInQueue = 0;
		}

		protected virtual void Absorb(byte[] data, int off, long databitlen)
		{
			if (bitsInQueue % 8 != 0)
			{
				throw new InvalidOperationException("attempt to absorb with odd length queue.");
			}
			if (squeezing)
			{
				throw new InvalidOperationException("attempt to absorb while squeezing.");
			}
			long num = 0L;
			while (num < databitlen)
			{
				if (bitsInQueue == 0 && databitlen >= rate && num <= databitlen - rate)
				{
					long num2 = (databitlen - num) / rate;
					for (long num3 = 0L; num3 < num2; num3++)
					{
						Array.Copy(data, (int)(off + num / 8 + num3 * chunk.Length), chunk, 0, chunk.Length);
						KeccakAbsorb(state, chunk, chunk.Length);
					}
					num += num2 * rate;
					continue;
				}
				int num4 = (int)(databitlen - num);
				if (num4 + bitsInQueue > rate)
				{
					num4 = rate - bitsInQueue;
				}
				int num5 = num4 % 8;
				num4 -= num5;
				Array.Copy(data, off + (int)(num / 8), dataQueue, bitsInQueue / 8, num4 / 8);
				bitsInQueue += num4;
				num += num4;
				if (bitsInQueue == rate)
				{
					AbsorbQueue();
				}
				if (num5 > 0)
				{
					int num6 = (1 << num5) - 1;
					dataQueue[bitsInQueue / 8] = (byte)(data[off + (int)(num / 8)] & num6);
					bitsInQueue += num5;
					num += num5;
				}
			}
		}

		private void PadAndSwitchToSqueezingPhase()
		{
			byte[] array;
			IntPtr intPtr;
			if (bitsInQueue + 1 == rate)
			{
				byte[] array2 = (array = dataQueue);
				int num = bitsInQueue / 8;
				intPtr = (IntPtr)num;
				array2[num] = (byte)(array[(long)intPtr] | (byte)(1 << bitsInQueue % 8));
				AbsorbQueue();
				ClearDataQueueSection(0, rate / 8);
			}
			else
			{
				ClearDataQueueSection((bitsInQueue + 7) / 8, rate / 8 - (bitsInQueue + 7) / 8);
				byte[] array3 = (array = dataQueue);
				int num2 = bitsInQueue / 8;
				intPtr = (IntPtr)num2;
				array3[num2] = (byte)(array[(long)intPtr] | (byte)(1 << bitsInQueue % 8));
			}
			byte[] array4 = (array = dataQueue);
			int num3 = (rate - 1) / 8;
			intPtr = (IntPtr)num3;
			array4[num3] = (byte)(array[(long)intPtr] | (byte)(1 << (rate - 1) % 8));
			AbsorbQueue();
			if (rate == 1024)
			{
				KeccakExtract1024bits(state, dataQueue);
				bitsAvailableForSqueezing = 1024;
			}
			else
			{
				KeccakExtract(state, dataQueue, rate / 64);
				bitsAvailableForSqueezing = rate;
			}
			squeezing = true;
		}

		protected virtual void Squeeze(byte[] output, int offset, long outputLength)
		{
			if (!squeezing)
			{
				PadAndSwitchToSqueezingPhase();
			}
			if (outputLength % 8 != 0)
			{
				throw new InvalidOperationException("outputLength not a multiple of 8");
			}
			int num2;
			for (long num = 0L; num < outputLength; num += num2)
			{
				if (bitsAvailableForSqueezing == 0)
				{
					KeccakPermutation(state);
					if (rate == 1024)
					{
						KeccakExtract1024bits(state, dataQueue);
						bitsAvailableForSqueezing = 1024;
					}
					else
					{
						KeccakExtract(state, dataQueue, rate / 64);
						bitsAvailableForSqueezing = rate;
					}
				}
				num2 = bitsAvailableForSqueezing;
				if (num2 > outputLength - num)
				{
					num2 = (int)(outputLength - num);
				}
				Array.Copy(dataQueue, (rate - bitsAvailableForSqueezing) / 8, output, offset + (int)(num / 8), num2 / 8);
				bitsAvailableForSqueezing -= num2;
			}
		}

		private static void FromBytesToWords(ulong[] stateAsWords, byte[] state)
		{
			for (int i = 0; i < 25; i++)
			{
				stateAsWords[i] = 0uL;
				int num = i * 8;
				for (int j = 0; j < 8; j++)
				{
					ulong[] array;
					ulong[] array2 = (array = stateAsWords);
					int num2 = i;
					IntPtr intPtr = (IntPtr)num2;
					array2[num2] = array[(long)intPtr] | (((ulong)state[num + j] & 0xFFuL) << 8 * j);
				}
			}
		}

		private static void FromWordsToBytes(byte[] state, ulong[] stateAsWords)
		{
			for (int i = 0; i < 25; i++)
			{
				int num = i * 8;
				for (int j = 0; j < 8; j++)
				{
					state[num + j] = (byte)(stateAsWords[i] >> 8 * j);
				}
			}
		}

		private void KeccakPermutation(byte[] state)
		{
			ulong[] stateAsWords = new ulong[state.Length / 8];
			FromBytesToWords(stateAsWords, state);
			KeccakPermutationOnWords(stateAsWords);
			FromWordsToBytes(state, stateAsWords);
		}

		private void KeccakPermutationAfterXor(byte[] state, byte[] data, int dataLengthInBytes)
		{
			for (int i = 0; i < dataLengthInBytes; i++)
			{
				byte[] array;
				byte[] array2 = (array = state);
				int num = i;
				IntPtr intPtr = (IntPtr)num;
				array2[num] = (byte)(array[(long)intPtr] ^ data[i]);
			}
			KeccakPermutation(state);
		}

		private void KeccakPermutationOnWords(ulong[] state)
		{
			for (int i = 0; i < 24; i++)
			{
				Theta(state);
				Rho(state);
				Pi(state);
				Chi(state);
				Iota(state, i);
			}
		}

		private void Theta(ulong[] A)
		{
			for (int i = 0; i < 5; i++)
			{
				C[i] = 0uL;
				for (int j = 0; j < 5; j++)
				{
					ulong[] c;
					ulong[] array = (c = C);
					int num = i;
					IntPtr intPtr = (IntPtr)num;
					array[num] = c[(long)intPtr] ^ A[i + 5 * j];
				}
			}
			for (int k = 0; k < 5; k++)
			{
				ulong num2 = (C[(k + 1) % 5] << 1) ^ (C[(k + 1) % 5] >> 63) ^ C[(k + 4) % 5];
				for (int l = 0; l < 5; l++)
				{
					ulong[] c;
					ulong[] array2 = (c = A);
					int num3 = k + 5 * l;
					IntPtr intPtr = (IntPtr)num3;
					array2[num3] = c[(long)intPtr] ^ num2;
				}
			}
		}

		private void Rho(ulong[] A)
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					int num = i + 5 * j;
					A[num] = ((KeccakRhoOffsets[num] != 0) ? ((A[num] << KeccakRhoOffsets[num]) ^ (A[num] >> 64 - KeccakRhoOffsets[num])) : A[num]);
				}
			}
		}

		private void Pi(ulong[] A)
		{
			Array.Copy(A, 0, tempA, 0, tempA.Length);
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					A[j + 5 * ((2 * i + 3 * j) % 5)] = tempA[i + 5 * j];
				}
			}
		}

		private void Chi(ulong[] A)
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					chiC[j] = A[j + 5 * i] ^ (~A[(j + 1) % 5 + 5 * i] & A[(j + 2) % 5 + 5 * i]);
				}
				for (int k = 0; k < 5; k++)
				{
					A[k + 5 * i] = chiC[k];
				}
			}
		}

		private static void Iota(ulong[] A, int indexRound)
		{
			ulong[] array;
			(array = A)[0] = array[0] ^ KeccakRoundConstants[indexRound];
		}

		private void KeccakAbsorb(byte[] byteState, byte[] data, int dataInBytes)
		{
			KeccakPermutationAfterXor(byteState, data, dataInBytes);
		}

		private void KeccakExtract1024bits(byte[] byteState, byte[] data)
		{
			Array.Copy(byteState, 0, data, 0, 128);
		}

		private void KeccakExtract(byte[] byteState, byte[] data, int laneCount)
		{
			Array.Copy(byteState, 0, data, 0, laneCount * 8);
		}

		public virtual IMemoable Copy()
		{
			return new KeccakDigest(this);
		}

		public virtual void Reset(IMemoable other)
		{
			KeccakDigest source = (KeccakDigest)other;
			CopyIn(source);
		}
	}
}
