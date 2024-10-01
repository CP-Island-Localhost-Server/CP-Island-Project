using System;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Ocsp
{
	public class RevokedStatus : CertificateStatus
	{
		internal readonly RevokedInfo info;

		public DateTime RevocationTime
		{
			get
			{
				return info.RevocationTime.ToDateTime();
			}
		}

		public bool HasRevocationReason
		{
			get
			{
				return info.RevocationReason != null;
			}
		}

		public int RevocationReason
		{
			get
			{
				if (info.RevocationReason == null)
				{
					throw new InvalidOperationException("attempt to get a reason where none is available");
				}
				return info.RevocationReason.Value.IntValue;
			}
		}

		public RevokedStatus(RevokedInfo info)
		{
			this.info = info;
		}

		public RevokedStatus(DateTime revocationDate, int reason)
		{
			info = new RevokedInfo(new DerGeneralizedTime(revocationDate), new CrlReason(reason));
		}
	}
}
