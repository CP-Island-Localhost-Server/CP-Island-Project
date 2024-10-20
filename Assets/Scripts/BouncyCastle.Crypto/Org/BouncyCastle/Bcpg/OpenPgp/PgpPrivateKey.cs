using System;
using Org.BouncyCastle.Crypto;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	public class PgpPrivateKey
	{
		private readonly long keyID;

		private readonly PublicKeyPacket publicKeyPacket;

		private readonly AsymmetricKeyParameter privateKey;

		public long KeyId
		{
			get
			{
				return keyID;
			}
		}

		public PublicKeyPacket PublicKeyPacket
		{
			get
			{
				return publicKeyPacket;
			}
		}

		public AsymmetricKeyParameter Key
		{
			get
			{
				return privateKey;
			}
		}

		public PgpPrivateKey(long keyID, PublicKeyPacket publicKeyPacket, AsymmetricKeyParameter privateKey)
		{
			if (!privateKey.IsPrivate)
			{
				throw new ArgumentException("Expected a private key", "privateKey");
			}
			this.keyID = keyID;
			this.publicKeyPacket = publicKeyPacket;
			this.privateKey = privateKey;
		}
	}
}
