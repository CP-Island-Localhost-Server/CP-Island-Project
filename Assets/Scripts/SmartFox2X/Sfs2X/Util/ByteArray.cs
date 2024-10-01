using System;
using System.IO;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using Sfs2X.Entities.Data;

namespace Sfs2X.Util
{
	public class ByteArray
	{
		private byte[] buffer;

		private int position = 0;

		private bool compressed = false;

		public byte[] Bytes
		{
			get
			{
				return buffer;
			}
			set
			{
				buffer = value;
				compressed = false;
			}
		}

		public int Length
		{
			get
			{
				return buffer.Length;
			}
		}

		public int Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		public int BytesAvailable
		{
			get
			{
				int num = buffer.Length - position;
				if (num > buffer.Length || num < 0)
				{
					num = 0;
				}
				return num;
			}
		}

		public bool Compressed
		{
			get
			{
				return compressed;
			}
			set
			{
				compressed = value;
			}
		}

		public ByteArray()
		{
			buffer = new byte[0];
		}

		public ByteArray(byte[] buf)
		{
			buffer = buf;
		}

		public void Compress()
		{
			if (compressed)
			{
				throw new Exception("Buffer is already compressed");
			}
			MemoryStream memoryStream = new MemoryStream();
			using (ZOutputStream zOutputStream = new ZOutputStream(memoryStream, 9))
			{
				zOutputStream.Write(buffer, 0, buffer.Length);
				zOutputStream.Flush();
			}
			buffer = memoryStream.ToArray();
			position = 0;
			compressed = true;
		}

		public void Uncompress()
		{
			MemoryStream memoryStream = new MemoryStream();
			using (ZOutputStream zOutputStream = new ZOutputStream(memoryStream))
			{
				zOutputStream.Write(buffer, 0, buffer.Length);
				zOutputStream.Flush();
			}
			buffer = memoryStream.ToArray();
			position = 0;
			compressed = false;
		}

		private void CheckCompressedWrite()
		{
			if (compressed)
			{
				throw new Exception("Only raw bytes can be written to a compressed array. Call Uncompress first.");
			}
		}

		private void CheckCompressedRead()
		{
			if (compressed)
			{
				throw new Exception("Only raw bytes can be read from a compressed array.");
			}
		}

		public byte[] ReverseOrder(byte[] dt)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return dt;
			}
			byte[] array = new byte[dt.Length];
			int num = 0;
			for (int num2 = dt.Length - 1; num2 >= 0; num2--)
			{
				array[num++] = dt[num2];
			}
			return array;
		}

		public void WriteByte(SFSDataType tp)
		{
			WriteByte(Convert.ToByte((int)tp));
		}

		public void WriteByte(byte b)
		{
			WriteBytes(new byte[1] { b });
		}

		public void WriteBytes(byte[] data)
		{
			WriteBytes(data, 0, data.Length);
		}

		public void WriteBytes(byte[] data, int ofs, int count)
		{
			byte[] dst = new byte[count + buffer.Length];
			Buffer.BlockCopy(buffer, 0, dst, 0, buffer.Length);
			Buffer.BlockCopy(data, ofs, dst, buffer.Length, count);
			buffer = dst;
		}

		public void WriteBool(bool b)
		{
			CheckCompressedWrite();
			WriteBytes(new byte[1] { (byte)(b ? 1 : 0) });
		}

		public void WriteInt(int i)
		{
			CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(i);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteUShort(ushort us)
		{
			CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(us);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteShort(short s)
		{
			CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(s);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteLong(long l)
		{
			CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(l);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteFloat(float f)
		{
			CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(f);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteDouble(double d)
		{
			CheckCompressedWrite();
			byte[] bytes = BitConverter.GetBytes(d);
			WriteBytes(ReverseOrder(bytes));
		}

		public void WriteUTF(string str)
		{
			CheckCompressedWrite();
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			int num = bytes.Length;
			if (num > 32767)
			{
				throw new FormatException("String length cannot be greater than " + short.MaxValue + " bytes!");
			}
			WriteUShort(Convert.ToUInt16(num));
			WriteBytes(bytes);
		}

		public void WriteText(string str)
		{
			CheckCompressedWrite();
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			WriteInt(bytes.Length);
			WriteBytes(bytes);
		}

		public byte ReadByte()
		{
			CheckCompressedRead();
			return buffer[position++];
		}

		public byte[] ReadBytes(int count)
		{
			byte[] array = new byte[count];
			Buffer.BlockCopy(buffer, position, array, 0, count);
			position += count;
			return array;
		}

		public bool ReadBool()
		{
			CheckCompressedRead();
			return buffer[position++] == 1;
		}

		public int ReadInt()
		{
			CheckCompressedRead();
			byte[] value = ReverseOrder(ReadBytes(4));
			return BitConverter.ToInt32(value, 0);
		}

		public ushort ReadUShort()
		{
			CheckCompressedRead();
			byte[] value = ReverseOrder(ReadBytes(2));
			return BitConverter.ToUInt16(value, 0);
		}

		public short ReadShort()
		{
			CheckCompressedRead();
			byte[] value = ReverseOrder(ReadBytes(2));
			return BitConverter.ToInt16(value, 0);
		}

		public long ReadLong()
		{
			CheckCompressedRead();
			byte[] value = ReverseOrder(ReadBytes(8));
			return BitConverter.ToInt64(value, 0);
		}

		public float ReadFloat()
		{
			CheckCompressedRead();
			byte[] value = ReverseOrder(ReadBytes(4));
			return BitConverter.ToSingle(value, 0);
		}

		public double ReadDouble()
		{
			CheckCompressedRead();
			byte[] value = ReverseOrder(ReadBytes(8));
			return BitConverter.ToDouble(value, 0);
		}

		public string ReadUTF()
		{
			CheckCompressedRead();
			ushort num = ReadUShort();
			string @string = Encoding.UTF8.GetString(buffer, position, num);
			position += num;
			return @string;
		}

		public string ReadText()
		{
			CheckCompressedRead();
			int num = ReadInt();
			string @string = Encoding.UTF8.GetString(buffer, position, num);
			position += num;
			return @string;
		}
	}
}
