using System;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	public class PgpSignatureList : PgpObject
	{
		private PgpSignature[] sigs;

		public PgpSignature this[int index]
		{
			get
			{
				return sigs[index];
			}
		}

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get
			{
				return sigs.Length;
			}
		}

		public int Count
		{
			get
			{
				return sigs.Length;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return sigs.Length == 0;
			}
		}

		public PgpSignatureList(PgpSignature[] sigs)
		{
			this.sigs = (PgpSignature[])sigs.Clone();
		}

		public PgpSignatureList(PgpSignature sig)
		{
			sigs = new PgpSignature[1] { sig };
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public PgpSignature Get(int index)
		{
			return this[index];
		}
	}
}
