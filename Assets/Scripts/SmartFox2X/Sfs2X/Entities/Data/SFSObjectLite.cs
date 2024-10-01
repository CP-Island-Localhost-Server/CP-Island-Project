using System;
using Sfs2X.Util;

namespace Sfs2X.Entities.Data
{
	public class SFSObjectLite : SFSObject
	{
		public new static ISFSObject NewInstance()
		{
			return new SFSObjectLite();
		}

		public override object GetClass(string key)
		{
			throw new NotSupportedException("SFSObjectLite doesn't support class serialization");
		}

		public override byte GetByte(string key)
		{
			int num = base.GetInt(key);
			if (num < 0)
			{
				num = 256 + num;
			}
			return Convert.ToByte(num);
		}

		public override short GetShort(string key)
		{
			int @int = base.GetInt(key);
			return Convert.ToInt16(@int);
		}

		public override long GetLong(string key)
		{
			SFSDataWrapper data = GetData(key);
			if (data == null)
			{
				return 0L;
			}
			object data2 = data.Data;
			if (data2 is int)
			{
				return Convert.ToInt64(data2);
			}
			return (long)data2;
		}

		public override float GetFloat(string key)
		{
			double @double = base.GetDouble(key);
			return Convert.ToSingle(@double);
		}

		public override bool[] GetBoolArray(string key)
		{
			ISFSArray sFSArray = base.GetSFSArray(key);
			if (sFSArray == null)
			{
				return null;
			}
			bool[] array = new bool[sFSArray.Size()];
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				array.SetValue(sFSArray.GetBool(i), i);
			}
			return array;
		}

		public override ByteArray GetByteArray(string key)
		{
			throw new NotSupportedException("SFSObjectLite doesn't support ByteArray transmission");
		}

		public override short[] GetShortArray(string key)
		{
			ISFSArray sFSArray = base.GetSFSArray(key);
			if (sFSArray == null)
			{
				return null;
			}
			short[] array = new short[sFSArray.Size()];
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				array.SetValue((short)sFSArray.GetInt(i), i);
			}
			return array;
		}

		public override int[] GetIntArray(string key)
		{
			ISFSArray sFSArray = base.GetSFSArray(key);
			if (sFSArray == null)
			{
				return null;
			}
			int[] array = new int[sFSArray.Size()];
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				array.SetValue(sFSArray.GetInt(i), i);
			}
			return array;
		}

		public override long[] GetLongArray(string key)
		{
			ISFSArray sFSArray = base.GetSFSArray(key);
			if (sFSArray == null)
			{
				return null;
			}
			long[] array = new long[sFSArray.Size()];
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				array.SetValue(sFSArray.GetLong(i), i);
			}
			return array;
		}

		public override float[] GetFloatArray(string key)
		{
			ISFSArray sFSArray = base.GetSFSArray(key);
			if (sFSArray == null)
			{
				return null;
			}
			float[] array = new float[sFSArray.Size()];
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				array.SetValue((float)sFSArray.GetDouble(i), i);
			}
			return array;
		}

		public override double[] GetDoubleArray(string key)
		{
			ISFSArray sFSArray = base.GetSFSArray(key);
			if (sFSArray == null)
			{
				return null;
			}
			double[] array = new double[sFSArray.Size()];
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				array.SetValue(sFSArray.GetDouble(i), i);
			}
			return array;
		}

		public override string[] GetUtfStringArray(string key)
		{
			ISFSArray sFSArray = base.GetSFSArray(key);
			if (sFSArray == null)
			{
				return null;
			}
			string[] array = new string[sFSArray.Size()];
			for (int i = 0; i < sFSArray.Size(); i++)
			{
				array.SetValue(sFSArray.GetUtfString(i), i);
			}
			return array;
		}
	}
}
