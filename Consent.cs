using System;
using System.IO;

namespace EasyMC
{
	internal class Consent
	{
		private static string EASYMC_DIR;

		private static string CONSENT_FILE;

		static Consent()
		{
			Consent.EASYMC_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "easymc");
			Consent.CONSENT_FILE = Path.Combine(Consent.EASYMC_DIR, "consent.json");
		}

		public Consent()
		{
		}

		public static bool HasConsent()
		{
			bool flag;
			try
			{
				if (!Directory.Exists(Consent.EASYMC_DIR))
				{
					Directory.CreateDirectory(Consent.EASYMC_DIR);
				}
				flag = (File.Exists(Consent.CONSENT_FILE) ? File.ReadAllText(Consent.CONSENT_FILE) == "true" : false);
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		public static void SaveConsent()
		{
			try
			{
				if (!Directory.Exists(Consent.EASYMC_DIR))
				{
					Directory.CreateDirectory(Consent.EASYMC_DIR);
				}
				File.WriteAllText(Consent.CONSENT_FILE, "true");
			}
			catch (Exception exception)
			{
			}
		}
	}
}