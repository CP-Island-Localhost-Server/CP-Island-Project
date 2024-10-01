using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SFSLitJson;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;
using Sfs2X.Util;

namespace Sfs2X.Protocol.Serialization
{
	public class DefaultSFSDataSerializer : ISFSDataSerializer
	{
		private static readonly string CLASS_MARKER_KEY = "$C";

		private static readonly string CLASS_FIELDS_KEY = "$F";

		private static readonly string FIELD_NAME_KEY = "N";

		private static readonly string FIELD_VALUE_KEY = "V";

		private static DefaultSFSDataSerializer instance;

		private static Assembly runningAssembly = null;

		public static DefaultSFSDataSerializer Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new DefaultSFSDataSerializer();
				}
				return instance;
			}
		}

		public static Assembly RunningAssembly
		{
			get
			{
				return runningAssembly;
			}
			set
			{
				runningAssembly = value;
			}
		}

		private DefaultSFSDataSerializer()
		{
		}

		public ByteArray Object2Binary(ISFSObject obj)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(Convert.ToByte(18));
			byteArray.WriteShort(Convert.ToInt16(obj.Size()));
			return Obj2bin(obj, byteArray);
		}

		private ByteArray Obj2bin(ISFSObject obj, ByteArray buffer)
		{
			string[] keys = obj.GetKeys();
			string[] array = keys;
			foreach (string text in array)
			{
				SFSDataWrapper data = obj.GetData(text);
				buffer = EncodeSFSObjectKey(buffer, text);
				buffer = EncodeObject(buffer, data.Type, data.Data);
			}
			return buffer;
		}

		public ByteArray Array2Binary(ISFSArray array)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(Convert.ToByte(17));
			byteArray.WriteShort(Convert.ToInt16(array.Size()));
			return Arr2bin(array, byteArray);
		}

		private ByteArray Arr2bin(ISFSArray array, ByteArray buffer)
		{
			for (int i = 0; i < array.Size(); i++)
			{
				SFSDataWrapper wrappedElementAt = array.GetWrappedElementAt(i);
				buffer = EncodeObject(buffer, wrappedElementAt.Type, wrappedElementAt.Data);
			}
			return buffer;
		}

		public ISFSObject Binary2Object(ByteArray data)
		{
			if (data.Length < 3)
			{
				throw new SFSCodecError("Can't decode an SFSObject. Byte data is insufficient. Size: " + data.Length + " byte(s)");
			}
			data.Position = 0;
			return DecodeSFSObject(data);
		}

		private ISFSObject DecodeSFSObject(ByteArray buffer)
		{
			SFSObject sFSObject = SFSObject.NewInstance();
			byte b = buffer.ReadByte();
			if (b != Convert.ToByte(18))
			{
				throw new SFSCodecError(string.Concat("Invalid SFSDataType. Expected: ", SFSDataType.SFS_OBJECT, ", found: ", b));
			}
			int num = buffer.ReadShort();
			if (num < 0)
			{
				throw new SFSCodecError("Can't decode SFSObject. Size is negative: " + num);
			}
			try
			{
				for (int i = 0; i < num; i++)
				{
					string text = buffer.ReadUTF();
					SFSDataWrapper sFSDataWrapper = DecodeObject(buffer);
					if (sFSDataWrapper != null)
					{
						sFSObject.Put(text, sFSDataWrapper);
						continue;
					}
					throw new SFSCodecError("Could not decode value for SFSObject with key: " + text);
				}
				return sFSObject;
			}
			catch (SFSCodecError sFSCodecError)
			{
				throw sFSCodecError;
			}
		}

		public ISFSArray Binary2Array(ByteArray data)
		{
			if (data.Length < 3)
			{
				throw new SFSCodecError("Can't decode an SFSArray. Byte data is insufficient. Size: " + data.Length + " byte(s)");
			}
			data.Position = 0;
			return DecodeSFSArray(data);
		}

		private ISFSArray DecodeSFSArray(ByteArray buffer)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			SFSDataType sFSDataType = (SFSDataType)Convert.ToInt32(buffer.ReadByte());
			if (sFSDataType != SFSDataType.SFS_ARRAY)
			{
				throw new SFSCodecError(string.Concat("Invalid SFSDataType. Expected: ", SFSDataType.SFS_ARRAY, ", found: ", sFSDataType));
			}
			int num = buffer.ReadShort();
			if (num < 0)
			{
				throw new SFSCodecError("Can't decode SFSArray. Size is negative: " + num);
			}
			try
			{
				for (int i = 0; i < num; i++)
				{
					SFSDataWrapper sFSDataWrapper = DecodeObject(buffer);
					if (sFSDataWrapper != null)
					{
						iSFSArray.Add(sFSDataWrapper);
						continue;
					}
					throw new SFSCodecError("Could not decode SFSArray item at index: " + i);
				}
				return iSFSArray;
			}
			catch (SFSCodecError sFSCodecError)
			{
				throw sFSCodecError;
			}
		}

		private SFSDataWrapper DecodeObject(ByteArray buffer)
		{
			SFSDataType sFSDataType = (SFSDataType)Convert.ToInt32(buffer.ReadByte());
			switch (sFSDataType)
			{
			case SFSDataType.NULL:
				return BinDecode_NULL(buffer);
			case SFSDataType.BOOL:
				return BinDecode_BOOL(buffer);
			case SFSDataType.BOOL_ARRAY:
				return BinDecode_BOOL_ARRAY(buffer);
			case SFSDataType.BYTE:
				return BinDecode_BYTE(buffer);
			case SFSDataType.BYTE_ARRAY:
				return BinDecode_BYTE_ARRAY(buffer);
			case SFSDataType.SHORT:
				return BinDecode_SHORT(buffer);
			case SFSDataType.SHORT_ARRAY:
				return BinDecode_SHORT_ARRAY(buffer);
			case SFSDataType.INT:
				return BinDecode_INT(buffer);
			case SFSDataType.INT_ARRAY:
				return BinDecode_INT_ARRAY(buffer);
			case SFSDataType.LONG:
				return BinDecode_LONG(buffer);
			case SFSDataType.LONG_ARRAY:
				return BinDecode_LONG_ARRAY(buffer);
			case SFSDataType.FLOAT:
				return BinDecode_FLOAT(buffer);
			case SFSDataType.FLOAT_ARRAY:
				return BinDecode_FLOAT_ARRAY(buffer);
			case SFSDataType.DOUBLE:
				return BinDecode_DOUBLE(buffer);
			case SFSDataType.DOUBLE_ARRAY:
				return BinDecode_DOUBLE_ARRAY(buffer);
			case SFSDataType.UTF_STRING:
				return BinDecode_UTF_STRING(buffer);
			case SFSDataType.TEXT:
				return BinDecode_TEXT(buffer);
			case SFSDataType.UTF_STRING_ARRAY:
				return BinDecode_UTF_STRING_ARRAY(buffer);
			case SFSDataType.SFS_ARRAY:
				buffer.Position--;
				return new SFSDataWrapper(17, DecodeSFSArray(buffer));
			case SFSDataType.SFS_OBJECT:
			{
				buffer.Position--;
				ISFSObject iSFSObject = DecodeSFSObject(buffer);
				byte type = Convert.ToByte(18);
				object data = iSFSObject;
				if (iSFSObject.ContainsKey(CLASS_MARKER_KEY) && iSFSObject.ContainsKey(CLASS_FIELDS_KEY))
				{
					type = Convert.ToByte(19);
					data = Sfs2Cs(iSFSObject);
				}
				return new SFSDataWrapper(type, data);
			}
			default:
				throw new Exception("Unknow SFSDataType ID: " + sFSDataType);
			}
		}

		private ByteArray EncodeObject(ByteArray buffer, int typeId, object data)
		{
			switch ((SFSDataType)typeId)
			{
			case SFSDataType.NULL:
				buffer = BinEncode_NULL(buffer);
				break;
			case SFSDataType.BOOL:
				buffer = BinEncode_BOOL(buffer, (bool)data);
				break;
			case SFSDataType.BYTE:
				buffer = BinEncode_BYTE(buffer, (byte)data);
				break;
			case SFSDataType.SHORT:
				buffer = BinEncode_SHORT(buffer, (short)data);
				break;
			case SFSDataType.INT:
				buffer = BinEncode_INT(buffer, (int)data);
				break;
			case SFSDataType.LONG:
				buffer = BinEncode_LONG(buffer, (long)data);
				break;
			case SFSDataType.FLOAT:
				buffer = BinEncode_FLOAT(buffer, (float)data);
				break;
			case SFSDataType.DOUBLE:
				buffer = BinEncode_DOUBLE(buffer, (double)data);
				break;
			case SFSDataType.UTF_STRING:
				buffer = BinEncode_UTF_STRING(buffer, (string)data);
				break;
			case SFSDataType.TEXT:
				buffer = BinEncode_TEXT(buffer, (string)data);
				break;
			case SFSDataType.BOOL_ARRAY:
				buffer = BinEncode_BOOL_ARRAY(buffer, (bool[])data);
				break;
			case SFSDataType.BYTE_ARRAY:
				buffer = BinEncode_BYTE_ARRAY(buffer, (ByteArray)data);
				break;
			case SFSDataType.SHORT_ARRAY:
				buffer = BinEncode_SHORT_ARRAY(buffer, (short[])data);
				break;
			case SFSDataType.INT_ARRAY:
				buffer = BinEncode_INT_ARRAY(buffer, (int[])data);
				break;
			case SFSDataType.LONG_ARRAY:
				buffer = BinEncode_LONG_ARRAY(buffer, (long[])data);
				break;
			case SFSDataType.FLOAT_ARRAY:
				buffer = BinEncode_FLOAT_ARRAY(buffer, (float[])data);
				break;
			case SFSDataType.DOUBLE_ARRAY:
				buffer = BinEncode_DOUBLE_ARRAY(buffer, (double[])data);
				break;
			case SFSDataType.UTF_STRING_ARRAY:
				buffer = BinEncode_UTF_STRING_ARRAY(buffer, (string[])data);
				break;
			case SFSDataType.SFS_ARRAY:
				buffer = AddData(buffer, Array2Binary((ISFSArray)data));
				break;
			case SFSDataType.SFS_OBJECT:
				buffer = AddData(buffer, Object2Binary((SFSObject)data));
				break;
			case SFSDataType.CLASS:
				buffer = AddData(buffer, Object2Binary(Cs2Sfs(data)));
				break;
			default:
				throw new SFSCodecError("Unrecognized type in SFSObject serialization: " + typeId);
			}
			return buffer;
		}

		private SFSDataWrapper BinDecode_NULL(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.NULL, null);
		}

		private SFSDataWrapper BinDecode_BOOL(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.BOOL, buffer.ReadBool());
		}

		private SFSDataWrapper BinDecode_BYTE(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.BYTE, buffer.ReadByte());
		}

		private SFSDataWrapper BinDecode_SHORT(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.SHORT, buffer.ReadShort());
		}

		private SFSDataWrapper BinDecode_INT(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.INT, buffer.ReadInt());
		}

		private SFSDataWrapper BinDecode_LONG(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.LONG, buffer.ReadLong());
		}

		private SFSDataWrapper BinDecode_FLOAT(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.FLOAT, buffer.ReadFloat());
		}

		private SFSDataWrapper BinDecode_DOUBLE(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.DOUBLE, buffer.ReadDouble());
		}

		private SFSDataWrapper BinDecode_UTF_STRING(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.UTF_STRING, buffer.ReadUTF());
		}

		private SFSDataWrapper BinDecode_TEXT(ByteArray buffer)
		{
			return new SFSDataWrapper(SFSDataType.UTF_STRING, buffer.ReadText());
		}

		private SFSDataWrapper BinDecode_BOOL_ARRAY(ByteArray buffer)
		{
			int typedArraySize = GetTypedArraySize(buffer);
			bool[] array = new bool[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadBool();
			}
			return new SFSDataWrapper(SFSDataType.BOOL_ARRAY, array);
		}

		private SFSDataWrapper BinDecode_BYTE_ARRAY(ByteArray buffer)
		{
			int num = buffer.ReadInt();
			if (num < 0)
			{
				throw new SFSCodecError("Array negative size: " + num);
			}
			ByteArray data = new ByteArray(buffer.ReadBytes(num));
			return new SFSDataWrapper(SFSDataType.BYTE_ARRAY, data);
		}

		private SFSDataWrapper BinDecode_SHORT_ARRAY(ByteArray buffer)
		{
			int typedArraySize = GetTypedArraySize(buffer);
			short[] array = new short[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadShort();
			}
			return new SFSDataWrapper(SFSDataType.SHORT_ARRAY, array);
		}

		private SFSDataWrapper BinDecode_INT_ARRAY(ByteArray buffer)
		{
			int typedArraySize = GetTypedArraySize(buffer);
			int[] array = new int[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadInt();
			}
			return new SFSDataWrapper(SFSDataType.INT_ARRAY, array);
		}

		private SFSDataWrapper BinDecode_LONG_ARRAY(ByteArray buffer)
		{
			int typedArraySize = GetTypedArraySize(buffer);
			long[] array = new long[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadLong();
			}
			return new SFSDataWrapper(SFSDataType.LONG_ARRAY, array);
		}

		private SFSDataWrapper BinDecode_FLOAT_ARRAY(ByteArray buffer)
		{
			int typedArraySize = GetTypedArraySize(buffer);
			float[] array = new float[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadFloat();
			}
			return new SFSDataWrapper(SFSDataType.FLOAT_ARRAY, array);
		}

		private SFSDataWrapper BinDecode_DOUBLE_ARRAY(ByteArray buffer)
		{
			int typedArraySize = GetTypedArraySize(buffer);
			double[] array = new double[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadDouble();
			}
			return new SFSDataWrapper(SFSDataType.DOUBLE_ARRAY, array);
		}

		private SFSDataWrapper BinDecode_UTF_STRING_ARRAY(ByteArray buffer)
		{
			int typedArraySize = GetTypedArraySize(buffer);
			string[] array = new string[typedArraySize];
			for (int i = 0; i < typedArraySize; i++)
			{
				array[i] = buffer.ReadUTF();
			}
			return new SFSDataWrapper(SFSDataType.UTF_STRING_ARRAY, array);
		}

		private int GetTypedArraySize(ByteArray buffer)
		{
			short num = buffer.ReadShort();
			if (num < 0)
			{
				throw new SFSCodecError("Array negative size: " + num);
			}
			return num;
		}

		private ByteArray BinEncode_NULL(ByteArray buffer)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte((byte)0);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_BOOL(ByteArray buffer, bool val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.BOOL);
			byteArray.WriteBool(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_BYTE(ByteArray buffer, byte val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.BYTE);
			byteArray.WriteByte(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_SHORT(ByteArray buffer, short val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.SHORT);
			byteArray.WriteShort(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_INT(ByteArray buffer, int val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.INT);
			byteArray.WriteInt(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_LONG(ByteArray buffer, long val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.LONG);
			byteArray.WriteLong(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_FLOAT(ByteArray buffer, float val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.FLOAT);
			byteArray.WriteFloat(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_DOUBLE(ByteArray buffer, double val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.DOUBLE);
			byteArray.WriteDouble(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_INT(ByteArray buffer, double val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.DOUBLE);
			byteArray.WriteDouble(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_UTF_STRING(ByteArray buffer, string val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.UTF_STRING);
			byteArray.WriteUTF(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_TEXT(ByteArray buffer, string val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.TEXT);
			byteArray.WriteText(val);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_BOOL_ARRAY(ByteArray buffer, bool[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.BOOL_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteBool(val[i]);
			}
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_BYTE_ARRAY(ByteArray buffer, ByteArray val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.BYTE_ARRAY);
			byteArray.WriteInt(val.Length);
			byteArray.WriteBytes(val.Bytes);
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_SHORT_ARRAY(ByteArray buffer, short[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.SHORT_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteShort(val[i]);
			}
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_INT_ARRAY(ByteArray buffer, int[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.INT_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteInt(val[i]);
			}
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_LONG_ARRAY(ByteArray buffer, long[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.LONG_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteLong(val[i]);
			}
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_FLOAT_ARRAY(ByteArray buffer, float[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.FLOAT_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteFloat(val[i]);
			}
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_DOUBLE_ARRAY(ByteArray buffer, double[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.DOUBLE_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteDouble(val[i]);
			}
			return AddData(buffer, byteArray);
		}

		private ByteArray BinEncode_UTF_STRING_ARRAY(ByteArray buffer, string[] val)
		{
			ByteArray byteArray = new ByteArray();
			byteArray.WriteByte(SFSDataType.UTF_STRING_ARRAY);
			byteArray.WriteShort(Convert.ToInt16(val.Length));
			for (int i = 0; i < val.Length; i++)
			{
				byteArray.WriteUTF(val[i]);
			}
			return AddData(buffer, byteArray);
		}

		private ByteArray EncodeSFSObjectKey(ByteArray buffer, string val)
		{
			buffer.WriteUTF(val);
			return buffer;
		}

		private ByteArray AddData(ByteArray buffer, ByteArray newData)
		{
			buffer.WriteBytes(newData.Bytes);
			return buffer;
		}

		public string Object2Json(Dictionary<string, object> map)
		{
			return JsonMapper.ToJson(map);
		}

		public void flattenObject(Dictionary<string, object> map, ISFSObject sfsObj)
		{
			string[] keys = sfsObj.GetKeys();
			foreach (string key in keys)
			{
				SFSDataWrapper data = sfsObj.GetData(key);
				if (data.Type == 18)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					map.Add(key, dictionary);
					flattenObject(dictionary, (ISFSObject)data.Data);
				}
				else if (data.Type == 17)
				{
					List<object> list = new List<object>();
					map.Add(key, list);
					flattenArray(list, (ISFSArray)data.Data);
				}
				else
				{
					map.Add(key, data.Data);
				}
			}
		}

		public string Array2Json(List<object> list)
		{
			return JsonMapper.ToJson(list);
		}

		public void flattenArray(List<object> list, ISFSArray sfsArray)
		{
			for (int i = 0; i < sfsArray.Size(); i++)
			{
				SFSDataWrapper wrappedElementAt = sfsArray.GetWrappedElementAt(i);
				if (wrappedElementAt.Type == 18)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					list.Add(dictionary);
					flattenObject(dictionary, (ISFSObject)wrappedElementAt.Data);
				}
				else if (wrappedElementAt.Type == 17)
				{
					List<object> list2 = new List<object>();
					list.Add(list2);
					flattenArray(list2, (ISFSArray)wrappedElementAt.Data);
				}
				else
				{
					list.Add(wrappedElementAt.Data);
				}
			}
		}

		public ISFSObject Json2Object(string jsonStr)
		{
			if (jsonStr.Length < 2)
			{
				throw new InvalidOperationException("Can't decode SFSObject: JSON String is too short. Len: " + jsonStr.Length);
			}
			JsonData jdo = JsonMapper.ToObject(jsonStr);
			return decodeSFSObject(jdo);
		}

		public ISFSArray Json2Array(string jsonStr)
		{
			if (jsonStr.Length < 2)
			{
				throw new InvalidOperationException("Can't decode SFSArray: JSON String is too short. Len: " + jsonStr.Length);
			}
			JsonData jdo = JsonMapper.ToObject(jsonStr);
			return decodeSFSArray(jdo);
		}

		private ISFSObject decodeSFSObject(JsonData jdo)
		{
			ISFSObject iSFSObject = SFSObjectLite.NewInstance();
			foreach (string key in jdo.Keys)
			{
				JsonData jdo2 = jdo[key];
				SFSDataWrapper sFSDataWrapper = decodeJsonObject(jdo2);
				if (sFSDataWrapper != null)
				{
					iSFSObject.Put(key, sFSDataWrapper);
					continue;
				}
				throw new InvalidOperationException("JSON > ISFSObject error: could not decode value for key: " + key);
			}
			return iSFSObject;
		}

		private ISFSArray decodeSFSArray(JsonData jdo)
		{
			ISFSArray iSFSArray = SFSArrayLite.NewInstance();
			for (int i = 0; i < jdo.Count; i++)
			{
				JsonData jsonData = jdo[i];
				SFSDataWrapper sFSDataWrapper = decodeJsonObject(jsonData);
				if (sFSDataWrapper != null)
				{
					iSFSArray.Add(sFSDataWrapper);
					continue;
				}
				throw new InvalidOperationException("JSON > ISFSArray error: could not decode value for object: " + jsonData);
			}
			return iSFSArray;
		}

		private SFSDataWrapper decodeJsonObject(JsonData jdo)
		{
			if (jdo == null)
			{
				return new SFSDataWrapper(SFSDataType.NULL, jdo);
			}
			if (jdo.IsInt)
			{
				return new SFSDataWrapper(SFSDataType.INT, (int)jdo);
			}
			if (jdo.IsLong)
			{
				return new SFSDataWrapper(SFSDataType.LONG, (long)jdo);
			}
			if (jdo.IsDouble)
			{
				return new SFSDataWrapper(SFSDataType.DOUBLE, (double)jdo);
			}
			if (jdo.IsBoolean)
			{
				return new SFSDataWrapper(SFSDataType.BOOL, (bool)jdo);
			}
			if (jdo.IsString)
			{
				return new SFSDataWrapper(SFSDataType.UTF_STRING, (string)jdo);
			}
			if (jdo.IsObject)
			{
				if (jdo.Keys.Count == 0)
				{
					return new SFSDataWrapper(SFSDataType.NULL, null);
				}
				return new SFSDataWrapper(SFSDataType.SFS_OBJECT, decodeSFSObject(jdo));
			}
			if (jdo.IsArray)
			{
				return new SFSDataWrapper(SFSDataType.SFS_ARRAY, decodeSFSArray(jdo));
			}
			throw new ArgumentException(string.Format("Unrecognized DataType while converting JsonData object to SFSObject. Object: %s, Type: %s", jdo.ToString(), (jdo != null) ? jdo.GetType().ToString() : "null"));
		}

		public ISFSObject Cs2Sfs(object csObj)
		{
			ISFSObject iSFSObject = SFSObject.NewInstance();
			ConvertCsObj(csObj, iSFSObject);
			return iSFSObject;
		}

		private void ConvertCsObj(object csObj, ISFSObject sfsObj)
		{
			Type type = csObj.GetType();
			string fullName = type.FullName;
			SerializableSFSType serializableSFSType = csObj as SerializableSFSType;
			if (serializableSFSType == null)
			{
				throw new SFSCodecError(string.Concat("Cannot serialize object: ", csObj, ", type: ", fullName, " -- It doesn't implement the SerializableSFSType interface"));
			}
			ISFSArray iSFSArray = SFSArray.NewInstance();
			sfsObj.PutUtfString(CLASS_MARKER_KEY, fullName);
			sfsObj.PutSFSArray(CLASS_FIELDS_KEY, iSFSArray);
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				string name = fieldInfo.Name;
				object value = fieldInfo.GetValue(csObj);
				ISFSObject iSFSObject = SFSObject.NewInstance();
				SFSDataWrapper sFSDataWrapper = WrapField(value);
				if (sFSDataWrapper != null)
				{
					iSFSObject.PutUtfString(FIELD_NAME_KEY, name);
					iSFSObject.Put(FIELD_VALUE_KEY, sFSDataWrapper);
					iSFSArray.AddSFSObject(iSFSObject);
					continue;
				}
				throw new SFSCodecError(string.Concat("Cannot serialize field of object: ", csObj, ", field: ", name, ", type: ", fieldInfo.GetType().Name, " -- unsupported type!"));
			}
		}

		private SFSDataWrapper WrapField(object val)
		{
			if (val == null)
			{
				return new SFSDataWrapper(SFSDataType.NULL, null);
			}
			SFSDataWrapper result = null;
			if (val is bool)
			{
				result = new SFSDataWrapper(SFSDataType.BOOL, val);
			}
			else if (val is byte)
			{
				result = new SFSDataWrapper(SFSDataType.BYTE, val);
			}
			else if (val is short)
			{
				result = new SFSDataWrapper(SFSDataType.SHORT, val);
			}
			else if (val is int)
			{
				result = new SFSDataWrapper(SFSDataType.INT, val);
			}
			else if (val is long)
			{
				result = new SFSDataWrapper(SFSDataType.LONG, val);
			}
			else if (val is float)
			{
				result = new SFSDataWrapper(SFSDataType.FLOAT, val);
			}
			else if (val is double)
			{
				result = new SFSDataWrapper(SFSDataType.DOUBLE, val);
			}
			else if (val is string)
			{
				result = new SFSDataWrapper(SFSDataType.UTF_STRING, val);
			}
			else if (val is ArrayList)
			{
				result = new SFSDataWrapper(SFSDataType.SFS_ARRAY, UnrollArray(val as ArrayList));
			}
			else if (val is SerializableSFSType)
			{
				result = new SFSDataWrapper(SFSDataType.SFS_OBJECT, Cs2Sfs(val));
			}
			else if (val is Hashtable)
			{
				result = new SFSDataWrapper(SFSDataType.SFS_OBJECT, UnrollDictionary(val as Hashtable));
			}
			return result;
		}

		private ISFSArray UnrollArray(ArrayList arr)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			foreach (object item in arr)
			{
				SFSDataWrapper sFSDataWrapper = WrapField(item);
				if (sFSDataWrapper == null)
				{
					throw new SFSCodecError(string.Concat("Cannot serialize field of array: ", item, " -- unsupported type!"));
				}
				iSFSArray.Add(sFSDataWrapper);
			}
			return iSFSArray;
		}

		private ISFSObject UnrollDictionary(Hashtable dict)
		{
			ISFSObject iSFSObject = SFSObject.NewInstance();
			foreach (string key in dict.Keys)
			{
				SFSDataWrapper sFSDataWrapper = WrapField(dict[key]);
				if (sFSDataWrapper == null)
				{
					throw new SFSCodecError(string.Concat("Cannot serialize field of dictionary with key: ", key, ", ", dict[key], " -- unsupported type!"));
				}
				iSFSObject.Put(key, sFSDataWrapper);
			}
			return iSFSObject;
		}

		public object Sfs2Cs(ISFSObject sfsObj)
		{
			if (!sfsObj.ContainsKey(CLASS_MARKER_KEY) || !sfsObj.ContainsKey(CLASS_FIELDS_KEY))
			{
				throw new SFSCodecError("The SFSObject passed does not represent any serialized class.");
			}
			string utfString = sfsObj.GetUtfString(CLASS_MARKER_KEY);
			Type type = null;
			type = ((runningAssembly != null) ? runningAssembly.GetType(utfString) : Type.GetType(utfString));
			if (type == null)
			{
				throw new SFSCodecError("Cannot find type: " + utfString);
			}
			object obj = Activator.CreateInstance(type);
			if (!(obj is SerializableSFSType))
			{
				throw new SFSCodecError(string.Concat("Cannot deserialize object: ", obj, ", type: ", utfString, " -- It doesn't implement the SerializableSFSType interface"));
			}
			ConvertSFSObject(sfsObj.GetSFSArray(CLASS_FIELDS_KEY), obj, type);
			return obj;
		}

		private void ConvertSFSObject(ISFSArray fieldList, object csObj, Type objType)
		{
			for (int i = 0; i < fieldList.Size(); i++)
			{
				ISFSObject sFSObject = fieldList.GetSFSObject(i);
				string utfString = sFSObject.GetUtfString(FIELD_NAME_KEY);
				SFSDataWrapper data = sFSObject.GetData(FIELD_VALUE_KEY);
				object value = UnwrapField(data);
				FieldInfo field = objType.GetField(utfString, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					field.SetValue(csObj, value);
				}
			}
		}

		private object UnwrapField(SFSDataWrapper wrapper)
		{
			object result = null;
			int type = wrapper.Type;
			if (type <= 8)
			{
				result = wrapper.Data;
			}
			else
			{
				switch (type)
				{
				case 17:
					result = RebuildArray(wrapper.Data as ISFSArray);
					break;
				case 18:
				{
					ISFSObject iSFSObject = wrapper.Data as ISFSObject;
					result = ((!iSFSObject.ContainsKey(CLASS_MARKER_KEY) || !iSFSObject.ContainsKey(CLASS_FIELDS_KEY)) ? RebuildDict(wrapper.Data as ISFSObject) : Sfs2Cs(iSFSObject));
					break;
				}
				case 19:
					result = wrapper.Data;
					break;
				}
			}
			return result;
		}

		private ArrayList RebuildArray(ISFSArray sfsArr)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < sfsArr.Size(); i++)
			{
				arrayList.Add(UnwrapField(sfsArr.GetWrappedElementAt(i)));
			}
			return arrayList;
		}

		private Hashtable RebuildDict(ISFSObject sfsObj)
		{
			Hashtable hashtable = new Hashtable();
			string[] keys = sfsObj.GetKeys();
			foreach (string key in keys)
			{
				hashtable[key] = UnwrapField(sfsObj.GetData(key));
			}
			return hashtable;
		}
	}
}
