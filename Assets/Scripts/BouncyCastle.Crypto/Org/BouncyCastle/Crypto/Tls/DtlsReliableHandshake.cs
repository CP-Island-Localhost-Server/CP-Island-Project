using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DtlsReliableHandshake
	{
		internal class Message
		{
			private readonly int mMessageSeq;

			private readonly byte mMsgType;

			private readonly byte[] mBody;

			public int Seq
			{
				get
				{
					return mMessageSeq;
				}
			}

			public byte Type
			{
				get
				{
					return mMsgType;
				}
			}

			public byte[] Body
			{
				get
				{
					return mBody;
				}
			}

			internal Message(int message_seq, byte msg_type, byte[] body)
			{
				mMessageSeq = message_seq;
				mMsgType = msg_type;
				mBody = body;
			}
		}

		internal class RecordLayerBuffer : MemoryStream
		{
			internal RecordLayerBuffer(int size)
				: base(size)
			{
			}

			internal void SendToRecordLayer(DtlsRecordLayer recordLayer)
			{
				byte[] buffer = GetBuffer();
				int len = (int)Length;
				recordLayer.Send(buffer, 0, len);
				Platform.Dispose(this);
			}
		}

		internal class Retransmit : DtlsHandshakeRetransmit
		{
			private readonly DtlsReliableHandshake mOuter;

			internal Retransmit(DtlsReliableHandshake outer)
			{
				mOuter = outer;
			}

			public void ReceivedHandshakeRecord(int epoch, byte[] buf, int off, int len)
			{
				mOuter.HandleRetransmittedHandshakeRecord(epoch, buf, off, len);
			}
		}

		private const int MAX_RECEIVE_AHEAD = 10;

		private readonly DtlsRecordLayer mRecordLayer;

		private TlsHandshakeHash mHandshakeHash;

		private IDictionary mCurrentInboundFlight = Platform.CreateHashtable();

		private IDictionary mPreviousInboundFlight = null;

		private IList mOutboundFlight = Platform.CreateArrayList();

		private bool mSending = true;

		private int mMessageSeq = 0;

		private int mNextReceiveSeq = 0;

		internal TlsHandshakeHash HandshakeHash
		{
			get
			{
				return mHandshakeHash;
			}
		}

		internal DtlsReliableHandshake(TlsContext context, DtlsRecordLayer transport)
		{
			mRecordLayer = transport;
			mHandshakeHash = new DeferredHash();
			mHandshakeHash.Init(context);
		}

		internal void NotifyHelloComplete()
		{
			mHandshakeHash = mHandshakeHash.NotifyPrfDetermined();
		}

		internal TlsHandshakeHash PrepareToFinish()
		{
			TlsHandshakeHash result = mHandshakeHash;
			mHandshakeHash = mHandshakeHash.StopTracking();
			return result;
		}

		internal void SendMessage(byte msg_type, byte[] body)
		{
			TlsUtilities.CheckUint24(body.Length);
			if (!mSending)
			{
				CheckInboundFlight();
				mSending = true;
				mOutboundFlight.Clear();
			}
			Message message = new Message(mMessageSeq++, msg_type, body);
			mOutboundFlight.Add(message);
			WriteMessage(message);
			UpdateHandshakeMessagesDigest(message);
		}

		internal byte[] ReceiveMessageBody(byte msg_type)
		{
			Message message = ReceiveMessage();
			if (message.Type != msg_type)
			{
				throw new TlsFatalAlert(10);
			}
			return message.Body;
		}

		internal Message ReceiveMessage()
		{
			if (mSending)
			{
				mSending = false;
				PrepareInboundFlight();
			}
			DtlsReassembler dtlsReassembler = (DtlsReassembler)mCurrentInboundFlight[mNextReceiveSeq];
			if (dtlsReassembler != null)
			{
				byte[] bodyIfComplete = dtlsReassembler.GetBodyIfComplete();
				if (bodyIfComplete != null)
				{
					mPreviousInboundFlight = null;
					return UpdateHandshakeMessagesDigest(new Message(mNextReceiveSeq++, dtlsReassembler.MsgType, bodyIfComplete));
				}
			}
			byte[] array = null;
			int num = 1000;
			while (true)
			{
				int receiveLimit = mRecordLayer.GetReceiveLimit();
				if (array == null || array.Length < receiveLimit)
				{
					array = new byte[receiveLimit];
				}
				try
				{
					while (true)
					{
						int num2 = mRecordLayer.Receive(array, 0, receiveLimit, num);
						if (num2 < 0)
						{
							break;
						}
						if (num2 < 12)
						{
							continue;
						}
						int num3 = TlsUtilities.ReadUint24(array, 9);
						if (num2 != num3 + 12)
						{
							continue;
						}
						int num4 = TlsUtilities.ReadUint16(array, 4);
						if (num4 > mNextReceiveSeq + 10)
						{
							continue;
						}
						byte msg_type = TlsUtilities.ReadUint8(array, 0);
						int num5 = TlsUtilities.ReadUint24(array, 1);
						int num6 = TlsUtilities.ReadUint24(array, 6);
						if (num6 + num3 > num5)
						{
							continue;
						}
						if (num4 < mNextReceiveSeq)
						{
							if (mPreviousInboundFlight == null)
							{
								continue;
							}
							DtlsReassembler dtlsReassembler2 = (DtlsReassembler)mPreviousInboundFlight[num4];
							if (dtlsReassembler2 != null)
							{
								dtlsReassembler2.ContributeFragment(msg_type, num5, array, 12, num6, num3);
								if (CheckAll(mPreviousInboundFlight))
								{
									ResendOutboundFlight();
									num = System.Math.Min(num * 2, 60000);
									ResetAll(mPreviousInboundFlight);
								}
							}
							continue;
						}
						DtlsReassembler dtlsReassembler3 = (DtlsReassembler)mCurrentInboundFlight[num4];
						if (dtlsReassembler3 == null)
						{
							dtlsReassembler3 = new DtlsReassembler(msg_type, num5);
							mCurrentInboundFlight[num4] = dtlsReassembler3;
						}
						dtlsReassembler3.ContributeFragment(msg_type, num5, array, 12, num6, num3);
						if (num4 == mNextReceiveSeq)
						{
							byte[] bodyIfComplete2 = dtlsReassembler3.GetBodyIfComplete();
							if (bodyIfComplete2 != null)
							{
								mPreviousInboundFlight = null;
								return UpdateHandshakeMessagesDigest(new Message(mNextReceiveSeq++, dtlsReassembler3.MsgType, bodyIfComplete2));
							}
						}
					}
				}
				catch (IOException)
				{
				}
				ResendOutboundFlight();
				num = System.Math.Min(num * 2, 60000);
			}
		}

		internal void Finish()
		{
			DtlsHandshakeRetransmit retransmit = null;
			if (!mSending)
			{
				CheckInboundFlight();
			}
			else if (mCurrentInboundFlight != null)
			{
				retransmit = new Retransmit(this);
			}
			mRecordLayer.HandshakeSuccessful(retransmit);
		}

		internal void ResetHandshakeMessagesDigest()
		{
			mHandshakeHash.Reset();
		}

		private void HandleRetransmittedHandshakeRecord(int epoch, byte[] buf, int off, int len)
		{
			if (len < 12)
			{
				return;
			}
			int num = TlsUtilities.ReadUint24(buf, off + 9);
			if (len != num + 12)
			{
				return;
			}
			int num2 = TlsUtilities.ReadUint16(buf, off + 4);
			if (num2 >= mNextReceiveSeq)
			{
				return;
			}
			byte b = TlsUtilities.ReadUint8(buf, off);
			int num3 = ((b == 20) ? 1 : 0);
			if (epoch != num3)
			{
				return;
			}
			int num4 = TlsUtilities.ReadUint24(buf, off + 1);
			int num5 = TlsUtilities.ReadUint24(buf, off + 6);
			if (num5 + num > num4)
			{
				return;
			}
			DtlsReassembler dtlsReassembler = (DtlsReassembler)mCurrentInboundFlight[num2];
			if (dtlsReassembler != null)
			{
				dtlsReassembler.ContributeFragment(b, num4, buf, off + 12, num5, num);
				if (CheckAll(mCurrentInboundFlight))
				{
					ResendOutboundFlight();
					ResetAll(mCurrentInboundFlight);
				}
			}
		}

		private void CheckInboundFlight()
		{
			foreach (object key in mCurrentInboundFlight.Keys)
			{
				int num = (int)key;
				int mNextReceiveSeq2 = mNextReceiveSeq;
			}
		}

		private void PrepareInboundFlight()
		{
			ResetAll(mCurrentInboundFlight);
			mPreviousInboundFlight = mCurrentInboundFlight;
			mCurrentInboundFlight = Platform.CreateHashtable();
		}

		private void ResendOutboundFlight()
		{
			mRecordLayer.ResetWriteEpoch();
			for (int i = 0; i < mOutboundFlight.Count; i++)
			{
				WriteMessage((Message)mOutboundFlight[i]);
			}
		}

		private Message UpdateHandshakeMessagesDigest(Message message)
		{
			if (message.Type != 0)
			{
				byte[] body = message.Body;
				byte[] array = new byte[12];
				TlsUtilities.WriteUint8(message.Type, array, 0);
				TlsUtilities.WriteUint24(body.Length, array, 1);
				TlsUtilities.WriteUint16(message.Seq, array, 4);
				TlsUtilities.WriteUint24(0, array, 6);
				TlsUtilities.WriteUint24(body.Length, array, 9);
				mHandshakeHash.BlockUpdate(array, 0, array.Length);
				mHandshakeHash.BlockUpdate(body, 0, body.Length);
			}
			return message;
		}

		private void WriteMessage(Message message)
		{
			int sendLimit = mRecordLayer.GetSendLimit();
			int num = sendLimit - 12;
			if (num < 1)
			{
				throw new TlsFatalAlert(80);
			}
			int num2 = message.Body.Length;
			int num3 = 0;
			do
			{
				int num4 = System.Math.Min(num2 - num3, num);
				WriteHandshakeFragment(message, num3, num4);
				num3 += num4;
			}
			while (num3 < num2);
		}

		private void WriteHandshakeFragment(Message message, int fragment_offset, int fragment_length)
		{
			RecordLayerBuffer recordLayerBuffer = new RecordLayerBuffer(12 + fragment_length);
			TlsUtilities.WriteUint8(message.Type, recordLayerBuffer);
			TlsUtilities.WriteUint24(message.Body.Length, recordLayerBuffer);
			TlsUtilities.WriteUint16(message.Seq, recordLayerBuffer);
			TlsUtilities.WriteUint24(fragment_offset, recordLayerBuffer);
			TlsUtilities.WriteUint24(fragment_length, recordLayerBuffer);
			recordLayerBuffer.Write(message.Body, fragment_offset, fragment_length);
			recordLayerBuffer.SendToRecordLayer(mRecordLayer);
		}

		private static bool CheckAll(IDictionary inboundFlight)
		{
			foreach (DtlsReassembler value in inboundFlight.Values)
			{
				if (value.GetBodyIfComplete() == null)
				{
					return false;
				}
			}
			return true;
		}

		private static void ResetAll(IDictionary inboundFlight)
		{
			foreach (DtlsReassembler value in inboundFlight.Values)
			{
				value.Reset();
			}
		}
	}
}
