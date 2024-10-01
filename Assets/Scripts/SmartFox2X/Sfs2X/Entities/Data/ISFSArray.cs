using System.Collections;
using Sfs2X.Util;

namespace Sfs2X.Entities.Data
{
	public interface ISFSArray : ICollection, IEnumerable
	{
		bool Contains(object obj);

		object GetElementAt(int index);

		SFSDataWrapper GetWrappedElementAt(int index);

		object RemoveElementAt(int index);

		int Size();

		ByteArray ToBinary();

		string ToJson();

		string GetDump(bool format);

		string GetDump();

		string GetHexDump();

		void AddNull();

		void AddBool(bool val);

		void AddByte(byte val);

		void AddShort(short val);

		void AddInt(int val);

		void AddLong(long val);

		void AddFloat(float val);

		void AddDouble(double val);

		void AddUtfString(string val);

		void AddText(string val);

		void AddBoolArray(bool[] val);

		void AddByteArray(ByteArray val);

		void AddShortArray(short[] val);

		void AddIntArray(int[] val);

		void AddLongArray(long[] val);

		void AddFloatArray(float[] val);

		void AddDoubleArray(double[] val);

		void AddUtfStringArray(string[] val);

		void AddSFSArray(ISFSArray val);

		void AddSFSObject(ISFSObject val);

		void AddClass(object val);

		void Add(SFSDataWrapper val);

		bool IsNull(int index);

		bool GetBool(int index);

		byte GetByte(int index);

		short GetShort(int index);

		int GetInt(int index);

		long GetLong(int index);

		float GetFloat(int index);

		double GetDouble(int index);

		string GetUtfString(int index);

		string GetText(int index);

		bool[] GetBoolArray(int index);

		ByteArray GetByteArray(int index);

		short[] GetShortArray(int index);

		int[] GetIntArray(int index);

		long[] GetLongArray(int index);

		float[] GetFloatArray(int index);

		double[] GetDoubleArray(int index);

		string[] GetUtfStringArray(int index);

		ISFSArray GetSFSArray(int index);

		ISFSObject GetSFSObject(int index);

		object GetClass(int index);
	}
}
