using EasyMC.Properties;
using System;
using System.IO;

namespace EasyMC
{
	internal class Certs
	{
		private static string appData;

		private static string easymcDir;

		private static string authserverPath;

		private static string sessionserverPath;

		static Certs()
		{
			Certs.appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			Certs.easymcDir = Path.Combine(Certs.appData, "easymc");
			Certs.authserverPath = Path.Combine(Certs.easymcDir, "authserver.mojang.com.crt");
			Certs.sessionserverPath = Path.Combine(Certs.easymcDir, "sessionserver.mojang.com.crt");
		}

		public Certs()
		{
		}

		public static void exportCerts()
		{
			if (!Directory.Exists(Certs.easymcDir))
			{
				Directory.CreateDirectory(Certs.easymcDir);
			}
			if (File.Exists(Certs.authserverPath))
			{
				File.Delete(Certs.authserverPath);
			}
			if (File.Exists(Certs.sessionserverPath))
			{
				File.Delete(Certs.sessionserverPath);
			}
			File.WriteAllBytes(Certs.authserverPath, Resources.authserver_mojang_com);
			File.WriteAllBytes(Certs.sessionserverPath, Resources.sessionserver_mojang_com);
		}

		private static Tuple<string, string> getKeytoolCertStore(string runtime)
		{
			return Tuple.Create<string, string>(Path.Combine(runtime, "bin", "keytool.exe"), Path.Combine(runtime, "lib", "security", "cacerts"));
		}

		public static string importToRuntime(string runtimeDir)
		{
			Tuple<string, string> keytoolCertStore = Certs.getKeytoolCertStore(runtimeDir);
			ProcessRunner processRunner = new ProcessRunner(keytoolCertStore.Item1, string.Concat(new string[] { "-storepass \"changeit\" -noprompt -import -alias authserver.mojang.com -file \"", Certs.authserverPath, "\" -keystore \"", keytoolCertStore.Item2, "\"" }));
			processRunner.run(250);
			if (processRunner.ExitCode != 0 && !processRunner.Output.ToLower().Contains("<authserver.mojang.com>"))
			{
				return string.Concat("Out: ", processRunner.Output, "\nErr: ", processRunner.Error);
			}
			ProcessRunner processRunner1 = new ProcessRunner(keytoolCertStore.Item1, string.Concat(new string[] { "-storepass \"changeit\" -noprompt -import -alias sessionserver.mojang.com -file \"", Certs.sessionserverPath, "\" -keystore \"", keytoolCertStore.Item2, "\"" }));
			processRunner1.run(250);
			if (processRunner1.ExitCode == 0 || processRunner1.Output.ToLower().Contains("<sessionserver.mojang.com>"))
			{
				return null;
			}
			return string.Concat("Out: ", processRunner1.Output, "\nErr: ", processRunner1.Error);
		}

		public static string importToSystem()
		{
			ProcessRunner processRunner = new ProcessRunner("certutil", string.Concat("-addstore \"Root\" \"", Certs.authserverPath, "\""));
			processRunner.run(250);
			if (processRunner.ExitCode == 0)
			{
				return null;
			}
			return string.Concat("Out: ", processRunner.Output, "\nErr: ", processRunner.Error);
		}
	}
}