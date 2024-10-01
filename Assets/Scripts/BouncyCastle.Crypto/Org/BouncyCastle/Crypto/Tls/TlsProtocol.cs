using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class TlsProtocol
	{
		internal class HandshakeMessage : MemoryStream
		{
			internal HandshakeMessage(byte handshakeType)
				: this(handshakeType, 60)
			{
			}

			internal HandshakeMessage(byte handshakeType, int length)
				: base(length + 4)
			{
				TlsUtilities.WriteUint8(handshakeType, this);
				TlsUtilities.WriteUint24(0, this);
			}

			internal void Write(byte[] data)
			{
				Write(data, 0, data.Length);
			}

			internal void WriteToRecordStream(TlsProtocol protocol)
			{
				long num = Length - 4;
				TlsUtilities.CheckUint24(num);
				Position = 1L;
				TlsUtilities.WriteUint24((int)num, this);
				byte[] buffer = GetBuffer();
				int len = (int)Length;
				protocol.WriteHandshakeMessage(buffer, 0, len);
				Platform.Dispose(this);
			}
		}

		protected const short CS_START = 0;

		protected const short CS_CLIENT_HELLO = 1;

		protected const short CS_SERVER_HELLO = 2;

		protected const short CS_SERVER_SUPPLEMENTAL_DATA = 3;

		protected const short CS_SERVER_CERTIFICATE = 4;

		protected const short CS_CERTIFICATE_STATUS = 5;

		protected const short CS_SERVER_KEY_EXCHANGE = 6;

		protected const short CS_CERTIFICATE_REQUEST = 7;

		protected const short CS_SERVER_HELLO_DONE = 8;

		protected const short CS_CLIENT_SUPPLEMENTAL_DATA = 9;

		protected const short CS_CLIENT_CERTIFICATE = 10;

		protected const short CS_CLIENT_KEY_EXCHANGE = 11;

		protected const short CS_CERTIFICATE_VERIFY = 12;

		protected const short CS_CLIENT_FINISHED = 13;

		protected const short CS_SERVER_SESSION_TICKET = 14;

		protected const short CS_SERVER_FINISHED = 15;

		protected const short CS_END = 16;

		private static readonly string TLS_ERROR_MESSAGE = "Internal TLS error, this could be an attack";

		private ByteQueue mApplicationDataQueue = new ByteQueue();

		private ByteQueue mAlertQueue = new ByteQueue(2);

		private ByteQueue mHandshakeQueue = new ByteQueue();

		internal RecordStream mRecordStream;

		protected SecureRandom mSecureRandom;

		private TlsStream mTlsStream = null;

		private volatile bool mClosed = false;

		private volatile bool mFailedWithError = false;

		private volatile bool mAppDataReady = false;

		private volatile bool mSplitApplicationDataRecords = true;

		private byte[] mExpectedVerifyData = null;

		protected TlsSession mTlsSession = null;

		protected SessionParameters mSessionParameters = null;

		protected SecurityParameters mSecurityParameters = null;

		protected Certificate mPeerCertificate = null;

		protected int[] mOfferedCipherSuites = null;

		protected byte[] mOfferedCompressionMethods = null;

		protected IDictionary mClientExtensions = null;

		protected IDictionary mServerExtensions = null;

		protected short mConnectionState = 0;

		protected bool mResumedSession = false;

		protected bool mReceivedChangeCipherSpec = false;

		protected bool mSecureRenegotiation = false;

		protected bool mAllowCertificateStatus = false;

		protected bool mExpectSessionTicket = false;

		protected bool mBlocking = true;

		protected ByteQueueStream mInputBuffers = null;

		protected ByteQueueStream mOutputBuffer = null;

		protected abstract TlsContext Context { get; }

		internal abstract AbstractTlsContext ContextAdmin { get; }

		protected abstract TlsPeer Peer { get; }

		public virtual Stream Stream
		{
			get
			{
				if (!mBlocking)
				{
					throw new InvalidOperationException("Cannot use Stream in non-blocking mode! Use OfferInput()/OfferOutput() instead.");
				}
				return mTlsStream;
			}
		}

		public virtual bool IsClosed
		{
			get
			{
				return mClosed;
			}
		}

		public TlsProtocol(Stream stream, SecureRandom secureRandom)
			: this(stream, stream, secureRandom)
		{
		}

		public TlsProtocol(Stream input, Stream output, SecureRandom secureRandom)
		{
			mRecordStream = new RecordStream(this, input, output);
			mSecureRandom = secureRandom;
		}

		public TlsProtocol(SecureRandom secureRandom)
		{
			mBlocking = false;
			mInputBuffers = new ByteQueueStream();
			mOutputBuffer = new ByteQueueStream();
			mRecordStream = new RecordStream(this, mInputBuffers, mOutputBuffer);
			mSecureRandom = secureRandom;
		}

		protected virtual void HandleChangeCipherSpecMessage()
		{
		}

		protected abstract void HandleHandshakeMessage(byte type, byte[] buf);

		protected virtual void HandleWarningMessage(byte description)
		{
		}

		protected virtual void ApplyMaxFragmentLengthExtension()
		{
			if (mSecurityParameters.maxFragmentLength >= 0)
			{
				if (!MaxFragmentLength.IsValid((byte)mSecurityParameters.maxFragmentLength))
				{
					throw new TlsFatalAlert(80);
				}
				int plaintextLimit = 1 << 8 + mSecurityParameters.maxFragmentLength;
				mRecordStream.SetPlaintextLimit(plaintextLimit);
			}
		}

		protected virtual void CheckReceivedChangeCipherSpec(bool expected)
		{
			if (expected != mReceivedChangeCipherSpec)
			{
				throw new TlsFatalAlert(10);
			}
		}

		protected virtual void CleanupHandshake()
		{
			if (mExpectedVerifyData != null)
			{
				Arrays.Fill(mExpectedVerifyData, 0);
				mExpectedVerifyData = null;
			}
			mSecurityParameters.Clear();
			mPeerCertificate = null;
			mOfferedCipherSuites = null;
			mOfferedCompressionMethods = null;
			mClientExtensions = null;
			mServerExtensions = null;
			mResumedSession = false;
			mReceivedChangeCipherSpec = false;
			mSecureRenegotiation = false;
			mAllowCertificateStatus = false;
			mExpectSessionTicket = false;
		}

		protected virtual void BlockForHandshake()
		{
			if (mBlocking)
			{
				while (mConnectionState != 16)
				{
					bool mClosed2 = mClosed;
					SafeReadRecord();
				}
			}
		}

		protected virtual void CompleteHandshake()
		{
			try
			{
				mRecordStream.FinaliseHandshake();
				mSplitApplicationDataRecords = !TlsUtilities.IsTlsV11(Context);
				if (!mAppDataReady)
				{
					mAppDataReady = true;
					if (mBlocking)
					{
						mTlsStream = new TlsStream(this);
					}
				}
				if (mTlsSession != null)
				{
					if (mSessionParameters == null)
					{
						mSessionParameters = new SessionParameters.Builder().SetCipherSuite(mSecurityParameters.CipherSuite).SetCompressionAlgorithm(mSecurityParameters.CompressionAlgorithm).SetMasterSecret(mSecurityParameters.MasterSecret)
							.SetPeerCertificate(mPeerCertificate)
							.SetPskIdentity(mSecurityParameters.PskIdentity)
							.SetSrpIdentity(mSecurityParameters.SrpIdentity)
							.SetServerExtensions(mServerExtensions)
							.Build();
						mTlsSession = new TlsSessionImpl(mTlsSession.SessionID, mSessionParameters);
					}
					ContextAdmin.SetResumableSession(mTlsSession);
				}
				Peer.NotifyHandshakeComplete();
			}
			finally
			{
				CleanupHandshake();
			}
		}

		protected internal void ProcessRecord(byte protocol, byte[] buf, int offset, int len)
		{
			switch (protocol)
			{
			case 21:
				mAlertQueue.AddData(buf, offset, len);
				ProcessAlert();
				break;
			case 23:
				if (!mAppDataReady)
				{
					throw new TlsFatalAlert(10);
				}
				mApplicationDataQueue.AddData(buf, offset, len);
				ProcessApplicationData();
				break;
			case 20:
				ProcessChangeCipherSpec(buf, offset, len);
				break;
			case 22:
				mHandshakeQueue.AddData(buf, offset, len);
				ProcessHandshake();
				break;
			case 24:
				if (!mAppDataReady)
				{
					throw new TlsFatalAlert(10);
				}
				break;
			}
		}

		private void ProcessHandshake()
		{
			bool flag;
			do
			{
				flag = false;
				if (mHandshakeQueue.Available < 4)
				{
					continue;
				}
				byte[] array = new byte[4];
				mHandshakeQueue.Read(array, 0, 4, 0);
				byte b = TlsUtilities.ReadUint8(array, 0);
				int num = TlsUtilities.ReadUint24(array, 1);
				if (mHandshakeQueue.Available < num + 4)
				{
					continue;
				}
				byte[] array2 = mHandshakeQueue.RemoveData(num, 4);
				CheckReceivedChangeCipherSpec(mConnectionState == 16 || b == 20);
				if (b != 0)
				{
					int num2 = 20;
					TlsContext context = Context;
					if (b == 20 && mExpectedVerifyData == null && context.SecurityParameters.MasterSecret != null)
					{
						mExpectedVerifyData = CreateVerifyData(!context.IsServer);
					}
					mRecordStream.UpdateHandshakeData(array, 0, 4);
					mRecordStream.UpdateHandshakeData(array2, 0, num);
				}
				HandleHandshakeMessage(b, array2);
				flag = true;
			}
			while (flag);
		}

		private void ProcessApplicationData()
		{
		}

		private void ProcessAlert()
		{
			while (mAlertQueue.Available >= 2)
			{
				byte[] array = mAlertQueue.RemoveData(2, 0);
				byte b = array[0];
				byte b2 = array[1];
				Peer.NotifyAlertReceived(b, b2);
				if (b == 2)
				{
					InvalidateSession();
					mFailedWithError = true;
					mClosed = true;
					mRecordStream.SafeClose();
					throw new IOException(TLS_ERROR_MESSAGE);
				}
				if (b2 == 0)
				{
					HandleClose(false);
				}
				HandleWarningMessage(b2);
			}
		}

		private void ProcessChangeCipherSpec(byte[] buf, int off, int len)
		{
			for (int i = 0; i < len; i++)
			{
				byte b = TlsUtilities.ReadUint8(buf, off + i);
				if (b != 1)
				{
					throw new TlsFatalAlert(50);
				}
				if (mReceivedChangeCipherSpec || mAlertQueue.Available > 0 || mHandshakeQueue.Available > 0)
				{
					throw new TlsFatalAlert(10);
				}
				mRecordStream.ReceivedReadCipherSpec();
				mReceivedChangeCipherSpec = true;
				HandleChangeCipherSpecMessage();
			}
		}

		protected internal virtual int ApplicationDataAvailable()
		{
			return mApplicationDataQueue.Available;
		}

		protected internal virtual int ReadApplicationData(byte[] buf, int offset, int len)
		{
			if (len < 1)
			{
				return 0;
			}
			while (mApplicationDataQueue.Available == 0)
			{
				if (mClosed)
				{
					if (mFailedWithError)
					{
						throw new IOException(TLS_ERROR_MESSAGE);
					}
					return 0;
				}
				SafeReadRecord();
			}
			len = System.Math.Min(len, mApplicationDataQueue.Available);
			mApplicationDataQueue.RemoveData(buf, offset, len, 0);
			return len;
		}

		protected virtual void SafeReadRecord()
		{
			try
			{
				if (!mRecordStream.ReadRecord())
				{
					throw new EndOfStreamException();
				}
			}
			catch (TlsFatalAlert tlsFatalAlert)
			{
				if (!mClosed)
				{
					FailWithError(2, tlsFatalAlert.AlertDescription, "Failed to read record", tlsFatalAlert);
				}
				throw tlsFatalAlert;
			}
			catch (Exception ex)
			{
				if (!mClosed)
				{
					FailWithError(2, 80, "Failed to read record", ex);
				}
				throw ex;
			}
		}

		protected virtual void SafeWriteRecord(byte type, byte[] buf, int offset, int len)
		{
			try
			{
				mRecordStream.WriteRecord(type, buf, offset, len);
			}
			catch (TlsFatalAlert tlsFatalAlert)
			{
				if (!mClosed)
				{
					FailWithError(2, tlsFatalAlert.AlertDescription, "Failed to write record", tlsFatalAlert);
				}
				throw tlsFatalAlert;
			}
			catch (Exception ex)
			{
				if (!mClosed)
				{
					FailWithError(2, 80, "Failed to write record", ex);
				}
				throw ex;
			}
		}

		protected internal virtual void WriteData(byte[] buf, int offset, int len)
		{
			if (mClosed)
			{
				if (mFailedWithError)
				{
					throw new IOException(TLS_ERROR_MESSAGE);
				}
				throw new IOException("Sorry, connection has been closed, you cannot write more data");
			}
			while (len > 0)
			{
				if (mSplitApplicationDataRecords)
				{
					SafeWriteRecord(23, buf, offset, 1);
					offset++;
					len--;
				}
				if (len > 0)
				{
					int num = System.Math.Min(len, mRecordStream.GetPlaintextLimit());
					SafeWriteRecord(23, buf, offset, num);
					offset += num;
					len -= num;
				}
			}
		}

		protected virtual void WriteHandshakeMessage(byte[] buf, int off, int len)
		{
			while (len > 0)
			{
				int num = System.Math.Min(len, mRecordStream.GetPlaintextLimit());
				SafeWriteRecord(22, buf, off, num);
				off += num;
				len -= num;
			}
		}

		public virtual void OfferInput(byte[] input)
		{
			if (mBlocking)
			{
				throw new InvalidOperationException("Cannot use OfferInput() in blocking mode! Use Stream instead.");
			}
			if (mClosed)
			{
				throw new IOException("Connection is closed, cannot accept any more input");
			}
			mInputBuffers.Write(input);
			while (mInputBuffers.Available >= 5)
			{
				byte[] buf = new byte[5];
				mInputBuffers.Peek(buf);
				int num = TlsUtilities.ReadUint16(buf, 3) + 5;
				if (mInputBuffers.Available < num)
				{
					break;
				}
				SafeReadRecord();
			}
		}

		public virtual int GetAvailableInputBytes()
		{
			if (mBlocking)
			{
				throw new InvalidOperationException("Cannot use GetAvailableInputBytes() in blocking mode! Use ApplicationDataAvailable() instead.");
			}
			return ApplicationDataAvailable();
		}

		public virtual int ReadInput(byte[] buffer, int offset, int length)
		{
			if (mBlocking)
			{
				throw new InvalidOperationException("Cannot use ReadInput() in blocking mode! Use Stream instead.");
			}
			return ReadApplicationData(buffer, offset, System.Math.Min(length, ApplicationDataAvailable()));
		}

		public virtual void OfferOutput(byte[] buffer, int offset, int length)
		{
			if (mBlocking)
			{
				throw new InvalidOperationException("Cannot use OfferOutput() in blocking mode! Use Stream instead.");
			}
			if (!mAppDataReady)
			{
				throw new IOException("Application data cannot be sent until the handshake is complete!");
			}
			WriteData(buffer, offset, length);
		}

		public virtual int GetAvailableOutputBytes()
		{
			if (mBlocking)
			{
				throw new InvalidOperationException("Cannot use GetAvailableOutputBytes() in blocking mode! Use Stream instead.");
			}
			return mOutputBuffer.Available;
		}

		public virtual int ReadOutput(byte[] buffer, int offset, int length)
		{
			if (mBlocking)
			{
				throw new InvalidOperationException("Cannot use ReadOutput() in blocking mode! Use Stream instead.");
			}
			return mOutputBuffer.Read(buffer, offset, length);
		}

		protected virtual void FailWithError(byte alertLevel, byte alertDescription, string message, Exception cause)
		{
			if (!mClosed)
			{
				mClosed = true;
				if (alertLevel == 2)
				{
					InvalidateSession();
					mFailedWithError = true;
				}
				RaiseAlert(alertLevel, alertDescription, message, cause);
				mRecordStream.SafeClose();
				if (alertLevel != 2)
				{
					return;
				}
			}
			throw new IOException(TLS_ERROR_MESSAGE);
		}

		protected virtual void InvalidateSession()
		{
			if (mSessionParameters != null)
			{
				mSessionParameters.Clear();
				mSessionParameters = null;
			}
			if (mTlsSession != null)
			{
				mTlsSession.Invalidate();
				mTlsSession = null;
			}
		}

		protected virtual void ProcessFinishedMessage(MemoryStream buf)
		{
			if (mExpectedVerifyData == null)
			{
				throw new TlsFatalAlert(80);
			}
			byte[] b = TlsUtilities.ReadFully(mExpectedVerifyData.Length, buf);
			AssertEmpty(buf);
			if (!Arrays.ConstantTimeAreEqual(mExpectedVerifyData, b))
			{
				throw new TlsFatalAlert(51);
			}
		}

		protected virtual void RaiseAlert(byte alertLevel, byte alertDescription, string message, Exception cause)
		{
			Peer.NotifyAlertRaised(alertLevel, alertDescription, message, cause);
			byte[] buf = new byte[2] { alertLevel, alertDescription };
			SafeWriteRecord(21, buf, 0, 2);
		}

		protected virtual void RaiseWarning(byte alertDescription, string message)
		{
			RaiseAlert(1, alertDescription, message, null);
		}

		protected virtual void SendCertificateMessage(Certificate certificate)
		{
			if (certificate == null)
			{
				certificate = Certificate.EmptyChain;
			}
			if (certificate.IsEmpty)
			{
				TlsContext context = Context;
				if (!context.IsServer)
				{
					ProtocolVersion serverVersion = Context.ServerVersion;
					if (serverVersion.IsSsl)
					{
						string message = serverVersion.ToString() + " client didn't provide credentials";
						RaiseWarning(41, message);
						return;
					}
				}
			}
			HandshakeMessage handshakeMessage = new HandshakeMessage(11);
			certificate.Encode(handshakeMessage);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual void SendChangeCipherSpecMessage()
		{
			byte[] array = new byte[1] { 1 };
			SafeWriteRecord(20, array, 0, array.Length);
			mRecordStream.SentWriteCipherSpec();
		}

		protected virtual void SendFinishedMessage()
		{
			byte[] array = CreateVerifyData(Context.IsServer);
			HandshakeMessage handshakeMessage = new HandshakeMessage(20, array.Length);
			handshakeMessage.Write(array, 0, array.Length);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual void SendSupplementalDataMessage(IList supplementalData)
		{
			HandshakeMessage handshakeMessage = new HandshakeMessage(23);
			WriteSupplementalData(handshakeMessage, supplementalData);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual byte[] CreateVerifyData(bool isServer)
		{
			TlsContext context = Context;
			string asciiLabel = (isServer ? "server finished" : "client finished");
			byte[] sslSender = (isServer ? TlsUtilities.SSL_SERVER : TlsUtilities.SSL_CLIENT);
			byte[] currentPrfHash = GetCurrentPrfHash(context, mRecordStream.HandshakeHash, sslSender);
			return TlsUtilities.CalculateVerifyData(context, asciiLabel, currentPrfHash);
		}

		public virtual void Close()
		{
			HandleClose(true);
		}

		protected virtual void HandleClose(bool user_canceled)
		{
			if (!mClosed)
			{
				if (user_canceled && !mAppDataReady)
				{
					RaiseWarning(90, "User canceled handshake");
				}
				FailWithError(1, 0, "Connection closed", null);
			}
		}

		protected internal virtual void Flush()
		{
			mRecordStream.Flush();
		}

		protected virtual short ProcessMaxFragmentLengthExtension(IDictionary clientExtensions, IDictionary serverExtensions, byte alertDescription)
		{
			short maxFragmentLengthExtension = TlsExtensionsUtilities.GetMaxFragmentLengthExtension(serverExtensions);
			if (maxFragmentLengthExtension >= 0 && (!MaxFragmentLength.IsValid((byte)maxFragmentLengthExtension) || (!mResumedSession && maxFragmentLengthExtension != TlsExtensionsUtilities.GetMaxFragmentLengthExtension(clientExtensions))))
			{
				throw new TlsFatalAlert(alertDescription);
			}
			return maxFragmentLengthExtension;
		}

		protected virtual void RefuseRenegotiation()
		{
			if (TlsUtilities.IsSsl(Context))
			{
				throw new TlsFatalAlert(40);
			}
			RaiseWarning(100, "Renegotiation not supported");
		}

		protected internal static void AssertEmpty(MemoryStream buf)
		{
			if (buf.Position < buf.Length)
			{
				throw new TlsFatalAlert(50);
			}
		}

		protected internal static byte[] CreateRandomBlock(bool useGmtUnixTime, IRandomGenerator randomGenerator)
		{
			byte[] array = new byte[32];
			randomGenerator.NextBytes(array);
			if (useGmtUnixTime)
			{
				TlsUtilities.WriteGmtUnixTime(array, 0);
			}
			return array;
		}

		protected internal static byte[] CreateRenegotiationInfo(byte[] renegotiated_connection)
		{
			return TlsUtilities.EncodeOpaque8(renegotiated_connection);
		}

		protected internal static void EstablishMasterSecret(TlsContext context, TlsKeyExchange keyExchange)
		{
			byte[] array = keyExchange.GeneratePremasterSecret();
			try
			{
				context.SecurityParameters.masterSecret = TlsUtilities.CalculateMasterSecret(context, array);
			}
			finally
			{
				if (array != null)
				{
					Arrays.Fill(array, 0);
				}
			}
		}

		protected internal static byte[] GetCurrentPrfHash(TlsContext context, TlsHandshakeHash handshakeHash, byte[] sslSender)
		{
			IDigest digest = handshakeHash.ForkPrfHash();
			if (sslSender != null && TlsUtilities.IsSsl(context))
			{
				digest.BlockUpdate(sslSender, 0, sslSender.Length);
			}
			return DigestUtilities.DoFinal(digest);
		}

		protected internal static IDictionary ReadExtensions(MemoryStream input)
		{
			if (input.Position >= input.Length)
			{
				return null;
			}
			byte[] buffer = TlsUtilities.ReadOpaque16(input);
			AssertEmpty(input);
			MemoryStream memoryStream = new MemoryStream(buffer, false);
			IDictionary dictionary = Platform.CreateHashtable();
			while (memoryStream.Position < memoryStream.Length)
			{
				int num = TlsUtilities.ReadUint16(memoryStream);
				byte[] value = TlsUtilities.ReadOpaque16(memoryStream);
				if (dictionary.Contains(num))
				{
					throw new TlsFatalAlert(47);
				}
				dictionary.Add(num, value);
			}
			return dictionary;
		}

		protected internal static IList ReadSupplementalDataMessage(MemoryStream input)
		{
			byte[] buffer = TlsUtilities.ReadOpaque24(input);
			AssertEmpty(input);
			MemoryStream memoryStream = new MemoryStream(buffer, false);
			IList list = Platform.CreateArrayList();
			while (memoryStream.Position < memoryStream.Length)
			{
				int dataType = TlsUtilities.ReadUint16(memoryStream);
				byte[] data = TlsUtilities.ReadOpaque16(memoryStream);
				list.Add(new SupplementalDataEntry(dataType, data));
			}
			return list;
		}

		protected internal static void WriteExtensions(Stream output, IDictionary extensions)
		{
			MemoryStream memoryStream = new MemoryStream();
			foreach (object key in extensions.Keys)
			{
				int num = (int)key;
				byte[] buf = (byte[])extensions[num];
				TlsUtilities.CheckUint16(num);
				TlsUtilities.WriteUint16(num, memoryStream);
				TlsUtilities.WriteOpaque16(buf, memoryStream);
			}
			byte[] buf2 = memoryStream.ToArray();
			TlsUtilities.WriteOpaque16(buf2, output);
		}

		protected internal static void WriteSupplementalData(Stream output, IList supplementalData)
		{
			MemoryStream memoryStream = new MemoryStream();
			foreach (SupplementalDataEntry supplementalDatum in supplementalData)
			{
				int dataType = supplementalDatum.DataType;
				TlsUtilities.CheckUint16(dataType);
				TlsUtilities.WriteUint16(dataType, memoryStream);
				TlsUtilities.WriteOpaque16(supplementalDatum.Data, memoryStream);
			}
			byte[] buf = memoryStream.ToArray();
			TlsUtilities.WriteOpaque24(buf, output);
		}

		protected internal static int GetPrfAlgorithm(TlsContext context, int ciphersuite)
		{
			bool flag = TlsUtilities.IsTlsV12(context);
			switch (ciphersuite)
			{
			case 59:
			case 60:
			case 61:
			case 62:
			case 63:
			case 64:
			case 103:
			case 104:
			case 105:
			case 106:
			case 107:
			case 156:
			case 158:
			case 160:
			case 162:
			case 164:
			case 168:
			case 170:
			case 172:
			case 186:
			case 187:
			case 188:
			case 189:
			case 190:
			case 191:
			case 192:
			case 193:
			case 194:
			case 195:
			case 196:
			case 197:
			case 49187:
			case 49189:
			case 49191:
			case 49193:
			case 49195:
			case 49197:
			case 49199:
			case 49201:
			case 49266:
			case 49268:
			case 49270:
			case 49272:
			case 49274:
			case 49276:
			case 49278:
			case 49280:
			case 49282:
			case 49284:
			case 49286:
			case 49288:
			case 49290:
			case 49292:
			case 49294:
			case 49296:
			case 49298:
			case 49308:
			case 49309:
			case 49310:
			case 49311:
			case 49312:
			case 49313:
			case 49314:
			case 49315:
			case 49316:
			case 49317:
			case 49318:
			case 49319:
			case 49320:
			case 49321:
			case 49322:
			case 49323:
			case 49324:
			case 49325:
			case 49326:
			case 49327:
			case 52243:
			case 52244:
			case 52245:
				if (flag)
				{
					return 1;
				}
				throw new TlsFatalAlert(47);
			case 157:
			case 159:
			case 161:
			case 163:
			case 165:
			case 169:
			case 171:
			case 173:
			case 49188:
			case 49190:
			case 49192:
			case 49194:
			case 49196:
			case 49198:
			case 49200:
			case 49202:
			case 49267:
			case 49269:
			case 49271:
			case 49273:
			case 49275:
			case 49277:
			case 49279:
			case 49281:
			case 49283:
			case 49285:
			case 49287:
			case 49289:
			case 49291:
			case 49293:
			case 49295:
			case 49297:
			case 49299:
				if (flag)
				{
					return 2;
				}
				throw new TlsFatalAlert(47);
			case 175:
			case 177:
			case 179:
			case 181:
			case 183:
			case 185:
			case 49208:
			case 49211:
			case 49301:
			case 49303:
			case 49305:
			case 49307:
				if (flag)
				{
					return 2;
				}
				return 0;
			default:
				if (flag)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
