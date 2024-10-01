using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sfs2X.Exceptions;
using Sfs2X.Protocol.Serialization;
using Sfs2X.Util;

namespace Sfs2X.Entities.Data
{
	public class SFSArray : ISFSArray, ICollection, IEnumerable
	{
		private ISFSDataSerializer serializer;

		private List<SFSDataWrapper> dataHolder;

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		int ICollection.Count
		{
			get
			{
				return dataHolder.Count;
			}
		}

		public SFSArray()
		{
			dataHolder = new List<SFSDataWrapper>();
			serializer = DefaultSFSDataSerializer.Instance;
		}

		void ICollection.CopyTo(Array toArray, int index)
		{
			foreach (SFSDataWrapper item in dataHolder)
			{
				toArray.SetValue(item, index);
				index++;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SFSArrayEnumerator(dataHolder);
		}

		public static SFSArray NewFromBinaryData(ByteArray ba)
		{
			return DefaultSFSDataSerializer.Instance.Binary2Array(ba) as SFSArray;
		}

		public static ISFSArray NewFromJsonData(string js)
		{
			return DefaultSFSDataSerializer.Instance.Json2Array(js);
		}

		public static SFSArray NewInstance()
		{
			return new SFSArray();
		}

		public bool Contains(object obj)
		{
			if (obj is ISFSArray || obj is ISFSObject)
			{
				throw new SFSError("ISFSArray and ISFSObject are not supported by this method.");
			}
			for (int i = 0; i < Size(); i++)
			{
				object elementAt = GetElementAt(i);
				if (object.Equals(elementAt, obj))
				{
					return true;
				}
			}
			return false;
		}

		public SFSDataWrapper GetWrappedElementAt(int index)
		{
			return dataHolder[index];
		}

		public object GetElementAt(int index)
		{
			object result = null;
			if (dataHolder[index] != null)
			{
				result = dataHolder[index].Data;
			}
			return result;
		}

		public object RemoveElementAt(int index)
		{
			if (index >= dataHolder.Count)
			{
				return null;
			}
			SFSDataWrapper sFSDataWrapper = dataHolder[index];
			dataHolder.RemoveAt(index);
			return sFSDataWrapper.Data;
		}

		public int Size()
		{
			return dataHolder.Count;
		}

		public ByteArray ToBinary()
		{
			return serializer.Array2Binary(this);
		}

		public string ToJson()
		{
			return serializer.Array2Json(flatten());
		}

		private List<object> flatten()
		{
			List<object> list = new List<object>();
			DefaultSFSDataSerializer.Instance.flattenArray(list, this);
			return list;
		}

		public string GetDump()
		{
			return GetDump(true);
		}

		public string GetDump(bool format)
		{
			if (!format)
			{
				return Dump();
			}
			return DefaultObjectDumpFormatter.PrettyPrintDump(Dump());
		}

		private string Dump()
		{
			StringBuilder stringBuilder = new StringBuilder(Convert.ToString(DefaultObjectDumpFormatter.TOKEN_INDENT_OPEN));
			for (int i = 0; i < dataHolder.Count; i++)
			{
				SFSDataWrapper sFSDataWrapper = dataHolder[i];
				int type = sFSDataWrapper.Type;
				string value;
				switch (type)
				{
				case 18:
					value = (sFSDataWrapper.Data as SFSObject).GetDump(false);
					break;
				case 17:
					value = (sFSDataWrapper.Data as SFSArray).GetDump(false);
					break;
				case 0:
					value = "NULL";
					break;
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
					value = string.Concat("[", sFSDataWrapper.Data, "]");
					break;
				default:
					value = sFSDataWrapper.Data.ToString();
					break;
				}
				stringBuilder.Append("(" + ((SFSDataType)type).ToString().ToLower() + ") ");
				stringBuilder.Append(value);
				stringBuilder.Append(Convert.ToString(DefaultObjectDumpFormatter.TOKEN_DIVIDER));
			}
			string text = stringBuilder.ToString();
			if (Size() > 0)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text + Convert.ToString(DefaultObjectDumpFormatter.TOKEN_INDENT_CLOSE);
		}

		public string GetHexDump()
		{
			return DefaultObjectDumpFormatter.HexDump(ToBinary());
		}

		public void AddNull()
		{
			AddObject(null, SFSDataType.NULL);
		}

		public void AddBool(bool val)
		{
			AddObject(val, SFSDataType.BOOL);
		}

		public void AddByte(byte val)
		{
			AddObject(val, SFSDataType.BYTE);
		}

		public void AddShort(short val)
		{
			AddObject(val, SFSDataType.SHORT);
		}

		public void AddInt(int val)
		{
			AddObject(val, SFSDataType.INT);
		}

		public void AddLong(long val)
		{
			AddObject(val, SFSDataType.LONG);
		}

		public void AddFloat(float val)
		{
			AddObject(val, SFSDataType.FLOAT);
		}

		public void AddDouble(double val)
		{
			AddObject(val, SFSDataType.DOUBLE);
		}

		public void AddUtfString(string val)
		{
			AddObject(val, SFSDataType.UTF_STRING);
		}

		public void AddText(string val)
		{
			AddObject(val, SFSDataType.TEXT);
		}

		public void AddBoolArray(bool[] val)
		{
			AddObject(val, SFSDataType.BOOL_ARRAY);
		}

		public void AddByteArray(ByteArray val)
		{
			AddObject(val, SFSDataType.BYTE_ARRAY);
		}

		public void AddShortArray(short[] val)
		{
			AddObject(val, SFSDataType.SHORT_ARRAY);
		}

		public void AddIntArray(int[] val)
		{
			AddObject(val, SFSDataType.INT_ARRAY);
		}

		public void AddLongArray(long[] val)
		{
			AddObject(val, SFSDataType.LONG_ARRAY);
		}

		public void AddFloatArray(float[] val)
		{
			AddObject(val, SFSDataType.FLOAT_ARRAY);
		}

		public void AddDoubleArray(double[] val)
		{
			AddObject(val, SFSDataType.DOUBLE_ARRAY);
		}

		public void AddUtfStringArray(string[] val)
		{
			AddObject(val, SFSDataType.UTF_STRING_ARRAY);
		}

		public void AddSFSArray(ISFSArray val)
		{
			AddObject(val, SFSDataType.SFS_ARRAY);
		}

		public void AddSFSObject(ISFSObject val)
		{
			AddObject(val, SFSDataType.SFS_OBJECT);
		}

		public void AddClass(object val)
		{
			AddObject(val, SFSDataType.CLASS);
		}

		public void Add(SFSDataWrapper wrappedObject)
		{
			dataHolder.Add(wrappedObject);
		}

		private void AddObject(object val, SFSDataType tp)
		{
			Add(new SFSDataWrapper((int)tp, val));
		}

		public bool IsNull(int index)
		{
			if (index >= dataHolder.Count)
			{
				return true;
			}
			SFSDataWrapper sFSDataWrapper = dataHolder[index];
			return sFSDataWrapper.Type == 0;
		}

		public T GetValue<T>(int index)
		{
			if (index >= dataHolder.Count)
			{
				return default(T);
			}
			SFSDataWrapper sFSDataWrapper = dataHolder[index];
			return (T)sFSDataWrapper.Data;
		}

		public virtual bool GetBool(int index)
		{
			return GetValue<bool>(index);
		}

		public virtual byte GetByte(int index)
		{
			return GetValue<byte>(index);
		}

		public virtual short GetShort(int index)
		{
			return GetValue<short>(index);
		}

		public virtual int GetInt(int index)
		{
			return GetValue<int>(index);
		}

		public virtual long GetLong(int index)
		{
			return GetValue<long>(index);
		}

		public virtual float GetFloat(int index)
		{
			return GetValue<float>(index);
		}

		public virtual double GetDouble(int index)
		{
			return GetValue<double>(index);
		}

		public string GetUtfString(int index)
		{
			return GetValue<string>(index);
		}

		public string GetText(int index)
		{
			return GetValue<string>(index);
		}

		private ICollection GetArray(int index)
		{
			return GetValue<ICollection>(index);
		}

		public virtual bool[] GetBoolArray(int index)
		{
			return (bool[])GetArray(index);
		}

		public virtual ByteArray GetByteArray(int index)
		{
			return GetValue<ByteArray>(index);
		}

		public virtual short[] GetShortArray(int index)
		{
			return (short[])GetArray(index);
		}

		public virtual int[] GetIntArray(int index)
		{
			return (int[])GetArray(index);
		}

		public virtual long[] GetLongArray(int index)
		{
			return (long[])GetArray(index);
		}

		public virtual float[] GetFloatArray(int index)
		{
			return (float[])GetArray(index);
		}

		public virtual double[] GetDoubleArray(int index)
		{
			return (double[])GetArray(index);
		}

		public virtual string[] GetUtfStringArray(int index)
		{
			return (string[])GetArray(index);
		}

		public virtual ISFSArray GetSFSArray(int index)
		{
			return GetValue<ISFSArray>(index);
		}

		public virtual object GetClass(int index)
		{
			SFSDataWrapper sFSDataWrapper = dataHolder[index];
			return (sFSDataWrapper == null) ? null : sFSDataWrapper.Data;
		}

		public virtual ISFSObject GetSFSObject(int index)
		{
			return GetValue<ISFSObject>(index);
		}
	}
}
