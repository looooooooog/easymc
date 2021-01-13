using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace EasyMC
{
	internal class Accounts
	{
		private static string ACCOUNTS_FILE;

		static Accounts()
		{
			Accounts.ACCOUNTS_FILE = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft/launcher_accounts.json");
		}

		public Accounts()
		{
		}

		private static void CreateProfilesFile()
		{
			File.WriteAllText(Accounts.ACCOUNTS_FILE, "{}");
		}

		private static string getActiveAccount(Dictionary<string, dynamic> root)
		{
			string item;
			try
			{
				if (root.ContainsKey("activeAccountLocalId"))
				{
					item = (string)root["activeAccountLocalId"];
				}
				else
				{
					item = null;
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
				return null;
			}
			return item;
		}

		public static string getActiveProfile()
		{
			string item;
			try
			{
				if (!File.Exists(Accounts.ACCOUNTS_FILE))
				{
					Accounts.CreateProfilesFile();
				}
				Dictionary<string, dynamic> strs = (new JavaScriptSerializer()).Deserialize<Dictionary<string, object>>(Accounts.ReadProfilesContent());
				string activeAccount = Accounts.getActiveAccount(strs);
				if (strs.ContainsKey("accounts"))
				{
					foreach (KeyValuePair<string, dynamic> keyValuePair in (Dictionary<string, object>)strs["accounts"])
					{
						if (keyValuePair.Key != activeAccount)
						{
							continue;
						}
						Dictionary<string, dynamic> value = (Dictionary<string, object>)keyValuePair.Value;
						if (!value.ContainsKey("username") || !value.ContainsKey("accessToken"))
						{
							continue;
						}
						if (!((string)value["username"]).EndsWith("@easymc.io"))
						{
							continue;
						}
						item = (string)value["accessToken"];
						return item;
					}
					item = null;
				}
				else
				{
					item = null;
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
				return null;
			}
			return item;
		}

		private static string ReadProfilesContent()
		{
			return File.ReadAllText(Accounts.ACCOUNTS_FILE);
		}

		public static void removeActiveProfile()
		{
			try
			{
				if (!File.Exists(Accounts.ACCOUNTS_FILE))
				{
					Accounts.CreateProfilesFile();
				}
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				Dictionary<string, dynamic> strs = javaScriptSerializer.Deserialize<Dictionary<string, object>>(Accounts.ReadProfilesContent());
				if (strs.ContainsKey("accounts"))
				{
					Dictionary<string, object> item = (Dictionary<string, object>)strs["accounts"];
					List<string> strs1 = new List<string>();
					foreach (KeyValuePair<string, dynamic> keyValuePair in item)
					{
						Dictionary<string, dynamic> value = (Dictionary<string, object>)keyValuePair.Value;
						if (!value.ContainsKey("username"))
						{
							continue;
						}
						if (!((string)value["username"]).EndsWith("@easymc.io"))
						{
							continue;
						}
						strs1.Add(keyValuePair.Key);
					}
					foreach (string str in strs1)
					{
						item.Remove(str);
					}
				}
				strs.Remove("activeAccountLocalId");
				Accounts.WriteProfilesContent(javaScriptSerializer.Serialize(strs));
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}

		public static void setActiveProfile(string session, string mcName, string uuid, string userId)
		{
			try
			{
				string str = uuid.Replace("-", string.Empty);
				if (!File.Exists(Accounts.ACCOUNTS_FILE))
				{
					Accounts.CreateProfilesFile();
				}
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				Dictionary<string, dynamic> strs = javaScriptSerializer.Deserialize<Dictionary<string, object>>(Accounts.ReadProfilesContent());
				if (!strs.ContainsKey("accounts"))
				{
					strs["accounts"] = new Dictionary<string, object>();
				}
				Dictionary<string, object> item = (Dictionary<string, object>)strs["accounts"];
				List<string> strs1 = new List<string>();
				foreach (KeyValuePair<string, dynamic> keyValuePair in item)
				{
					Dictionary<string, dynamic> value = (Dictionary<string, object>)keyValuePair.Value;
					if (!value.ContainsKey("username"))
					{
						continue;
					}
					if (!((string)value["username"]).EndsWith("@easymc.io"))
					{
						continue;
					}
					strs1.Add(keyValuePair.Key);
				}
				foreach (string str1 in strs1)
				{
					item.Remove(str1);
				}
				Dictionary<string, object> strs2 = new Dictionary<string, object>();
				strs2["id"] = str;
				strs2["name"] = mcName;
				Dictionary<string, object> strs3 = new Dictionary<string, object>();
				strs3["accessToken"] = session;
				strs3["eligibleForMigration"] = false;
				strs3["hasMultipleProfiles"] = false;
				strs3["legacy"] = false;
				strs3["localId"] = str;
				strs3["minecraftProfile"] = strs2;
				strs3["persistent"] = true;
				strs3["remoteId"] = userId;
				strs3["type"] = "Mojang";
				strs3["userProperties"] = new List<string>();
				strs3["username"] = string.Concat(mcName, "@easymc.io");
				item[str] = strs3;
				strs["activeAccountLocalId"] = str;
				Accounts.WriteProfilesContent(javaScriptSerializer.Serialize(strs));
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}

		private static void WriteProfilesContent(string content)
		{
			File.WriteAllText(Accounts.ACCOUNTS_FILE, content);
		}
	}
}