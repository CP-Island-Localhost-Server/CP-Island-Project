namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class FixedPointPreCompInfo : PreCompInfo
	{
		protected ECPoint[] m_preComp = null;

		protected int m_width = -1;

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

		public virtual int Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}
	}
}
