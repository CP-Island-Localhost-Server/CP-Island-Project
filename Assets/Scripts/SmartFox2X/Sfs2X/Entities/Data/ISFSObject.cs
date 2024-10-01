using Sfs2X.Util;

namespace Sfs2X.Entities.Data
{
	public interface ISFSObject
	{
		bool IsNull(string key);

		bool ContainsKey(string key);

		void RemoveElement(string key);

		string[] GetKeys();

		int Size();

		ByteArray ToBinary();

		string ToJson();

		string GetDump(bool format);

		string GetDump();

		string GetHexDump();

		SFSDataWrapper GetData(string key);

		bool GetBool(string key);

		byte GetByte(string key);

		short GetShort(string key);

		int GetInt(string key);

		long GetLong(string key);

		float GetFloat(string key);

		double GetDouble(string key);

		string GetUtfString(string key);

		string GetText(string key);

		bool[] GetBoolArray(string key);

		ByteArray GetByteArray(string key);

		short[] GetShortArray(string key);

		int[] GetIntArray(string key);

		long[] GetLongArray(string key);

		float[] GetFloatArray(string key);

		double[] GetDoubleArray(string key);

		string[] GetUtfStringArray(string key);

		ISFSArray GetSFSArray(string key);

		ISFSObject GetSFSObject(string key);

		object GetClass(string key);

		void PutNull(string key);

		void PutBool(string key, bool val);

		void PutByte(string key, byte val);

		void PutShort(string key, short val);

		void PutInt(string key, int val);

		void PutLong(string key, long val);

		void PutFloat(string key, float val);

		void PutDouble(string key, double val);

		void PutUtfString(string key, string val);

		void PutText(string key, string val);

		void PutBoolArray(string key, bool[] val);

		void PutByteArray(string key, ByteArray val);

		void PutShortArray(string key, short[] val);

		void PutIntArray(string key, int[] val);

		void PutLongArray(string key, long[] val);

		void PutFloatArray(string key, float[] val);

		void PutDoubleArray(string key, double[] val);

		void PutUtfStringArray(string key, string[] val);

		void PutSFSArray(string key, ISFSArray val);

		void PutSFSObject(string key, ISFSObject val);

		void PutClass(string key, object val);

		void Put(string key, SFSDataWrapper val);
	}
}
