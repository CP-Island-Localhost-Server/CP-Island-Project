using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Math.Field
{
	internal class GenericPolynomialExtensionField : IPolynomialExtensionField, IExtensionField, IFiniteField
	{
		protected readonly IFiniteField subfield;

		protected readonly IPolynomial minimalPolynomial;

		public virtual BigInteger Characteristic
		{
			get
			{
				return subfield.Characteristic;
			}
		}

		public virtual int Dimension
		{
			get
			{
				return subfield.Dimension * minimalPolynomial.Degree;
			}
		}

		public virtual IFiniteField Subfield
		{
			get
			{
				return subfield;
			}
		}

		public virtual int Degree
		{
			get
			{
				return minimalPolynomial.Degree;
			}
		}

		public virtual IPolynomial MinimalPolynomial
		{
			get
			{
				return minimalPolynomial;
			}
		}

		internal GenericPolynomialExtensionField(IFiniteField subfield, IPolynomial polynomial)
		{
			this.subfield = subfield;
			minimalPolynomial = polynomial;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			GenericPolynomialExtensionField genericPolynomialExtensionField = obj as GenericPolynomialExtensionField;
			if (genericPolynomialExtensionField == null)
			{
				return false;
			}
			if (subfield.Equals(genericPolynomialExtensionField.subfield))
			{
				return minimalPolynomial.Equals(genericPolynomialExtensionField.minimalPolynomial);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return subfield.GetHashCode() ^ Integers.RotateLeft(minimalPolynomial.GetHashCode(), 16);
		}
	}
}
