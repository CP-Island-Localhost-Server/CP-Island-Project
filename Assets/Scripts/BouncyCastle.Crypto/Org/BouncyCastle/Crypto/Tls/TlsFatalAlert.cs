using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsFatalAlert : IOException
	{
		private readonly byte alertDescription;

		public virtual byte AlertDescription
		{
			get
			{
				return alertDescription;
			}
		}

		public TlsFatalAlert(byte alertDescription)
			: this(alertDescription, null)
		{
		}

		public TlsFatalAlert(byte alertDescription, Exception alertCause)
			: base(Org.BouncyCastle.Crypto.Tls.AlertDescription.GetText(alertDescription), alertCause)
		{
			this.alertDescription = alertDescription;
		}
	}
}
