namespace Org.BouncyCastle.Crypto.Tls
{
	public class PskTlsClient : AbstractTlsClient
	{
		protected TlsPskIdentity mPskIdentity;

		public PskTlsClient(TlsPskIdentity pskIdentity)
			: this(new DefaultTlsCipherFactory(), pskIdentity)
		{
		}

		public PskTlsClient(TlsCipherFactory cipherFactory, TlsPskIdentity pskIdentity)
			: base(cipherFactory)
		{
			mPskIdentity = pskIdentity;
		}

		public override int[] GetCipherSuites()
		{
			return new int[4] { 49207, 49205, 178, 144 };
		}

		public override TlsKeyExchange GetKeyExchange()
		{
			int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(mSelectedCipherSuite);
			switch (keyExchangeAlgorithm)
			{
			case 13:
			case 14:
			case 15:
			case 24:
				return CreatePskKeyExchange(keyExchangeAlgorithm);
			default:
				throw new TlsFatalAlert(80);
			}
		}

		public override TlsAuthentication GetAuthentication()
		{
			throw new TlsFatalAlert(80);
		}

		protected virtual TlsKeyExchange CreatePskKeyExchange(int keyExchange)
		{
			return new TlsPskKeyExchange(keyExchange, mSupportedSignatureAlgorithms, mPskIdentity, null, null, mNamedCurves, mClientECPointFormats, mServerECPointFormats);
		}
	}
}
