using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
	public sealed class PgpUtilities
	{
		private const int ReadAhead = 60;

		private PgpUtilities()
		{
		}

		public static MPInteger[] DsaSigToMpi(byte[] encoding)
		{
			DerInteger derInteger;
			DerInteger derInteger2;
			try
			{
				Asn1Sequence asn1Sequence = (Asn1Sequence)Asn1Object.FromByteArray(encoding);
				derInteger = (DerInteger)asn1Sequence[0];
				derInteger2 = (DerInteger)asn1Sequence[1];
			}
			catch (IOException exception)
			{
				throw new PgpException("exception encoding signature", exception);
			}
			return new MPInteger[2]
			{
				new MPInteger(derInteger.Value),
				new MPInteger(derInteger2.Value)
			};
		}

		public static MPInteger[] RsaSigToMpi(byte[] encoding)
		{
			return new MPInteger[1]
			{
				new MPInteger(new BigInteger(1, encoding))
			};
		}

		public static string GetDigestName(HashAlgorithmTag hashAlgorithm)
		{
			switch (hashAlgorithm)
			{
			case HashAlgorithmTag.Sha1:
				return "SHA1";
			case HashAlgorithmTag.MD2:
				return "MD2";
			case HashAlgorithmTag.MD5:
				return "MD5";
			case HashAlgorithmTag.RipeMD160:
				return "RIPEMD160";
			case HashAlgorithmTag.Sha224:
				return "SHA224";
			case HashAlgorithmTag.Sha256:
				return "SHA256";
			case HashAlgorithmTag.Sha384:
				return "SHA384";
			case HashAlgorithmTag.Sha512:
				return "SHA512";
			default:
				throw new PgpException("unknown hash algorithm tag in GetDigestName: " + hashAlgorithm);
			}
		}

		public static string GetSignatureName(PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm)
		{
			string text;
			switch (keyAlgorithm)
			{
			case PublicKeyAlgorithmTag.RsaGeneral:
			case PublicKeyAlgorithmTag.RsaSign:
				text = "RSA";
				break;
			case PublicKeyAlgorithmTag.Dsa:
				text = "DSA";
				break;
			case PublicKeyAlgorithmTag.EC:
				text = "ECDH";
				break;
			case PublicKeyAlgorithmTag.ECDsa:
				text = "ECDSA";
				break;
			case PublicKeyAlgorithmTag.ElGamalEncrypt:
			case PublicKeyAlgorithmTag.ElGamalGeneral:
				text = "ElGamal";
				break;
			default:
				throw new PgpException("unknown algorithm tag in signature:" + keyAlgorithm);
			}
			return GetDigestName(hashAlgorithm) + "with" + text;
		}

		public static string GetSymmetricCipherName(SymmetricKeyAlgorithmTag algorithm)
		{
			switch (algorithm)
			{
			case SymmetricKeyAlgorithmTag.Null:
				return null;
			case SymmetricKeyAlgorithmTag.TripleDes:
				return "DESEDE";
			case SymmetricKeyAlgorithmTag.Idea:
				return "IDEA";
			case SymmetricKeyAlgorithmTag.Cast5:
				return "CAST5";
			case SymmetricKeyAlgorithmTag.Blowfish:
				return "Blowfish";
			case SymmetricKeyAlgorithmTag.Safer:
				return "SAFER";
			case SymmetricKeyAlgorithmTag.Des:
				return "DES";
			case SymmetricKeyAlgorithmTag.Aes128:
				return "AES";
			case SymmetricKeyAlgorithmTag.Aes192:
				return "AES";
			case SymmetricKeyAlgorithmTag.Aes256:
				return "AES";
			case SymmetricKeyAlgorithmTag.Twofish:
				return "Twofish";
			case SymmetricKeyAlgorithmTag.Camellia128:
				return "Camellia";
			case SymmetricKeyAlgorithmTag.Camellia192:
				return "Camellia";
			case SymmetricKeyAlgorithmTag.Camellia256:
				return "Camellia";
			default:
				throw new PgpException("unknown symmetric algorithm: " + algorithm);
			}
		}

		public static int GetKeySize(SymmetricKeyAlgorithmTag algorithm)
		{
			switch (algorithm)
			{
			case SymmetricKeyAlgorithmTag.Des:
				return 64;
			case SymmetricKeyAlgorithmTag.Idea:
			case SymmetricKeyAlgorithmTag.Cast5:
			case SymmetricKeyAlgorithmTag.Blowfish:
			case SymmetricKeyAlgorithmTag.Safer:
			case SymmetricKeyAlgorithmTag.Aes128:
			case SymmetricKeyAlgorithmTag.Camellia128:
				return 128;
			case SymmetricKeyAlgorithmTag.TripleDes:
			case SymmetricKeyAlgorithmTag.Aes192:
			case SymmetricKeyAlgorithmTag.Camellia192:
				return 192;
			case SymmetricKeyAlgorithmTag.Aes256:
			case SymmetricKeyAlgorithmTag.Twofish:
			case SymmetricKeyAlgorithmTag.Camellia256:
				return 256;
			default:
				throw new PgpException("unknown symmetric algorithm: " + algorithm);
			}
		}

		public static KeyParameter MakeKey(SymmetricKeyAlgorithmTag algorithm, byte[] keyBytes)
		{
			string symmetricCipherName = GetSymmetricCipherName(algorithm);
			return ParameterUtilities.CreateKeyParameter(symmetricCipherName, keyBytes);
		}

		public static KeyParameter MakeRandomKey(SymmetricKeyAlgorithmTag algorithm, SecureRandom random)
		{
			int keySize = GetKeySize(algorithm);
			byte[] array = new byte[(keySize + 7) / 8];
			random.NextBytes(array);
			return MakeKey(algorithm, array);
		}

		internal static byte[] EncodePassPhrase(char[] passPhrase, bool utf8)
		{
			if (passPhrase != null)
			{
				if (!utf8)
				{
					return Strings.ToByteArray(passPhrase);
				}
				return Encoding.UTF8.GetBytes(passPhrase);
			}
			return null;
		}

		public static KeyParameter MakeKeyFromPassPhrase(SymmetricKeyAlgorithmTag algorithm, S2k s2k, char[] passPhrase)
		{
			return DoMakeKeyFromPassPhrase(algorithm, s2k, EncodePassPhrase(passPhrase, false), true);
		}

		public static KeyParameter MakeKeyFromPassPhraseUtf8(SymmetricKeyAlgorithmTag algorithm, S2k s2k, char[] passPhrase)
		{
			return DoMakeKeyFromPassPhrase(algorithm, s2k, EncodePassPhrase(passPhrase, true), true);
		}

		public static KeyParameter MakeKeyFromPassPhraseRaw(SymmetricKeyAlgorithmTag algorithm, S2k s2k, byte[] rawPassPhrase)
		{
			return DoMakeKeyFromPassPhrase(algorithm, s2k, rawPassPhrase, false);
		}

		internal static KeyParameter DoMakeKeyFromPassPhrase(SymmetricKeyAlgorithmTag algorithm, S2k s2k, byte[] rawPassPhrase, bool clearPassPhrase)
		{
			int keySize = GetKeySize(algorithm);
			byte[] array = new byte[(keySize + 7) / 8];
			int num = 0;
			int num2 = 0;
			while (num < array.Length)
			{
				IDigest digest;
				if (s2k != null)
				{
					string digestName = GetDigestName(s2k.HashAlgorithm);
					try
					{
						digest = DigestUtilities.GetDigest(digestName);
					}
					catch (Exception exception)
					{
						throw new PgpException("can't find S2k digest", exception);
					}
					for (int i = 0; i != num2; i++)
					{
						digest.Update(0);
					}
					byte[] iV = s2k.GetIV();
					switch (s2k.Type)
					{
					case 0:
						digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
						break;
					case 1:
						digest.BlockUpdate(iV, 0, iV.Length);
						digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
						break;
					case 3:
					{
						long iterationCount = s2k.IterationCount;
						digest.BlockUpdate(iV, 0, iV.Length);
						digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
						iterationCount -= iV.Length + rawPassPhrase.Length;
						while (iterationCount > 0)
						{
							if (iterationCount < iV.Length)
							{
								digest.BlockUpdate(iV, 0, (int)iterationCount);
								break;
							}
							digest.BlockUpdate(iV, 0, iV.Length);
							iterationCount -= iV.Length;
							if (iterationCount < rawPassPhrase.Length)
							{
								digest.BlockUpdate(rawPassPhrase, 0, (int)iterationCount);
								iterationCount = 0L;
							}
							else
							{
								digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
								iterationCount -= rawPassPhrase.Length;
							}
						}
						break;
					}
					default:
						throw new PgpException("unknown S2k type: " + s2k.Type);
					}
				}
				else
				{
					try
					{
						digest = DigestUtilities.GetDigest("MD5");
						for (int j = 0; j != num2; j++)
						{
							digest.Update(0);
						}
						digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
					}
					catch (Exception exception2)
					{
						throw new PgpException("can't find MD5 digest", exception2);
					}
				}
				byte[] array2 = DigestUtilities.DoFinal(digest);
				if (array2.Length > array.Length - num)
				{
					Array.Copy(array2, 0, array, num, array.Length - num);
				}
				else
				{
					Array.Copy(array2, 0, array, num, array2.Length);
				}
				num += array2.Length;
				num2++;
			}
			if (clearPassPhrase && rawPassPhrase != null)
			{
				Array.Clear(rawPassPhrase, 0, rawPassPhrase.Length);
			}
			return MakeKey(algorithm, array);
		}

		public static void WriteFileToLiteralData(Stream output, char fileType, FileInfo file)
		{
			PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
			Stream pOut = pgpLiteralDataGenerator.Open(output, fileType, file.Name, file.Length, file.LastWriteTime);
			PipeFileContents(file, pOut, 4096);
		}

		public static void WriteFileToLiteralData(Stream output, char fileType, FileInfo file, byte[] buffer)
		{
			PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();
			Stream pOut = pgpLiteralDataGenerator.Open(output, fileType, file.Name, file.LastWriteTime, buffer);
			PipeFileContents(file, pOut, buffer.Length);
		}

		private static void PipeFileContents(FileInfo file, Stream pOut, int bufSize)
		{
			FileStream fileStream = file.OpenRead();
			byte[] array = new byte[bufSize];
			int count;
			while ((count = fileStream.Read(array, 0, array.Length)) > 0)
			{
				pOut.Write(array, 0, count);
			}
			Platform.Dispose(pOut);
			Platform.Dispose(fileStream);
		}

		private static bool IsPossiblyBase64(int ch)
		{
			if ((ch < 65 || ch > 90) && (ch < 97 || ch > 122) && (ch < 48 || ch > 57) && ch != 43 && ch != 47 && ch != 13)
			{
				return ch == 10;
			}
			return true;
		}

		public static Stream GetDecoderStream(Stream inputStream)
		{
			if (!inputStream.CanSeek)
			{
				throw new ArgumentException("inputStream must be seek-able", "inputStream");
			}
			long position = inputStream.Position;
			int num = inputStream.ReadByte();
			if (((uint)num & 0x80u) != 0)
			{
				inputStream.Position = position;
				return inputStream;
			}
			if (!IsPossiblyBase64(num))
			{
				inputStream.Position = position;
				return new ArmoredInputStream(inputStream);
			}
			byte[] array = new byte[60];
			int i = 1;
			int num2 = 1;
			array[0] = (byte)num;
			for (; i != 60; i++)
			{
				if ((num = inputStream.ReadByte()) < 0)
				{
					break;
				}
				if (!IsPossiblyBase64(num))
				{
					inputStream.Position = position;
					return new ArmoredInputStream(inputStream);
				}
				if (num != 10 && num != 13)
				{
					array[num2++] = (byte)num;
				}
			}
			inputStream.Position = position;
			if (i < 4)
			{
				return new ArmoredInputStream(inputStream);
			}
			byte[] array2 = new byte[8];
			Array.Copy(array, 0, array2, 0, array2.Length);
			byte[] array3 = Base64.Decode(array2);
			bool hasHeaders = (array3[0] & 0x80) == 0;
			return new ArmoredInputStream(inputStream, hasHeaders);
		}

		internal static IWrapper CreateWrapper(SymmetricKeyAlgorithmTag encAlgorithm)
		{
			switch (encAlgorithm)
			{
			case SymmetricKeyAlgorithmTag.Aes128:
			case SymmetricKeyAlgorithmTag.Aes192:
			case SymmetricKeyAlgorithmTag.Aes256:
				return WrapperUtilities.GetWrapper("AESWRAP");
			case SymmetricKeyAlgorithmTag.Camellia128:
			case SymmetricKeyAlgorithmTag.Camellia192:
			case SymmetricKeyAlgorithmTag.Camellia256:
				return WrapperUtilities.GetWrapper("CAMELLIAWRAP");
			default:
				throw new PgpException("unknown wrap algorithm: " + encAlgorithm);
			}
		}

		internal static byte[] GenerateIV(int length, SecureRandom random)
		{
			byte[] array = new byte[length];
			random.NextBytes(array);
			return array;
		}

		internal static S2k GenerateS2k(HashAlgorithmTag hashAlgorithm, int s2kCount, SecureRandom random)
		{
			byte[] iv = GenerateIV(8, random);
			return new S2k(hashAlgorithm, iv, s2kCount);
		}
	}
}
