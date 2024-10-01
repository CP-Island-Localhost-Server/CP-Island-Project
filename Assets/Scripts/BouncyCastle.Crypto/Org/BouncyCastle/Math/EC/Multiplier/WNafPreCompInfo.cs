namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class WNafPreCompInfo : PreCompInfo
	{
		protected ECPoint[] m_preComp = null;

		protected ECPoint[] m_preCompNeg = null;

		protected ECPoint m_twice = null;

		public virtual ECPoint[] PreComp
		{
			get
			{
				return m_preComp;
			}
			set
			{
				m_preComp = value;
			}
		}

		public virtual ECPoint[] PreCompNeg
		{
			get
			{
				return m_preCompNeg;
			}
			set
			{
				m_preCompNeg = value;
			}
		}

		public virtual ECPoint Twice
		{
			get
			{
				return m_twice;
			}
			set
			{
				m_twice = value;
			}
		}
	}
}
