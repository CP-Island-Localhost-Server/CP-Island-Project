using System;
using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class XSalsa20Engine : Salsa20Engine
	{
		public override string AlgorithmName
		{
			get
			{
				return "XSalsa20";
			}
		}

		protected override int NonceSize
		{
			get
			{
				return 24;
			}
		}

		protected override void SetKey(byte[] keyBytes, byte[] ivBytes)
		{
			if (keyBytes.Length != 32)
			{
				throw new ArgumentException(AlgorithmName + " requires a 256 bit key");
			}
			base.SetKey(keyBytes, ivBytes);
			engineState[8] = Pack.LE_To_UInt32(ivBytes, 8);
			engineState[9] = Pack.LE_To_UInt32(ivBytes, 12);
			uint[] array = new uint[engineState.Length];
			Salsa20Engine.SalsaCore(20, engineState, array);
			engineState[1] = array[0] - engineState[0];
			engineState[2] = array[5] - engineState[5];
			engineState[3] = array[10] - engineState[10];
			engineState[4] = array[15] - engineState[15];
			engineState[11] = array[6] - engineState[6];
			engineState[12] = array[7] - engineState[7];
			engineState[13] = array[8] - engineState[8];
			engineState[14] = array[9] - engineState[9];
			engineState[6] = Pack.LE_To_UInt32(ivBytes, 16);
			engineState[7] = Pack.LE_To_UInt32(ivBytes, 20);
			ResetCounter();
		}
	}
}
