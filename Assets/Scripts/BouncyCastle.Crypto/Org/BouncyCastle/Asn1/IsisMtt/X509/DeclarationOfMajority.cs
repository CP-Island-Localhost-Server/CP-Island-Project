using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.IsisMtt.X509
{
	public class DeclarationOfMajority : Asn1Encodable, IAsn1Choice
	{
		public enum Choice
		{
			NotYoungerThan = 0,
			FullAgeAtCountry = 1,
			DateOfBirth = 2
		}

		private readonly Asn1TaggedObject declaration;

		public Choice Type
		{
			get
			{
				return (Choice)declaration.TagNo;
			}
		}

		public virtual int NotYoungerThan
		{
			get
			{
				if (declaration.TagNo == 0)
				{
					return DerInteger.GetInstance(declaration, false).Value.IntValue;
				}
				return -1;
			}
		}

		public virtual Asn1Sequence FullAgeAtCountry
		{
			get
			{
				Choice tagNo = (Choice)declaration.TagNo;
				if (tagNo == Choice.FullAgeAtCountry)
				{
					return Asn1Sequence.GetInstance(declaration, false);
				}
				return null;
			}
		}

		public virtual DerGeneralizedTime DateOfBirth
		{
			get
			{
				Choice tagNo = (Choice)declaration.TagNo;
				if (tagNo == Choice.DateOfBirth)
				{
					return DerGeneralizedTime.GetInstance(declaration, false);
				}
				return null;
			}
		}

		public DeclarationOfMajority(int notYoungerThan)
		{
			declaration = new DerTaggedObject(false, 0, new DerInteger(notYoungerThan));
		}

		public DeclarationOfMajority(bool fullAge, string country)
		{
			if (country.Length > 2)
			{
				throw new ArgumentException("country can only be 2 characters");
			}
			DerPrintableString derPrintableString = new DerPrintableString(country, true);
			declaration = new DerTaggedObject(false, 1, (!fullAge) ? new DerSequence(DerBoolean.False, derPrintableString) : new DerSequence(derPrintableString));
		}

		public DeclarationOfMajority(DerGeneralizedTime dateOfBirth)
		{
			declaration = new DerTaggedObject(false, 2, dateOfBirth);
		}

		public static DeclarationOfMajority GetInstance(object obj)
		{
			if (obj == null || obj is DeclarationOfMajority)
			{
				return (DeclarationOfMajority)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return new DeclarationOfMajority((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		private DeclarationOfMajority(Asn1TaggedObject o)
		{
			if (o.TagNo > 2)
			{
				throw new ArgumentException("Bad tag number: " + o.TagNo);
			}
			declaration = o;
		}

		public override Asn1Object ToAsn1Object()
		{
			return declaration;
		}
	}
}
