using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class SecurityParameters
	{
		internal int entity = -1;

		internal int cipherSuite = -1;

		internal byte compressionAlgorithm = 0;

		internal int prfAlgorithm = -1;

		internal int verifyDataLength = -1;

		internal byte[] masterSecret = null;

		internal byte[] clientRandom = null;

		internal byte[] serverRandom = null;

		internal byte[] sessionHash = null;

		internal byte[] pskIdentity = null;

		internal byte[] srpIdentity = null;

		internal short maxFragmentLength = -1;

		internal bool truncatedHMac = false;

		internal bool encryptThenMac = false;

		internal bool extendedMasterSecret = false;

		public virtual int Entity
		{
			get
			{
				return entity;
			}
		}

		public virtual int CipherSuite
		{
			get
			{
				return cipherSuite;
			}
		}

		public byte CompressionAlgorithm
		{
			get
			{
				return compressionAlgorithm;
			}
		}

		public virtual int PrfAlgorithm
		{
			get
			{
				return prfAlgorithm;
			}
		}

		public virtual int VerifyDataLength
		{
			get
			{
				return verifyDataLength;
			}
		}

		public virtual byte[] MasterSecret
		{
			get
			{
				return masterSecret;
			}
		}

		public virtual byte[] ClientRandom
		{
			get
			{
				return clientRandom;
			}
		}

		public virtual byte[] ServerRandom
		{
			get
			{
				return serverRandom;
			}
		}

		public virtual byte[] SessionHash
		{
			get
			{
				return sessionHash;
			}
		}

		public virtual byte[] PskIdentity
		{
			get
			{
				return pskIdentity;
			}
		}

		public virtual byte[] SrpIdentity
		{
			get
			{
				return srpIdentity;
			}
		}

		internal virtual void Clear()
		{
			if (masterSecret != null)
			{
				Arrays.Fill(masterSecret, 0);
				masterSecret = null;
			}
		}
	}
}
