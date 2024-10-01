using System;
using Sfs2X.Bitswarm;
using Sfs2X.Exceptions;
using Sfs2X.FSM;
using Sfs2X.Logging;
using Sfs2X.Protocol;
using Sfs2X.Protocol.Serialization;
using Sfs2X.Util;

namespace Sfs2X.Core
{
	public class SFSIOHandler : IoHandler
	{
		public static readonly int SHORT_BYTE_SIZE = 2;

		public static readonly int INT_BYTE_SIZE = 4;

		private readonly ByteArray EMPTY_BUFFER = new ByteArray();

		private BitSwarmClient bitSwarm;

		private Logger log;

		private PendingPacket pendingPacket;

		private IProtocolCodec protocolCodec;

		private IPacketEncrypter packetEncrypter;

		private int skipBytes = 0;

		private FiniteStateMachine fsm;

		public IProtocolCodec Codec
		{
			get
			{
				return protocolCodec;
			}
		}

		private PacketReadState ReadState
		{
			get
			{
				return (PacketReadState)fsm.GetCurrentState();
			}
		}

		public SFSIOHandler(ISocketClient bitSwarm)
		{
			this.bitSwarm = bitSwarm as BitSwarmClient;
			log = bitSwarm.Log;
			packetEncrypter = new DefaultPacketEncrypter(this.bitSwarm);
			protocolCodec = new SFSProtocolCodec(this, bitSwarm);
			InitStates();
		}

		private void InitStates()
		{
			fsm = new FiniteStateMachine();
			fsm.AddAllStates(typeof(PacketReadState));
			fsm.AddStateTransition(PacketReadState.WAIT_NEW_PACKET, PacketReadState.WAIT_DATA_SIZE, PacketReadTransition.HeaderReceived);
			fsm.AddStateTransition(PacketReadState.WAIT_DATA_SIZE, PacketReadState.WAIT_DATA, PacketReadTransition.SizeReceived);
			fsm.AddStateTransition(PacketReadState.WAIT_DATA_SIZE, PacketReadState.WAIT_DATA_SIZE_FRAGMENT, PacketReadTransition.IncompleteSize);
			fsm.AddStateTransition(PacketReadState.WAIT_DATA_SIZE_FRAGMENT, PacketReadState.WAIT_DATA, PacketReadTransition.WholeSizeReceived);
			fsm.AddStateTransition(PacketReadState.WAIT_DATA, PacketReadState.WAIT_NEW_PACKET, PacketReadTransition.PacketFinished);
			fsm.AddStateTransition(PacketReadState.WAIT_DATA, PacketReadState.INVALID_DATA, PacketReadTransition.InvalidData);
			fsm.AddStateTransition(PacketReadState.INVALID_DATA, PacketReadState.WAIT_NEW_PACKET, PacketReadTransition.InvalidDataFinished);
			fsm.SetCurrentState(PacketReadState.WAIT_NEW_PACKET);
		}

		public void OnDataRead(ByteArray data)
		{
			if (data.Length == 0)
			{
				throw new SFSError("Unexpected empty packet data: no readable bytes available!");
			}
			if (bitSwarm != null && bitSwarm.Sfs.Debug)
			{
				if (data.Length > 1024)
				{
					log.Info("Data Read: Size > 1024, dump omitted");
				}
				else
				{
					log.Info("Data Read: " + DefaultObjectDumpFormatter.HexDump(data));
				}
			}
			data.Position = 0;
			while (data.Length > 0)
			{
				if (ReadState == PacketReadState.WAIT_NEW_PACKET)
				{
					data = HandleNewPacket(data);
				}
				else if (ReadState == PacketReadState.WAIT_DATA_SIZE)
				{
					data = HandleDataSize(data);
				}
				else if (ReadState == PacketReadState.WAIT_DATA_SIZE_FRAGMENT)
				{
					data = HandleDataSizeFragment(data);
				}
				else if (ReadState == PacketReadState.WAIT_DATA)
				{
					data = HandlePacketData(data);
				}
				else if (ReadState == PacketReadState.INVALID_DATA)
				{
					data = HandleInvalidData(data);
				}
			}
		}

