using System;
using System.IO;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class RecordStream
	{
		private const int DEFAULT_PLAINTEXT_LIMIT = 16384;

		internal const int TLS_HEADER_SIZE = 5;

		internal const int TLS_HEADER_TYPE_OFFSET = 0;

		internal const int TLS_HEADER_VERSION_OFFSET = 1;

		internal const int TLS_HEADER_LENGTH_OFFSET = 3;

		private TlsProtocol mHandler;

		private Stream mInput;

		private Stream mOutput;

		private TlsCompression mPendingCompression = null;

		private TlsCompression mReadCompression = null;

		private TlsCompression mWriteCompression = null;

		private TlsCipher mPendingCipher = null;

		private TlsCipher mReadCipher = null;

		private TlsCipher mWriteCipher = null;

		private long mReadSeqNo = 0L;

		private long mWriteSeqNo = 0L;

		private MemoryStream mBuffer = new MemoryStream();

		private TlsHandshakeHash mHandshakeHash = null;

		private ProtocolVersion mReadVersion = null;

		private ProtocolVersion mWriteVersion = null;

		private bool mRestrictReadVersion = true;

		private int mPlaintextLimit;

		private int mCompressedLimit;

		private int mCiphertextLimit;

		internal virtual ProtocolVersion ReadVersion
		{
			get
			{
				return mReadVersion;
			}
			set
			{
				mReadVersion = value;
			}
		}

		internal virtual TlsHandshakeHash HandshakeHash
		{
			get
			{
				return mHandshakeHash;
			}
		}

		internal RecordStream(TlsProtocol handler, Stream input, Stream output)
		{
			mHandler = handler;
			mInput = input;
			mOutput = output;
			mReadCompression = new TlsNullCompression();
			mWriteCompression = mReadCompression;
		}

		internal virtual void Init(TlsContext context)
		{
			mReadCipher = new TlsNullCipher(context);
			mWriteCipher = mReadCipher;
			mHandshakeHash = new DeferredHash();
			mHandshakeHash.Init(context);
			SetPlaintextLimit(16384);
		}

		internal virtual int GetPlaintextLimit()
		{
			return mPlaintextLimit;
		}

		internal virtual void SetPlaintextLimit(int plaintextLimit)
		{
			mPlaintextLimit = plaintextLimit;
			mCompressedLimit = mPlaintextLimit + 1024;
			mCiphertextLimit = mCompressedLimit + 1024;
		}

		internal virtual void SetWriteVersion(ProtocolVersion writeVersion)
		{
			mWriteVersion = writeVersion;
		}

		internal virtual void SetRestrictReadVersion(bool enabled)
		{
			mRestrictReadVersion = enabled;
		}

		internal virtual void SetPendingConnectionState(TlsCompression tlsCompression, TlsCipher tlsCipher)
		{
			mPendingCompression = tlsCompression;
			mPendingCipher = tlsCipher;
		}

		internal virtual void SentWriteCipherSpec()
		{
			if (mPendingCompression == null || mPendingCipher == null)
			{
				throw new TlsFatalAlert(40);
			}
			mWriteCompression = mPendingCompression;
			mWriteCipher = mPendingCipher;
			mWriteSeqNo = 0L;
		}

		internal virtual void ReceivedReadCipherSpec()
		{
			if (mPendingCompression == null || mPendingCipher == null)
			{
				throw new TlsFatalAlert(40);
			}
			mReadCompression = mPendingCompression;
			mReadCipher = mPendingCipher;
			mReadSeqNo = 0L;
		}

		internal virtual void FinaliseHandshake()
		{
			if (mReadCompression != mPendingCompression || mWriteCompression != mPendingCompression || mReadCipher != mPendingCipher || mWriteCipher != mPendingCipher)
			{
				throw new TlsFatalAlert(40);
			}
			mPendingCompression = null;
			mPendingCipher = null;
		}

		internal virtual bool ReadRecord()
		{
			byte[] array = TlsUtilities.ReadAllOrNothing(5, mInput);
			if (array == null)
			{
				return false;
			}
			byte b = TlsUtilities.ReadUint8(array, 0);
			CheckType(b, 10);
			if (!mRestrictReadVersion)
			{
				int num = TlsUtilities.ReadVersionRaw(array, 1);
				if ((num & 0xFFFFFF00u) != 768)
				{
					throw new TlsFatalAlert(47);
				}
			}
			else
			{
				ProtocolVersion protocolVersion = TlsUtilities.ReadVersion(array, 1);
				if (mReadVersion == null)
				{
					mReadVersion = protocolVersion;
				}
				else if (!protocolVersion.Equals(mReadVersion))
				{
					throw new TlsFatalAlert(47);
				}
			}
			int len = TlsUtilities.ReadUint16(array, 3);
			byte[] array2 = DecodeAndVerify(b, mInput, len);
			mHandler.ProcessRecord(b, array2, 0, array2.Length);
			return true;
		}

		internal virtual byte[] DecodeAndVerify(byte type, Stream input, int len)
		{
			CheckLength(len, mCiphertextLimit, 22);
			byte[] array = TlsUtilities.ReadFully(len, input);
			byte[] array2 = mReadCipher.DecodeCiphertext(mReadSeqNo++, type, array, 0, array.Length);
			CheckLength(array2.Length, mCompressedLimit, 22);
			Stream stream = mReadCompression.Decompress(mBuffer);
			if (stream != mBuffer)
			{
				stream.Write(array2, 0, array2.Length);
				stream.Flush();
				array2 = GetBufferContents();
			}
			CheckLength(array2.Length, mPlaintextLimit, 30);
			if (array2.Length < 1 && type != 23)
			{
				throw new TlsFatalAlert(47);
			}
			return array2;
		}

		internal virtual void WriteRecord(byte type, byte[] plaintext, int plaintextOffset, int plaintextLength)
		{
			if (mWriteVersion != null)
			{
				CheckType(type, 80);
				CheckLength(plaintextLength, mPlaintextLimit, 80);
				if (plaintextLength < 1 && type != 23)
				{
					throw new TlsFatalAlert(80);
				}
				if (type == 22)
				{
					UpdateHandshakeData(plaintext, plaintextOffset, plaintextLength);
				}
				Stream stream = mWriteCompression.Compress(mBuffer);
				byte[] array;
				if (stream == mBuffer)
				{
					array = mWriteCipher.EncodePlaintext(mWriteSeqNo++, type, plaintext, plaintextOffset, plaintextLength);
				}
				else
				{
					stream.Write(plaintext, plaintextOffset, plaintextLength);
					stream.Flush();
					byte[] bufferContents = GetBufferContents();
					CheckLength(bufferContents.Length, plaintextLength + 1024, 80);
					array = mWriteCipher.EncodePlaintext(mWriteSeqNo++, type, bufferContents, 0, bufferContents.Length);
				}
				CheckLength(array.Length, mCiphertextLimit, 80);
				byte[] array2 = new byte[array.Length + 5];
				TlsUtilities.WriteUint8(type, array2, 0);
				TlsUtilities.WriteVersion(mWriteVersion, array2, 1);
				TlsUtilities.WriteUint16(array.Length, array2, 3);
				Array.Copy(array, 0, array2, 5, array.Length);
				mOutput.Write(array2, 0, array2.Length);
				mOutput.Flush();
			}
		}

		internal virtual void NotifyHelloComplete()
		{
			mHandshakeHash = mHandshakeHash.NotifyPrfDetermined();
		}

		internal virtual TlsHandshakeHash PrepareToFinish()
		{
			TlsHandshakeHash result = mHandshakeHash;
			mHandshakeHash = mHandshakeHash.StopTracking();
			return result;
		}

		internal virtual void UpdateHandshakeData(byte[] message, int offset, int len)
		{
			mHandshakeHash.BlockUpdate(message, offset, len);
		}

		internal virtual void SafeClose()
		{
			try
			{
				Platform.Dispose(mInput);
			}
			catch (IOException)
			{
			}
			try
			{
				Platform.Dispose(mOutput);
			}
			catch (IOException)
			{
			}
		}

		internal virtual void Flush()
		{
			mOutput.Flush();
		}

		private byte[] GetBufferContents()
		{
			byte[] result = mBuffer.ToArray();
			mBuffer.SetLength(0L);
			return result;
		}

		private static void CheckType(byte type, byte alertDescription)
		{
			switch (type)
			{
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
				return;
			}
			throw new TlsFatalAlert(alertDescription);
		}

		private static void CheckLength(int length, int limit, byte alertDescription)
		{
			if (length > limit)
			{
				throw new TlsFatalAlert(alertDescription);
			}
		}
	}
}
