using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Script.Serialization;

namespace EasyMC
{
	internal class Api
	{
		public Api()
		{
		}

		public static dynamic FetchClientSettings()
		{
			return Api.Perform("/client/settings", null);
		}

		private static dynamic Perform(string endpoint, string postBody = null)
		{
			object message;
			try
			{
				HttpWebRequest length = (HttpWebRequest)WebRequest.Create(string.Concat(Config.API_URL, endpoint));
				length.Timeout = 8000;
				if (postBody != null)
				{
					byte[] bytes = Encoding.ASCII.GetBytes(postBody);
					length.Method = "POST";
					length.ContentType = "application/json";
					length.ContentLength = (long)((int)bytes.Length);
					using (Stream requestStream = length.GetRequestStream())
					{
						requestStream.Write(bytes, 0, (int)bytes.Length);
					}
				}
				StreamReader streamReader = new StreamReader(((HttpWebResponse)length.GetResponse()).GetResponseStream());
				string end = streamReader.ReadToEnd();
				streamReader.Close();
				dynamic json = Api.strToJson(end);
				if (json != (dynamic)null)
				{
					message = json;
				}
				else
				{
					message = "Could not perform request! 2";
				}
			}
			catch (WebException webException1)
			{
				WebException webException = webException1;
				if (webException.Response != null)
				{
					dynamic obj = Api.strToJson((new StreamReader(webException.Response.GetResponseStream())).ReadToEnd());
					if (obj != (dynamic)null)
					{
						Dictionary<string, dynamic> strs = (Dictionary<string, object>)obj;
						message = (!strs.ContainsKey("error") ? webException.Message : (string)strs["error"]);
					}
					else
					{
						message = "Could not perform request! 1";
					}
				}
				else
				{
					message = webException.Message;
				}
			}
			catch (Exception exception)
			{
				message = exception.Message;
			}
			return message;
		}

		public static dynamic RedeemToken(string token)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			Dictionary<string, object> strs = new Dictionary<string, object>();
			strs["token"] = token;
			return Api.Perform("/token/redeem", javaScriptSerializer.Serialize(strs));
		}

		public static dynamic SessionStatus(List<string> sessions)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			Dictionary<string, object> strs = new Dictionary<string, object>();
			strs["sessions"] = new List<string>(sessions);
			return Api.Perform("/session/status", javaScriptSerializer.Serialize(strs));
		}

		private static dynamic strToJson(string content)
		{
			object obj;
			try
			{
				obj = (new JavaScriptSerializer()).Deserialize<Dictionary<string, object>>(content);
			}
			catch (Exception exception)
			{
				obj = null;
			}
			return obj;
		}
	}
}