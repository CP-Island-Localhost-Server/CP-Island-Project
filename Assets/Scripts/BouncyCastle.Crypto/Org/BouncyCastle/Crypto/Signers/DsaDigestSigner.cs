using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class DsaDigestSigner : ISigner
	{
		private readonly IDigest digest;

		private readonly IDsa dsaSigner;

		private bool forSigning;

		public virtual string AlgorithmName
		{
			get
			{
				return digest.AlgorithmName + "with" + dsaSigner.AlgorithmName;
			}
		}

		public DsaDigestSigner(IDsa signer, IDigest digest)
		{
			this.digest = digest;
			dsaSigner = signer;
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			this.forSigning = forSigning;
			AsymmetricKeyParameter asymmetricKeyParameter = ((!(parameters is ParametersWithRandom)) ? ((AsymmetricKeyParameter)parameters) : ((AsymmetricKeyParameter)((ParametersWithRandom)parameters).Parameters));
			if (forSigning && !asymmetricKeyParameter.IsPrivate)
			{
				throw new InvalidKeyException("Signing Requires Private Key.");
			}
			if (!forSigning && asymmetricKeyParameter.IsPrivate)
			{
				throw new InvalidKeyException("Verification Requires Public Key.");
			}
			Reset();
			dsaSigner.Init(forSigning, parameters);
		}

		public virtual void Update(byte input)
		{
			digest.Update(input);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int length)
		{
			digest.BlockUpdate(input, inOff, length);
		}

		public virtual byte[] GenerateSignature()
		{
			if (!forSigning)
			{
				throw new InvalidOperationException("DSADigestSigner not initialised for signature generation.");
			}
			byte[] array = new byte[digest.GetDigestSize()];
			digest.DoFinal(array, 0);
			BigInteger[] array2 = dsaSigner.GenerateSignature(array);
			return DerEncode(array2[0], array2[1]);
		}

		public virtual bool VerifySignature(byte[] signature)
		{
			if (forSigning)
			{
				throw new InvalidOperationException("DSADigestSigner not initialised for verification");
			}
			byte[] array = new byte[digest.GetDigestSize()];
			digest.DoFinal(array, 0);
			try
			{
				BigInteger[] array2 = DerDecode(signature);
				return dsaSigner.VerifySignature(array, array2[0], array2[1]);
			}
			catch (IOException)
			{
				return false;
			}
		}

		public virtual void Reset()
		{
			digest.Reset();
		}

		private byte[] DerEncode(BigInteger r, BigInteger s)
		{
			return new DerSequence(new DerInteger(r), new DerInteger(s)).GetDerEncoded();
		}

		private BigInteger[] DerDecode(byte[] encoding)
		{
			Asn1Sequence asn1Sequence = (Asn1Sequence)Asn1Object.FromByteArray(encoding);
			return new BigInteger[2]
			{
				((DerInteger)asn1Sequence[0]).Value,
				((DerInteger)asn1Sequence[1]).Value
			};
		}
	}
}
