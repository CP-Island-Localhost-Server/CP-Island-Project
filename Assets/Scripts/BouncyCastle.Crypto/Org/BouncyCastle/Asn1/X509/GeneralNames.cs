using System;
using System.Text;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
	public class GeneralNames : Asn1Encodable
	{
		private readonly GeneralName[] names;

		public static GeneralNames GetInstance(object obj)
		{
			if (obj == null || obj is GeneralNames)
			{
				return (GeneralNames)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new GeneralNames((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		public static GeneralNames GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public GeneralNames(GeneralName name)
		{
			names = new GeneralName[1] { name };
		}

		public GeneralNames(GeneralName[] names)
		{
			this.names = (GeneralName[])names.Clone();
		}

		private GeneralNames(Asn1Sequence seq)
		{
			names = new GeneralName[seq.Count];
			for (int i = 0; i != seq.Count; i++)
			{
				names[i] = GeneralName.GetInstance(seq[i]);
			}
		}

		public GeneralName[] GetNames()
		{
			return (GeneralName[])names.Clone();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(names);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string newLine = Platform.NewLine;
			stringBuilder.Append("GeneralNames:");
			stringBuilder.Append(newLine);
			GeneralName[] array = names;
			foreach (GeneralName value in array)
			{
				stringBuilder.Append("    ");
				stringBuilder.Append(value);
				stringBuilder.Append(newLine);
			}
			return stringBuilder.ToString();
		}
	}
}
