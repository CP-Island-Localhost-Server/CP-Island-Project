using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class CertificatePolicies : Asn1Encodable
	{
		private readonly PolicyInformation[] policyInformation;

		public static CertificatePolicies GetInstance(object obj)
		{
			if (obj == null || obj is CertificatePolicies)
			{
				return (CertificatePolicies)obj;
			}
			return new CertificatePolicies(Asn1Sequence.GetInstance(obj));
		}

		public static CertificatePolicies GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public CertificatePolicies(PolicyInformation name)
		{
			policyInformation = new PolicyInformation[1] { name };
		}

		public CertificatePolicies(PolicyInformation[] policyInformation)
		{
			this.policyInformation = policyInformation;
		}

		private CertificatePolicies(Asn1Sequence seq)
		{
			policyInformation = new PolicyInformation[seq.Count];
			for (int i = 0; i < seq.Count; i++)
			{
				policyInformation[i] = PolicyInformation.GetInstance(seq[i]);
			}
		}

		public virtual PolicyInformation[] GetPolicyInformation()
		{
			return (PolicyInformation[])policyInformation.Clone();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(policyInformation);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("CertificatePolicies:");
			if (policyInformation != null && policyInformation.Length > 0)
			{
				stringBuilder.Append(' ');
				stringBuilder.Append(policyInformation[0]);
				for (int i = 1; i < policyInformation.Length; i++)
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(policyInformation[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
