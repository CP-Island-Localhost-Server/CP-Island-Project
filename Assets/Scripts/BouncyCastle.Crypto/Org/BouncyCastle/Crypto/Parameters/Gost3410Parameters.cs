using System;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class Gost3410Parameters : ICipherParameters
	{
		private readonly BigInteger p;

		private readonly BigInteger q;

		private readonly BigInteger a;

		private readonly Gost3410ValidationParameters validation;

		public BigInteger P
		{
			get
			{
				return p;
			}
		}

		public BigInteger Q
		{
			get
			{
				return q;
			}
		}

		public BigInteger A
		{
			get
			{
				return a;
			}
		}

		public Gost3410ValidationParameters ValidationParameters
		{
			get
			{
				return validation;
			}
		}

		public Gost3410Parameters(BigInteger p, BigInteger q, BigInteger a)
			: this(p, q, a, null)
		{
		}

		public Gost3410Parameters(BigInteger p, BigInteger q, BigInteger a, Gost3410ValidationParameters validation)
		{
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			if (q == null)
			{
				throw new ArgumentNullException("q");
			}
			if (a == null)
			{
				throw new ArgumentNullException("a");
			}
			this.p = p;
			this.q = q;
			this.a = a;
			this.validation = validation;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			Gost3410Parameters gost3410Parameters = obj as Gost3410Parameters;
			if (gost3410Parameters == null)
			{
				return false;
			}
			return Equals(gost3410Parameters);
		}

		protected bool Equals(Gost3410Parameters other)
		{
			if (p.Equals(other.p) && q.Equals(other.q))
			{
				return a.Equals(other.a);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return p.GetHashCode() ^ q.GetHashCode() ^ a.GetHashCode();
		}
	}
}
