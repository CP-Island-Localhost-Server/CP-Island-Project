using System;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Bcpg
{
	public class DsaPublicBcpgKey : BcpgObject, IBcpgKey
	{
		private readonly MPInteger p;

		private readonly MPInteger q;

		private readonly MPInteger g;

		private readonly MPInteger y;

		public string Format
		{
			get
			{
				return "PGP";
			}
		}

		public BigInteger G
		{
			get
			{
				return g.Value;
			}
		}

		public BigInteger P
		{
			get
			{
				return p.Value;
			}
		}

		public BigInteger Q
		{
			get
			{
				return q.Value;
			}
		}

		public BigInteger Y
		{
			get
			{
				return y.Value;
			}
		}

		public DsaPublicBcpgKey(BcpgInputStream bcpgIn)
		{
			p = new MPInteger(bcpgIn);
			q = new MPInteger(bcpgIn);
			g = new MPInteger(bcpgIn);
			y = new MPInteger(bcpgIn);
		}

		public DsaPublicBcpgKey(BigInteger p, BigInteger q, BigInteger g, BigInteger y)
		{
			this.p = new MPInteger(p);
			this.q = new MPInteger(q);
			this.g = new MPInteger(g);
			this.y = new MPInteger(y);
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
			bcpgOut.WriteObjects(p, q, g, y);
		}
	}
}
