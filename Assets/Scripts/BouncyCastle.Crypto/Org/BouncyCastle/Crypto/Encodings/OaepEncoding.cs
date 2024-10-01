using System;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Encodings
{
	public class OaepEncoding : IAsymmetricBlockCipher
	{
		private byte[] defHash;

		private IDigest hash;

		private IDigest mgf1Hash;

		private IAsymmetricBlockCipher engine;

		private SecureRandom random;

		private bool forEncryption;

		public string AlgorithmName
		{
			get
			{
				return engine.AlgorithmName + "/OAEPPadding";
			}
		}

		public OaepEncoding(IAsymmetricBlockCipher cipher)
			: this(cipher, new Sha1Digest(), null)
		{
		}

		public OaepEncoding(IAsymmetricBlockCipher cipher, IDigest hash)
			: this(cipher, hash, null)
		{
		}

		public OaepEncoding(IAsymmetricBlockCipher cipher, IDigest hash, byte[] encodingParams)
			: this(cipher, hash, hash, encodingParams)
		{
		}

		public OaepEncoding(IAsymmetricBlockCipher cipher, IDigest hash, IDigest mgf1Hash, byte[] encodingParams)
		{
			engine = cipher;
			this.hash = hash;
			this.mgf1Hash = mgf1Hash;
			defHash = new byte[hash.GetDigestSize()];
			if (encodingParams != null)
			{
				hash.BlockUpdate(encodingParams, 0, encodingParams.Length);
			}
			hash.DoFinal(defHash, 0);
		}

		public IAsymmetricBlockCipher GetUnderlyingCipher()
		{
			return engine;
		}

		public void Init(bool forEncryption, ICipherParameters param)
		{
			if (param is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)param;
				random = parametersWithRandom.Random;
			}
			else
			{
				random = new SecureRandom();
			}
			engine.Init(forEncryption, param);
			this.forEncryption = forEncryption;
		}

		public int GetInputBlockSize()
		{
			int inputBlockSize = engine.GetInputBlockSize();
			if (forEncryption)
			{
				return inputBlockSize - 1 - 2 * defHash.Length;
			}
			return inputBlockSize;
		}

		public int GetOutputBlockSize()
		{
			int outputBlockSize = engine.GetOutputBlockSize();
			if (forEncryption)
			{
				return outputBlockSize;
			}
			return outputBlockSize - 1 - 2 * defHash.Length;
		}

		public byte[] ProcessBlock(byte[] inBytes, int inOff, int inLen)
		{
			if (forEncryption)
			{
				return EncodeBlock(inBytes, inOff, inLen);
			}
			return DecodeBlock(inBytes, inOff, inLen);
		}

		private byte[] EncodeBlock(byte[] inBytes, int inOff, int inLen)
		{
			byte[] array = new byte[GetInputBlockSize() + 1 + 2 * defHash.Length];
			Array.Copy(inBytes, inOff, array, array.Length - inLen, inLen);
			array[array.Length - inLen - 1] = 1;
			Array.Copy(defHash, 0, array, defHash.Length, defHash.Length);
			byte[] nextBytes = SecureRandom.GetNextBytes(random, defHash.Length);
			byte[] array2 = maskGeneratorFunction1(nextBytes, 0, nextBytes.Length, array.Length - defHash.Length);
			for (int i = defHash.Length; i != array.Length; i++)
			{
				byte[] array3;
				byte[] array4 = (array3 = array);
				int num = i;
				IntPtr intPtr = (IntPtr)num;
				array4[num] = (byte)(array3[(long)intPtr] ^ array2[i - defHash.Length]);
			}
			Array.Copy(nextBytes, 0, array, 0, defHash.Length);
			array2 = maskGeneratorFunction1(array, defHash.Length, array.Length - defHash.Length, defHash.Length);
			for (int j = 0; j != defHash.Length; j++)
			{
				byte[] array3;
				byte[] array5 = (array3 = array);
				int num2 = j;
				IntPtr intPtr = (IntPtr)num2;
				array5[num2] = (byte)(array3[(long)intPtr] ^ array2[j]);
			}
			return engine.ProcessBlock(array, 0, array.Length);
		}

		private byte[] DecodeBlock(byte[] inBytes, int inOff, int inLen)
		{
			byte[] array = engine.ProcessBlock(inBytes, inOff, inLen);
			byte[] array2;
			if (array.Length < engine.GetOutputBlockSize())
			{
				array2 = new byte[engine.GetOutputBlockSize()];
				Array.Copy(array, 0, array2, array2.Length - array.Length, array.Length);
			}
			else
			{
				array2 = array;
			}
			if (array2.Length < 2 * defHash.Length + 1)
			{
				throw new InvalidCipherTextException("data too short");
			}
			byte[] array3 = maskGeneratorFunction1(array2, defHash.Length, array2.Length - defHash.Length, defHash.Length);
			for (int i = 0; i != defHash.Length; i++)
			{
				byte[] array4;
				byte[] array5 = (array4 = array2);
				int num = i;
				IntPtr intPtr = (IntPtr)num;
				array5[num] = (byte)(array4[(long)intPtr] ^ array3[i]);
			}
			array3 = maskGeneratorFunction1(array2, 0, defHash.Length, array2.Length - defHash.Length);
			for (int j = defHash.Length; j != array2.Length; j++)
			{
				byte[] array4;
				byte[] array6 = (array4 = array2);
				int num2 = j;
				IntPtr intPtr = (IntPtr)num2;
				array6[num2] = (byte)(array4[(long)intPtr] ^ array3[j - defHash.Length]);
			}
			int num3 = 0;
			for (int k = 0; k < defHash.Length; k++)
			{
				num3 |= (byte)(defHash[k] ^ array2[defHash.Length + k]);
			}
			if (num3 != 0)
			{
				throw new InvalidCipherTextException("data hash wrong");
			}
			int l;
			for (l = 2 * defHash.Length; l != array2.Length && array2[l] == 0; l++)
			{
			}
			if (l >= array2.Length - 1 || array2[l] != 1)
			{
				throw new InvalidCipherTextException("data start wrong " + l);
			}
			l++;
			byte[] array7 = new byte[array2.Length - l];
			Array.Copy(array2, l, array7, 0, array7.Length);
			return array7;
		}

		private void ItoOSP(int i, byte[] sp)
		{
			sp[0] = (byte)((uint)i >> 24);
			sp[1] = (byte)((uint)i >> 16);
			sp[2] = (byte)((uint)i >> 8);
			sp[3] = (byte)i;
		}

		private byte[] maskGeneratorFunction1(byte[] Z, int zOff, int zLen, int length)
		{
			byte[] array = new byte[length];
			byte[] array2 = new byte[mgf1Hash.GetDigestSize()];
			byte[] array3 = new byte[4];
			int num = 0;
			hash.Reset();
			do
			{
				ItoOSP(num, array3);
				mgf1Hash.BlockUpdate(Z, zOff, zLen);
				mgf1Hash.BlockUpdate(array3, 0, array3.Length);
				mgf1Hash.DoFinal(array2, 0);
				Array.Copy(array2, 0, array, num * array2.Length, array2.Length);
			}
			while (++num < length / array2.Length);
			if (num * array2.Length < length)
			{
				ItoOSP(num, array3);
				mgf1Hash.BlockUpdate(Z, zOff, zLen);
				mgf1Hash.BlockUpdate(array3, 0, array3.Length);
				mgf1Hash.DoFinal(array2, 0);
				Array.Copy(array2, 0, array, num * array2.Length, array.Length - num * array2.Length);
			}
			return array;
		}
	}
}
