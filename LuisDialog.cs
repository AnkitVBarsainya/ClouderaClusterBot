using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Text;
namespace ClouderaAdmin
{
	[LuisModel("528f8386-dce0-469d-b990-cb62c95278bc", "5e5829d38925423eb4f2b9362ec1774d")]
	[Serializable]
	public class LuisDialog : LuisDialog<object>
	{
		//IDialogContext context;
		//MethodGlossary methodObj = new MethodGlossary();
		String serviceInProcess = null;
		String faultyServName = null;

		[LuisIntent("Greetings")]
		public async Task GreetService(IDialogContext context, LuisResult result)
		{
			context.ConversationData.RemoveValue("Complement");
			context.ConversationData.RemoveValue("Bye");
			context.ConversationData.RemoveValue("FormalQuestions");
			context.ConversationData.RemoveValue("ClusterStatus");
			context.ConversationData.RemoveValue("RestartService");
			context.ConversationData.RemoveValue("NoRestart");
			context.ConversationData.RemoveValue("Awaiting");
			context.ConversationData.SetValue("Greetings", result.Intents[0].Intent);


			if (result.Intents[0].Score > 0.20)
			{
				String MyHour = DateTime.Now.ToString(("hh"));
				int MyHourInt = Int32.Parse(MyHour);
				String Mytime = DateTime.Now.ToString(("tt"));
				/*string ReplyTime;
				if (MyHourInt + 4.5 >= 1 && MyHourInt + 4.5 <= 12 && Mytime == "AM")
					ReplyTime = "Good Morning";
				else if (MyHourInt + 4.5 >= 1 && MyHourInt + 4.5 <= 4 && Mytime == "PM")
					ReplyTime = "Good Afternoon";
				else if (MyHourInt + 4.5 >= 5 && MyHourInt + 4.5 <= 11 && Mytime == "PM")
					ReplyTime = "Good Evening";
				else
					ReplyTime = "nice to see you";*/
				await context.PostAsync("Hi " + context.Activity.From.Name + $", ..Tell me how can i help you :-).");

			}
			else
			{
				await context.PostAsync($"sorry, I am not smart enough to understand that just yet :-( please ask me something  else");
			}
			context.Wait(MessageReceived);
		}

		[LuisIntent("Complement")]
		public async Task ComplementService(IDialogContext context, LuisResult result)
		{
			context.ConversationData.RemoveValue("Bye");
			context.ConversationData.RemoveValue("FormalQuestions");
			context.ConversationData.RemoveValue("ClusterStatus");
			context.ConversationData.RemoveValue("RestartService");
			context.ConversationData.RemoveValue("NoRestart");
			context.ConversationData.RemoveValue("Awaiting");
			context.ConversationData.SetValue("Complement", result.Intents[0].Intent);

			if (result.Intents[0].Score > 0.20)
			{
				Random rnd = new Random();
				int MyNum = rnd.Next(1, 6);
				if (MyNum == 1)
				{
					await context.PostAsync($"You are welcome:)");
				}
				else if (MyNum == 2)
				{
					await context.PostAsync($"No worries, that's my job :-)");
				}
				else if (MyNum == 3)
				{
					await context.PostAsync($"My Pleasure :-)  I don't get tired helping people");
				}
				else if (MyNum == 4)
				{
					await context.PostAsync($"That's fine,always a pleasure to help you anyway:)");
				}
				else if (MyNum == 5)
				{
					await context.PostAsync($"No problem :-), Please let me know if you need any more help");
				}
				else
				{
					await context.PostAsync($"anytime, I don't get tired helping people");
				}
			}
			else
			{
				await context.PostAsync($"sorry, I am not smart enough to understand that just yet :-( please ask me something  else");
			}
			context.Wait(MessageReceived);
		}
		[LuisIntent("Bye")]
		public async Task AdiosService(IDialogContext context, LuisResult result)
		{
			context.ConversationData.RemoveValue("Complement");

			context.ConversationData.RemoveValue("FormalQuestions");
			context.ConversationData.RemoveValue("ClusterStatus");
			context.ConversationData.RemoveValue("RestartService");
			context.ConversationData.RemoveValue("NoRestart");
			context.ConversationData.RemoveValue("Awaiting");
			context.ConversationData.SetValue("Bye", result.Intents[0].Intent);

			if (result.Intents[0].Score > 0.20)
			{
				Random rnd2 = new Random();
				int MyNum2 = rnd2.Next(1, 3);
				if (MyNum2 == 1)
				{
					await context.PostAsync($"Okay, see you later,TC :-)");
				}
				else if (MyNum2 == 2)

				{
					await context.PostAsync($"Good Bye! have a good time ahead :-)");
				}
				else
				{
					await context.PostAsync($"Bye, Take care");
				}
			}
			else
			{
				await context.PostAsync($"sorry, I am not smart enough to understand that just yet :-( please ask me something  else");
			}
			context.Wait(MessageReceived);
		}

