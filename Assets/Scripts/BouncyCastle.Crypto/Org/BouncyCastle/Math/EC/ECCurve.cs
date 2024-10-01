using System;
using System.Collections;
using Org.BouncyCastle.Math.EC.Endo;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Math.Field;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Math.EC
{
	public abstract class ECCurve
	{
		public class Config
		{
			protected ECCurve outer;

			protected int coord;

			protected ECEndomorphism endomorphism;

			protected ECMultiplier multiplier;

			internal Config(ECCurve outer, int coord, ECEndomorphism endomorphism, ECMultiplier multiplier)
			{
				this.outer = outer;
				this.coord = coord;
				this.endomorphism = endomorphism;
				this.multiplier = multiplier;
			}

			public Config SetCoordinateSystem(int coord)
			{
				this.coord = coord;
				return this;
			}

			public Config SetEndomorphism(ECEndomorphism endomorphism)
			{
				this.endomorphism = endomorphism;
				return this;
			}

			public Config SetMultiplier(ECMultiplier multiplier)
			{
				this.multiplier = multiplier;
				return this;
			}

			public ECCurve Create()
			{
				if (!outer.SupportsCoordinateSystem(coord))
				{
					throw new InvalidOperationException("unsupported coordinate system");
				}
				ECCurve eCCurve = outer.CloneCurve();
				if (eCCurve == outer)
				{
					throw new InvalidOperationException("implementation returned current curve");
				}
				eCCurve.m_coord = coord;
				eCCurve.m_endomorphism = endomorphism;
				eCCurve.m_multiplier = multiplier;
				return eCCurve;
			}
		}

		public const int COORD_AFFINE = 0;

		public const int COORD_HOMOGENEOUS = 1;

		public const int COORD_JACOBIAN = 2;

		public const int COORD_JACOBIAN_CHUDNOVSKY = 3;

		public const int COORD_JACOBIAN_MODIFIED = 4;

		public const int COORD_LAMBDA_AFFINE = 5;

		public const int COORD_LAMBDA_PROJECTIVE = 6;

		public const int COORD_SKEWED = 7;

		protected readonly IFiniteField m_field;

		protected ECFieldElement m_a;

		protected ECFieldElement m_b;

		protected BigInteger m_order;

		protected BigInteger m_cofactor;

		protected int m_coord = 0;

		protected ECEndomorphism m_endomorphism = null;

		protected ECMultiplier m_multiplier = null;

		public abstract int FieldSize { get; }

		public abstract ECPoint Infinity { get; }

		public virtual IFiniteField Field
		{
			get
			{
				return m_field;
			}
		}

		public virtual ECFieldElement A
		{
			get
			{
				return m_a;
			}
		}

		public virtual ECFieldElement B
		{
			get
			{
				return m_b;
			}
		}

		public virtual BigInteger Order
		{
			get
			{
				return m_order;
			}
		}

		public virtual BigInteger Cofactor
		{
			get
			{
				return m_cofactor;
			}
		}

		public virtual int CoordinateSystem
		{
			get
			{
				return m_coord;
			}
		}

		public static int[] GetAllCoordinateSystems()
		{
			return new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
		}

		protected ECCurve(IFiniteField field)
		{
			m_field = field;
		}

		public abstract ECFieldElement FromBigInteger(BigInteger x);

		public abstract bool IsValidFieldElement(BigInteger x);

		public virtual Config Configure()
		{
			return new Config(this, m_coord, m_endomorphism, m_multiplier);
		}

		public virtual ECPoint ValidatePoint(BigInteger x, BigInteger y)
		{
			ECPoint eCPoint = CreatePoint(x, y);
			if (!eCPoint.IsValid())
			{
				throw new ArgumentException("Invalid point coordinates");
			}
			return eCPoint;
		}

		[Obsolete("Per-point compression property will be removed")]
		public virtual ECPoint ValidatePoint(BigInteger x, BigInteger y, bool withCompression)
		{
			ECPoint eCPoint = CreatePoint(x, y, withCompression);
			if (!eCPoint.IsValid())
			{
				throw new ArgumentException("Invalid point coordinates");
			}
			return eCPoint;
		}

		public virtual ECPoint CreatePoint(BigInteger x, BigInteger y)
		{
			return CreatePoint(x, y, false);
		}

		[Obsolete("Per-point compression property will be removed")]
		public virtual ECPoint CreatePoint(BigInteger x, BigInteger y, bool withCompression)
		{
			return CreateRawPoint(FromBigInteger(x), FromBigInteger(y), withCompression);
		}

		protected abstract ECCurve CloneCurve();

		protected internal abstract ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression);

		protected internal abstract ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression);

		protected virtual ECMultiplier CreateDefaultMultiplier()
		{
			GlvEndomorphism glvEndomorphism = m_endomorphism as GlvEndomorphism;
			if (glvEndomorphism != null)
			{
				return new GlvMultiplier(this, glvEndomorphism);
			}
			return new WNafL2RMultiplier();
		}

		public virtual bool SupportsCoordinateSystem(int coord)
		{
			return coord == 0;
		}

		public virtual PreCompInfo GetPreCompInfo(ECPoint point, string name)
		{
			CheckPoint(point);
			lock (point)
			{
				IDictionary preCompTable = point.m_preCompTable;
				return (preCompTable == null) ? null : ((PreCompInfo)preCompTable[name]);
			}
		}

		public virtual void SetPreCompInfo(ECPoint point, string name, PreCompInfo preCompInfo)
		{
			CheckPoint(point);
			lock (point)
			{
				IDictionary dictionary = point.m_preCompTable;
				if (dictionary == null)
				{
					dictionary = (point.m_preCompTable = Platform.CreateHashtable(4));
				}
				dictionary[name] = preCompInfo;
			}
		}

		public virtual ECPoint ImportPoint(ECPoint p)
		{
			if (this == p.Curve)
			{
				return p;
			}
			if (p.IsInfinity)
			{
				return Infinity;
			}
			p = p.Normalize();
			return ValidatePoint(p.XCoord.ToBigInteger(), p.YCoord.ToBigInteger(), p.IsCompressed);
		}

		public virtual void NormalizeAll(ECPoint[] points)
		{
			NormalizeAll(points, 0, points.Length, null);
		}

		public virtual void NormalizeAll(ECPoint[] points, int off, int len, ECFieldElement iso)
		{
			CheckPoints(points, off, len);
			int coordinateSystem = CoordinateSystem;
			if (coordinateSystem == 0 || coordinateSystem == 5)
			{
				if (iso != null)
				{
					throw new ArgumentException("not valid for affine coordinates", "iso");
				}
				return;
			}
			ECFieldElement[] array = new ECFieldElement[len];
			int[] array2 = new int[len];
			int num = 0;
			for (int i = 0; i < len; i++)
			{
				ECPoint eCPoint = points[off + i];
				if (eCPoint != null && (iso != null || !eCPoint.IsNormalized()))
				{
					array[num] = eCPoint.GetZCoord(0);
					array2[num++] = off + i;
				}
			}
			if (num != 0)
			{
				ECAlgorithms.MontgomeryTrick(array, 0, num, iso);
				for (int j = 0; j < num; j++)
				{
					int num2 = array2[j];
					points[num2] = points[num2].Normalize(array[j]);
				}
			}
		}

		protected virtual void CheckPoint(ECPoint point)
		{
			if (point == null || this != point.Curve)
			{
				throw new ArgumentException("must be non-null and on this curve", "point");
			}
		}

		protected virtual void CheckPoints(ECPoint[] points)
		{
			CheckPoints(points, 0, points.Length);
		}

		protected virtual void CheckPoints(ECPoint[] points, int off, int len)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			if (off < 0 || len < 0 || off > points.Length - len)
			{
				throw new ArgumentException("invalid range specified", "points");
			}
			for (int i = 0; i < len; i++)
			{
				ECPoint eCPoint = points[off + i];
				if (eCPoint != null && this != eCPoint.Curve)
				{
					throw new ArgumentException("entries must be null or on this curve", "points");
				}
			}
		}

		public virtual bool Equals(ECCurve other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			if (Field.Equals(other.Field) && A.ToBigInteger().Equals(other.A.ToBigInteger()))
			{
				return B.ToBigInteger().Equals(other.B.ToBigInteger());
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ECCurve);
		}

		public override int GetHashCode()
		{
			return Field.GetHashCode() ^ Integers.RotateLeft(A.ToBigInteger().GetHashCode(), 8) ^ Integers.RotateLeft(B.ToBigInteger().GetHashCode(), 16);
		}

		protected abstract ECPoint DecompressPoint(int yTilde, BigInteger X1);

		public virtual ECEndomorphism GetEndomorphism()
		{
			return m_endomorphism;
		}

		public virtual ECMultiplier GetMultiplier()
		{
			lock (this)
			{
				if (m_multiplier == null)
				{
					m_multiplier = CreateDefaultMultiplier();
				}
				return m_multiplier;
			}
		}

		public virtual ECPoint DecodePoint(byte[] encoded)
		{
			ECPoint eCPoint = null;
			int num = (FieldSize + 7) / 8;
			byte b = encoded[0];
			switch (b)
			{
			case 0:
				if (encoded.Length != 1)
				{
					throw new ArgumentException("Incorrect length for infinity encoding", "encoded");
				}
				eCPoint = Infinity;
				break;
			case 2:
			case 3:
			{
				if (encoded.Length != num + 1)
				{
					throw new ArgumentException("Incorrect length for compressed encoding", "encoded");
				}
				int yTilde = b & 1;
				BigInteger x3 = new BigInteger(1, encoded, 1, num);
				eCPoint = DecompressPoint(yTilde, x3);
				if (!eCPoint.SatisfiesCofactor())
				{
					throw new ArgumentException("Invalid point");
				}
				break;
			}
			case 4:
			{
				if (encoded.Length != 2 * num + 1)
				{
					throw new ArgumentException("Incorrect length for uncompressed encoding", "encoded");
				}
				BigInteger x2 = new BigInteger(1, encoded, 1, num);
				BigInteger y = new BigInteger(1, encoded, 1 + num, num);
				eCPoint = ValidatePoint(x2, y);
				break;
			}
			case 6:
			case 7:
			{
				if (encoded.Length != 2 * num + 1)
				{
					throw new ArgumentException("Incorrect length for hybrid encoding", "encoded");
				}
				BigInteger x = new BigInteger(1, encoded, 1, num);
				BigInteger bigInteger = new BigInteger(1, encoded, 1 + num, num);
				if (bigInteger.TestBit(0) != (b == 7))
				{
					throw new ArgumentException("Inconsistent Y coordinate in hybrid encoding", "encoded");
				}
				eCPoint = ValidatePoint(x, bigInteger);
				break;
			}
			default:
				throw new FormatException("Invalid point encoding " + b);
			}
			if (b != 0 && eCPoint.IsInfinity)
			{
				throw new ArgumentException("Invalid infinity encoding", "encoded");
			}
			return eCPoint;
		}
	}
}
