using System;

namespace EasyMC
{
	internal class Config
	{
		public static string VERSION;

		public static string API_URL;

		static Config()
		{
			Config.VERSION = "1.2.3";
			Config.API_URL = "https://api.easymc.io/v1";
		}

		public Config()
		{
		}
	}
}