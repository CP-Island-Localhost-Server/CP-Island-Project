using System.Collections;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Operators
{
	public class Asn1SignatureFactory : ISignatureFactory
	{
		private readonly AlgorithmIdentifier algID;

		private readonly string algorithm;

		private readonly AsymmetricKeyParameter privateKey;

		private readonly SecureRandom random;

		public object AlgorithmDetails
		{
			get
			{
				return algID;
			}
		}

		public static IEnumerable SignatureAlgNames
		{
			get
			{
				return X509Utilities.GetAlgNames();
			}
		}

		public Asn1SignatureFactory(string algorithm, AsymmetricKeyParameter privateKey)
			: this(algorithm, privateKey, null)
		{
		}

		public Asn1SignatureFactory(string algorithm, AsymmetricKeyParameter privateKey, SecureRandom random)
		{
			DerObjectIdentifier algorithmOid = X509Utilities.GetAlgorithmOid(algorithm);
			this.algorithm = algorithm;
			this.privateKey = privateKey;
			this.random = random;
			algID = X509Utilities.GetSigAlgID(algorithmOid, algorithm);
		}

		public IStreamCalculator CreateCalculator()
		{
			ISigner signer = SignerUtilities.GetSigner(algorithm);
			if (random != null)
			{
				signer.Init(true, new ParametersWithRandom(privateKey, random));
			}
			else
			{
				signer.Init(true, privateKey);
			}
			return new SigCalculator(signer);
		}
	}
}
