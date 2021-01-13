using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace EasyMC
{
	internal class Sessions
	{
		private static string EASYMC_DIR;

		private static string SESSIONS_FILE;

		static Sessions()
		{
			Sessions.EASYMC_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "easymc");
			Sessions.SESSIONS_FILE = Path.Combine(Sessions.EASYMC_DIR, "sessions.json");
		}

		public Sessions()
		{
		}

		public static void addSession(string session, string mcName, string uuid, string userId, string token)
		{
			List<Session> sessions = Sessions.getSessions();
			foreach (Session session1 in new List<Session>(sessions))
			{
				if (session1.uuid != uuid)
				{
					continue;
				}
				sessions.Remove(session1);
			}
			sessions.Add(new Session(session, mcName, uuid, userId, token));
			Sessions.WriteSessionsContent(sessions);
		}

		public static List<Session> getSessions()
		{
			string str = Sessions.ReadSessionsContent();
			if (str == null)
			{
				return new List<Session>();
			}
			Dictionary<string, object> strs = (new JavaScriptSerializer()).Deserialize<Dictionary<string, object>>(str);
			List<Session> sessions = new List<Session>();
			foreach (KeyValuePair<string, dynamic> keyValuePair in strs)
			{
				Dictionary<string, dynamic> value = (Dictionary<string, object>)keyValuePair.Value;
				if (!value.ContainsKey("mcName") || !value.ContainsKey("uuid") || !value.ContainsKey("userId") || !value.ContainsKey("token"))
				{
					continue;
				}
				sessions.Add(new Session(keyValuePair.Key, value["mcName"], value["uuid"], value["userId"], value["token"]));
			}
			return sessions;
		}

		public static void loadSessions()
		{
			MessageBox.Show(Sessions.ReadSessionsContent());
		}

		private static string ReadSessionsContent()
		{
			string str;
			try
			{
				if (!Directory.Exists(Sessions.EASYMC_DIR))
				{
					Directory.CreateDirectory(Sessions.EASYMC_DIR);
				}
				if (!File.Exists(Sessions.SESSIONS_FILE))
				{
					File.WriteAllText(Sessions.SESSIONS_FILE, "{}");
				}
				str = File.ReadAllText(Sessions.SESSIONS_FILE);
			}
			catch (Exception exception)
			{
				str = null;
			}
			return str;
		}

		public static void removeSession(string session)
		{
			List<Session> sessions = Sessions.getSessions();
			Session session1 = sessions.Single<Session>((Session sess) => sess.session == session);
			if (session1 != null)
			{
				sessions.Remove(session1);
				Sessions.WriteSessionsContent(sessions);
			}
		}

		private static void WriteSessionsContent(List<Session> sessions)
		{
			try
			{
				if (!Directory.Exists(Sessions.EASYMC_DIR))
				{
					Directory.CreateDirectory(Sessions.EASYMC_DIR);
				}
				Dictionary<string, object> strs = new Dictionary<string, object>();
				foreach (Session session in sessions)
				{
					Dictionary<string, string> strs1 = new Dictionary<string, string>()
					{
						{ "mcName", session.mcName },
						{ "uuid", session.uuid },
						{ "userId", session.userId },
						{ "token", session.token }
					};
					strs.Add(session.session, strs1);
				}
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				File.WriteAllText(Sessions.SESSIONS_FILE, javaScriptSerializer.Serialize(strs));
			}
			catch (Exception exception)
			{
			}
		}
	}
}