		[LuisIntent("")]
		[LuisIntent("None")]
		public async Task NoneIntent(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("I could not understand you. Please try a different question.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("ClusterStatus")]
		public async Task ClusterStatusService(IDialogContext context, LuisResult result)
		{
			context.ConversationData.RemoveValue("Complement");
			context.ConversationData.RemoveValue("Bye");
			context.ConversationData.RemoveValue("FormalQuestions");
			context.ConversationData.RemoveValue("RestartService");
			context.ConversationData.RemoveValue("NoRestart");
			context.ConversationData.RemoveValue("Awaiting");
			context.ConversationData.SetValue("ClusterStatus", result.Intents[0].Intent);
			// String clusterStatus = await methodObj.CurlClouderaManager(context);
			String clusterStatus = MethodGlossary.LoadHttpPageWithBasicAuthentication(context, null);
			// dynamic resultJson = JsonConvert.DeserializeObject<dynamic>(clusterStatus);
			
			JToken jsonToken = JObject.Parse(clusterStatus);
			// jsonObject.

			if (jsonToken.SelectToken("entityStatus").ToString().Equals("GOOD", StringComparison.InvariantCultureIgnoreCase))
			{
				//if Cluster status is good. Stop here. No need to investigate further.
				await context.PostAsync("Cluster seems to be in a good shape right now.");
				await context.PostAsync("Let me know if you need more help");
				context.Wait(MessageReceived);
			}
			// Else Continue
			else
			{
				await context.PostAsync("Looks like there is some issue with the cluster. Wait up while I gather more details.");

				String allServiceStatus = MethodGlossary.LoadHttpPageWithBasicAuthentication(context, "ALL_SERVICES");
				await context.PostAsync("Cluster Status string" + allServiceStatus);
				// dynamic allServiceJson = JsonConvert.DeserializeObject<dynamic>(allServiceStatus);
				JToken allServiceToken = JToken.Parse(allServiceStatus);
				var countServices = allServiceToken.Value<JArray>("items").Count;
				List<String> listOfConcerningServices = new List<String>();
				Dictionary<String, String> concerningServices = new Dictionary<String, String>();
				for (int i = 0; i < countServices; i++)
				{
					var servName = allServiceToken.Value<JArray>("items")[i].SelectToken("name").ToString();
					var servStatus = allServiceToken.Value<JArray>("items")[i].SelectToken("healthSummary").ToString();
					if (!servStatus.Equals("GOOD", StringComparison.InvariantCultureIgnoreCase))
					{
						concerningServices.Add(servName, servStatus);
						listOfConcerningServices.Add(servName);
					}
				}

				// Loop all services. List down all in Concerning health & in Stopped status separately 
				if (listOfConcerningServices.Count > 1)
				{
					StringBuilder stringOfConcServ = null;
					for (int i = 0; i < listOfConcerningServices.Count; i++)
					{
						if (listOfConcerningServices.Count - 1 == i)
						{
							stringOfConcServ.Append(listOfConcerningServices[i]);
						}
						else
						{
							stringOfConcServ.Append(listOfConcerningServices[i]).Append(", ");
						}
					}
					await context.PostAsync("Here is the list of all services in either concerning or stopped status");
					await context.PostAsync(stringOfConcServ.ToString());
					await context.PostAsync("Let me know what you'd like to do");
					context.Wait(MessageReceived);
				}
				else if (listOfConcerningServices.Count == 1)
				{

					String faultyServStatus = null;
					foreach (KeyValuePair<String, String> dysfunctServ in concerningServices) { faultyServName = dysfunctServ.Key; faultyServStatus = dysfunctServ.Value; }
					await context.PostAsync(faultyServName + " is in " + faultyServStatus + " status. Would you like me to re/start it for you?");
					context.Wait(MessageReceived);
				}

			}

		}

		[LuisIntent("RestartService")]
		public async Task RestartService(IDialogContext context, LuisResult result)
		{
			context.ConversationData.RemoveValue("Complement");
			context.ConversationData.RemoveValue("Bye");
			context.ConversationData.RemoveValue("FormalQuestions");
			context.ConversationData.RemoveValue("ClusterStatus");
			context.ConversationData.RemoveValue("NoRestart");
			context.ConversationData.RemoveValue("Awaiting");
			context.ConversationData.SetValue("RestartService", result.Intents[0].Intent);
			string serviceClusterName = null;
			string serviceCommonName = null;

			String responseStr;
			// Parse Service name first
			if (String.IsNullOrEmpty(faultyServName))
			{
				if(String.IsNullOrEmpty(result.Entities[0].Entity))
				serviceClusterName = MethodGlossary.GetClusterServiceName(context, result.Entities[0].Entity);
			}
			else
			{
				serviceClusterName = faultyServName;
			}



			// Check service Status before restarting/starting
			String serviceStatus = MethodGlossary.LoadHttpPageWithBasicAuthentication(context, serviceClusterName);
			JToken serviceToken = JToken.Parse(serviceStatus);
			var serviceState = serviceToken.SelectToken("serviceState").ToString();
			// JObject jsonServiceStat = JObject.Parse(serviceStatus);
			// if "serviceState" is Started
			if (serviceState.Equals("STARTED", StringComparison.InvariantCultureIgnoreCase))
			{
				responseStr = await MethodGlossary.LoadHttpPageWithBasicAuthentication(context, serviceClusterName, "restart");
			}
			// Else 
			else
			{
				responseStr = await MethodGlossary.LoadHttpPageWithBasicAuthentication(context, serviceClusterName, "start");
			}
			// Parse o/p for success/failure
			serviceToken = JToken.Parse(responseStr);
			var opStatus = serviceToken.SelectToken("active").ToString();
			// JObject jsonServiceBoot = JObject.Parse(responseStr);
			if (opStatus.Equals(true))
			{
				// Store started service name for next stage
				serviceInProcess = serviceClusterName;
				// inform user of status
				await context.PostAsync(serviceClusterName.ToUpper() + " has been re/started. Will ping you once it's available. ");
				context.Wait(MessageReceived);
				await AwaitRestart(context, result);
			}
			else
			{
				await context.PostAsync("There was some problem re/starting " + serviceClusterName.ToUpper() + ". Here's the relevant response");
				await context.PostAsync(serviceToken.SelectToken("resultMessage").ToString());
				context.Wait(MessageReceived);
			}


		}
		[LuisIntent("NoRestart")]
		public async Task NoRestart(IDialogContext context, LuisResult result)
		{
			context.ConversationData.RemoveValue("Complement");
			context.ConversationData.RemoveValue("Bye");
			context.ConversationData.RemoveValue("FormalQuestions");
			context.ConversationData.RemoveValue("ClusterStatus");
			context.ConversationData.RemoveValue("RestartService");

			context.ConversationData.RemoveValue("Awaiting");
			context.ConversationData.SetValue("NoRestart", result.Intents[0].Intent);
			await context.PostAsync("Cool. Here's the Cloudera Manager link for you. ");
			await context.PostAsync("http://129.146.74.163:7180/cmf/login");
		}

		[LuisIntent("Awaiting")]
		public async Task AwaitRestart(IDialogContext context, LuisResult result)
		{
			context.ConversationData.RemoveValue("Complement");
			context.ConversationData.RemoveValue("Bye");
			context.ConversationData.RemoveValue("FormalQuestions");
			context.ConversationData.RemoveValue("ClusterStatus");
			context.ConversationData.RemoveValue("RestartService");
			context.ConversationData.RemoveValue("NoRestart");
			context.ConversationData.SetValue("Awaiting", result.Intents[0].Intent);
			String responseStr;

			responseStr = MethodGlossary.LoadHttpPageWithBasicAuthentication(context, serviceInProcess);
			// parse response, 

			// If Completed
			await context.PostAsync(serviceInProcess.ToUpper() + " is ready now.");
			serviceInProcess = null;
			// if not completed yet, wait 30 secs & call AwaitRestart
			Thread.Sleep(30000);
			await AwaitRestart(context, result);
		}
	}
}