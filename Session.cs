using System;

namespace EasyMC
{
	internal class Session
	{
		public string session;

		public string mcName;

		public string uuid;

		public string userId;

		public string token;

		public Session(string session, string mcName, string uuid, string userId, string token)
		{
			this.session = session;
			this.mcName = mcName;
			this.uuid = uuid;
			this.userId = userId;
			this.token = token;
		}
	}
}