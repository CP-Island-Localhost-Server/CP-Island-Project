namespace Org.BouncyCastle.Bcpg
{
	public class CompressedDataPacket : InputStreamPacket
	{
		private readonly CompressionAlgorithmTag algorithm;

		public CompressionAlgorithmTag Algorithm
		{
			get
			{
				return algorithm;
			}
		}

		internal CompressedDataPacket(BcpgInputStream bcpgIn)
			: base(bcpgIn)
		{
			algorithm = (CompressionAlgorithmTag)bcpgIn.ReadByte();
		}
	}
}
