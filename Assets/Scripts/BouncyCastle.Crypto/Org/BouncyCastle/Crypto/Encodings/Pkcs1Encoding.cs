using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Encodings
{
	public class Pkcs1Encoding : IAsymmetricBlockCipher
	{
		public const string StrictLengthEnabledProperty = "Org.BouncyCastle.Pkcs1.Strict";

		private const int HeaderLength = 10;

		private static readonly bool[] strictLengthEnabled;

		private SecureRandom random;

		private IAsymmetricBlockCipher engine;

		private bool forEncryption;

		private bool forPrivateKey;

		private bool useStrictLength;

		private int pLen = -1;

		private byte[] fallback = null;

		public static bool StrictLengthEnabled
		{
			get
			{
				return strictLengthEnabled[0];
			}
			set
			{
				strictLengthEnabled[0] = value;
			}
		}

		public string AlgorithmName
		{
			get
			{
				return engine.AlgorithmName + "/PKCS1Padding";
			}
		}

		static Pkcs1Encoding()
		{
			string environmentVariable = Platform.GetEnvironmentVariable("Org.BouncyCastle.Pkcs1.Strict");
			strictLengthEnabled = new bool[1] { environmentVariable == null || environmentVariable.Equals("true") };
		}

		public Pkcs1Encoding(IAsymmetricBlockCipher cipher)
		{
			engine = cipher;
			useStrictLength = StrictLengthEnabled;
		}

		public Pkcs1Encoding(IAsymmetricBlockCipher cipher, int pLen)
		{
			engine = cipher;
			useStrictLength = StrictLengthEnabled;
			this.pLen = pLen;
		}

		public Pkcs1Encoding(IAsymmetricBlockCipher cipher, byte[] fallback)
		{
			engine = cipher;
			useStrictLength = StrictLengthEnabled;
			this.fallback = fallback;
			pLen = fallback.Length;
		}

		public IAsymmetricBlockCipher GetUnderlyingCipher()
		{
			return engine;
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			AsymmetricKeyParameter asymmetricKeyParameter;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				random = parametersWithRandom.Random;
				asymmetricKeyParameter = (AsymmetricKeyParameter)parametersWithRandom.Parameters;
			}
			else
			{
				random = new SecureRandom();
				asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
			}
			engine.Init(forEncryption, parameters);
			forPrivateKey = asymmetricKeyParameter.IsPrivate;
			this.forEncryption = forEncryption;
		}

		public int GetInputBlockSize()
		{
			int inputBlockSize = engine.GetInputBlockSize();
			if (!forEncryption)
			{
				return inputBlockSize;
			}
			return inputBlockSize - 10;
		}

		public int GetOutputBlockSize()
		{
			int outputBlockSize = engine.GetOutputBlockSize();
			if (!forEncryption)
			{
				return outputBlockSize - 10;
			}
			return outputBlockSize;
		}

		public byte[] ProcessBlock(byte[] input, int inOff, int length)
		{
			if (!forEncryption)
			{
				return DecodeBlock(input, inOff, length);
			}
			return EncodeBlock(input, inOff, length);
		}

		private byte[] EncodeBlock(byte[] input, int inOff, int inLen)
		{
			if (inLen > GetInputBlockSize())
			{
				throw new ArgumentException("input data too large", "inLen");
			}
			byte[] array = new byte[engine.GetInputBlockSize()];
			if (forPrivateKey)
			{
				array[0] = 1;
				for (int i = 1; i != array.Length - inLen - 1; i++)
				{
					array[i] = byte.MaxValue;
				}
			}
			else
			{
				random.NextBytes(array);
				array[0] = 2;
				for (int j = 1; j != array.Length - inLen - 1; j++)
				{
					while (array[j] == 0)
					{
						array[j] = (byte)random.NextInt();
					}
				}
			}
			array[array.Length - inLen - 1] = 0;
			Array.Copy(input, inOff, array, array.Length - inLen, inLen);
			return engine.ProcessBlock(array, 0, array.Length);
		}

		private static int CheckPkcs1Encoding(byte[] encoded, int pLen)
		{
			int num = 0;
			num |= encoded[0] ^ 2;
			int num2 = encoded.Length - (pLen + 1);
			for (int i = 1; i < num2; i++)
			{
				int num3 = encoded[i];
				num3 |= num3 >> 1;
				num3 |= num3 >> 2;
				num3 |= num3 >> 4;
				num |= (num3 & 1) - 1;
			}
			num |= encoded[encoded.Length - (pLen + 1)];
			num |= num >> 1;
			num |= num >> 2;
			num |= num >> 4;
			return ~((num & 1) - 1);
		}

		private byte[] DecodeBlockOrRandom(byte[] input, int inOff, int inLen)
		{
			if (!forPrivateKey)
			{
				throw new InvalidCipherTextException("sorry, this method is only for decryption, not for signing");
			}
			byte[] array = engine.ProcessBlock(input, inOff, inLen);
			byte[] array2 = null;
			if (fallback == null)
			{
				array2 = new byte[pLen];
				random.NextBytes(array2);
			}
			else
			{
				array2 = fallback;
			}
			if (array.Length < GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block truncated");
			}
			if (useStrictLength && array.Length != engine.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block incorrect size");
			}
			int num = CheckPkcs1Encoding(array, pLen);
			byte[] array3 = new byte[pLen];
			for (int i = 0; i < pLen; i++)
			{
				array3[i] = (byte)((array[i + (array.Length - pLen)] & ~num) | (array2[i] & num));
			}
			return array3;
		}

		private byte[] DecodeBlock(byte[] input, int inOff, int inLen)
		{
			if (pLen != -1)
			{
				return DecodeBlockOrRandom(input, inOff, inLen);
			}
			byte[] array = engine.ProcessBlock(input, inOff, inLen);
			if (array.Length < GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block truncated");
			}
			byte b = array[0];
			if (b != 1 && b != 2)
			{
				throw new InvalidCipherTextException("unknown block type");
			}
			if (useStrictLength && array.Length != engine.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block incorrect size");
			}
			int i;
			for (i = 1; i != array.Length; i++)
			{
				byte b2 = array[i];
				if (b2 == 0)
				{
					break;
				}
				if (b == 1 && b2 != byte.MaxValue)
				{
					throw new InvalidCipherTextException("block padding incorrect");
				}
			}
			i++;
			if (i > array.Length || i < 10)
			{
				throw new InvalidCipherTextException("no data in block");
			}
			byte[] array2 = new byte[array.Length - i];
			Array.Copy(array, i, array2, 0, array2.Length);
			return array2;
		}
	}
}
