using System;
using System.Collections;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Pkix
{
	public class PkixNameConstraintValidator
	{
		private ISet excludedSubtreesDN = new HashSet();

		private ISet excludedSubtreesDNS = new HashSet();

		private ISet excludedSubtreesEmail = new HashSet();

		private ISet excludedSubtreesURI = new HashSet();

		private ISet excludedSubtreesIP = new HashSet();

		private ISet permittedSubtreesDN;

		private ISet permittedSubtreesDNS;

		private ISet permittedSubtreesEmail;

		private ISet permittedSubtreesURI;

		private ISet permittedSubtreesIP;

		private static bool WithinDNSubtree(Asn1Sequence dns, Asn1Sequence subtree)
		{
			if (subtree.Count < 1)
			{
				return false;
			}
			if (subtree.Count > dns.Count)
			{
				return false;
			}
			for (int num = subtree.Count - 1; num >= 0; num--)
			{
				if (!subtree[num].Equals(dns[num]))
				{
					return false;
				}
			}
			return true;
		}

		public void CheckPermittedDN(Asn1Sequence dns)
		{
			CheckPermittedDN(permittedSubtreesDN, dns);
		}

		public void CheckExcludedDN(Asn1Sequence dns)
		{
			CheckExcludedDN(excludedSubtreesDN, dns);
		}

		private void CheckPermittedDN(ISet permitted, Asn1Sequence dns)
		{
			if (permitted == null || (permitted.Count == 0 && dns.Count == 0))
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Asn1Sequence subtree = (Asn1Sequence)enumerator.Current;
				if (WithinDNSubtree(dns, subtree))
				{
					return;
				}
			}
			throw new PkixNameConstraintValidatorException("Subject distinguished name is not from a permitted subtree");
		}

		private void CheckExcludedDN(ISet excluded, Asn1Sequence dns)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Asn1Sequence subtree = (Asn1Sequence)enumerator.Current;
				if (WithinDNSubtree(dns, subtree))
				{
					throw new PkixNameConstraintValidatorException("Subject distinguished name is from an excluded subtree");
				}
			}
		}

		private ISet IntersectDN(ISet permitted, ISet dns)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = dns.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Asn1Sequence instance = Asn1Sequence.GetInstance(((GeneralSubtree)enumerator.Current).Base.Name.ToAsn1Object());
				if (permitted == null)
				{
					if (instance != null)
					{
						set.Add(instance);
					}
					continue;
				}
				IEnumerator enumerator2 = permitted.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Asn1Sequence asn1Sequence = (Asn1Sequence)enumerator2.Current;
					if (WithinDNSubtree(instance, asn1Sequence))
					{
						set.Add(instance);
					}
					else if (WithinDNSubtree(asn1Sequence, instance))
					{
						set.Add(asn1Sequence);
					}
				}
			}
			return set;
		}

		private ISet UnionDN(ISet excluded, Asn1Sequence dn)
		{
			if (excluded.IsEmpty)
			{
				if (dn == null)
				{
					return excluded;
				}
				excluded.Add(dn);
				return excluded;
			}
			ISet set = new HashSet();
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Asn1Sequence asn1Sequence = (Asn1Sequence)enumerator.Current;
				if (WithinDNSubtree(dn, asn1Sequence))
				{
					set.Add(asn1Sequence);
					continue;
				}
				if (WithinDNSubtree(asn1Sequence, dn))
				{
					set.Add(dn);
					continue;
				}
				set.Add(asn1Sequence);
				set.Add(dn);
			}
			return set;
		}

		private ISet IntersectEmail(ISet permitted, ISet emails)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = emails.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = ExtractNameAsString(((GeneralSubtree)enumerator.Current).Base);
				if (permitted == null)
				{
					if (text != null)
					{
						set.Add(text);
					}
					continue;
				}
				IEnumerator enumerator2 = permitted.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					string email = (string)enumerator2.Current;
					intersectEmail(text, email, set);
				}
			}
			return set;
		}

		private ISet UnionEmail(ISet excluded, string email)
		{
			if (excluded.IsEmpty)
			{
				if (email == null)
				{
					return excluded;
				}
				excluded.Add(email);
				return excluded;
			}
			ISet set = new HashSet();
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string email2 = (string)enumerator.Current;
				unionEmail(email2, email, set);
			}
			return set;
		}

		private ISet IntersectIP(ISet permitted, ISet ips)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = ips.GetEnumerator();
			while (enumerator.MoveNext())
			{
				byte[] octets = Asn1OctetString.GetInstance(((GeneralSubtree)enumerator.Current).Base.Name).GetOctets();
				if (permitted == null)
				{
					if (octets != null)
					{
						set.Add(octets);
					}
					continue;
				}
				IEnumerator enumerator2 = permitted.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					byte[] ipWithSubmask = (byte[])enumerator2.Current;
					set.AddAll(IntersectIPRange(ipWithSubmask, octets));
				}
			}
			return set;
		}

		private ISet UnionIP(ISet excluded, byte[] ip)
		{
			if (excluded.IsEmpty)
			{
				if (ip == null)
				{
					return excluded;
				}
				excluded.Add(ip);
				return excluded;
			}
			ISet set = new HashSet();
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				byte[] ipWithSubmask = (byte[])enumerator.Current;
				set.AddAll(UnionIPRange(ipWithSubmask, ip));
			}
			return set;
		}

		private ISet UnionIPRange(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
		{
			ISet set = new HashSet();
			if (Arrays.AreEqual(ipWithSubmask1, ipWithSubmask2))
			{
				set.Add(ipWithSubmask1);
			}
			else
			{
				set.Add(ipWithSubmask1);
				set.Add(ipWithSubmask2);
			}
			return set;
		}

		private ISet IntersectIPRange(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
		{
			if (ipWithSubmask1.Length != ipWithSubmask2.Length)
			{
				return new HashSet();
			}
			byte[][] array = ExtractIPsAndSubnetMasks(ipWithSubmask1, ipWithSubmask2);
			byte[] ip = array[0];
			byte[] array2 = array[1];
			byte[] ip2 = array[2];
			byte[] array3 = array[3];
			byte[][] array4 = MinMaxIPs(ip, array2, ip2, array3);
			byte[] ip3 = Min(array4[1], array4[3]);
			byte[] ip4 = Max(array4[0], array4[2]);
			if (CompareTo(ip4, ip3) == 1)
			{
				return new HashSet();
			}
			byte[] ip5 = Or(array4[0], array4[2]);
			byte[] subnetMask = Or(array2, array3);
			ISet set = new HashSet();
			set.Add(IpWithSubnetMask(ip5, subnetMask));
			return set;
		}

		private byte[] IpWithSubnetMask(byte[] ip, byte[] subnetMask)
		{
			int num = ip.Length;
			byte[] array = new byte[num * 2];
			Array.Copy(ip, 0, array, 0, num);
			Array.Copy(subnetMask, 0, array, num, num);
			return array;
		}

		private byte[][] ExtractIPsAndSubnetMasks(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
		{
			int num = ipWithSubmask1.Length / 2;
			byte[] array = new byte[num];
			byte[] array2 = new byte[num];
			Array.Copy(ipWithSubmask1, 0, array, 0, num);
			Array.Copy(ipWithSubmask1, num, array2, 0, num);
			byte[] array3 = new byte[num];
			byte[] array4 = new byte[num];
			Array.Copy(ipWithSubmask2, 0, array3, 0, num);
			Array.Copy(ipWithSubmask2, num, array4, 0, num);
			return new byte[4][] { array, array2, array3, array4 };
		}

		private byte[][] MinMaxIPs(byte[] ip1, byte[] subnetmask1, byte[] ip2, byte[] subnetmask2)
		{
			int num = ip1.Length;
			byte[] array = new byte[num];
			byte[] array2 = new byte[num];
			byte[] array3 = new byte[num];
			byte[] array4 = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (byte)(ip1[i] & subnetmask1[i]);
				array2[i] = (byte)((ip1[i] & subnetmask1[i]) | ~subnetmask1[i]);
				array3[i] = (byte)(ip2[i] & subnetmask2[i]);
				array4[i] = (byte)((ip2[i] & subnetmask2[i]) | ~subnetmask2[i]);
			}
			return new byte[4][] { array, array2, array3, array4 };
		}

		private void CheckPermittedEmail(ISet permitted, string email)
		{
			if (permitted == null)
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string constraint = (string)enumerator.Current;
				if (EmailIsConstrained(email, constraint))
				{
					return;
				}
			}
			if (email.Length == 0 && permitted.Count == 0)
			{
				return;
			}
			throw new PkixNameConstraintValidatorException("Subject email address is not from a permitted subtree.");
		}

		private void CheckExcludedEmail(ISet excluded, string email)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string constraint = (string)enumerator.Current;
				if (EmailIsConstrained(email, constraint))
				{
					throw new PkixNameConstraintValidatorException("Email address is from an excluded subtree.");
				}
			}
		}

		private void CheckPermittedIP(ISet permitted, byte[] ip)
		{
			if (permitted == null)
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				byte[] constraint = (byte[])enumerator.Current;
				if (IsIPConstrained(ip, constraint))
				{
					return;
				}
			}
			if (ip.Length == 0 && permitted.Count == 0)
			{
				return;
			}
			throw new PkixNameConstraintValidatorException("IP is not from a permitted subtree.");
		}

		private void checkExcludedIP(ISet excluded, byte[] ip)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				byte[] constraint = (byte[])enumerator.Current;
				if (IsIPConstrained(ip, constraint))
				{
					throw new PkixNameConstraintValidatorException("IP is from an excluded subtree.");
				}
			}
		}

		private bool IsIPConstrained(byte[] ip, byte[] constraint)
		{
			int num = ip.Length;
			if (num != constraint.Length / 2)
			{
				return false;
			}
			byte[] array = new byte[num];
			Array.Copy(constraint, num, array, 0, num);
			byte[] array2 = new byte[num];
			byte[] array3 = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = (byte)(constraint[i] & array[i]);
				array3[i] = (byte)(ip[i] & array[i]);
			}
			return Arrays.AreEqual(array2, array3);
		}

		private bool EmailIsConstrained(string email, string constraint)
		{
			string text = email.Substring(email.IndexOf('@') + 1);
			if (constraint.IndexOf('@') != -1)
			{
				if (Platform.ToUpperInvariant(email).Equals(Platform.ToUpperInvariant(constraint)))
				{
					return true;
				}
			}
			else if (!constraint[0].Equals((object)'.'))
			{
				if (Platform.ToUpperInvariant(text).Equals(Platform.ToUpperInvariant(constraint)))
				{
					return true;
				}
			}
			else if (WithinDomain(text, constraint))
			{
				return true;
			}
			return false;
		}

		private bool WithinDomain(string testDomain, string domain)
		{
			string text = domain;
			if (Platform.StartsWith(text, "."))
			{
				text = text.Substring(1);
			}
			string[] array = text.Split('.');
			string[] array2 = testDomain.Split('.');
			if (array2.Length <= array.Length)
			{
				return false;
			}
			int num = array2.Length - array.Length;
			for (int i = -1; i < array.Length; i++)
			{
				if (i == -1)
				{
					if (array2[i + num].Equals(""))
					{
						return false;
					}
				}
				else if (!Platform.EqualsIgnoreCase(array2[i + num], array[i]))
				{
					return false;
				}
			}
			return true;
		}

		private void CheckPermittedDNS(ISet permitted, string dns)
		{
			if (permitted == null)
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current;
				if (WithinDomain(dns, text) || Platform.ToUpperInvariant(dns).Equals(Platform.ToUpperInvariant(text)))
				{
					return;
				}
			}
			if (dns.Length == 0 && permitted.Count == 0)
			{
				return;
			}
			throw new PkixNameConstraintValidatorException("DNS is not from a permitted subtree.");
		}

		private void checkExcludedDNS(ISet excluded, string dns)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current;
				if (WithinDomain(dns, text) || Platform.EqualsIgnoreCase(dns, text))
				{
					throw new PkixNameConstraintValidatorException("DNS is from an excluded subtree.");
				}
			}
		}

		private void unionEmail(string email1, string email2, ISet union)
		{
			if (email1.IndexOf('@') != -1)
			{
				string text = email1.Substring(email1.IndexOf('@') + 1);
				if (email2.IndexOf('@') != -1)
				{
					if (Platform.EqualsIgnoreCase(email1, email2))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
				}
				else if (Platform.StartsWith(email2, "."))
				{
					if (WithinDomain(text, email2))
					{
						union.Add(email2);
						return;
					}
					union.Add(email1);
					union.Add(email2);
				}
				else if (Platform.EqualsIgnoreCase(text, email2))
				{
					union.Add(email2);
				}
				else
				{
					union.Add(email1);
					union.Add(email2);
				}
			}
			else if (Platform.StartsWith(email1, "."))
			{
				if (email2.IndexOf('@') != -1)
				{
					string testDomain = email2.Substring(email1.IndexOf('@') + 1);
					if (WithinDomain(testDomain, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
				}
				else if (Platform.StartsWith(email2, "."))
				{
					if (WithinDomain(email1, email2) || Platform.EqualsIgnoreCase(email1, email2))
					{
						union.Add(email2);
						return;
					}
					if (WithinDomain(email2, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
				}
				else if (WithinDomain(email2, email1))
				{
					union.Add(email1);
				}
				else
				{
					union.Add(email1);
					union.Add(email2);
				}
			}
			else if (email2.IndexOf('@') != -1)
			{
				string a = email2.Substring(email1.IndexOf('@') + 1);
				if (Platform.EqualsIgnoreCase(a, email1))
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(email1, email2))
				{
					union.Add(email2);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.EqualsIgnoreCase(email1, email2))
			{
				union.Add(email1);
			}
			else
			{
				union.Add(email1);
				union.Add(email2);
			}
		}

		private void unionURI(string email1, string email2, ISet union)
		{
			if (email1.IndexOf('@') != -1)
			{
				string text = email1.Substring(email1.IndexOf('@') + 1);
				if (email2.IndexOf('@') != -1)
				{
					if (Platform.EqualsIgnoreCase(email1, email2))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
				}
				else if (Platform.StartsWith(email2, "."))
				{
					if (WithinDomain(text, email2))
					{
						union.Add(email2);
						return;
					}
					union.Add(email1);
					union.Add(email2);
				}
				else if (Platform.EqualsIgnoreCase(text, email2))
				{
					union.Add(email2);
				}
				else
				{
					union.Add(email1);
					union.Add(email2);
				}
			}
			else if (Platform.StartsWith(email1, "."))
			{
				if (email2.IndexOf('@') != -1)
				{
					string testDomain = email2.Substring(email1.IndexOf('@') + 1);
					if (WithinDomain(testDomain, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
				}
				else if (Platform.StartsWith(email2, "."))
				{
					if (WithinDomain(email1, email2) || Platform.EqualsIgnoreCase(email1, email2))
					{
						union.Add(email2);
						return;
					}
					if (WithinDomain(email2, email1))
					{
						union.Add(email1);
						return;
					}
					union.Add(email1);
					union.Add(email2);
				}
				else if (WithinDomain(email2, email1))
				{
					union.Add(email1);
				}
				else
				{
					union.Add(email1);
					union.Add(email2);
				}
			}
			else if (email2.IndexOf('@') != -1)
			{
				string a = email2.Substring(email1.IndexOf('@') + 1);
				if (Platform.EqualsIgnoreCase(a, email1))
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(email1, email2))
				{
					union.Add(email2);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.EqualsIgnoreCase(email1, email2))
			{
				union.Add(email1);
			}
			else
			{
				union.Add(email1);
				union.Add(email2);
			}
		}

		private ISet intersectDNS(ISet permitted, ISet dnss)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = dnss.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = ExtractNameAsString(((GeneralSubtree)enumerator.Current).Base);
				if (permitted == null)
				{
					if (text != null)
					{
						set.Add(text);
					}
					continue;
				}
				IEnumerator enumerator2 = permitted.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					string text2 = (string)enumerator2.Current;
					if (WithinDomain(text2, text))
					{
						set.Add(text2);
					}
					else if (WithinDomain(text, text2))
					{
						set.Add(text);
					}
				}
			}
			return set;
		}

		protected ISet unionDNS(ISet excluded, string dns)
		{
			if (excluded.IsEmpty)
			{
				if (dns == null)
				{
					return excluded;
				}
				excluded.Add(dns);
				return excluded;
			}
			ISet set = new HashSet();
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = (string)enumerator.Current;
				if (WithinDomain(text, dns))
				{
					set.Add(dns);
					continue;
				}
				if (WithinDomain(dns, text))
				{
					set.Add(text);
					continue;
				}
				set.Add(text);
				set.Add(dns);
			}
			return set;
		}

		private void intersectEmail(string email1, string email2, ISet intersect)
		{
			if (email1.IndexOf('@') != -1)
			{
				string text = email1.Substring(email1.IndexOf('@') + 1);
				if (email2.IndexOf('@') != -1)
				{
					if (Platform.EqualsIgnoreCase(email1, email2))
					{
						intersect.Add(email1);
					}
				}
				else if (Platform.StartsWith(email2, "."))
				{
					if (WithinDomain(text, email2))
					{
						intersect.Add(email1);
					}
				}
				else if (Platform.EqualsIgnoreCase(text, email2))
				{
					intersect.Add(email1);
				}
			}
			else if (Platform.StartsWith(email1, "."))
			{
				if (email2.IndexOf('@') != -1)
				{
					string testDomain = email2.Substring(email1.IndexOf('@') + 1);
					if (WithinDomain(testDomain, email1))
					{
						intersect.Add(email2);
					}
				}
				else if (Platform.StartsWith(email2, "."))
				{
					if (WithinDomain(email1, email2) || Platform.EqualsIgnoreCase(email1, email2))
					{
						intersect.Add(email1);
					}
					else if (WithinDomain(email2, email1))
					{
						intersect.Add(email2);
					}
				}
				else if (WithinDomain(email2, email1))
				{
					intersect.Add(email2);
				}
			}
			else if (email2.IndexOf('@') != -1)
			{
				string a = email2.Substring(email2.IndexOf('@') + 1);
				if (Platform.EqualsIgnoreCase(a, email1))
				{
					intersect.Add(email2);
				}
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(email1, email2))
				{
					intersect.Add(email1);
				}
			}
			else if (Platform.EqualsIgnoreCase(email1, email2))
			{
				intersect.Add(email1);
			}
		}

		private void checkExcludedURI(ISet excluded, string uri)
		{
			if (excluded.IsEmpty)
			{
				return;
			}
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string constraint = (string)enumerator.Current;
				if (IsUriConstrained(uri, constraint))
				{
					throw new PkixNameConstraintValidatorException("URI is from an excluded subtree.");
				}
			}
		}

		private ISet intersectURI(ISet permitted, ISet uris)
		{
			ISet set = new HashSet();
			IEnumerator enumerator = uris.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = ExtractNameAsString(((GeneralSubtree)enumerator.Current).Base);
				if (permitted == null)
				{
					if (text != null)
					{
						set.Add(text);
					}
					continue;
				}
				IEnumerator enumerator2 = permitted.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					string email = (string)enumerator2.Current;
					intersectURI(email, text, set);
				}
			}
			return set;
		}

		private ISet unionURI(ISet excluded, string uri)
		{
			if (excluded.IsEmpty)
			{
				if (uri == null)
				{
					return excluded;
				}
				excluded.Add(uri);
				return excluded;
			}
			ISet set = new HashSet();
			IEnumerator enumerator = excluded.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string email = (string)enumerator.Current;
				unionURI(email, uri, set);
			}
			return set;
		}

		private void intersectURI(string email1, string email2, ISet intersect)
		{
			if (email1.IndexOf('@') != -1)
			{
				string text = email1.Substring(email1.IndexOf('@') + 1);
				if (email2.IndexOf('@') != -1)
				{
					if (Platform.EqualsIgnoreCase(email1, email2))
					{
						intersect.Add(email1);
					}
				}
				else if (Platform.StartsWith(email2, "."))
				{
					if (WithinDomain(text, email2))
					{
						intersect.Add(email1);
					}
				}
				else if (Platform.EqualsIgnoreCase(text, email2))
				{
					intersect.Add(email1);
				}
			}
			else if (Platform.StartsWith(email1, "."))
			{
				if (email2.IndexOf('@') != -1)
				{
					string testDomain = email2.Substring(email1.IndexOf('@') + 1);
					if (WithinDomain(testDomain, email1))
					{
						intersect.Add(email2);
					}
				}
				else if (Platform.StartsWith(email2, "."))
				{
					if (WithinDomain(email1, email2) || Platform.EqualsIgnoreCase(email1, email2))
					{
						intersect.Add(email1);
					}
					else if (WithinDomain(email2, email1))
					{
						intersect.Add(email2);
					}
				}
				else if (WithinDomain(email2, email1))
				{
					intersect.Add(email2);
				}
			}
			else if (email2.IndexOf('@') != -1)
			{
				string a = email2.Substring(email2.IndexOf('@') + 1);
				if (Platform.EqualsIgnoreCase(a, email1))
				{
					intersect.Add(email2);
				}
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(email1, email2))
				{
					intersect.Add(email1);
				}
			}
			else if (Platform.EqualsIgnoreCase(email1, email2))
			{
				intersect.Add(email1);
			}
		}

		private void CheckPermittedURI(ISet permitted, string uri)
		{
			if (permitted == null)
			{
				return;
			}
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string constraint = (string)enumerator.Current;
				if (IsUriConstrained(uri, constraint))
				{
					return;
				}
			}
			if (uri.Length == 0 && permitted.Count == 0)
			{
				return;
			}
			throw new PkixNameConstraintValidatorException("URI is not from a permitted subtree.");
		}

		private bool IsUriConstrained(string uri, string constraint)
		{
			string text = ExtractHostFromURL(uri);
			if (!Platform.StartsWith(constraint, "."))
			{
				if (Platform.EqualsIgnoreCase(text, constraint))
				{
					return true;
				}
			}
			else if (WithinDomain(text, constraint))
			{
				return true;
			}
			return false;
		}

		private static string ExtractHostFromURL(string url)
		{
			string text = url.Substring(url.IndexOf(':') + 1);
			int num = Platform.IndexOf(text, "//");
			if (num != -1)
			{
				text = text.Substring(num + 2);
			}
			if (text.LastIndexOf(':') != -1)
			{
				text = text.Substring(0, text.LastIndexOf(':'));
			}
			text = text.Substring(text.IndexOf(':') + 1);
			text = text.Substring(text.IndexOf('@') + 1);
			if (text.IndexOf('/') != -1)
			{
				text = text.Substring(0, text.IndexOf('/'));
			}
			return text;
		}

		public void checkPermitted(GeneralName name)
		{
			switch (name.TagNo)
			{
			case 1:
				CheckPermittedEmail(permittedSubtreesEmail, ExtractNameAsString(name));
				break;
			case 2:
				CheckPermittedDNS(permittedSubtreesDNS, DerIA5String.GetInstance(name.Name).GetString());
				break;
			case 4:
				CheckPermittedDN(Asn1Sequence.GetInstance(name.Name.ToAsn1Object()));
				break;
			case 6:
				CheckPermittedURI(permittedSubtreesURI, DerIA5String.GetInstance(name.Name).GetString());
				break;
			case 7:
			{
				byte[] octets = Asn1OctetString.GetInstance(name.Name).GetOctets();
				CheckPermittedIP(permittedSubtreesIP, octets);
				break;
			}
			case 3:
			case 5:
				break;
			}
		}

		public void checkExcluded(GeneralName name)
		{
			switch (name.TagNo)
			{
			case 1:
				CheckExcludedEmail(excludedSubtreesEmail, ExtractNameAsString(name));
				break;
			case 2:
				checkExcludedDNS(excludedSubtreesDNS, DerIA5String.GetInstance(name.Name).GetString());
				break;
			case 4:
				CheckExcludedDN(Asn1Sequence.GetInstance(name.Name.ToAsn1Object()));
				break;
			case 6:
				checkExcludedURI(excludedSubtreesURI, DerIA5String.GetInstance(name.Name).GetString());
				break;
			case 7:
			{
				byte[] octets = Asn1OctetString.GetInstance(name.Name).GetOctets();
				checkExcludedIP(excludedSubtreesIP, octets);
				break;
			}
			case 3:
			case 5:
				break;
			}
		}

		public void IntersectPermittedSubtree(Asn1Sequence permitted)
		{
			IDictionary dictionary = Platform.CreateHashtable();
			IEnumerator enumerator = permitted.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GeneralSubtree instance = GeneralSubtree.GetInstance(enumerator.Current);
				int tagNo = instance.Base.TagNo;
				if (dictionary[tagNo] == null)
				{
					dictionary[tagNo] = new HashSet();
				}
				((ISet)dictionary[tagNo]).Add(instance);
			}
			IEnumerator enumerator2 = dictionary.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator2.Current;
				switch ((int)dictionaryEntry.Key)
				{
				case 1:
					permittedSubtreesEmail = IntersectEmail(permittedSubtreesEmail, (ISet)dictionaryEntry.Value);
					break;
				case 2:
					permittedSubtreesDNS = intersectDNS(permittedSubtreesDNS, (ISet)dictionaryEntry.Value);
					break;
				case 4:
					permittedSubtreesDN = IntersectDN(permittedSubtreesDN, (ISet)dictionaryEntry.Value);
					break;
				case 6:
					permittedSubtreesURI = intersectURI(permittedSubtreesURI, (ISet)dictionaryEntry.Value);
					break;
				case 7:
					permittedSubtreesIP = IntersectIP(permittedSubtreesIP, (ISet)dictionaryEntry.Value);
					break;
				}
			}
		}

		private string ExtractNameAsString(GeneralName name)
		{
			return DerIA5String.GetInstance(name.Name).GetString();
		}

		public void IntersectEmptyPermittedSubtree(int nameType)
		{
			switch (nameType)
			{
			case 1:
				permittedSubtreesEmail = new HashSet();
				break;
			case 2:
				permittedSubtreesDNS = new HashSet();
				break;
			case 4:
				permittedSubtreesDN = new HashSet();
				break;
			case 6:
				permittedSubtreesURI = new HashSet();
				break;
			case 7:
				permittedSubtreesIP = new HashSet();
				break;
			case 3:
			case 5:
				break;
			}
		}

		public void AddExcludedSubtree(GeneralSubtree subtree)
		{
			GeneralName @base = subtree.Base;
			switch (@base.TagNo)
			{
			case 1:
				excludedSubtreesEmail = UnionEmail(excludedSubtreesEmail, ExtractNameAsString(@base));
				break;
			case 2:
				excludedSubtreesDNS = unionDNS(excludedSubtreesDNS, ExtractNameAsString(@base));
				break;
			case 4:
				excludedSubtreesDN = UnionDN(excludedSubtreesDN, (Asn1Sequence)@base.Name.ToAsn1Object());
				break;
			case 6:
				excludedSubtreesURI = unionURI(excludedSubtreesURI, ExtractNameAsString(@base));
				break;
			case 7:
				excludedSubtreesIP = UnionIP(excludedSubtreesIP, Asn1OctetString.GetInstance(@base.Name).GetOctets());
				break;
			case 3:
			case 5:
				break;
			}
		}

		private static byte[] Max(byte[] ip1, byte[] ip2)
		{
			for (int i = 0; i < ip1.Length; i++)
			{
				if ((ip1[i] & 0xFFFF) > (ip2[i] & 0xFFFF))
				{
					return ip1;
				}
			}
			return ip2;
		}

		private static byte[] Min(byte[] ip1, byte[] ip2)
		{
			for (int i = 0; i < ip1.Length; i++)
			{
				if ((ip1[i] & 0xFFFF) < (ip2[i] & 0xFFFF))
				{
					return ip1;
				}
			}
			return ip2;
		}

		private static int CompareTo(byte[] ip1, byte[] ip2)
		{
			if (Arrays.AreEqual(ip1, ip2))
			{
				return 0;
			}
			if (Arrays.AreEqual(Max(ip1, ip2), ip1))
			{
				return 1;
			}
			return -1;
		}

		private static byte[] Or(byte[] ip1, byte[] ip2)
		{
			byte[] array = new byte[ip1.Length];
			for (int i = 0; i < ip1.Length; i++)
			{
				array[i] = (byte)(ip1[i] | ip2[i]);
			}
			return array;
		}

		[Obsolete("Use GetHashCode instead")]
		public int HashCode()
		{
			return GetHashCode();
		}

		public override int GetHashCode()
		{
			return HashCollection(excludedSubtreesDN) + HashCollection(excludedSubtreesDNS) + HashCollection(excludedSubtreesEmail) + HashCollection(excludedSubtreesIP) + HashCollection(excludedSubtreesURI) + HashCollection(permittedSubtreesDN) + HashCollection(permittedSubtreesDNS) + HashCollection(permittedSubtreesEmail) + HashCollection(permittedSubtreesIP) + HashCollection(permittedSubtreesURI);
		}

		private int HashCollection(ICollection coll)
		{
			if (coll == null)
			{
				return 0;
			}
			int num = 0;
			IEnumerator enumerator = coll.GetEnumerator();
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				num = ((!(current is byte[])) ? (num + current.GetHashCode()) : (num + Arrays.GetHashCode((byte[])current)));
			}
			return num;
		}

		public override bool Equals(object o)
		{
			if (!(o is PkixNameConstraintValidator))
			{
				return false;
			}
			PkixNameConstraintValidator pkixNameConstraintValidator = (PkixNameConstraintValidator)o;
			if (CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesDN, excludedSubtreesDN) && CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesDNS, excludedSubtreesDNS) && CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesEmail, excludedSubtreesEmail) && CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesIP, excludedSubtreesIP) && CollectionsAreEqual(pkixNameConstraintValidator.excludedSubtreesURI, excludedSubtreesURI) && CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesDN, permittedSubtreesDN) && CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesDNS, permittedSubtreesDNS) && CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesEmail, permittedSubtreesEmail) && CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesIP, permittedSubtreesIP))
			{
				return CollectionsAreEqual(pkixNameConstraintValidator.permittedSubtreesURI, permittedSubtreesURI);
			}
			return false;
		}

		private bool CollectionsAreEqual(ICollection coll1, ICollection coll2)
		{
			if (coll1 == coll2)
			{
				return true;
			}
			if (coll1 == null || coll2 == null)
			{
				return false;
			}
			if (coll1.Count != coll2.Count)
			{
				return false;
			}
			IEnumerator enumerator = coll1.GetEnumerator();
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				IEnumerator enumerator2 = coll2.GetEnumerator();
				bool flag = false;
				while (enumerator2.MoveNext())
				{
					object current2 = enumerator2.Current;
					if (SpecialEquals(current, current2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private bool SpecialEquals(object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			if (o1 is byte[] && o2 is byte[])
			{
				return Arrays.AreEqual((byte[])o1, (byte[])o2);
			}
			return o1.Equals(o2);
		}

		private string StringifyIP(byte[] ip)
		{
			string text = "";
			for (int i = 0; i < ip.Length / 2; i++)
			{
				text = text + (ip[i] & 0xFF) + ".";
			}
			text = text.Substring(0, text.Length - 1);
			text += "/";
			for (int j = ip.Length / 2; j < ip.Length; j++)
			{
				text = text + (ip[j] & 0xFF) + ".";
			}
			return text.Substring(0, text.Length - 1);
		}

		private string StringifyIPCollection(ISet ips)
		{
			string text = "";
			text += "[";
			IEnumerator enumerator = ips.GetEnumerator();
			while (enumerator.MoveNext())
			{
				text = text + StringifyIP((byte[])enumerator.Current) + ",";
			}
			if (text.Length > 1)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text + "]";
		}

		public override string ToString()
		{
			string text = "";
			text += "permitted:\n";
			if (permittedSubtreesDN != null)
			{
				text += "DN:\n";
				text = text + permittedSubtreesDN.ToString() + "\n";
			}
			if (permittedSubtreesDNS != null)
			{
				text += "DNS:\n";
				text = text + permittedSubtreesDNS.ToString() + "\n";
			}
			if (permittedSubtreesEmail != null)
			{
				text += "Email:\n";
				text = text + permittedSubtreesEmail.ToString() + "\n";
			}
			if (permittedSubtreesURI != null)
			{
				text += "URI:\n";
				text = text + permittedSubtreesURI.ToString() + "\n";
			}
			if (permittedSubtreesIP != null)
			{
				text += "IP:\n";
				text = text + StringifyIPCollection(permittedSubtreesIP) + "\n";
			}
			text += "excluded:\n";
			if (!excludedSubtreesDN.IsEmpty)
			{
				text += "DN:\n";
				text = text + excludedSubtreesDN.ToString() + "\n";
			}
			if (!excludedSubtreesDNS.IsEmpty)
			{
				text += "DNS:\n";
				text = text + excludedSubtreesDNS.ToString() + "\n";
			}
			if (!excludedSubtreesEmail.IsEmpty)
			{
				text += "Email:\n";
				text = text + excludedSubtreesEmail.ToString() + "\n";
			}
			if (!excludedSubtreesURI.IsEmpty)
			{
				text += "URI:\n";
				text = text + excludedSubtreesURI.ToString() + "\n";
			}
			if (!excludedSubtreesIP.IsEmpty)
			{
				text += "IP:\n";
				text = text + StringifyIPCollection(excludedSubtreesIP) + "\n";
			}
			return text;
		}
	}
}
