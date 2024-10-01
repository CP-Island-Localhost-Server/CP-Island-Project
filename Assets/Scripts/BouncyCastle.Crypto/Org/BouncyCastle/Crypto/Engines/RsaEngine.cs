using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class RsaEngine : IAsymmetricBlockCipher
	{
		private RsaCoreEngine core;

		public virtual string AlgorithmName
		{
			get
			{
				return "RSA";
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (core == null)
			{
				core = new RsaCoreEngine();
			}
			core.Init(forEncryption, parameters);
		}

		public virtual int GetInputBlockSize()
		{
			return core.GetInputBlockSize();
		}

		public virtual int GetOutputBlockSize()
		{
			return core.GetOutputBlockSize();
		}

		public virtual byte[] ProcessBlock(byte[] inBuf, int inOff, int inLen)
		{
			if (core == null)
			{
				throw new InvalidOperationException("RSA engine not initialised");
			}
			return core.ConvertOutput(core.ProcessBlock(core.ConvertInput(inBuf, inOff, inLen)));
		}
	}
}
