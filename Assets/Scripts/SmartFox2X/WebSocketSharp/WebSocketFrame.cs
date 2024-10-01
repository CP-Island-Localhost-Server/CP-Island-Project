using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebSocketSharp
{
	internal class WebSocketFrame : IEnumerable, IEnumerable<byte>
	{
		private byte[] _extPayloadLength;

		private Fin _fin;

		private Mask _mask;

		private byte[] _maskingKey;

		private Opcode _opcode;

		private PayloadData _payloadData;

		private byte _payloadLength;

		private Rsv _rsv1;

		private Rsv _rsv2;

		private Rsv _rsv3;

		internal static readonly byte[] EmptyUnmaskPingBytes;

		public byte[] ExtendedPayloadLength
		{
			get
			{
				return _extPayloadLength;
			}
		}

		public Fin Fin
		{
			get
			{
				return _fin;
			}
		}

		public bool IsBinary
		{
			get
			{
				return _opcode == Opcode.Binary;
			}
		}

		public bool IsClose
		{
			get
			{
				return _opcode == Opcode.Close;
			}
		}

		public bool IsCompressed
		{
			get
			{
				return _rsv1 == Rsv.On;
			}
		}

		public bool IsContinuation
		{
			get
			{
				return _opcode == Opcode.Cont;
			}
		}

		public bool IsControl
		{
			get
			{
				return _opcode == Opcode.Close || _opcode == Opcode.Ping || _opcode == Opcode.Pong;
			}
		}

		public bool IsData
		{
			get
			{
				return _opcode == Opcode.Binary || _opcode == Opcode.Text;
			}
		}

		public bool IsFinal
		{
			get
			{
				return _fin == Fin.Final;
			}
		}

		public bool IsFragmented
		{
			get
			{
				return _fin == Fin.More || _opcode == Opcode.Cont;
			}
		}

		public bool IsMasked
		{
			get
			{
				return _mask == Mask.Mask;
			}
		}

		public bool IsPerMessageCompressed
		{
			get
			{
				return (_opcode == Opcode.Binary || _opcode == Opcode.Text) && _rsv1 == Rsv.On;
			}
		}

		public bool IsPing
		{
			get
			{
				return _opcode == Opcode.Ping;
			}
		}

		public bool IsPong
		{
			get
			{
				return _opcode == Opcode.Pong;
			}
		}

		public bool IsText
		{
			get
			{
				return _opcode == Opcode.Text;
			}
		}

		public ulong Length
		{
			get
			{
				return (ulong)(2L + (long)(_extPayloadLength.Length + _maskingKey.Length)) + _payloadData.Length;
			}
		}

		public Mask Mask
		{
			get
			{
				return _mask;
			}
		}

		public byte[] MaskingKey
		{
			get
			{
				return _maskingKey;
			}
		}

		public Opcode Opcode
		{
			get
			{
				return _opcode;
			}
		}

		public PayloadData PayloadData
		{
			get
			{
				return _payloadData;
			}
		}

		public byte PayloadLength
		{
			get
			{
				return _payloadLength;
			}
		}

		public Rsv Rsv1
		{
			get
			{
				return _rsv1;
			}
		}

		public Rsv Rsv2
		{
			get
			{
				return _rsv2;
			}
		}

		public Rsv Rsv3
		{
			get
			{
				return _rsv3;
			}
		}

		private WebSocketFrame()
		{
		}

		internal WebSocketFrame(Opcode opcode, PayloadData payloadData, bool mask)
			: this(Fin.Final, opcode, payloadData, false, mask)
		{
		}

		internal WebSocketFrame(Fin fin, Opcode opcode, byte[] data, bool compressed, bool mask)
			: this(fin, opcode, new PayloadData(data), compressed, mask)
		{
		}

		internal WebSocketFrame(Fin fin, Opcode opcode, PayloadData payloadData, bool compressed, bool mask)
		{
			_fin = fin;
			_rsv1 = ((isData(opcode) && compressed) ? Rsv.On : Rsv.Off);
			_rsv2 = Rsv.Off;
			_rsv3 = Rsv.Off;
			_opcode = opcode;
			ulong length = payloadData.Length;
			if (length < 126)
			{
				_payloadLength = (byte)length;
				_extPayloadLength = new byte[0];
			}
			else if (length < 65536)
			{
				_payloadLength = 126;
				_extPayloadLength = ((ushort)length).InternalToByteArray(ByteOrder.Big);
			}
			else
			{
				_payloadLength = 127;
				_extPayloadLength = length.InternalToByteArray(ByteOrder.Big);
			}
			if (mask)
			{
				_mask = Mask.Mask;
				_maskingKey = createMaskingKey();
				payloadData.Mask(_maskingKey);
			}
			else
			{
				_mask = Mask.Unmask;
				_maskingKey = new byte[0];
			}
			_payloadData = payloadData;
		}

		static WebSocketFrame()
		{
			EmptyUnmaskPingBytes = CreatePingFrame(false).ToByteArray();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static byte[] createMaskingKey()
		{
			byte[] array = new byte[4];
			Random random = new Random();
			random.NextBytes(array);
			return array;
		}

		private static string dump(WebSocketFrame frame)
		{
			ulong length = frame.Length;
			long num = (long)(length / 4);
			int num2 = (int)(length % 4);
			int num3;
			string arg5;
			if (num < 10000)
			{
				num3 = 4;
				arg5 = "{0,4}";
			}
			else if (num < 65536)
			{
				num3 = 4;
				arg5 = "{0,4:X}";
			}
			else if (num < 4294967296L)
			{
				num3 = 8;
				arg5 = "{0,8:X}";
			}
			else
			{
				num3 = 16;
				arg5 = "{0,16:X}";
			}
			string arg6 = string.Format("{{0,{0}}}", num3);
			string format = string.Format("\n{0} 01234567 89ABCDEF 01234567 89ABCDEF\n{0}+--------+--------+--------+--------+\\n", arg6);
			string lineFmt = string.Format("{0}|{{1,8}} {{2,8}} {{3,8}} {{4,8}}|\n", arg5);
			string format2 = string.Format("{0}+--------+--------+--------+--------+", arg6);
			StringBuilder output = new StringBuilder(64);
			Func<Action<string, string, string, string>> func = delegate
			{
				long lineCnt = 0L;
				return delegate(string arg1, string arg2, string arg3, string arg4)
				{
					output.AppendFormat(lineFmt, ++lineCnt, arg1, arg2, arg3, arg4);
				};
			};
			output.AppendFormat(format, string.Empty);
			Action<string, string, string, string> action = func();
			byte[] array = frame.ToByteArray();
			for (long num4 = 0L; num4 <= num; num4++)
			{
				long num5 = num4 * 4;
				if (num4 < num)
				{
					action(Convert.ToString(array[num5], 2).PadLeft(8, '0'), Convert.ToString(array[num5 + 1], 2).PadLeft(8, '0'), Convert.ToString(array[num5 + 2], 2).PadLeft(8, '0'), Convert.ToString(array[num5 + 3], 2).PadLeft(8, '0'));
				}
				else if (num2 > 0)
				{
					action(Convert.ToString(array[num5], 2).PadLeft(8, '0'), (num2 < 2) ? string.Empty : Convert.ToString(array[num5 + 1], 2).PadLeft(8, '0'), (num2 != 3) ? string.Empty : Convert.ToString(array[num5 + 2], 2).PadLeft(8, '0'), string.Empty);
				}
			}
			output.AppendFormat(format2, string.Empty);
			return output.ToString();
		}

		private static bool isControl(Opcode opcode)
		{
			return opcode == Opcode.Close || opcode == Opcode.Ping || opcode == Opcode.Pong;
		}

		private static bool isData(Opcode opcode)
		{
			return opcode == Opcode.Text || opcode == Opcode.Binary;
		}

		private static string print(WebSocketFrame frame)
		{
			string text = frame._opcode.ToString();
			byte payloadLength = frame._payloadLength;
			string text2 = ((payloadLength < 126) ? string.Empty : ((payloadLength != 126) ? frame._extPayloadLength.ToUInt64(ByteOrder.Big).ToString() : frame._extPayloadLength.ToUInt16(ByteOrder.Big).ToString()));
			bool isMasked = frame.IsMasked;
			string text3 = ((!isMasked) ? string.Empty : BitConverter.ToString(frame._maskingKey));
			string text4 = ((payloadLength == 0) ? string.Empty : ((payloadLength > 125) ? string.Format("A {0} frame.", text.ToLower()) : ((isMasked || frame.IsFragmented || frame.IsCompressed || !frame.IsText) ? frame._payloadData.ToString() : Encoding.UTF8.GetString(frame._payloadData.ApplicationData))));
			string format = "\n                    FIN: {0}\n                   RSV1: {1}\n                   RSV2: {2}\n                   RSV3: {3}\n                 Opcode: {4}\n                   MASK: {5}\n         Payload Length: {6}\nExtended Payload Length: {7}\n            Masking Key: {8}\n           Payload Data: {9}";
			return string.Format(format, frame._fin, frame._rsv1, frame._rsv2, frame._rsv3, text, frame._mask, payloadLength, text2, text3, text4);
		}

		private static WebSocketFrame read(byte[] header, Stream stream, bool unmask)
		{
			Fin fin = (((header[0] & 0x80) == 128) ? Fin.Final : Fin.More);
			Rsv rsv = (((header[0] & 0x40) == 64) ? Rsv.On : Rsv.Off);
			Rsv rsv2 = (((header[0] & 0x20) == 32) ? Rsv.On : Rsv.Off);
			Rsv rsv3 = (((header[0] & 0x10) == 16) ? Rsv.On : Rsv.Off);
			Opcode opcode = (Opcode)(header[0] & 0xFu);
			Mask mask = (((header[1] & 0x80) == 128) ? Mask.Mask : Mask.Unmask);
			byte b = (byte)(header[1] & 0x7Fu);
			string text = ((isControl(opcode) && b > 125) ? "A control frame has a payload data which is greater than the allowable max size." : ((isControl(opcode) && fin == Fin.More) ? "A control frame is fragmented." : ((isData(opcode) || rsv != Rsv.On) ? null : "A non data frame is compressed.")));
			if (text != null)
			{
				throw new WebSocketException(CloseStatusCode.ProtocolError, text);
			}
			WebSocketFrame webSocketFrame = new WebSocketFrame();
			webSocketFrame._fin = fin;
			webSocketFrame._rsv1 = rsv;
			webSocketFrame._rsv2 = rsv2;
			webSocketFrame._rsv3 = rsv3;
			webSocketFrame._opcode = opcode;
			webSocketFrame._mask = mask;
			webSocketFrame._payloadLength = b;
			int num = ((b >= 126) ? ((b != 126) ? 8 : 2) : 0);
			byte[] array = ((num <= 0) ? new byte[0] : stream.ReadBytes(num));
			if (num > 0 && array.Length != num)
			{
				throw new WebSocketException("The 'Extended Payload Length' of a frame cannot be read from the data source.");
			}
			webSocketFrame._extPayloadLength = array;
			bool flag = mask == Mask.Mask;
			byte[] array2 = ((!flag) ? new byte[0] : stream.ReadBytes(4));
			if (flag && array2.Length != 4)
			{
				throw new WebSocketException("The 'Masking Key' of a frame cannot be read from the data source.");
			}
			webSocketFrame._maskingKey = array2;
			ulong num2 = ((b < 126) ? b : ((b != 126) ? array.ToUInt64(ByteOrder.Big) : array.ToUInt16(ByteOrder.Big)));
			byte[] array3 = null;
			if (num2 != 0)
			{
				if (b > 126 && num2 > long.MaxValue)
				{
					throw new WebSocketException(CloseStatusCode.TooBig, "The length of 'Payload Data' of a frame is greater than the allowable max length.");
				}
				array3 = ((b <= 126) ? stream.ReadBytes((int)num2) : stream.ReadBytes((long)num2, 1024));
				if (array3.LongLength != (long)num2)
				{
					throw new WebSocketException("The 'Payload Data' of a frame cannot be read from the data source.");
				}
			}
			else
			{
				array3 = new byte[0];
			}
			webSocketFrame._payloadData = new PayloadData(array3, flag);
			if (unmask && flag)
			{
				webSocketFrame.Unmask();
			}
			return webSocketFrame;
		}

		internal static WebSocketFrame CreateCloseFrame(PayloadData payloadData, bool mask)
		{
			return new WebSocketFrame(Fin.Final, Opcode.Close, payloadData, false, mask);
		}

		internal static WebSocketFrame CreatePingFrame(bool mask)
		{
			return new WebSocketFrame(Fin.Final, Opcode.Ping, new PayloadData(), false, mask);
		}

		internal static WebSocketFrame CreatePingFrame(byte[] data, bool mask)
		{
			return new WebSocketFrame(Fin.Final, Opcode.Ping, new PayloadData(data), false, mask);
		}

		internal static WebSocketFrame Read(Stream stream)
		{
			return Read(stream, true);
		}

		internal static WebSocketFrame Read(Stream stream, bool unmask)
		{
			byte[] array = stream.ReadBytes(2);
			if (array.Length != 2)
			{
				throw new WebSocketException("The header part of a frame cannot be read from the data source.");
			}
			return read(array, stream, unmask);
		}

		internal static void ReadAsync(Stream stream, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			ReadAsync(stream, true, completed, error);
		}

		internal static void ReadAsync(Stream stream, bool unmask, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			stream.ReadBytesAsync(2, delegate(byte[] header)
			{
				if (header.Length != 2)
				{
					throw new WebSocketException("The header part of a frame cannot be read from the data source.");
				}
				WebSocketFrame obj = read(header, stream, unmask);
				if (completed != null)
				{
					completed(obj);
				}
			}, error);
		}

		internal void Unmask()
		{
			if (_mask != 0)
			{
				_mask = Mask.Unmask;
				_payloadData.Mask(_maskingKey);
				_maskingKey = new byte[0];
			}
		}

		public IEnumerator<byte> GetEnumerator()
		{
			byte[] array = ToByteArray();
			for (int i = 0; i < array.Length; i++)
			{
				yield return array[i];
			}
		}

		public void Print(bool dumped)
		{
			Console.WriteLine((!dumped) ? print(this) : dump(this));
		}

		public string PrintToString(bool dumped)
		{
			return (!dumped) ? print(this) : dump(this);
		}

		public byte[] ToByteArray()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int fin = (int)_fin;
				fin = (fin << 1) + (int)_rsv1;
				fin = (fin << 1) + (int)_rsv2;
				fin = (fin << 1) + (int)_rsv3;
				fin = (fin << 4) + (int)_opcode;
				fin = (fin << 1) + (int)_mask;
				fin = (fin << 7) + _payloadLength;
				memoryStream.Write(((ushort)fin).InternalToByteArray(ByteOrder.Big), 0, 2);
				if (_payloadLength > 125)
				{
					memoryStream.Write(_extPayloadLength, 0, _extPayloadLength.Length);
				}
				if (_mask == Mask.Mask)
				{
					memoryStream.Write(_maskingKey, 0, _maskingKey.Length);
				}
				if (_payloadLength > 0)
				{
					byte[] array = _payloadData.ToByteArray();
					if (_payloadLength < 127)
					{
						memoryStream.Write(array, 0, array.Length);
					}
					else
					{
						memoryStream.WriteBytes(array);
					}
				}
				memoryStream.Close();
				return memoryStream.ToArray();
			}
		}

		public override string ToString()
		{
			return BitConverter.ToString(ToByteArray());
		}
	}
}
