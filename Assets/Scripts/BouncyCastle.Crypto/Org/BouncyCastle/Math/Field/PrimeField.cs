namespace Org.BouncyCastle.Math.Field
{
	internal class PrimeField : IFiniteField
	{
		protected readonly BigInteger characteristic;

		public virtual BigInteger Characteristic
		{
			get
			{
				return characteristic;
			}
		}

		public virtual int Dimension
		{
			get
			{
				return 1;
			}
		}

		internal PrimeField(BigInteger characteristic)
		{
			this.characteristic = characteristic;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			PrimeField primeField = obj as PrimeField;
			if (primeField == null)
			{
				return false;
			}
			return characteristic.Equals(primeField.characteristic);
		}

		public override int GetHashCode()
		{
			return characteristic.GetHashCode();
		}
	}
}
