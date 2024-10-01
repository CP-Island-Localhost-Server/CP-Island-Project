using System;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1
{
	public abstract class Asn1Sequence : Asn1Object, IEnumerable
	{
		private class Asn1SequenceParserImpl : Asn1SequenceParser, IAsn1Convertible
		{
			private readonly Asn1Sequence outer;

			private readonly int max;

			private int index;

			public Asn1SequenceParserImpl(Asn1Sequence outer)
			{
				this.outer = outer;
				max = outer.Count;
			}

			public IAsn1Convertible ReadObject()
			{
				if (index == max)
				{
					return null;
				}
				Asn1Encodable asn1Encodable = outer[index++];
				if (asn1Encodable is Asn1Sequence)
				{
					return ((Asn1Sequence)asn1Encodable).Parser;
				}
				if (asn1Encodable is Asn1Set)
				{
					return ((Asn1Set)asn1Encodable).Parser;
				}
				return asn1Encodable;
			}

			public Asn1Object ToAsn1Object()
			{
				return outer;
			}
		}

		private readonly IList seq;

		public virtual Asn1SequenceParser Parser
		{
			get
			{
				return new Asn1SequenceParserImpl(this);
			}
		}

		public virtual Asn1Encodable this[int index]
		{
			get
			{
				return (Asn1Encodable)seq[index];
			}
		}

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get
			{
				return Count;
			}
		}

		public virtual int Count
		{
			get
			{
				return seq.Count;
			}
		}

		public static Asn1Sequence GetInstance(object obj)
		{
			if (obj == null || obj is Asn1Sequence)
			{
				return (Asn1Sequence)obj;
			}
			if (obj is Asn1SequenceParser)
			{
				return GetInstance(((Asn1SequenceParser)obj).ToAsn1Object());
			}
			if (obj is byte[])
			{
				try
				{
					return GetInstance(Asn1Object.FromByteArray((byte[])obj));
				}
				catch (IOException ex)
				{
					throw new ArgumentException("failed to construct sequence from byte[]: " + ex.Message);
				}
			}
			if (obj is Asn1Encodable)
			{
				Asn1Object asn1Object = ((Asn1Encodable)obj).ToAsn1Object();
				if (asn1Object is Asn1Sequence)
				{
					return (Asn1Sequence)asn1Object;
				}
			}
			throw new ArgumentException("Unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
		}

		public static Asn1Sequence GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			Asn1Object @object = obj.GetObject();
			if (explicitly)
			{
				if (!obj.IsExplicit())
				{
					throw new ArgumentException("object implicit - explicit expected.");
				}
				return (Asn1Sequence)@object;
			}
			if (obj.IsExplicit())
			{
				if (obj is BerTaggedObject)
				{
					return new BerSequence(@object);
				}
				return new DerSequence(@object);
			}
			if (@object is Asn1Sequence)
			{
				return (Asn1Sequence)@object;
			}
			throw new ArgumentException("Unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
		}

		protected internal Asn1Sequence(int capacity)
		{
			seq = Platform.CreateArrayList(capacity);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return seq.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
		public IEnumerator GetObjects()
		{
			return GetEnumerator();
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public Asn1Encodable GetObjectAt(int index)
		{
			return this[index];
		}

		protected override int Asn1GetHashCode()
		{
			int num = Count;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					num *= 17;
					num = ((current != null) ? (num ^ current.GetHashCode()) : (num ^ DerNull.Instance.GetHashCode()));
				}
				return num;
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

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			Asn1Sequence asn1Sequence = asn1Object as Asn1Sequence;
			if (asn1Sequence == null)
			{
				return false;
			}
			if (Count != asn1Sequence.Count)
			{
				return false;
			}
			IEnumerator enumerator = GetEnumerator();
			IEnumerator enumerator2 = asn1Sequence.GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				Asn1Object asn1Object2 = GetCurrent(enumerator).ToAsn1Object();
				Asn1Object obj = GetCurrent(enumerator2).ToAsn1Object();
				if (!asn1Object2.Equals(obj))
				{
					return false;
				}
			}
			return true;
		}

		private Asn1Encodable GetCurrent(IEnumerator e)
		{
			Asn1Encodable asn1Encodable = (Asn1Encodable)e.Current;
			if (asn1Encodable == null)
			{
				return DerNull.Instance;
			}
			return asn1Encodable;
		}

		protected internal void AddObject(Asn1Encodable obj)
		{
			seq.Add(obj);
		}

		public override string ToString()
		{
			return CollectionUtilities.ToString(seq);
		}
	}
}
