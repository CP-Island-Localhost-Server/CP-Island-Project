using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Cms
{
	public class OriginatorID : X509CertStoreSelector
	{
		public override int GetHashCode()
		{
			int num = Arrays.GetHashCode(base.SubjectKeyIdentifier);
			BigInteger bigInteger = base.SerialNumber;
			if (bigInteger != null)
			{
				num ^= bigInteger.GetHashCode();
			}
			X509Name x509Name = base.Issuer;
			if (x509Name != null)
			{
				num ^= x509Name.GetHashCode();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return false;
			}
			OriginatorID originatorID = obj as OriginatorID;
			if (originatorID == null)
			{
				return false;
			}
			if (Arrays.AreEqual(base.SubjectKeyIdentifier, originatorID.SubjectKeyIdentifier) && object.Equals(base.SerialNumber, originatorID.SerialNumber))
			{
				return X509CertStoreSelector.IssuersMatch(base.Issuer, originatorID.Issuer);
			}
			return false;
		}
	}
}
