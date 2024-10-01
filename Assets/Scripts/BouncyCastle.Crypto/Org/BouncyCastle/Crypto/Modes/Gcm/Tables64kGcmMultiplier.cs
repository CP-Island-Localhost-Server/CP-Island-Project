using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	public class Tables64kGcmMultiplier : IGcmMultiplier
	{
		private byte[] H;

		private uint[][][] M;

		public void Init(byte[] H)
		{
			if (M == null)
			{
				M = new uint[16][][];
			}
			else if (Arrays.AreEqual(this.H, H))
			{
				return;
			}
			this.H = Arrays.Clone(H);
			M[0] = new uint[256][];
			M[0][0] = new uint[4];
			M[0][128] = GcmUtilities.AsUints(H);
			for (int num = 64; num >= 1; num >>= 1)
			{
				uint[] array = (uint[])M[0][num + num].Clone();
				GcmUtilities.MultiplyP(array);
				M[0][num] = array;
			}
			int num2 = 0;
			while (true)
			{
				for (int i = 2; i < 256; i += i)
				{
					for (int j = 1; j < i; j++)
					{
						uint[] array2 = (uint[])M[num2][i].Clone();
						GcmUtilities.Xor(array2, M[num2][j]);
						M[num2][i + j] = array2;
					}
				}
				if (++num2 == 16)
				{
					break;
				}
				M[num2] = new uint[256][];
				M[num2][0] = new uint[4];
				for (int num3 = 128; num3 > 0; num3 >>= 1)
				{
					uint[] array3 = (uint[])M[num2 - 1][num3].Clone();
					GcmUtilities.MultiplyP8(array3);
					M[num2][num3] = array3;
				}
			}
		}

		public void MultiplyH(byte[] x)
		{
			uint[] array = new uint[4];
			for (int i = 0; i != 16; i++)
			{
				uint[] array2 = M[i][x[i]];
				uint[] array3;
				(array3 = array)[0] = array3[0] ^ array2[0];
				(array3 = array)[1] = array3[1] ^ array2[1];
				(array3 = array)[2] = array3[2] ^ array2[2];
				(array3 = array)[3] = array3[3] ^ array2[3];
			}
			Pack.UInt32_To_BE(array, x, 0);
		}
	}
}