		public void OnDataRead(string jsonData)
		{
		}

		private ByteArray HandleNewPacket(ByteArray data)
		{
			log.Debug("Handling New Packet of size " + data.Length);
			byte b = data.ReadByte();
			if (~(b & 0x80) > 0)
			{
				throw new SFSError("Unexpected header byte: " + b + "\n" + DefaultObjectDumpFormatter.HexDump(data));
			}
			PacketHeader header = PacketHeader.FromBinary(b);
			pendingPacket = new PendingPacket(header);
			fsm.ApplyTransition(PacketReadTransition.HeaderReceived);
			return ResizeByteArray(data, 1, data.Length - 1);
		}

		private ByteArray HandleDataSize(ByteArray data)
		{
			log.Debug("Handling Header Size. Length: " + data.Length + " (" + ((!pendingPacket.Header.BigSized) ? "small" : "big") + ")");
			int num = -1;
			int num2 = SHORT_BYTE_SIZE;
			if (pendingPacket.Header.BigSized)
			{
				if (data.Length >= INT_BYTE_SIZE)
				{
					num = data.ReadInt();
				}
				num2 = 4;
			}
			else if (data.Length >= SHORT_BYTE_SIZE)
			{
				num = data.ReadUShort();
			}
			log.Debug("Data size is " + num);
			if (num != -1)
			{
				pendingPacket.Header.ExpectedLength = num;
				data = ResizeByteArray(data, num2, data.Length - num2);
				fsm.ApplyTransition(PacketReadTransition.SizeReceived);
			}
			else
			{
				fsm.ApplyTransition(PacketReadTransition.IncompleteSize);
				pendingPacket.Buffer.WriteBytes(data.Bytes);
				data = EMPTY_BUFFER;
			}
			return data;
		}

		private ByteArray HandleDataSizeFragment(ByteArray data)
		{
			log.Debug("Handling Size fragment. Data: " + data.Length);
			int num = ((!pendingPacket.Header.BigSized) ? (SHORT_BYTE_SIZE - pendingPacket.Buffer.Length) : (INT_BYTE_SIZE - pendingPacket.Buffer.Length));
			if (data.Length >= num)
			{
				pendingPacket.Buffer.WriteBytes(data.Bytes, 0, num);
				int count = ((!pendingPacket.Header.BigSized) ? 2 : 4);
				ByteArray byteArray = new ByteArray();
				byteArray.WriteBytes(pendingPacket.Buffer.Bytes, 0, count);
				byteArray.Position = 0;
				int num2 = ((!pendingPacket.Header.BigSized) ? byteArray.ReadShort() : byteArray.ReadInt());
				log.Debug("DataSize is ready: " + num2 + " bytes");
				pendingPacket.Header.ExpectedLength = num2;
				pendingPacket.Buffer = new ByteArray();
				fsm.ApplyTransition(PacketReadTransition.WholeSizeReceived);
				data = ((data.Length <= num) ? EMPTY_BUFFER : ResizeByteArray(data, num, data.Length - num));
			}
			else
			{
				pendingPacket.Buffer.WriteBytes(data.Bytes);
				data = EMPTY_BUFFER;
			}
			return data;
		}

