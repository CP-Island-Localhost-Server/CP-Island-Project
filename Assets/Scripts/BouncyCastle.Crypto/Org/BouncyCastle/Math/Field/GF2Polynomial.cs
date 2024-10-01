using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Math.Field
{
	internal class GF2Polynomial : IPolynomial
	{
		protected readonly int[] exponents;

		public virtual int Degree
		{
			get
			{
				return exponents[exponents.Length - 1];
			}
		}

		internal GF2Polynomial(int[] exponents)
		{
			this.exponents = Arrays.Clone(exponents);
		}

		public virtual int[] GetExponentsPresent()
		{
			return Arrays.Clone(exponents);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			GF2Polynomial gF2Polynomial = obj as GF2Polynomial;
			if (gF2Polynomial == null)
			{
				return false;
			}
			return Arrays.AreEqual(exponents, gF2Polynomial.exponents);
		}

		public override int GetHashCode()
		{
			return Arrays.GetHashCode(exponents);
		}
	}
}
