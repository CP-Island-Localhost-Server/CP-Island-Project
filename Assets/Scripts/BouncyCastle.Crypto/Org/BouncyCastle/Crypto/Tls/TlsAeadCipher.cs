using System;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsAeadCipher : TlsCipher
	{
		protected readonly TlsContext context;

		protected readonly int macSize;

		protected readonly int nonce_explicit_length;

		protected readonly IAeadBlockCipher encryptCipher;

		protected readonly IAeadBlockCipher decryptCipher;

		protected readonly byte[] encryptImplicitNonce;

		protected readonly byte[] decryptImplicitNonce;

		public TlsAeadCipher(TlsContext context, IAeadBlockCipher clientWriteCipher, IAeadBlockCipher serverWriteCipher, int cipherKeySize, int macSize)
		{
			if (!TlsUtilities.IsTlsV12(context))
			{
				throw new TlsFatalAlert(80);
			}
			this.context = context;
			this.macSize = macSize;
			nonce_explicit_length = 8;
			int num = 4;
			int num2 = 2 * cipherKeySize + 2 * num;
			byte[] array = TlsUtilities.CalculateKeyBlock(context, num2);
			int num3 = 0;
			KeyParameter keyParameter = new KeyParameter(array, num3, cipherKeySize);
			num3 += cipherKeySize;
			KeyParameter keyParameter2 = new KeyParameter(array, num3, cipherKeySize);
			num3 += cipherKeySize;
			byte[] array2 = Arrays.CopyOfRange(array, num3, num3 + num);
			num3 += num;
			byte[] array3 = Arrays.CopyOfRange(array, num3, num3 + num);
			num3 += num;
			if (num3 != num2)
			{
				throw new TlsFatalAlert(80);
			}
			KeyParameter key;
			KeyParameter key2;
			if (context.IsServer)
			{
				encryptCipher = serverWriteCipher;
				decryptCipher = clientWriteCipher;
				encryptImplicitNonce = array3;
				decryptImplicitNonce = array2;
				key = keyParameter2;
				key2 = keyParameter;
			}
			else
			{
				encryptCipher = clientWriteCipher;
				decryptCipher = serverWriteCipher;
				encryptImplicitNonce = array2;
				decryptImplicitNonce = array3;
				key = keyParameter;
				key2 = keyParameter2;
			}
			byte[] nonce = new byte[num + nonce_explicit_length];
			encryptCipher.Init(true, new AeadParameters(key, 8 * macSize, nonce));
			decryptCipher.Init(false, new AeadParameters(key2, 8 * macSize, nonce));
		}

		public virtual int GetPlaintextLimit(int ciphertextLimit)
		{
			return ciphertextLimit - macSize - nonce_explicit_length;
		}

		public virtual byte[] EncodePlaintext(long seqNo, byte type, byte[] plaintext, int offset, int len)
		{
			byte[] array = new byte[encryptImplicitNonce.Length + nonce_explicit_length];
			Array.Copy(encryptImplicitNonce, 0, array, 0, encryptImplicitNonce.Length);
			TlsUtilities.WriteUint64(seqNo, array, encryptImplicitNonce.Length);
			int outputSize = encryptCipher.GetOutputSize(len);
			byte[] array2 = new byte[nonce_explicit_length + outputSize];
			Array.Copy(array, encryptImplicitNonce.Length, array2, 0, nonce_explicit_length);
			int num = nonce_explicit_length;
			byte[] additionalData = GetAdditionalData(seqNo, type, len);
			AeadParameters parameters = new AeadParameters(null, 8 * macSize, array, additionalData);
			try
			{
				encryptCipher.Init(true, parameters);
				num += encryptCipher.ProcessBytes(plaintext, offset, len, array2, num);
				num += encryptCipher.DoFinal(array2, num);
			}
			catch (Exception alertCause)
			{
				throw new TlsFatalAlert(80, alertCause);
			}
			if (num != array2.Length)
			{
				throw new TlsFatalAlert(80);
			}
			return array2;
		}

		public virtual byte[] DecodeCiphertext(long seqNo, byte type, byte[] ciphertext, int offset, int len)
		{
			if (GetPlaintextLimit(len) < 0)
			{
				throw new TlsFatalAlert(50);
			}
			byte[] array = new byte[decryptImplicitNonce.Length + nonce_explicit_length];
			Array.Copy(decryptImplicitNonce, 0, array, 0, decryptImplicitNonce.Length);
			Array.Copy(ciphertext, offset, array, decryptImplicitNonce.Length, nonce_explicit_length);
			int inOff = offset + nonce_explicit_length;
			int len2 = len - nonce_explicit_length;
			int outputSize = decryptCipher.GetOutputSize(len2);
			byte[] array2 = new byte[outputSize];
			int num = 0;
			byte[] additionalData = GetAdditionalData(seqNo, type, outputSize);
			AeadParameters parameters = new AeadParameters(null, 8 * macSize, array, additionalData);
			try
			{
				decryptCipher.Init(false, parameters);
				num += decryptCipher.ProcessBytes(ciphertext, inOff, len2, array2, num);
				num += decryptCipher.DoFinal(array2, num);
			}
			catch (Exception alertCause)
			{
				throw new TlsFatalAlert(20, alertCause);
			}
			if (num != array2.Length)
			{
				throw new TlsFatalAlert(80);
			}
			return array2;
		}

		protected virtual byte[] GetAdditionalData(long seqNo, byte type, int len)
		{
			byte[] array = new byte[13];
			TlsUtilities.WriteUint64(seqNo, array, 0);
			TlsUtilities.WriteUint8(type, array, 8);
			TlsUtilities.WriteVersion(context.ServerVersion, array, 9);
			TlsUtilities.WriteUint16(len, array, 11);
			return array;
		}
	}
}
