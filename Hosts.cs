using System;
using System.Collections;
using System.IO;

namespace EasyMC
{
	internal class Hosts
	{
		private static string HOSTS_FILE;

		private static string AUTHSERVER;

		private static string SESSIONSERVER;

		static Hosts()
		{
			Hosts.HOSTS_FILE = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.System), "\\drivers\\etc\\hosts");
			Hosts.AUTHSERVER = "authserver.mojang.com";
			Hosts.SESSIONSERVER = "sessionserver.mojang.com";
		}

		public Hosts()
		{
		}

		public static bool hasHosts()
		{
			bool flag;
			try
			{
				File.SetAttributes(Hosts.HOSTS_FILE, FileAttributes.Normal);
				StreamReader streamReader = new StreamReader(Hosts.HOSTS_FILE);
				bool flag1 = false;
				bool flag2 = false;
				while (true)
				{
					string str = streamReader.ReadLine();
					string str1 = str;
					if (str == null)
					{
						break;
					}
					if (str1.Contains(" "))
					{
						string lower = str1.Split(new char[] { ' ' })[1].ToLower();
						if (lower == Hosts.AUTHSERVER)
						{
							flag1 = true;
						}
						if (lower == Hosts.SESSIONSERVER)
						{
							flag2 = true;
						}
					}
				}
				streamReader.Close();
				flag = flag1 & flag2;
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		public static bool matchesHosts(string host)
		{
			bool flag;
			try
			{
				File.SetAttributes(Hosts.HOSTS_FILE, FileAttributes.Normal);
				StreamReader streamReader = new StreamReader(Hosts.HOSTS_FILE);
				bool flag1 = false;
				bool flag2 = false;
				while (true)
				{
					string str = streamReader.ReadLine();
					string str1 = str;
					if (str == null)
					{
						break;
					}
					if (str1.Contains(" "))
					{
						string lower = str1.Split(new char[] { ' ' })[0].ToLower();
						string lower1 = str1.Split(new char[] { ' ' })[1].ToLower();
						if (lower1 == Hosts.AUTHSERVER && lower == host)
						{
							flag1 = true;
						}
						if (lower1 == Hosts.SESSIONSERVER && lower == host)
						{
							flag2 = true;
						}
					}
				}
				streamReader.Close();
				flag = flag1 & flag2;
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		public static string removeHosts()
		{
			string str;
			ArrayList arrayLists = new ArrayList();
			try
			{
				File.SetAttributes(Hosts.HOSTS_FILE, FileAttributes.Normal);
				StreamReader streamReader = new StreamReader(Hosts.HOSTS_FILE);
				while (true)
				{
					string str1 = streamReader.ReadLine();
					string str2 = str1;
					if (str1 == null)
					{
						break;
					}
					if (str2.Contains(" "))
					{
						string lower = str2.Split(new char[] { ' ' })[1].ToLower();
						if (lower != Hosts.AUTHSERVER && lower != Hosts.SESSIONSERVER)
						{
							arrayLists.Add(str2);
						}
					}
					else if (str2.Length > 0)
					{
						arrayLists.Add(str2);
					}
				}
				streamReader.Close();
			}
			catch (Exception exception)
			{
				str = string.Concat("Could not retrieve hosts content!\n\n", exception.Message);
				return str;
			}
			try
			{
				File.SetAttributes(Hosts.HOSTS_FILE, FileAttributes.Normal);
				using (StreamWriter streamWriter = new StreamWriter(Hosts.HOSTS_FILE))
				{
					foreach (string arrayList in arrayLists)
					{
						streamWriter.WriteLine(arrayList);
					}
					streamWriter.Flush();
					streamWriter.Close();
				}
				str = null;
			}
			catch (UnauthorizedAccessException unauthorizedAccessException)
			{
				str = string.Concat("Could not modify the hosts file, maybe your antivirus is preventing the authenticator from accessing it.\nTry to disabling your antivirus.\n\n", unauthorizedAccessException.Message);
			}
			catch (DirectoryNotFoundException directoryNotFoundException)
			{
				str = string.Concat("Could not find the hosts file directory, please check if \"C:\\Windows\\System32\\drivers\\etc\" exists.\n\n", directoryNotFoundException.Message);
			}
			catch (Exception exception1)
			{
				str = string.Concat("Could not modify the hosts file, maybe your antivirus is preventing the authenticator from accessing it.\nTry to disabling your antivirus.\n\n", exception1.Message);
			}
			return str;
		}

		public static string writeHosts(string ip)
		{
			string str;
			try
			{
				File.SetAttributes(Hosts.HOSTS_FILE, FileAttributes.Normal);
				using (StreamWriter streamWriter = new StreamWriter(Hosts.HOSTS_FILE, true))
				{
					streamWriter.WriteLine();
					streamWriter.WriteLine(string.Concat(ip, " ", Hosts.AUTHSERVER));
					streamWriter.WriteLine(string.Concat(ip, " ", Hosts.SESSIONSERVER));
					streamWriter.Flush();
					streamWriter.Close();
				}
				str = null;
			}
			catch (UnauthorizedAccessException unauthorizedAccessException)
			{
				str = string.Concat("Could not modify hosts file.\nTry to disable your antivirus.\n\n", unauthorizedAccessException.Message);
			}
			catch (DirectoryNotFoundException directoryNotFoundException)
			{
				str = string.Concat("Could not find the hosts file directory, please check if \"C:\\Windows\\System32\\drivers\\etc\" exists.\n\n", directoryNotFoundException.Message);
			}
			catch (Exception exception)
			{
				str = string.Concat("Could not modify hosts file.\nTry to disable your antivirus.\n\n", exception.Message);
			}
			return str;
		}
	}
}