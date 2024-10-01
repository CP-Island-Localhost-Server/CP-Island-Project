using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class DerSet : Asn1Set
	{
		public static readonly DerSet Empty = new DerSet();

		public static DerSet FromVector(Asn1EncodableVector v)
		{
			if (v.Count >= 1)
			{
				return new DerSet(v);
			}
			return Empty;
		}

		internal static DerSet FromVector(Asn1EncodableVector v, bool needsSorting)
		{
			if (v.Count >= 1)
			{
				return new DerSet(v, needsSorting);
			}
			return Empty;
		}

		public DerSet()
			: base(0)
		{
		}

		public DerSet(Asn1Encodable obj)
			: base(1)
		{
			AddObject(obj);
		}

		public DerSet(params Asn1Encodable[] v)
			: base(v.Length)
		{
			foreach (Asn1Encodable obj in v)
			{
				AddObject(obj);
			}
			Sort();
		}

		public DerSet(Asn1EncodableVector v)
			: this(v, true)
		{
		}

		internal DerSet(Asn1EncodableVector v, bool needsSorting)
			: base(v.Count)
		{
			foreach (Asn1Encodable item in v)
			{
				AddObject(item);
			}
			if (needsSorting)
			{
				Sort();
			}
		}

		internal override void Encode(DerOutputStream derOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			DerOutputStream derOutputStream = new DerOutputStream(memoryStream);
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Asn1Encodable obj = (Asn1Encodable)enumerator.Current;
					derOutputStream.WriteObject(obj);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			Platform.Dispose(derOutputStream);
			byte[] bytes = memoryStream.ToArray();
			derOut.WriteEncoded(49, bytes);
		}
	}
}
