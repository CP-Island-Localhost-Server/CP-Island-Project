using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DefaultTlsCipherFactory : AbstractTlsCipherFactory
	{
		public override TlsCipher CreateCipher(TlsContext context, int encryptionAlgorithm, int macAlgorithm)
		{
			switch (encryptionAlgorithm)
			{
			case 7:
				return CreateDesEdeCipher(context, macAlgorithm);
			case 102:
				return CreateChaCha20Poly1305(context);
			case 8:
				return CreateAESCipher(context, 16, macAlgorithm);
			case 15:
				return CreateCipher_Aes_Ccm(context, 16, 16);
			case 16:
				return CreateCipher_Aes_Ccm(context, 16, 8);
			case 17:
				return CreateCipher_Aes_Ccm(context, 32, 16);
			case 18:
				return CreateCipher_Aes_Ccm(context, 32, 8);
			case 10:
				return CreateCipher_Aes_Gcm(context, 16, 16);
			case 9:
				return CreateAESCipher(context, 32, macAlgorithm);
			case 11:
				return CreateCipher_Aes_Gcm(context, 32, 16);
			case 12:
				return CreateCamelliaCipher(context, 16, macAlgorithm);
			case 19:
				return CreateCipher_Camellia_Gcm(context, 16, 16);
			case 13:
				return CreateCamelliaCipher(context, 32, macAlgorithm);
			case 20:
				return CreateCipher_Camellia_Gcm(context, 32, 16);
			case 100:
				return CreateSalsa20Cipher(context, 12, 32, macAlgorithm);
			case 0:
				return CreateNullCipher(context, macAlgorithm);
			case 2:
				return CreateRC4Cipher(context, 16, macAlgorithm);
			case 101:
				return CreateSalsa20Cipher(context, 20, 32, macAlgorithm);
			case 14:
				return CreateSeedCipher(context, macAlgorithm);
			default:
				throw new TlsFatalAlert(80);
			}
		}

		protected virtual TlsBlockCipher CreateAESCipher(TlsContext context, int cipherKeySize, int macAlgorithm)
		{
			return new TlsBlockCipher(context, CreateAesBlockCipher(), CreateAesBlockCipher(), CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), cipherKeySize);
		}

		protected virtual TlsBlockCipher CreateCamelliaCipher(TlsContext context, int cipherKeySize, int macAlgorithm)
		{
			return new TlsBlockCipher(context, CreateCamelliaBlockCipher(), CreateCamelliaBlockCipher(), CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), cipherKeySize);
		}

		protected virtual TlsCipher CreateChaCha20Poly1305(TlsContext context)
		{
			return new Chacha20Poly1305(context);
		}

		protected virtual TlsAeadCipher CreateCipher_Aes_Ccm(TlsContext context, int cipherKeySize, int macSize)
		{
			return new TlsAeadCipher(context, CreateAeadBlockCipher_Aes_Ccm(), CreateAeadBlockCipher_Aes_Ccm(), cipherKeySize, macSize);
		}

		protected virtual TlsAeadCipher CreateCipher_Aes_Gcm(TlsContext context, int cipherKeySize, int macSize)
		{
			return new TlsAeadCipher(context, CreateAeadBlockCipher_Aes_Gcm(), CreateAeadBlockCipher_Aes_Gcm(), cipherKeySize, macSize);
		}

		protected virtual TlsAeadCipher CreateCipher_Camellia_Gcm(TlsContext context, int cipherKeySize, int macSize)
		{
			return new TlsAeadCipher(context, CreateAeadBlockCipher_Camellia_Gcm(), CreateAeadBlockCipher_Camellia_Gcm(), cipherKeySize, macSize);
		}

		protected virtual TlsBlockCipher CreateDesEdeCipher(TlsContext context, int macAlgorithm)
		{
			return new TlsBlockCipher(context, CreateDesEdeBlockCipher(), CreateDesEdeBlockCipher(), CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), 24);
		}

		protected virtual TlsNullCipher CreateNullCipher(TlsContext context, int macAlgorithm)
		{
			return new TlsNullCipher(context, CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm));
		}

		protected virtual TlsStreamCipher CreateRC4Cipher(TlsContext context, int cipherKeySize, int macAlgorithm)
		{
			return new TlsStreamCipher(context, CreateRC4StreamCipher(), CreateRC4StreamCipher(), CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), cipherKeySize, false);
		}

		protected virtual TlsStreamCipher CreateSalsa20Cipher(TlsContext context, int rounds, int cipherKeySize, int macAlgorithm)
		{
			return new TlsStreamCipher(context, CreateSalsa20StreamCipher(rounds), CreateSalsa20StreamCipher(rounds), CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), cipherKeySize, true);
		}

		protected virtual TlsBlockCipher CreateSeedCipher(TlsContext context, int macAlgorithm)
		{
			return new TlsBlockCipher(context, CreateSeedBlockCipher(), CreateSeedBlockCipher(), CreateHMacDigest(macAlgorithm), CreateHMacDigest(macAlgorithm), 16);
		}

		protected virtual IBlockCipher CreateAesEngine()
		{
			return new AesEngine();
		}

		protected virtual IBlockCipher CreateCamelliaEngine()
		{
			return new CamelliaEngine();
		}

		protected virtual IBlockCipher CreateAesBlockCipher()
		{
			return new CbcBlockCipher(CreateAesEngine());
		}

		protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aes_Ccm()
		{
			return new CcmBlockCipher(CreateAesEngine());
		}

		protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aes_Gcm()
		{
			return new GcmBlockCipher(CreateAesEngine());
		}

		protected virtual IAeadBlockCipher CreateAeadBlockCipher_Camellia_Gcm()
		{
			return new GcmBlockCipher(CreateCamelliaEngine());
		}

		protected virtual IBlockCipher CreateCamelliaBlockCipher()
		{
			return new CbcBlockCipher(CreateCamelliaEngine());
		}

		protected virtual IBlockCipher CreateDesEdeBlockCipher()
		{
			return new CbcBlockCipher(new DesEdeEngine());
		}

		protected virtual IStreamCipher CreateRC4StreamCipher()
		{
			return new RC4Engine();
		}

		protected virtual IStreamCipher CreateSalsa20StreamCipher(int rounds)
		{
			return new Salsa20Engine(rounds);
		}

		protected virtual IBlockCipher CreateSeedBlockCipher()
		{
			return new CbcBlockCipher(new SeedEngine());
		}

		protected virtual IDigest CreateHMacDigest(int macAlgorithm)
		{
			switch (macAlgorithm)
			{
			case 0:
				return null;
			case 1:
				return TlsUtilities.CreateHash(1);
			case 2:
				return TlsUtilities.CreateHash(2);
			case 3:
				return TlsUtilities.CreateHash(4);
			case 4:
				return TlsUtilities.CreateHash(5);
			case 5:
				return TlsUtilities.CreateHash(6);
			default:
				throw new TlsFatalAlert(80);
			}
		}
	}
}
