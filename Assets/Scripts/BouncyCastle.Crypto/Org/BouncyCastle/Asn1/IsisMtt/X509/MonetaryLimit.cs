using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.IsisMtt.X509
{
	public class MonetaryLimit : Asn1Encodable
	{
		private readonly DerPrintableString currency;

		private readonly DerInteger amount;

		private readonly DerInteger exponent;

		public virtual string Currency
		{
			get
			{
				return currency.GetString();
			}
		}

		public virtual BigInteger Amount
		{
			get
			{
				return amount.Value;
			}
		}

		public virtual BigInteger Exponent
		{
			get
			{
				return exponent.Value;
			}
		}

		public static MonetaryLimit GetInstance(object obj)
		{
			if (obj == null || obj is MonetaryLimit)
			{
				return (MonetaryLimit)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new MonetaryLimit(Asn1Sequence.GetInstance(obj));
			}
			throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		private MonetaryLimit(Asn1Sequence seq)
		{
			if (seq.Count != 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			currency = DerPrintableString.GetInstance(seq[0]);
			amount = DerInteger.GetInstance(seq[1]);
			exponent = DerInteger.GetInstance(seq[2]);
		}

		public MonetaryLimit(string currency, int amount, int exponent)
		{
			this.currency = new DerPrintableString(currency, true);
			this.amount = new DerInteger(amount);
			this.exponent = new DerInteger(exponent);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(currency, amount, exponent);
		}
	}
}
