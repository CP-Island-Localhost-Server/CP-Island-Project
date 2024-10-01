namespace Org.BouncyCastle.Crypto.Modes
{
	public interface IAeadBlockCipher
	{
		string AlgorithmName { get; }

		IBlockCipher GetUnderlyingCipher();

		void Init(bool forEncryption, ICipherParameters parameters);

		int GetBlockSize();

		void ProcessAadByte(byte input);

		void ProcessAadBytes(byte[] inBytes, int inOff, int len);

		int ProcessByte(byte input, byte[] outBytes, int outOff);

		int ProcessBytes(byte[] inBytes, int inOff, int len, byte[] outBytes, int outOff);

		int DoFinal(byte[] outBytes, int outOff);

		byte[] GetMac();

		int GetUpdateOutputSize(int len);

		int GetOutputSize(int len);

		void Reset();
	}
}
