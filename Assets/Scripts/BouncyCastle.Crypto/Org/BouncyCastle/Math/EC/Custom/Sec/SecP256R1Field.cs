using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP256R1Field
	{
		internal const uint P7 = uint.MaxValue;

		internal const uint PExt15 = 4294967294u;

		internal static readonly uint[] P = new uint[8] { 4294967295u, 4294967295u, 4294967295u, 0u, 0u, 0u, 1u, 4294967295u };

		internal static readonly uint[] PExt = new uint[16]
		{
			1u, 0u, 0u, 4294967294u, 4294967295u, 4294967295u, 4294967294u, 1u, 4294967294u, 1u,
			4294967294u, 1u, 1u, 4294967294u, 2u, 4294967294u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			if (Nat256.Add(x, y, z) != 0 || (z[7] == uint.MaxValue && Nat256.Gte(z, P)))
			{
				AddPInvTo(z);
			}
		}

		public static void AddExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if (Nat.Add(16, xx, yy, zz) != 0 || (zz[15] >= 4294967294u && Nat.Gte(16, zz, PExt)))
			{
				Nat.SubFrom(16, PExt, zz);
			}
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			if (Nat.Inc(8, x, z) != 0 || (z[7] == uint.MaxValue && Nat256.Gte(z, P)))
			{
				AddPInvTo(z);
			}
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat256.FromBigInteger(x);
			if (array[7] == uint.MaxValue && Nat256.Gte(array, P))
			{
				Nat256.SubFrom(P, array);
			}
			return array;
		}

		public static void Half(uint[] x, uint[] z)
		{
			if ((x[0] & 1) == 0)
			{
				Nat.ShiftDownBit(8, x, 0u, z);
				return;
			}
			uint c = Nat256.Add(x, P, z);
			Nat.ShiftDownBit(8, z, c);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Mul(x, y, array);
			Reduce(array, z);
		}

		public static void MultiplyAddToExt(uint[] x, uint[] y, uint[] zz)
		{
			if (Nat256.MulAddTo(x, y, zz) != 0 || (zz[15] >= 4294967294u && Nat.Gte(16, zz, PExt)))
			{
				Nat.SubFrom(16, PExt, zz);
			}
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat256.IsZero(x))
			{
				Nat256.Zero(z);
			}
			else
			{
				Nat256.Sub(P, x, z);
			}
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			long num = xx[8];
			long num2 = xx[9];
			long num3 = xx[10];
			long num4 = xx[11];
			long num5 = xx[12];
			long num6 = xx[13];
			long num7 = xx[14];
			long num8 = xx[15];
			num -= 6;
			long num9 = num + num2;
			long num10 = num2 + num3;
			long num11 = num3 + num4 - num8;
			long num12 = num4 + num5;
			long num13 = num5 + num6;
			long num14 = num6 + num7;
			long num15 = num7 + num8;
			long num16 = 0L;
			num16 += xx[0] + num9 - num12 - num14;
			z[0] = (uint)num16;
			num16 >>= 32;
			num16 += xx[1] + num10 - num13 - num15;
			z[1] = (uint)num16;
			num16 >>= 32;
			num16 += xx[2] + num11 - num14;
			z[2] = (uint)num16;
			num16 >>= 32;
			num16 += xx[3] + (num12 << 1) + num6 - num8 - num9;
			z[3] = (uint)num16;
			num16 >>= 32;
			num16 += xx[4] + (num13 << 1) + num7 - num10;
			z[4] = (uint)num16;
			num16 >>= 32;
			num16 += xx[5] + (num14 << 1) - num11;
			z[5] = (uint)num16;
			num16 >>= 32;
			num16 += xx[6] + (num15 << 1) + num14 - num9;
			z[6] = (uint)num16;
			num16 >>= 32;
			num16 += xx[7] + (num8 << 1) + num - num11 - num13;
			z[7] = (uint)num16;
			num16 >>= 32;
			num16 += 6;
			Reduce32((uint)num16, z);
		}

		public static void Reduce32(uint x, uint[] z)
		{
			long num = 0L;
			if (x != 0)
			{
				long num2 = x;
				num += z[0] + num2;
				z[0] = (uint)num;
				num >>= 32;
				if (num != 0)
				{
					num += z[1];
					z[1] = (uint)num;
					num >>= 32;
					num += z[2];
					z[2] = (uint)num;
					num >>= 32;
				}
				num += z[3] - num2;
				z[3] = (uint)num;
				num >>= 32;
				if (num != 0)
				{
					num += z[4];
					z[4] = (uint)num;
					num >>= 32;
					num += z[5];
					z[5] = (uint)num;
					num >>= 32;
				}
				num += z[6] - num2;
				z[6] = (uint)num;
				num >>= 32;
				num += z[7] + num2;
				z[7] = (uint)num;
				num >>= 32;
			}
			if (num != 0 || (z[7] == uint.MaxValue && Nat256.Gte(z, P)))
			{
				AddPInvTo(z);
			}
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Square(x, array);
			Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat256.CreateExt();
			Nat256.Square(x, array);
			Reduce(array, z);
			while (--n > 0)
			{
				Nat256.Square(z, array);
				Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			if (Nat256.Sub(x, y, z) != 0)
			{
				SubPInvFrom(z);
			}
		}

		public static void SubtractExt(uint[] xx, uint[] yy, uint[] zz)
		{
			if (Nat.Sub(16, xx, yy, zz) != 0)
			{
				Nat.AddTo(16, PExt, zz);
			}
		}

		public static void Twice(uint[] x, uint[] z)
		{
			if (Nat.ShiftUpBit(8, x, 0u, z) != 0 || (z[7] == uint.MaxValue && Nat256.Gte(z, P)))
			{
				AddPInvTo(z);
			}
		}

		private static void AddPInvTo(uint[] z)
		{
			long num = (long)z[0] + 1L;
			z[0] = (uint)num;
			num >>= 32;
			if (num != 0)
			{
				num += z[1];
				z[1] = (uint)num;
				num >>= 32;
				num += z[2];
				z[2] = (uint)num;
				num >>= 32;
			}
			num += (long)z[3] - 1L;
			z[3] = (uint)num;
			num >>= 32;
			if (num != 0)
			{
				num += z[4];
				z[4] = (uint)num;
				num >>= 32;
				num += z[5];
				z[5] = (uint)num;
				num >>= 32;
			}
			num += (long)z[6] - 1L;
			z[6] = (uint)num;
			num >>= 32;
			num += (long)z[7] + 1L;
			z[7] = (uint)num;
		}

		private static void SubPInvFrom(uint[] z)
		{
			long num = (long)z[0] - 1L;
			z[0] = (uint)num;
			num >>= 32;
			if (num != 0)
			{
				num += z[1];
				z[1] = (uint)num;
				num >>= 32;
				num += z[2];
				z[2] = (uint)num;
				num >>= 32;
			}
			num += (long)z[3] + 1L;
			z[3] = (uint)num;
			num >>= 32;
			if (num != 0)
			{
				num += z[4];
				z[4] = (uint)num;
				num >>= 32;
				num += z[5];
				z[5] = (uint)num;
				num >>= 32;
			}
			num += (long)z[6] + 1L;
			z[6] = (uint)num;
			num >>= 32;
			num += (long)z[7] - 1L;
			z[7] = (uint)num;
		}
	}
}
