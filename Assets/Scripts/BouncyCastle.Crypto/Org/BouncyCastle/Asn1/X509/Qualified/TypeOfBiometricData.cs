using System;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509.Qualified
{
	public class TypeOfBiometricData : Asn1Encodable, IAsn1Choice
	{
		public const int Picture = 0;

		public const int HandwrittenSignature = 1;

		internal Asn1Encodable obj;

		public bool IsPredefined
		{
			get
			{
				return obj is DerInteger;
			}
		}

		public int PredefinedBiometricType
		{
			get
			{
				return ((DerInteger)obj).Value.IntValue;
			}
		}

		public DerObjectIdentifier BiometricDataOid
		{
			get
			{
				return (DerObjectIdentifier)obj;
			}
		}

		public static TypeOfBiometricData GetInstance(object obj)
		{
			if (obj == null || obj is TypeOfBiometricData)
			{
				return (TypeOfBiometricData)obj;
			}
			if (obj is DerInteger)
			{
				DerInteger instance = DerInteger.GetInstance(obj);
				int intValue = instance.Value.IntValue;
				return new TypeOfBiometricData(intValue);
			}
			if (obj is DerObjectIdentifier)
			{
				DerObjectIdentifier instance2 = DerObjectIdentifier.GetInstance(obj);
				return new TypeOfBiometricData(instance2);
			}
			throw new ArgumentException("unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
		}

		public TypeOfBiometricData(int predefinedBiometricType)
		{
			if (predefinedBiometricType == 0 || predefinedBiometricType == 1)
			{
				obj = new DerInteger(predefinedBiometricType);
				return;
			}
			throw new ArgumentException("unknow PredefinedBiometricType : " + predefinedBiometricType);
		}

		public TypeOfBiometricData(DerObjectIdentifier biometricDataOid)
		{
			obj = biometricDataOid;
		}

		public override Asn1Object ToAsn1Object()
		{
			return obj.ToAsn1Object();
		}
	}
}
