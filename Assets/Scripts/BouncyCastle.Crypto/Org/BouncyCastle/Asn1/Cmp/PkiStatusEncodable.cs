using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PkiStatusEncodable : Asn1Encodable
	{
		public static readonly PkiStatusEncodable granted = new PkiStatusEncodable(PkiStatus.Granted);

		public static readonly PkiStatusEncodable grantedWithMods = new PkiStatusEncodable(PkiStatus.GrantedWithMods);

		public static readonly PkiStatusEncodable rejection = new PkiStatusEncodable(PkiStatus.Rejection);

		public static readonly PkiStatusEncodable waiting = new PkiStatusEncodable(PkiStatus.Waiting);

		public static readonly PkiStatusEncodable revocationWarning = new PkiStatusEncodable(PkiStatus.RevocationWarning);

		public static readonly PkiStatusEncodable revocationNotification = new PkiStatusEncodable(PkiStatus.RevocationNotification);

		public static readonly PkiStatusEncodable keyUpdateWaiting = new PkiStatusEncodable(PkiStatus.KeyUpdateWarning);

		private readonly DerInteger status;

		public virtual BigInteger Value
		{
			get
			{
				return status.Value;
			}
		}

		private PkiStatusEncodable(PkiStatus status)
			: this(new DerInteger((int)status))
		{
		}

		private PkiStatusEncodable(DerInteger status)
		{
			this.status = status;
		}

		public static PkiStatusEncodable GetInstance(object obj)
		{
			if (obj is PkiStatusEncodable)
			{
				return (PkiStatusEncodable)obj;
			}
			if (obj is DerInteger)
			{
				return new PkiStatusEncodable((DerInteger)obj);
			}
			throw new ArgumentException("Invalid object: " + Platform.GetTypeName(obj), "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			return status;
		}
	}
}
