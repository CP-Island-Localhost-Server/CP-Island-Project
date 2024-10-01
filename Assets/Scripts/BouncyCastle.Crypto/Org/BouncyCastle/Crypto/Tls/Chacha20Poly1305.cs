using System;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class Chacha20Poly1305 : TlsCipher
	{
		protected readonly TlsContext context;

		protected readonly ChaChaEngine encryptCipher;

		protected readonly ChaChaEngine decryptCipher;

		public Chacha20Poly1305(TlsContext context)
		{
			if (!TlsUtilities.IsTlsV12(context))
			{
				throw new TlsFatalAlert(80);
			}
			this.context = context;
			byte[] key = TlsUtilities.CalculateKeyBlock(context, 64);
			KeyParameter keyParameter = new KeyParameter(key, 0, 32);
			KeyParameter keyParameter2 = new KeyParameter(key, 32, 32);
			encryptCipher = new ChaChaEngine(20);
			decryptCipher = new ChaChaEngine(20);
			KeyParameter parameters;
			KeyParameter parameters2;
			if (context.IsServer)
			{
				parameters = keyParameter2;
				parameters2 = keyParameter;
			}
			else
			{
				parameters = keyParameter;
				parameters2 = keyParameter2;
			}
			byte[] iv = new byte[8];
			encryptCipher.Init(true, new ParametersWithIV(parameters, iv));
			decryptCipher.Init(false, new ParametersWithIV(parameters2, iv));
		}

		public virtual int GetPlaintextLimit(int ciphertextLimit)
		{
			return ciphertextLimit - 16;
		}

		public virtual byte[] EncodePlaintext(long seqNo, byte type, byte[] plaintext, int offset, int len)
		{
			int num = len + 16;
			KeyParameter macKey = InitRecordMac(encryptCipher, true, seqNo);
			byte[] array = new byte[num];
			encryptCipher.ProcessBytes(plaintext, offset, len, array, 0);
			byte[] additionalData = GetAdditionalData(seqNo, type, len);
			byte[] array2 = CalculateRecordMac(macKey, additionalData, array, 0, len);
			Array.Copy(array2, 0, array, len, array2.Length);
			return array;
		}

		public virtual byte[] DecodeCiphertext(long seqNo, byte type, byte[] ciphertext, int offset, int len)
		{
			if (GetPlaintextLimit(len) < 0)
			{
				throw new TlsFatalAlert(50);
			}
			int num = len - 16;
			byte[] b = Arrays.CopyOfRange(ciphertext, offset + num, offset + len);
			KeyParameter macKey = InitRecordMac(decryptCipher, false, seqNo);
			byte[] additionalData = GetAdditionalData(seqNo, type, num);
			byte[] a = CalculateRecordMac(macKey, additionalData, ciphertext, offset, num);
			if (!Arrays.ConstantTimeAreEqual(a, b))
			{
				throw new TlsFatalAlert(20);
			}
			byte[] array = new byte[num];
			decryptCipher.ProcessBytes(ciphertext, offset, num, array, 0);
			return array;
		}

		protected virtual KeyParameter InitRecordMac(ChaChaEngine cipher, bool forEncryption, long seqNo)
		{
			byte[] array = new byte[8];
			TlsUtilities.WriteUint64(seqNo, array, 0);
			cipher.Init(forEncryption, new ParametersWithIV(null, array));
			byte[] array2 = new byte[64];
			cipher.ProcessBytes(array2, 0, array2.Length, array2, 0);
			Array.Copy(array2, 0, array2, 32, 16);
			KeyParameter keyParameter = new KeyParameter(array2, 16, 32);
			Poly1305KeyGenerator.Clamp(keyParameter.GetKey());
			return keyParameter;
		}

		protected virtual byte[] CalculateRecordMac(KeyParameter macKey, byte[] additionalData, byte[] buf, int off, int len)
		{
			IMac mac = new Poly1305();
			mac.Init(macKey);
			UpdateRecordMac(mac, additionalData, 0, additionalData.Length);
			UpdateRecordMac(mac, buf, off, len);
			return MacUtilities.DoFinal(mac);
		}

		protected virtual void UpdateRecordMac(IMac mac, byte[] buf, int off, int len)
		{
			mac.BlockUpdate(buf, off, len);
			byte[] array = Pack.UInt64_To_LE((ulong)len);
			mac.BlockUpdate(array, 0, array.Length);
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
