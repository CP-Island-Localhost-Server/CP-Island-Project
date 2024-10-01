using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace WebSocketSharp.Net
{
	public class ServerSslConfiguration : SslConfiguration
	{
		private X509Certificate2 _cert;

		private bool _clientCertRequired;

		public bool ClientCertificateRequired
		{
			get
			{
				return _clientCertRequired;
			}
			set
			{
				_clientCertRequired = value;
			}
		}

		public RemoteCertificateValidationCallback ClientCertificateValidationCallback
		{
			get
			{
				return base.CertificateValidationCallback;
			}
			set
			{
				base.CertificateValidationCallback = value;
			}
		}

		public X509Certificate2 ServerCertificate
		{
			get
			{
				return _cert;
			}
			set
			{
				_cert = value;
			}
		}

		public ServerSslConfiguration(X509Certificate2 serverCertificate)
			: this(serverCertificate, false, SslProtocols.Default, false)
		{
		}

		public ServerSslConfiguration(X509Certificate2 serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
			: base(enabledSslProtocols, checkCertificateRevocation)
		{
			_cert = serverCertificate;
			_clientCertRequired = clientCertificateRequired;
		}
	}
}
