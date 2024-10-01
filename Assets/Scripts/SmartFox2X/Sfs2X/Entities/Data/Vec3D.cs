using System;

namespace Sfs2X.Entities.Data
{
	public class Vec3D
	{
		private float fx;

		private float fy;

		private float fz;

		private int ix;

		private int iy;

		private int iz;

		private bool useFloat;

		public float FloatX
		{
			get
			{
				return fx;
			}
		}

		public float FloatY
		{
			get
			{
				return fy;
			}
		}

		public float FloatZ
		{
			get
			{
				return fz;
			}
		}

		public int IntX
		{
			get
			{
				return ix;
			}
		}

		public int IntY
		{
			get
			{
				return iy;
			}
		}

		public int IntZ
		{
			get
			{
				return iz;
			}
		}

		private Vec3D()
		{
		}

		public Vec3D(int px, int py, int pz)
		{
			ix = px;
			iy = py;
			iz = pz;
			useFloat = false;
		}

		public Vec3D(int px, int py)
			: this(px, py, 0)
		{
		}

		public Vec3D(float px, float py, float pz)
		{
			fx = px;
			fy = py;
			fz = pz;
			useFloat = true;
		}

		public Vec3D(float px, float py)
			: this(px, py, 0f)
		{
		}

		public static Vec3D fromArray(object array)
		{
			if (array is SFSArrayLite)
			{
				SFSArrayLite sFSArrayLite = array as SFSArrayLite;
				object elementAt = sFSArrayLite.GetElementAt(0);
				object elementAt2 = sFSArrayLite.GetElementAt(1);
				object value = ((sFSArrayLite.Size() <= 2) ? ((object)0) : sFSArrayLite.GetElementAt(2));
				array = ((!(elementAt is double)) ? ((object)new int[3]
				{
					Convert.ToInt32(elementAt),
					Convert.ToInt32(elementAt2),
					Convert.ToInt32(value)
				}) : ((object)new float[3]
				{
					Convert.ToSingle(elementAt),
					Convert.ToSingle(elementAt2),
					Convert.ToSingle(value)
				}));
			}
			if (array is int[])
			{
				return fromIntArray((int[])array);
			}
			if (array is float[])
			{
				return fromFloatArray((float[])array);
			}
			throw new ArgumentException("Invalid Array Type, cannot convert to Vec3D!");
		}

		private static Vec3D fromIntArray(int[] array)
		{
			if (array.Length != 3)
			{
				throw new ArgumentException("Wrong array size. Vec3D requires an array with 3 parameters (x,y,z)");
			}
			return new Vec3D(array[0], array[1], array[2]);
		}

		private static Vec3D fromFloatArray(float[] array)
		{
			if (array.Length != 3)
			{
				throw new ArgumentException("Wrong array size. Vec3D requires an array with 3 parameters (x,y,z)");
			}
			return new Vec3D(array[0], array[1], array[2]);
		}

		public bool IsFloat()
		{
			return useFloat;
		}

		public int[] ToIntArray()
		{
			return new int[3] { ix, iy, iz };
		}

		public float[] ToFloatArray()
		{
			return new float[3] { fx, fy, fz };
		}

		public override string ToString()
		{
			if (IsFloat())
			{
				return string.Format("({0},{1},{2})", fx, fy, fz);
			}
			return string.Format("({0},{1},{2})", ix, iy, iz);
		}
	}
}
