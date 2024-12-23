using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DtlsEpoch
	{
		private readonly DtlsReplayWindow mReplayWindow = new DtlsReplayWindow();

		private readonly int mEpoch;

		private readonly TlsCipher mCipher;

		private long mSequenceNumber = 0L;

		internal TlsCipher Cipher
		{
			get
			{
				return mCipher;
			}
		}

		internal int Epoch
		{
			get
			{
				return mEpoch;
			}
		}

		internal DtlsReplayWindow ReplayWindow
		{
			get
			{
				return mReplayWindow;
			}
		}

		internal long SequenceNumber
		{
			get
			{
				return mSequenceNumber;
			}
		}

		internal DtlsEpoch(int epoch, TlsCipher cipher)
		{
			if (epoch < 0)
			{
				throw new ArgumentException("must be >= 0", "epoch");
			}
			if (cipher == null)
			{
				throw new ArgumentNullException("cipher");
			}
			mEpoch = epoch;
			mCipher = cipher;
		}

		internal long AllocateSequenceNumber()
		{
			return mSequenceNumber++;
		}
	}
}
