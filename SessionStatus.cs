using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EasyMC
{
	internal class SessionStatus
	{
		private static Dictionary<string, bool> statusCache;

		static SessionStatus()
		{
			SessionStatus.statusCache = new Dictionary<string, bool>();
		}

		public SessionStatus()
		{
		}

		public static Dictionary<string, bool> getStatus(List<string> sessions)
		{
			Dictionary<string, bool> strs = new Dictionary<string, bool>();
			List<string> strs1 = new List<string>();
			foreach (string session in sessions)
			{
				if (!SessionStatus.statusCache.ContainsKey(session))
				{
					strs1.Add(session);
				}
				else
				{
					strs[session] = SessionStatus.statusCache[session];
				}
			}
			if (strs1.Count > 0)
			{
				dynamic obj = Api.SessionStatus(strs1);
				if (obj is string)
				{
					return strs;
				}
				foreach (KeyValuePair<string, dynamic> item in (Dictionary<string, object>)obj)
				{
					Dictionary<string, dynamic> value = (Dictionary<string, object>)item.Value;
					if (!value.ContainsKey("expired") || !(value["expired"] is bool))
					{
						continue;
					}
					strs[item.Key] = (bool)value["expired"];
					SessionStatus.statusCache[item.Key] = (bool)value["expired"];
				}
			}
			return strs;
		}
	}
}