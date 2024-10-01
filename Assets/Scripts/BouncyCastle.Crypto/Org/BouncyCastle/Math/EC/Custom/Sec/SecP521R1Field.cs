using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
	internal class SecP521R1Field
	{
		private const int P16 = 511;

		internal static readonly uint[] P = new uint[17]
		{
			4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u,
			4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 511u
		};

		public static void Add(uint[] x, uint[] y, uint[] z)
		{
			uint num = Nat.Add(16, x, y, z) + x[16] + y[16];
			if (num > 511 || (num == 511 && Nat.Eq(16, z, P)))
			{
				num += Nat.Inc(16, z);
				num &= 0x1FFu;
			}
			z[16] = num;
		}

		public static void AddOne(uint[] x, uint[] z)
		{
			uint num = Nat.Inc(16, x, z) + x[16];
			if (num > 511 || (num == 511 && Nat.Eq(16, z, P)))
			{
				num += Nat.Inc(16, z);
				num &= 0x1FFu;
			}
			z[16] = num;
		}

		public static uint[] FromBigInteger(BigInteger x)
		{
			uint[] array = Nat.FromBigInteger(521, x);
			if (Nat.Eq(17, array, P))
			{
				Nat.Zero(17, array);
			}
			return array;
		}

		public static void Half(uint[] x, uint[] z)
		{
			uint num = x[16];
			uint num2 = Nat.ShiftDownBit(16, x, num, z);
			z[16] = (num >> 1) | (num2 >> 23);
		}

		public static void Multiply(uint[] x, uint[] y, uint[] z)
		{
			uint[] array = Nat.Create(33);
			ImplMultiply(x, y, array);
			Reduce(array, z);
		}

		public static void Negate(uint[] x, uint[] z)
		{
			if (Nat.IsZero(17, x))
			{
				Nat.Zero(17, z);
			}
			else
			{
				Nat.Sub(17, P, x, z);
			}
		}

		public static void Reduce(uint[] xx, uint[] z)
		{
			uint num = xx[32];
			uint num2 = Nat.ShiftDownBits(16, xx, 16, 9, num, z, 0) >> 23;
			num2 += num >> 9;
			num2 += Nat.AddTo(16, xx, z);
			if (num2 > 511 || (num2 == 511 && Nat.Eq(16, z, P)))
			{
				num2 += Nat.Inc(16, z);
				num2 &= 0x1FFu;
			}
			z[16] = num2;
		}

		public static void Reduce23(uint[] z)
		{
			uint num = z[16];
			uint num2 = Nat.AddWordTo(16, num >> 9, z) + (num & 0x1FF);
			if (num2 > 511 || (num2 == 511 && Nat.Eq(16, z, P)))
			{
				num2 += Nat.Inc(16, z);
				num2 &= 0x1FFu;
			}
			z[16] = num2;
		}

		public static void Square(uint[] x, uint[] z)
		{
			uint[] array = Nat.Create(33);
			ImplSquare(x, array);
			Reduce(array, z);
		}

		public static void SquareN(uint[] x, int n, uint[] z)
		{
			uint[] array = Nat.Create(33);
			ImplSquare(x, array);
			Reduce(array, z);
			while (--n > 0)
			{
				ImplSquare(z, array);
				Reduce(array, z);
			}
		}

		public static void Subtract(uint[] x, uint[] y, uint[] z)
		{
			int num = Nat.Sub(16, x, y, z) + (int)(x[16] - y[16]);
			if (num < 0)
			{
				num += Nat.Dec(16, z);
				num &= 0x1FF;
			}
			z[16] = (uint)num;
		}

		public static void Twice(uint[] x, uint[] z)
		{
			uint num = x[16];
			uint num2 = Nat.ShiftUpBit(16, x, num << 23, z) | (num << 1);
			z[16] = num2 & 0x1FFu;
		}

		protected static void ImplMultiply(uint[] x, uint[] y, uint[] zz)
		{
			Nat512.Mul(x, y, zz);
			uint num = x[16];
			uint num2 = y[16];
			zz[32] = Nat.Mul31BothAdd(16, num, y, num2, x, zz, 16) + num * num2;
		}

		protected static void ImplSquare(uint[] x, uint[] zz)
		{
			Nat512.Square(x, zz);
			uint num = x[16];
			zz[32] = Nat.MulWordAddTo(16, num << 1, x, 0, zz, 16) + num * num;
		}
	}
}
