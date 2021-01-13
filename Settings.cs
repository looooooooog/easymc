using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EasyMC
{
	internal class Settings
	{
		private string apiUrl;

		public string version;

		public string authServer;

		public string updateUrl;

		public string updateFile;

		public string headUrl;

		public string renderUrl;

		public Settings(string apiUrl)
		{
			this.apiUrl = apiUrl;
		}

		public string fetch()
		{
			dynamic obj = Api.FetchClientSettings();
			if (obj is string)
			{
				return (string)obj;
			}
			Dictionary<string, dynamic> strs = (Dictionary<string, object>)obj;
			if (strs.ContainsKey("version"))
			{
				this.version = (string)strs["version"];
			}
			if (strs.ContainsKey("authServer"))
			{
				this.authServer = (string)strs["authServer"];
			}
			if (strs.ContainsKey("updateUrl"))
			{
				this.updateUrl = (string)strs["updateUrl"];
			}
			if (strs.ContainsKey("updateFile"))
			{
				this.updateFile = (string)strs["updateFile"];
			}
			if (strs.ContainsKey("headUrl"))
			{
				this.headUrl = (string)strs["headUrl"];
			}
			if (strs.ContainsKey("renderUrl"))
			{
				this.renderUrl = (string)strs["renderUrl"];
			}
			return null;
		}
	}
}