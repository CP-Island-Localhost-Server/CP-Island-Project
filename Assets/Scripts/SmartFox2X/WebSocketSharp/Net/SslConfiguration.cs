using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace WebSocketSharp.Net
{
	public abstract class SslConfiguration
	{
		private LocalCertificateSelectionCallback _certSelectionCallback;

		private RemoteCertificateValidationCallback _certValidationCallback;

		private bool _checkCertRevocation;

		private SslProtocols _enabledProtocols;

		protected LocalCertificateSelectionCallback CertificateSelectionCallback
		{
			get
			{
				return (object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => (X509Certificate)null;
			}
			set
			{
				_certSelectionCallback = value;
			}
		}

		protected RemoteCertificateValidationCallback CertificateValidationCallback
		{
			get
			{
				return (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
			}
			set
			{
				_certValidationCallback = value;
			}
		}

		public bool CheckCertificateRevocation
		{
			get
			{
				return _checkCertRevocation;
			}
			set
			{
				_checkCertRevocation = value;
			}
		}

		public SslProtocols EnabledSslProtocols
		{
			get
			{
				return _enabledProtocols;
			}
			set
			{
				_enabledProtocols = value;
			}
		}

		protected SslConfiguration(SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			_enabledProtocols = enabledSslProtocols;
			_checkCertRevocation = checkCertificateRevocation;
		}
	}
}
