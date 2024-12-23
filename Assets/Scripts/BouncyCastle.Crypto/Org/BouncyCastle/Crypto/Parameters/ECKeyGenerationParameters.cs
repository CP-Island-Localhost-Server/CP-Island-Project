using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ECKeyGenerationParameters : KeyGenerationParameters
	{
		private readonly ECDomainParameters domainParams;

		private readonly DerObjectIdentifier publicKeyParamSet;

		public ECDomainParameters DomainParameters
		{
			get
			{
				return domainParams;
			}
		}

		public DerObjectIdentifier PublicKeyParamSet
		{
			get
			{
				return publicKeyParamSet;
			}
		}

		public ECKeyGenerationParameters(ECDomainParameters domainParameters, SecureRandom random)
			: base(random, domainParameters.N.BitLength)
		{
			domainParams = domainParameters;
		}

		public ECKeyGenerationParameters(DerObjectIdentifier publicKeyParamSet, SecureRandom random)
			: this(ECKeyParameters.LookupParameters(publicKeyParamSet), random)
		{
			this.publicKeyParamSet = publicKeyParamSet;
		}
	}
}
