using System;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class Rfc3211WrapEngine : IWrapper
	{
		private CbcBlockCipher engine;

		private ParametersWithIV param;

		private bool forWrapping;

		private SecureRandom rand;

		public virtual string AlgorithmName
		{
			get
			{
				return engine.GetUnderlyingCipher().AlgorithmName + "/RFC3211Wrap";
			}
		}

		public Rfc3211WrapEngine(IBlockCipher engine)
		{
			this.engine = new CbcBlockCipher(engine);
		}

		public virtual void Init(bool forWrapping, ICipherParameters param)
		{
			this.forWrapping = forWrapping;
			if (param is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)param;
				rand = parametersWithRandom.Random;
				this.param = (ParametersWithIV)parametersWithRandom.Parameters;
				return;
			}
			if (forWrapping)
			{
				rand = new SecureRandom();
			}
			this.param = (ParametersWithIV)param;
		}

		public virtual byte[] Wrap(byte[] inBytes, int inOff, int inLen)
		{
			if (!forWrapping)
			{
				throw new InvalidOperationException("not set for wrapping");
			}
			engine.Init(true, param);
			int blockSize = engine.GetBlockSize();
			byte[] array = ((inLen + 4 >= blockSize * 2) ? new byte[((inLen + 4) % blockSize == 0) ? (inLen + 4) : (((inLen + 4) / blockSize + 1) * blockSize)] : new byte[blockSize * 2]);
			array[0] = (byte)inLen;
			array[1] = (byte)(~inBytes[inOff]);
			array[2] = (byte)(~inBytes[inOff + 1]);
			array[3] = (byte)(~inBytes[inOff + 2]);
			Array.Copy(inBytes, inOff, array, 4, inLen);
			rand.NextBytes(array, inLen + 4, array.Length - inLen - 4);
			for (int i = 0; i < array.Length; i += blockSize)
			{
				engine.ProcessBlock(array, i, array, i);
			}
			for (int j = 0; j < array.Length; j += blockSize)
			{
				engine.ProcessBlock(array, j, array, j);
			}
			return array;
		}

		public virtual byte[] Unwrap(byte[] inBytes, int inOff, int inLen)
		{
			if (forWrapping)
			{
				throw new InvalidOperationException("not set for unwrapping");
			}
			int blockSize = engine.GetBlockSize();
			if (inLen < 2 * blockSize)
			{
				throw new InvalidCipherTextException("input too short");
			}
			byte[] array = new byte[inLen];
			byte[] array2 = new byte[blockSize];
			Array.Copy(inBytes, inOff, array, 0, inLen);
			Array.Copy(inBytes, inOff, array2, 0, array2.Length);
			engine.Init(false, new ParametersWithIV(param.Parameters, array2));
			for (int i = blockSize; i < array.Length; i += blockSize)
			{
				engine.ProcessBlock(array, i, array, i);
			}
			Array.Copy(array, array.Length - array2.Length, array2, 0, array2.Length);
			engine.Init(false, new ParametersWithIV(param.Parameters, array2));
			engine.ProcessBlock(array, 0, array, 0);
			engine.Init(false, param);
			for (int j = 0; j < array.Length; j += blockSize)
			{
				engine.ProcessBlock(array, j, array, j);
			}
			if ((array[0] & 0xFF) > array.Length - 4)
			{
				throw new InvalidCipherTextException("wrapped key corrupted");
			}
			byte[] array3 = new byte[array[0] & 0xFF];
			Array.Copy(array, 4, array3, 0, array[0]);
			int num = 0;
			for (int k = 0; k != 3; k++)
			{
				byte b = (byte)(~array[1 + k]);
				num |= b ^ array3[k];
			}
			if (num != 0)
			{
				throw new InvalidCipherTextException("wrapped key fails checksum");
			}
			return array3;
		}
	}
}
