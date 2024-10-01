using System;
using Sfs2X.Util;

namespace Sfs2X.Entities.Data
{
	public class SFSArrayLite : SFSArray
	{
		public new static ISFSArray NewInstance()
		{
			return new SFSArrayLite();
		}

		public override object GetClass(int index)
		{
			throw new NotSupportedException("SFSArrayLite doesn't support class serialization");
		}

		public override byte GetByte(int index)
		{
			int num = base.GetInt(index);
			if (num < 0)
			{
				num = 256 + num;
			}
			return Convert.ToByte(num);
		}

		public override short GetShort(int index)
		{
			int @int = base.GetInt(index);
			return Convert.ToInt16(@int);
		}

		public override long GetLong(int index)
		{
			SFSDataWrapper wrappedElementAt = GetWrappedElementAt(index);
			if (wrappedElementAt == null)
			{
				return 0L;
			}
			object data = wrappedElementAt.Data;
			if (data is int)
			{
				return Convert.ToInt64(data);
			}
			return (long)data;
		}

		public override float GetFloat(int index)
		{
			double @double = base.GetDouble(index);
			return Convert.ToSingle(@double);
		}

		public override bool[] GetBoolArray(int index)
		{
			ISFSArray sFSArray = base.GetSFSArray(index);
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

		public override ByteArray GetByteArray(int index)
		{
			throw new NotSupportedException("SFSArrayLite doesn't support ByteArray transmission");
		}

		public override short[] GetShortArray(int index)
		{
			ISFSArray sFSArray = base.GetSFSArray(index);
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

		public override int[] GetIntArray(int index)
		{
			ISFSArray sFSArray = base.GetSFSArray(index);
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

		public override long[] GetLongArray(int index)
		{
			ISFSArray sFSArray = base.GetSFSArray(index);
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

		public override float[] GetFloatArray(int index)
		{
			ISFSArray sFSArray = base.GetSFSArray(index);
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

		public override double[] GetDoubleArray(int index)
		{
			ISFSArray sFSArray = base.GetSFSArray(index);
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

		public override string[] GetUtfStringArray(int index)
		{
			ISFSArray sFSArray = base.GetSFSArray(index);
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
