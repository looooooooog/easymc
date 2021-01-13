using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace EasyMC
{
	internal class Launcher
	{
		private readonly static string DEFAULT_PATH;

		private readonly static string OLD_PATH;

		static Launcher()
		{
			Launcher.DEFAULT_PATH = Path.Combine(Launcher.ProgramFilesx86(), "Minecraft Launcher", "runtime");
			Launcher.OLD_PATH = Path.Combine(Launcher.ProgramFilesx86(), "Minecraft", "runtime");
		}

		public Launcher()
		{
		}

		public static string getDefaultRuntimeDir()
		{
			if (Directory.Exists(Launcher.DEFAULT_PATH))
			{
				return Launcher.DEFAULT_PATH;
			}
			if (Directory.Exists(Launcher.OLD_PATH))
			{
				return Launcher.OLD_PATH;
			}
			return null;
		}

		public static ArrayList getJres(string runtimeDir)
		{
			ArrayList arrayLists = new ArrayList();
			string[] directories = Directory.GetDirectories(runtimeDir);
			for (int i = 0; i < (int)directories.Length; i++)
			{
				string str = directories[i];
				if (!Launcher.isValidJre(str))
				{
					string[] strArrays = Directory.GetDirectories(str);
					for (int j = 0; j < (int)strArrays.Length; j++)
					{
						string str1 = strArrays[j];
						if (Launcher.isValidJre(str1))
						{
							arrayLists.Add(str1);
						}
					}
				}
				else
				{
					arrayLists.Add(str);
				}
			}
			return arrayLists;
		}

		public static string getRuntimeDir()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Title = "Select your Minecraft",
				Filter = "Minecraft Launcher|*.exe",
				FilterIndex = 1
			};
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return "abort";
			}
			string str = openFileDialog.FileName.ToString();
			string directoryName = Path.GetDirectoryName(str);
			Path.GetFileName(str);
			string[] directories = Directory.GetDirectories(directoryName);
			for (int i = 0; i < (int)directories.Length; i++)
			{
				string str1 = directories[i];
				if (Path.GetFileName(str1) == "runtime")
				{
					return str1;
				}
			}
			return null;
		}

		private static bool isValidJre(string path)
		{
			string str = Path.Combine(path, "bin", "keytool.exe");
			string str1 = Path.Combine(path, "lib", "security", "cacerts");
			if (!File.Exists(str))
			{
				return false;
			}
			return File.Exists(str1);
		}

		private static string ProgramFilesx86()
		{
			if (8 != IntPtr.Size && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))
			{
				return Environment.GetEnvironmentVariable("ProgramFiles");
			}
			return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
		}
	}
}