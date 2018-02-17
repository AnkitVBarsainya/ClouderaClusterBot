using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
namespace ClouderaAdmin
{
	[Serializable]
	public class MethodGlossary
	{

		private static String username = "cloudera";
		private static String password = "Cloudera_123";
		string credentials = "cloudera:Cloudera_123";
		private static String baseURL = "http://cdedge-57885b8f.westeurope.cloudapp.azure.com:7180/api/v18/clusters/cluster1/";

		private static String services = "services/";

		public async Task<String> CurlClouderaManager(IDialogContext context, String serviceName, String operation)
		{
			string strResponseValue = string.Empty;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseURL);

			request.Method = "GET";

			String authHeaer = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
			// request.Headers.Add("Authentication" + " " + authHeaer);


			HttpWebResponse response = null;

			try
			{
				response = (HttpWebResponse)request.GetResponse();


				//Proecess the resppnse stream... (could be JSON, XML or HTML etc..._

				using (Stream responseStream = response.GetResponseStream())
				{
					if (responseStream != null)
					{
						using (StreamReader reader = new StreamReader(responseStream))
						{
							strResponseValue = reader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				strResponseValue = "{\"errorMessages\":[\"" + ex.Message.ToString() + "\"],\"errors\":{}}";
			}
			finally
			{
				if (response != null)
				{
					((IDisposable)response).Dispose();
				}
			}

			return strResponseValue;
			// return null;
		}
		public async Task<String> CurlClouderaManager(IDialogContext context)
		{
			var client = new HttpClient();
			StringBuilder responseStr = new StringBuilder();
			var requestContent = new FormUrlEncodedContent(new[] {
				new KeyValuePair<string, string>("text", "This is a block of text"),
			});
			HttpResponseMessage response;
			String finalURL = baseURL;
			response = await client.PostAsync(finalURL, requestContent);

			HttpContent responseContent = response.Content;
			using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
			{

				responseStr.Append(await reader.ReadToEndAsync());
			}


			return responseStr.ToString();

		}

		public static string LoadHttpPageWithBasicAuthentication(IDialogContext context, String serviceName)
		{
			String finalURL = null;
			if (String.IsNullOrEmpty(serviceName))
			{
				finalURL = baseURL;
			}
			else if (serviceName.Equals("ALL_SERVICES"))
			{
				finalURL = baseURL + services;
			}
			else
			{
				finalURL = baseURL + services + serviceName;
			}
			Uri myUri = new Uri(finalURL);
			WebRequest myWebRequest = HttpWebRequest.Create(myUri);

			HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;

			NetworkCredential myNetworkCredential = new NetworkCredential(username, password);

			CredentialCache myCredentialCache = new CredentialCache();
			myCredentialCache.Add(myUri, "Basic", myNetworkCredential);

			myHttpWebRequest.PreAuthenticate = true;
			myHttpWebRequest.Credentials = myCredentialCache;

			WebResponse myWebResponse = myWebRequest.GetResponse();

			Stream responseStream = myWebResponse.GetResponseStream();

			StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

			string pageContent = myStreamReader.ReadToEnd();
			
			responseStream.Close();

			myWebResponse.Close();

			return pageContent;
		}


		public static async Task<string> LoadHttpPageWithBasicAuthentication(IDialogContext context, String serviceName, String operation)
		{
			// string url = "http://localhost:8080/geoserver/rest/workspaces";
			var
				finalURL = baseURL + services + serviceName + "/commands/" + operation;

			WebRequest request = WebRequest.Create(finalURL);

			// request.ContentType = "application/json";
			request.Method = "POST";

			string authInfo = "cloudera:Cloudera_123";
			request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(authInfo));

			WebResponse myWebResponse = request.GetResponse();
			Stream responseStream = myWebResponse.GetResponseStream();
			StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
			string pageContent = myStreamReader.ReadToEnd();
			responseStream.Close();
			myWebResponse.Close();
			return pageContent;
		}


		public static String GetClusterServiceName(IDialogContext context, String serviceCommonName)
		{
			// Map the service common name to Cluster's version 
			String serviceClusterName = null;
			String allServiceStatus = LoadHttpPageWithBasicAuthentication(context, "ALL_SERVICES");
			JToken allServiceToken = JToken.Parse(allServiceStatus);
			var countServices = allServiceToken.Value<JArray>("items").Count;

			for (int i = 0; i < countServices; i++)
			{
				var tmpServMapping = allServiceToken.Value<JArray>("items")[i].SelectToken("name").ToString();
				if (tmpServMapping.Contains(serviceCommonName.Trim()))
				{
					serviceClusterName = allServiceToken.Value<JArray>("items")[i].SelectToken("name").ToString();
					continue;
				}
			}
			return serviceClusterName;
		}
	}

	

}