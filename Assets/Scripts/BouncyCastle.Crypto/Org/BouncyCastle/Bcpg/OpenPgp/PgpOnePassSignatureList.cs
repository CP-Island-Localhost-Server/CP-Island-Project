using System;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	public class PgpOnePassSignatureList : PgpObject
	{
		private readonly PgpOnePassSignature[] sigs;

		public PgpOnePassSignature this[int index]
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

		public PgpOnePassSignatureList(PgpOnePassSignature[] sigs)
		{
			this.sigs = (PgpOnePassSignature[])sigs.Clone();
		}

		public PgpOnePassSignatureList(PgpOnePassSignature sig)
		{
			sigs = new PgpOnePassSignature[1] { sig };
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public PgpOnePassSignature Get(int index)
		{
			return this[index];
		}
	}
}
