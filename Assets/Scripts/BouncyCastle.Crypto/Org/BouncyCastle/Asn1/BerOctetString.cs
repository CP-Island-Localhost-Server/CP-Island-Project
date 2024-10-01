using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class BerOctetString : DerOctetString, IEnumerable
	{
		private const int MaxLength = 1000;

		private readonly IEnumerable octs;

		public static BerOctetString FromSequence(Asn1Sequence seq)
		{
			IList list = Platform.CreateArrayList();
			foreach (Asn1Encodable item in seq)
			{
				list.Add(item);
			}
			return new BerOctetString(list);
		}

		private static byte[] ToBytes(IEnumerable octs)
		{
			MemoryStream memoryStream = new MemoryStream();
			foreach (DerOctetString oct in octs)
			{
				byte[] octets = oct.GetOctets();
				memoryStream.Write(octets, 0, octets.Length);
			}
			return memoryStream.ToArray();
		}

		public BerOctetString(byte[] str)
			: base(str)
		{
		}

		public BerOctetString(IEnumerable octets)
			: base(ToBytes(octets))
		{
			octs = octets;
		}

		public BerOctetString(Asn1Object obj)
			: base(obj)
		{
		}

		public BerOctetString(Asn1Encodable obj)
			: base(obj.ToAsn1Object())
		{
		}

		public override byte[] GetOctets()
		{
			return str;
		}

		public IEnumerator GetEnumerator()
		{
			if (octs == null)
			{
				return GenerateOcts().GetEnumerator();
			}
			return octs.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
		public IEnumerator GetObjects()
		{
			return GetEnumerator();
		}

		private IList GenerateOcts()
		{
			IList list = Platform.CreateArrayList();
			for (int i = 0; i < str.Length; i += 1000)
			{
				int num = System.Math.Min(str.Length, i + 1000);
				byte[] array = new byte[num - i];
				Array.Copy(str, i, array, 0, array.Length);
				list.Add(new DerOctetString(array));
			}
			return list;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			if (derOut is Asn1OutputStream || derOut is BerOutputStream)
			{
				derOut.WriteByte(36);
				derOut.WriteByte(128);
				{
					IEnumerator enumerator = GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							DerOctetString obj = (DerOctetString)enumerator.Current;
							derOut.WriteObject(obj);
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
				}
				derOut.WriteByte(0);
				derOut.WriteByte(0);
			}
			else
			{
				base.Encode(derOut);
			}
		}
	}
}
