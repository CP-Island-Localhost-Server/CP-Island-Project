using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Operators
{
	public class Asn1VerifierFactory : IVerifierFactory
	{
		private readonly AlgorithmIdentifier algID;

		private readonly AsymmetricKeyParameter publicKey;

		public object AlgorithmDetails
		{
			get
			{
				return algID;
			}
		}

		public Asn1VerifierFactory(string algorithm, AsymmetricKeyParameter publicKey)
		{
			DerObjectIdentifier algorithmOid = X509Utilities.GetAlgorithmOid(algorithm);
			this.publicKey = publicKey;
			algID = X509Utilities.GetSigAlgID(algorithmOid, algorithm);
		}

		public Asn1VerifierFactory(AlgorithmIdentifier algorithm, AsymmetricKeyParameter publicKey)
		{
			this.publicKey = publicKey;
			algID = algorithm;
		}

		public IStreamCalculator CreateCalculator()
		{
			ISigner signer = SignerUtilities.GetSigner(X509Utilities.GetSignatureName(algID));
			signer.Init(false, publicKey);
			return new VerifierCalculator(signer);
		}
	}
}
