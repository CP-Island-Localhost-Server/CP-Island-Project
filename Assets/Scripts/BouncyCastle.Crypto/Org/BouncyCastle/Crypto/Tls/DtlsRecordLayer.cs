using System;
using System.IO;
using Org.BouncyCastle.Utilities.Date;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DtlsRecordLayer : DatagramTransport
	{
		private const int RECORD_HEADER_LENGTH = 13;

		private const int MAX_FRAGMENT_LENGTH = 16384;

		private const long TCP_MSL = 120000L;

		private const long RETRANSMIT_TIMEOUT = 240000L;

		private readonly DatagramTransport mTransport;

		private readonly TlsContext mContext;

		private readonly TlsPeer mPeer;

		private readonly ByteQueue mRecordQueue = new ByteQueue();

		private volatile bool mClosed = false;

		private volatile bool mFailed = false;

		private volatile ProtocolVersion mReadVersion = null;

		private volatile ProtocolVersion mWriteVersion = null;

		private volatile bool mInHandshake;

		private volatile int mPlaintextLimit;

		private DtlsEpoch mCurrentEpoch;

		private DtlsEpoch mPendingEpoch;

		private DtlsEpoch mReadEpoch;

		private DtlsEpoch mWriteEpoch;

		private DtlsHandshakeRetransmit mRetransmit = null;

		private DtlsEpoch mRetransmitEpoch = null;

		private long mRetransmitExpiry = 0L;

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

		internal DtlsRecordLayer(DatagramTransport transport, TlsContext context, TlsPeer peer, byte contentType)
		{
			mTransport = transport;
			mContext = context;
			mPeer = peer;
			mInHandshake = true;
			mCurrentEpoch = new DtlsEpoch(0, new TlsNullCipher(context));
			mPendingEpoch = null;
			mReadEpoch = mCurrentEpoch;
			mWriteEpoch = mCurrentEpoch;
			SetPlaintextLimit(16384);
		}

		internal virtual void SetPlaintextLimit(int plaintextLimit)
		{
			mPlaintextLimit = plaintextLimit;
		}

		internal virtual void SetWriteVersion(ProtocolVersion writeVersion)
		{
			mWriteVersion = writeVersion;
		}

		internal virtual void InitPendingEpoch(TlsCipher pendingCipher)
		{
			if (mPendingEpoch != null)
			{
				throw new InvalidOperationException();
			}
			mPendingEpoch = new DtlsEpoch(mWriteEpoch.Epoch + 1, pendingCipher);
		}

		internal virtual void HandshakeSuccessful(DtlsHandshakeRetransmit retransmit)
		{
			if (mReadEpoch == mCurrentEpoch || mWriteEpoch == mCurrentEpoch)
			{
				throw new InvalidOperationException();
			}
			if (retransmit != null)
			{
				mRetransmit = retransmit;
				mRetransmitEpoch = mCurrentEpoch;
				mRetransmitExpiry = DateTimeUtilities.CurrentUnixMs() + 240000;
			}
			mInHandshake = false;
			mCurrentEpoch = mPendingEpoch;
			mPendingEpoch = null;
		}

		internal virtual void ResetWriteEpoch()
		{
			if (mRetransmitEpoch != null)
			{
				mWriteEpoch = mRetransmitEpoch;
			}
			else
			{
				mWriteEpoch = mCurrentEpoch;
			}
		}

		public virtual int GetReceiveLimit()
		{
			return System.Math.Min(mPlaintextLimit, mReadEpoch.Cipher.GetPlaintextLimit(mTransport.GetReceiveLimit() - 13));
		}

		public virtual int GetSendLimit()
		{
			return System.Math.Min(mPlaintextLimit, mWriteEpoch.Cipher.GetPlaintextLimit(mTransport.GetSendLimit() - 13));
		}

		public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
		{
			byte[] array = null;
			while (true)
			{
				int num = System.Math.Min(len, GetReceiveLimit()) + 13;
				if (array == null || array.Length < num)
				{
					array = new byte[num];
				}
				try
				{
					if (mRetransmit != null && DateTimeUtilities.CurrentUnixMs() > mRetransmitExpiry)
					{
						mRetransmit = null;
						mRetransmitEpoch = null;
					}
					int num2 = ReceiveRecord(array, 0, num, waitMillis);
					if (num2 < 0)
					{
						return num2;
					}
					if (num2 < 13)
					{
						continue;
					}
					int num3 = TlsUtilities.ReadUint16(array, 11);
					if (num2 != num3 + 13)
					{
						continue;
					}
					byte b = TlsUtilities.ReadUint8(array, 0);
					switch (b)
					{
					case 20:
					case 21:
					case 22:
					case 23:
					case 24:
						{
							int num4 = TlsUtilities.ReadUint16(array, 3);
							DtlsEpoch dtlsEpoch = null;
							if (num4 == mReadEpoch.Epoch)
							{
								dtlsEpoch = mReadEpoch;
							}
							else if (b == 22 && mRetransmitEpoch != null && num4 == mRetransmitEpoch.Epoch)
							{
								dtlsEpoch = mRetransmitEpoch;
							}
							if (dtlsEpoch == null)
							{
								break;
							}
							long num5 = TlsUtilities.ReadUint48(array, 5);
							if (dtlsEpoch.ReplayWindow.ShouldDiscard(num5))
							{
								break;
							}
							ProtocolVersion protocolVersion = TlsUtilities.ReadVersion(array, 1);
							if (!protocolVersion.IsDtls || (mReadVersion != null && !mReadVersion.Equals(protocolVersion)))
							{
								break;
							}
							byte[] array2 = dtlsEpoch.Cipher.DecodeCiphertext(GetMacSequenceNumber(dtlsEpoch.Epoch, num5), b, array, 13, num2 - 13);
							dtlsEpoch.ReplayWindow.ReportAuthenticated(num5);
							if (array2.Length > mPlaintextLimit)
							{
								break;
							}
							if (mReadVersion == null)
							{
								mReadVersion = protocolVersion;
							}
							switch (b)
							{
							case 21:
								if (array2.Length == 2)
								{
									byte b2 = array2[0];
									byte b3 = array2[1];
									mPeer.NotifyAlertReceived(b2, b3);
									if (b2 == 2)
									{
										Fail(b3);
										throw new TlsFatalAlert(b3);
									}
									if (b3 == 0)
									{
										CloseTransport();
									}
								}
								goto end_IL_0088;
							case 23:
								if (!mInHandshake)
								{
									break;
								}
								goto end_IL_0088;
							case 20:
							{
								for (int i = 0; i < array2.Length; i++)
								{
									byte b4 = TlsUtilities.ReadUint8(array2, i);
									if (b4 == 1 && mPendingEpoch != null)
									{
										mReadEpoch = mPendingEpoch;
									}
								}
								goto end_IL_0088;
							}
							case 22:
								if (mInHandshake)
								{
									break;
								}
								if (mRetransmit != null)
								{
									mRetransmit.ReceivedHandshakeRecord(num4, array2, 0, array2.Length);
								}
								goto end_IL_0088;
							case 24:
								goto end_IL_0088;
							}
							if (!mInHandshake && mRetransmit != null)
							{
								mRetransmit = null;
								mRetransmitEpoch = null;
							}
							Array.Copy(array2, 0, buf, off, array2.Length);
							return array2.Length;
						}
						end_IL_0088:
						break;
					}
				}
				catch (IOException ex)
				{
					throw ex;
				}
			}
		}

		public virtual void Send(byte[] buf, int off, int len)
		{
			byte contentType = 23;
			if (mInHandshake || mWriteEpoch == mRetransmitEpoch)
			{
				contentType = 22;
				byte b = TlsUtilities.ReadUint8(buf, off);
				if (b == 20)
				{
					DtlsEpoch dtlsEpoch = null;
					if (mInHandshake)
					{
						dtlsEpoch = mPendingEpoch;
					}
					else if (mWriteEpoch == mRetransmitEpoch)
					{
						dtlsEpoch = mCurrentEpoch;
					}
					if (dtlsEpoch == null)
					{
						throw new InvalidOperationException();
					}
					byte[] array = new byte[1] { 1 };
					SendRecord(20, array, 0, array.Length);
					mWriteEpoch = dtlsEpoch;
				}
			}
			SendRecord(contentType, buf, off, len);
		}

		public virtual void Close()
		{
			if (!mClosed)
			{
				if (mInHandshake)
				{
					Warn(90, "User canceled handshake");
				}
				CloseTransport();
			}
		}

		internal virtual void Fail(byte alertDescription)
		{
			if (!mClosed)
			{
				try
				{
					RaiseAlert(2, alertDescription, null, null);
				}
				catch (Exception)
				{
				}
				mFailed = true;
				CloseTransport();
			}
		}

		internal virtual void Warn(byte alertDescription, string message)
		{
			RaiseAlert(1, alertDescription, message, null);
		}

		private void CloseTransport()
		{
			if (mClosed)
			{
				return;
			}
			try
			{
				if (!mFailed)
				{
					Warn(0, null);
				}
				mTransport.Close();
			}
			catch (Exception)
			{
			}
			mClosed = true;
		}

		private void RaiseAlert(byte alertLevel, byte alertDescription, string message, Exception cause)
		{
			mPeer.NotifyAlertRaised(alertLevel, alertDescription, message, cause);
			SendRecord(21, new byte[2] { alertLevel, alertDescription }, 0, 2);
		}

		private int ReceiveRecord(byte[] buf, int off, int len, int waitMillis)
		{
			if (mRecordQueue.Available > 0)
			{
				int num = 0;
				if (mRecordQueue.Available >= 13)
				{
					byte[] buf2 = new byte[2];
					mRecordQueue.Read(buf2, 0, 2, 11);
					num = TlsUtilities.ReadUint16(buf2, 0);
				}
				int num2 = System.Math.Min(mRecordQueue.Available, 13 + num);
				mRecordQueue.RemoveData(buf, off, num2, 0);
				return num2;
			}
			int num3 = mTransport.Receive(buf, off, len, waitMillis);
			if (num3 >= 13)
			{
				int num4 = TlsUtilities.ReadUint16(buf, off + 11);
				int num5 = 13 + num4;
				if (num3 > num5)
				{
					mRecordQueue.AddData(buf, off + num5, num3 - num5);
					num3 = num5;
				}
			}
			return num3;
		}

		private void SendRecord(byte contentType, byte[] buf, int off, int len)
		{
			if (mWriteVersion != null)
			{
				if (len > mPlaintextLimit)
				{
					throw new TlsFatalAlert(80);
				}
				if (len < 1 && contentType != 23)
				{
					throw new TlsFatalAlert(80);
				}
				int epoch = mWriteEpoch.Epoch;
				long num = mWriteEpoch.AllocateSequenceNumber();
				byte[] array = mWriteEpoch.Cipher.EncodePlaintext(GetMacSequenceNumber(epoch, num), contentType, buf, off, len);
				byte[] array2 = new byte[array.Length + 13];
				TlsUtilities.WriteUint8(contentType, array2, 0);
				ProtocolVersion version = mWriteVersion;
				TlsUtilities.WriteVersion(version, array2, 1);
				TlsUtilities.WriteUint16(epoch, array2, 3);
				TlsUtilities.WriteUint48(num, array2, 5);
				TlsUtilities.WriteUint16(array.Length, array2, 11);
				Array.Copy(array, 0, array2, 13, array.Length);
				mTransport.Send(array2, 0, array2.Length);
			}
		}

		private static long GetMacSequenceNumber(int epoch, long sequence_number)
		{
			return ((epoch & 0xFFFFFFFFu) << 48) | sequence_number;
		}
	}
}
