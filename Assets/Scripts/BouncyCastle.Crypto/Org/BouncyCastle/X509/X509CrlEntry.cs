using System;
using System.Collections;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Extension;

namespace Org.BouncyCastle.X509
{
	public class X509CrlEntry : X509ExtensionBase
	{
		private CrlEntry c;

		private bool isIndirect;

		private X509Name previousCertificateIssuer;

		private X509Name certificateIssuer;

		public BigInteger SerialNumber
		{
			get
			{
				return c.UserCertificate.Value;
			}
		}

		public DateTime RevocationDate
		{
			get
			{
				return c.RevocationDate.ToDateTime();
			}
		}

		public bool HasExtensions
		{
			get
			{
				return c.Extensions != null;
			}
		}

		public X509CrlEntry(CrlEntry c)
		{
			this.c = c;
			certificateIssuer = loadCertificateIssuer();
		}

		public X509CrlEntry(CrlEntry c, bool isIndirect, X509Name previousCertificateIssuer)
		{
			this.c = c;
			this.isIndirect = isIndirect;
			this.previousCertificateIssuer = previousCertificateIssuer;
			certificateIssuer = loadCertificateIssuer();
		}

		private X509Name loadCertificateIssuer()
		{
			if (!isIndirect)
			{
				return null;
			}
			Asn1OctetString extensionValue = GetExtensionValue(X509Extensions.CertificateIssuer);
			if (extensionValue == null)
			{
				return previousCertificateIssuer;
			}
			try
			{
				GeneralName[] names = GeneralNames.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue)).GetNames();
				for (int i = 0; i < names.Length; i++)
				{
					if (names[i].TagNo == 4)
					{
						return X509Name.GetInstance(names[i].Name);
					}
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		public X509Name GetCertificateIssuer()
		{
			return certificateIssuer;
		}

		protected override X509Extensions GetX509Extensions()
		{
			return c.Extensions;
		}

		public byte[] GetEncoded()
		{
			try
			{
				return c.GetDerEncoded();
			}
			catch (Exception ex)
			{
				throw new CrlException(ex.ToString());
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string newLine = Platform.NewLine;
			stringBuilder.Append("        userCertificate: ").Append(SerialNumber).Append(newLine);
			stringBuilder.Append("         revocationDate: ").Append(RevocationDate).Append(newLine);
			stringBuilder.Append("      certificateIssuer: ").Append(GetCertificateIssuer()).Append(newLine);
			X509Extensions extensions = c.Extensions;
			if (extensions != null)
			{
				IEnumerator enumerator = extensions.ExtensionOids.GetEnumerator();
				if (enumerator.MoveNext())
				{
					stringBuilder.Append("   crlEntryExtensions:").Append(newLine);
					do
					{
						DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)enumerator.Current;
						X509Extension extension = extensions.GetExtension(derObjectIdentifier);
						if (extension.Value != null)
						{
							Asn1Object asn1Object = Asn1Object.FromByteArray(extension.Value.GetOctets());
							stringBuilder.Append("                       critical(").Append(extension.IsCritical).Append(") ");
							try
							{
								if (derObjectIdentifier.Equals(X509Extensions.ReasonCode))
								{
									stringBuilder.Append(new CrlReason(DerEnumerated.GetInstance(asn1Object)));
								}
								else if (derObjectIdentifier.Equals(X509Extensions.CertificateIssuer))
								{
									stringBuilder.Append("Certificate issuer: ").Append(GeneralNames.GetInstance((Asn1Sequence)asn1Object));
								}
								else
								{
									stringBuilder.Append(derObjectIdentifier.Id);
									stringBuilder.Append(" value = ").Append(Asn1Dump.DumpAsString(asn1Object));
								}
								stringBuilder.Append(newLine);
							}
							catch (Exception)
							{
								stringBuilder.Append(derObjectIdentifier.Id);
								stringBuilder.Append(" value = ").Append("*****").Append(newLine);
							}
						}
						else
						{
							stringBuilder.Append(newLine);
						}
					}
					while (enumerator.MoveNext());
				}
			}
			return stringBuilder.ToString();
		}
	}
}