		private ByteArray HandlePacketData(ByteArray data)
		{
			int num = pendingPacket.Header.ExpectedLength - pendingPacket.Buffer.Length;
			bool flag = data.Length > num;
			ByteArray result = new ByteArray(data.Bytes);
			try
			{
				log.Debug("Handling Data: " + data.Length + ", previous state: " + pendingPacket.Buffer.Length + "/" + pendingPacket.Header.ExpectedLength);
				if (data.Length >= num)
				{
					pendingPacket.Buffer.WriteBytes(data.Bytes, 0, num);
					log.Debug("<<< Packet Complete >>>");
					if (pendingPacket.Header.Encrypted)
					{
						packetEncrypter.Decrypt(pendingPacket.Buffer);
					}
					if (pendingPacket.Header.Compressed)
					{
						pendingPacket.Buffer.Uncompress();
					}
					protocolCodec.OnPacketRead(pendingPacket.Buffer);
					fsm.ApplyTransition(PacketReadTransition.PacketFinished);
				}
				else
				{
					pendingPacket.Buffer.WriteBytes(data.Bytes);
				}
				data = ((!flag) ? EMPTY_BUFFER : ResizeByteArray(data, num, data.Length - num));
			}
			catch (Exception ex)
			{
				log.Error("Error handling data: " + ex.Message + " " + ex.StackTrace);
				skipBytes = num;
				fsm.ApplyTransition(PacketReadTransition.InvalidData);
				return result;
			}
			return data;
		}

		private ByteArray HandleInvalidData(ByteArray data)
		{
			if (skipBytes == 0)
			{
				fsm.ApplyTransition(PacketReadTransition.InvalidDataFinished);
				return data;
			}
			int num = Math.Min(data.Length, skipBytes);
			data = ResizeByteArray(data, num, data.Length - num);
			skipBytes -= num;
			return data;
		}

		private ByteArray ResizeByteArray(ByteArray array, int pos, int len)
		{
			byte[] array2 = new byte[len];
			Buffer.BlockCopy(array.Bytes, pos, array2, 0, len);
			return new ByteArray(array2);
		}

		private void WriteBinaryData(PacketHeader header, ByteArray binData, bool udp)
		{
			ByteArray byteArray = new ByteArray();
			if (header.Compressed)
			{
				binData.Compress();
			}
			if (header.Encrypted)
			{
				packetEncrypter.Encrypt(binData);
			}
			byteArray.WriteByte(header.Encode());
			if (header.BigSized)
			{
				byteArray.WriteInt(binData.Length);
			}
			else
			{
				byteArray.WriteUShort(Convert.ToUInt16(binData.Length));
			}
			byteArray.WriteBytes(binData.Bytes);
			if (bitSwarm.UseBlueBox)
			{
				bitSwarm.HttpClient.Send(byteArray);
			}
			else if (bitSwarm.Socket.IsConnected)
			{
				if (udp)
				{
					WriteUDP(byteArray);
				}
				else
				{
					WriteTCP(byteArray);
				}
			}
		}

		public void OnDataWrite(IMessage message)
		{
			ByteArray byteArray = message.Content.ToBinary();
			bool compressed = byteArray.Length > bitSwarm.CompressionThreshold;
			bool encrypted = bitSwarm.CryptoKey != null;
			if (byteArray.Length > bitSwarm.MaxMessageSize)
			{
				throw new SFSCodecError("Message size is too big: " + byteArray.Length + ", the server limit is: " + bitSwarm.MaxMessageSize);
			}
			int num = SHORT_BYTE_SIZE;
			if (byteArray.Length > 65535)
			{
				num = INT_BYTE_SIZE;
			}
			bool useBlueBox = bitSwarm.UseBlueBox;
			PacketHeader header = new PacketHeader(encrypted, compressed, useBlueBox, num == INT_BYTE_SIZE);
			if (bitSwarm.Debug)
			{
				log.Info("Data written: " + message.Content.GetHexDump());
			}
			bitSwarm.ThreadManager.EnqueueSend(WriteBinaryData, header, byteArray, message.IsUDP);
		}

		private void WriteTCP(ByteArray writeBuffer)
		{
			bitSwarm.Socket.Write(writeBuffer.Bytes);
		}

		private void WriteUDP(ByteArray writeBuffer)
		{
			bitSwarm.UdpManager.Send(writeBuffer);
		}
	}
}
