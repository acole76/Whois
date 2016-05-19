using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Whois
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("USAGE: Whois domain_name");
				return;
			}

			string Domain_Name = args[0];
			
			string str = Whois.lookup(Domain_Name, Whois.RecordType.domain);
			string needle = "UTC <<<";
			int pos = str.IndexOf(needle);

			if (pos > 0)
			{
				Console.WriteLine(str.Substring(0, pos + needle.Length));
			}
			else
			{
				Console.WriteLine(str);
			}
		}
	}

	/// <summary>
	/// A class to lookup whois information.
	/// </summary>
	class Whois
	{
		public enum RecordType { domain, nameserver, registrar };
		/// <summary>
		/// retrieves whois information
		/// </summary>
		/// <param name="domainname">The registrar or domain or name server whose whois information to be retrieved</param>
		/// <param name="recordType">The type of record i.e a domain, nameserver or a registrar</param>
		/// <returns>The string containg the whois information</returns>
		public static string lookup(string domainname, RecordType recordType)
		{
			string Who_Is_Server = "whois.geektools.com";
			List<string> res = Raw_Lookup(domainname, recordType, Who_Is_Server);
			string result = "";
			foreach (string st in res)
			{
				result += st + "\n";
			}
			return result;
		}        /// <summary>
		/// retrieves whois information
		/// </summary>
		/// <param name="domainname">The registrar or domain or name server whose whois information to be retrieved</param>
		/// <param name="recordType">The type of record i.e a domain, nameserver or a registrar</param>
		/// <param name="returnlist">use "whois.internic.net" if you dont know whoisservers</param>
		/// <returns>The string list containg the whois information</returns>
		public static List<string> Raw_Lookup(string domainname, RecordType recordType, string Whois_Server_Address)
		{
			List<string> result = new List<string>();
			try
			{
				TcpClient tcp = new TcpClient();
				tcp.Connect(Whois_Server_Address, 43);
				string strDomain = recordType.ToString() + " " + domainname + "\r\n";
				byte[] bytDomain = Encoding.ASCII.GetBytes(strDomain.ToCharArray());
				Stream s = tcp.GetStream();
				s.Write(bytDomain, 0, strDomain.Length);
				StreamReader sr = new StreamReader(tcp.GetStream(), Encoding.ASCII);
				string strLine = "";
				
				while (null != (strLine = sr.ReadLine()))
				{
					result.Add(strLine);
				}
				tcp.Close();
			}
			catch(Exception e)
			{
				result.Add(e.Message);
			}

			return result;
		}
	}
}