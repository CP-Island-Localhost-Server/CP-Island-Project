namespace Org.BouncyCastle.Crypto.Tls
{
	public class SupplementalDataEntry
	{
		protected readonly int mDataType;

		protected readonly byte[] mData;

		public virtual int DataType
		{
			get
			{
				return mDataType;
			}
		}

		public virtual byte[] Data
		{
			get
			{
				return mData;
			}
		}

		public SupplementalDataEntry(int dataType, byte[] data)
		{
			mDataType = dataType;
			mData = data;
		}
	}
}
