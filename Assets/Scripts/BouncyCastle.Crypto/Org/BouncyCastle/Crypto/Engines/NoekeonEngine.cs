using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class NoekeonEngine : IBlockCipher
	{
		private const int GenericSize = 16;

		private static readonly uint[] nullVector;

		private static readonly uint[] roundConstants;

		private uint[] state = new uint[4];

		private uint[] subKeys = new uint[4];

		private uint[] decryptKeys = new uint[4];

		private bool _initialised;

		private bool _forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "Noekeon";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		public NoekeonEngine()
		{
			_initialised = false;
		}

		public virtual int GetBlockSize()
		{
			return 16;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("Invalid parameters passed to Noekeon init - " + Platform.GetTypeName(parameters), "parameters");
			}
			_forEncryption = forEncryption;
			_initialised = true;
			KeyParameter keyParameter = (KeyParameter)parameters;
			setKey(keyParameter.GetKey());
		}

		public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (!_initialised)
			{
				throw new InvalidOperationException(AlgorithmName + " not initialised");
			}
			Check.DataLength(input, inOff, 16, "input buffer too short");
			Check.OutputLength(output, outOff, 16, "output buffer too short");
			if (!_forEncryption)
			{
				return decryptBlock(input, inOff, output, outOff);
			}
			return encryptBlock(input, inOff, output, outOff);
		}

		public virtual void Reset()
		{
		}

		private void setKey(byte[] key)
		{
			subKeys[0] = Pack.BE_To_UInt32(key, 0);
			subKeys[1] = Pack.BE_To_UInt32(key, 4);
			subKeys[2] = Pack.BE_To_UInt32(key, 8);
			subKeys[3] = Pack.BE_To_UInt32(key, 12);
		}

		private int encryptBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			state[0] = Pack.BE_To_UInt32(input, inOff);
			state[1] = Pack.BE_To_UInt32(input, inOff + 4);
			state[2] = Pack.BE_To_UInt32(input, inOff + 8);
			state[3] = Pack.BE_To_UInt32(input, inOff + 12);
			int i;
			uint[] array;
			for (i = 0; i < 16; i++)
			{
				(array = state)[0] = array[0] ^ roundConstants[i];
				theta(state, subKeys);
				pi1(state);
				gamma(state);
				pi2(state);
			}
			(array = state)[0] = array[0] ^ roundConstants[i];
			theta(state, subKeys);
			Pack.UInt32_To_BE(state[0], output, outOff);
			Pack.UInt32_To_BE(state[1], output, outOff + 4);
			Pack.UInt32_To_BE(state[2], output, outOff + 8);
			Pack.UInt32_To_BE(state[3], output, outOff + 12);
			return 16;
		}

		private int decryptBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			state[0] = Pack.BE_To_UInt32(input, inOff);
			state[1] = Pack.BE_To_UInt32(input, inOff + 4);
			state[2] = Pack.BE_To_UInt32(input, inOff + 8);
			state[3] = Pack.BE_To_UInt32(input, inOff + 12);
			Array.Copy(subKeys, 0, decryptKeys, 0, subKeys.Length);
			theta(decryptKeys, nullVector);
			int num;
			uint[] array;
			for (num = 16; num > 0; num--)
			{
				theta(state, decryptKeys);
				(array = state)[0] = array[0] ^ roundConstants[num];
				pi1(state);
				gamma(state);
				pi2(state);
			}
			theta(state, decryptKeys);
			(array = state)[0] = array[0] ^ roundConstants[num];
			Pack.UInt32_To_BE(state[0], output, outOff);
			Pack.UInt32_To_BE(state[1], output, outOff + 4);
			Pack.UInt32_To_BE(state[2], output, outOff + 8);
			Pack.UInt32_To_BE(state[3], output, outOff + 12);
			return 16;
		}

		private void gamma(uint[] a)
		{
			uint[] array;
			(array = a)[1] = array[1] ^ (~a[3] & ~a[2]);
			(array = a)[0] = array[0] ^ (a[2] & a[1]);
			uint num = a[3];
			a[3] = a[0];
			a[0] = num;
			(array = a)[2] = array[2] ^ (a[0] ^ a[1] ^ a[3]);
			(array = a)[1] = array[1] ^ (~a[3] & ~a[2]);
			(array = a)[0] = array[0] ^ (a[2] & a[1]);
		}

		private void theta(uint[] a, uint[] k)
		{
			uint num = a[0] ^ a[2];
			num ^= rotl(num, 8) ^ rotl(num, 24);
			uint[] array;
			(array = a)[1] = array[1] ^ num;
			(array = a)[3] = array[3] ^ num;
			for (int i = 0; i < 4; i++)
			{
				uint[] array2 = (array = a);
				int num2 = i;
				IntPtr intPtr = (IntPtr)num2;
				array2[num2] = array[(long)intPtr] ^ k[i];
			}
			num = a[1] ^ a[3];
			num ^= rotl(num, 8) ^ rotl(num, 24);
			(array = a)[0] = array[0] ^ num;
			(array = a)[2] = array[2] ^ num;
		}

		private void pi1(uint[] a)
		{
			a[1] = rotl(a[1], 1);
			a[2] = rotl(a[2], 5);
			a[3] = rotl(a[3], 2);
		}

		private void pi2(uint[] a)
		{
			a[1] = rotl(a[1], 31);
			a[2] = rotl(a[2], 27);
			a[3] = rotl(a[3], 30);
		}

		private uint rotl(uint x, int y)
		{
			return (x << y) | (x >> 32 - y);
		}

		static NoekeonEngine()
		{
			uint[] array = new uint[4];
			nullVector = array;
			roundConstants = new uint[17]
			{
				128u, 27u, 54u, 108u, 216u, 171u, 77u, 154u, 47u, 94u,
				188u, 99u, 198u, 151u, 53u, 106u, 212u
			};
		}
	}
}
