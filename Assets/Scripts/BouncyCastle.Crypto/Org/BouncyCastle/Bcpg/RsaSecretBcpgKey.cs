using System;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Bcpg
{
	public class RsaSecretBcpgKey : BcpgObject, IBcpgKey
	{
		private readonly MPInteger d;

		private readonly MPInteger p;

		private readonly MPInteger q;

		private readonly MPInteger u;

		private readonly BigInteger expP;

		private readonly BigInteger expQ;

		private readonly BigInteger crt;

		public BigInteger Modulus
		{
			get
			{
				return p.Value.Multiply(q.Value);
			}
		}

		public BigInteger PrivateExponent
		{
			get
			{
				return d.Value;
			}
		}

		public BigInteger PrimeP
		{
			get
			{
				return p.Value;
			}
		}

		public BigInteger PrimeQ
		{
			get
			{
				return q.Value;
			}
		}

		public BigInteger PrimeExponentP
		{
			get
			{
				return expP;
			}
		}

		public BigInteger PrimeExponentQ
		{
			get
			{
				return expQ;
			}
		}

		public BigInteger CrtCoefficient
		{
			get
			{
				return crt;
			}
		}

		public string Format
		{
			get
			{
				return "PGP";
			}
		}

		public RsaSecretBcpgKey(BcpgInputStream bcpgIn)
		{
			d = new MPInteger(bcpgIn);
			p = new MPInteger(bcpgIn);
			q = new MPInteger(bcpgIn);
			u = new MPInteger(bcpgIn);
			expP = d.Value.Remainder(p.Value.Subtract(BigInteger.One));
			expQ = d.Value.Remainder(q.Value.Subtract(BigInteger.One));
			crt = q.Value.ModInverse(p.Value);
		}

		public RsaSecretBcpgKey(BigInteger d, BigInteger p, BigInteger q)
		{
			int num = p.CompareTo(q);
			if (num >= 0)
			{
				if (num == 0)
				{
					throw new ArgumentException("p and q cannot be equal");
				}
				BigInteger bigInteger = p;
				p = q;
				q = bigInteger;
			}
			this.d = new MPInteger(d);
			this.p = new MPInteger(p);
			this.q = new MPInteger(q);
			u = new MPInteger(p.ModInverse(q));
			expP = d.Remainder(p.Subtract(BigInteger.One));
			expQ = d.Remainder(q.Subtract(BigInteger.One));
			crt = q.ModInverse(p);
		}

		public override byte[] GetEncoded()
		{
			try
			{
				return base.GetEncoded();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WriteObjects(d, p, q, u);
		}
	}
}
