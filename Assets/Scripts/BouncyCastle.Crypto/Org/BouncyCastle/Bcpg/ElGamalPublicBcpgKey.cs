using System;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Bcpg
{
	public class ElGamalPublicBcpgKey : BcpgObject, IBcpgKey
	{
		internal MPInteger p;

		internal MPInteger g;

		internal MPInteger y;

		public string Format
		{
			get
			{
				return "PGP";
			}
		}

		public BigInteger P
		{
			get
			{
				return p.Value;
			}
		}

		public BigInteger G
		{
			get
			{
				return g.Value;
			}
		}

		public BigInteger Y
		{
			get
			{
				return y.Value;
			}
		}

		public ElGamalPublicBcpgKey(BcpgInputStream bcpgIn)
		{
			p = new MPInteger(bcpgIn);
			g = new MPInteger(bcpgIn);
			y = new MPInteger(bcpgIn);
		}

		public ElGamalPublicBcpgKey(BigInteger p, BigInteger g, BigInteger y)
		{
			this.p = new MPInteger(p);
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
			bcpgOut.WriteObjects(p, g, y);
		}
	}
}
