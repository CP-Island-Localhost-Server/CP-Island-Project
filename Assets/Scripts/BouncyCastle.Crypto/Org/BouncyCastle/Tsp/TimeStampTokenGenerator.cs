using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Tsp
{
	public class TimeStampTokenGenerator
	{
		private int accuracySeconds = -1;

		private int accuracyMillis = -1;

		private int accuracyMicros = -1;

		private bool ordering = false;

		private GeneralName tsa = null;

		private string tsaPolicyOID;

		private AsymmetricKeyParameter key;

		private X509Certificate cert;

		private string digestOID;

		private Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr;

		private Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr;

		private IX509Store x509Certs;

		private IX509Store x509Crls;

		public TimeStampTokenGenerator(AsymmetricKeyParameter key, X509Certificate cert, string digestOID, string tsaPolicyOID)
			: this(key, cert, digestOID, tsaPolicyOID, null, null)
		{
		}

		public TimeStampTokenGenerator(AsymmetricKeyParameter key, X509Certificate cert, string digestOID, string tsaPolicyOID, Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		{
			this.key = key;
			this.cert = cert;
			this.digestOID = digestOID;
			this.tsaPolicyOID = tsaPolicyOID;
			this.unsignedAttr = unsignedAttr;
			TspUtil.ValidateCertificate(cert);
			IDictionary dictionary = ((signedAttr == null) ? Platform.CreateHashtable() : signedAttr.ToDictionary());
			try
			{
				byte[] hash = DigestUtilities.CalculateDigest("SHA-1", cert.GetEncoded());
				EssCertID essCertID = new EssCertID(hash);
				Org.BouncyCastle.Asn1.Cms.Attribute attribute = new Org.BouncyCastle.Asn1.Cms.Attribute(PkcsObjectIdentifiers.IdAASigningCertificate, new DerSet(new SigningCertificate(essCertID)));
				dictionary[attribute.AttrType] = attribute;
			}
			catch (CertificateEncodingException e)
			{
				throw new TspException("Exception processing certificate.", e);
			}
			catch (SecurityUtilityException e2)
			{
				throw new TspException("Can't find a SHA-1 implementation.", e2);
			}
			this.signedAttr = new Org.BouncyCastle.Asn1.Cms.AttributeTable(dictionary);
		}

		public void SetCertificates(IX509Store certificates)
		{
			x509Certs = certificates;
		}

		public void SetCrls(IX509Store crls)
		{
			x509Crls = crls;
		}

		public void SetAccuracySeconds(int accuracySeconds)
		{
			this.accuracySeconds = accuracySeconds;
		}

		public void SetAccuracyMillis(int accuracyMillis)
		{
			this.accuracyMillis = accuracyMillis;
		}

		public void SetAccuracyMicros(int accuracyMicros)
		{
			this.accuracyMicros = accuracyMicros;
		}

		public void SetOrdering(bool ordering)
		{
			this.ordering = ordering;
		}

		public void SetTsa(GeneralName tsa)
		{
			this.tsa = tsa;
		}

		public TimeStampToken Generate(TimeStampRequest request, BigInteger serialNumber, DateTime genTime)
		{
			DerObjectIdentifier algorithm = new DerObjectIdentifier(request.MessageImprintAlgOid);
			AlgorithmIdentifier hashAlgorithm = new AlgorithmIdentifier(algorithm, DerNull.Instance);
			MessageImprint messageImprint = new MessageImprint(hashAlgorithm, request.GetMessageImprintDigest());
			Accuracy accuracy = null;
			if (accuracySeconds > 0 || accuracyMillis > 0 || accuracyMicros > 0)
			{
				DerInteger seconds = null;
				if (accuracySeconds > 0)
				{
					seconds = new DerInteger(accuracySeconds);
				}
				DerInteger millis = null;
				if (accuracyMillis > 0)
				{
					millis = new DerInteger(accuracyMillis);
				}
				DerInteger micros = null;
				if (accuracyMicros > 0)
				{
					micros = new DerInteger(accuracyMicros);
				}
				accuracy = new Accuracy(seconds, millis, micros);
			}
			DerBoolean derBoolean = null;
			if (ordering)
			{
				derBoolean = DerBoolean.GetInstance(ordering);
			}
			DerInteger nonce = null;
			if (request.Nonce != null)
			{
				nonce = new DerInteger(request.Nonce);
			}
			DerObjectIdentifier tsaPolicyId = new DerObjectIdentifier(tsaPolicyOID);
			if (request.ReqPolicy != null)
			{
				tsaPolicyId = new DerObjectIdentifier(request.ReqPolicy);
			}
			TstInfo tstInfo = new TstInfo(tsaPolicyId, messageImprint, new DerInteger(serialNumber), new DerGeneralizedTime(genTime), accuracy, derBoolean, nonce, tsa, request.Extensions);
			try
			{
				CmsSignedDataGenerator cmsSignedDataGenerator = new CmsSignedDataGenerator();
				byte[] derEncoded = tstInfo.GetDerEncoded();
				if (request.CertReq)
				{
					cmsSignedDataGenerator.AddCertificates(x509Certs);
				}
				cmsSignedDataGenerator.AddCrls(x509Crls);
				cmsSignedDataGenerator.AddSigner(key, cert, digestOID, signedAttr, unsignedAttr);
				CmsSignedData signedData = cmsSignedDataGenerator.Generate(PkcsObjectIdentifiers.IdCTTstInfo.Id, new CmsProcessableByteArray(derEncoded), true);
				return new TimeStampToken(signedData);
			}
			catch (CmsException e)
			{
				throw new TspException("Error generating time-stamp token", e);
			}
			catch (IOException e2)
			{
				throw new TspException("Exception encoding info", e2);
			}
			catch (X509StoreException e3)
			{
				throw new TspException("Exception handling CertStore", e3);
			}
		}
	}
}
