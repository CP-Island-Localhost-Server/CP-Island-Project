using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class PbeS2Parameters : Asn1Encodable
	{
		private readonly KeyDerivationFunc func;

		private readonly EncryptionScheme scheme;

		public KeyDerivationFunc KeyDerivationFunc
		{
			get
			{
				return func;
			}
		}

		public EncryptionScheme EncryptionScheme
		{
			get
			{
				return scheme;
			}
		}

		public static PbeS2Parameters GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			PbeS2Parameters pbeS2Parameters = obj as PbeS2Parameters;
			if (pbeS2Parameters != null)
			{
				return pbeS2Parameters;
			}
			return new PbeS2Parameters(Asn1Sequence.GetInstance(obj));
		}

		public PbeS2Parameters(KeyDerivationFunc keyDevFunc, EncryptionScheme encScheme)
		{
			func = keyDevFunc;
			scheme = encScheme;
		}

		[Obsolete("Use GetInstance() instead")]
		public PbeS2Parameters(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			Asn1Sequence asn1Sequence = (Asn1Sequence)seq[0].ToAsn1Object();
			if (asn1Sequence[0].Equals(PkcsObjectIdentifiers.IdPbkdf2))
			{
				func = new KeyDerivationFunc(PkcsObjectIdentifiers.IdPbkdf2, Pbkdf2Params.GetInstance(asn1Sequence[1]));
			}
			else
			{
				func = new KeyDerivationFunc(asn1Sequence);
			}
			scheme = EncryptionScheme.GetInstance(seq[1].ToAsn1Object());
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(func, scheme);
		}
	}
}
