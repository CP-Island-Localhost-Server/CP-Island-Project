using System;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Encryption
{
	internal class PkzipClassicEncryptCryptoTransform : PkzipClassicCryptoBase, ICryptoTransform, IDisposable
	{
		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		public int InputBlockSize
		{
			get
			{
				return 1;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return 1;
			}
		}

		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		internal PkzipClassicEncryptCryptoTransform(byte[] keyBlock)
		{
			SetKeys(keyBlock);
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			byte[] array = new byte[inputCount];
			TransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
			return array;
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			for (int i = inputOffset; i < inputOffset + inputCount; i++)
			{
				byte ch = inputBuffer[i];
				outputBuffer[outputOffset++] = (byte)(inputBuffer[i] ^ TransformByte());
				UpdateKeys(ch);
			}
			return inputCount;
		}

		public void Dispose()
		{
			Reset();
		}
	}
}
