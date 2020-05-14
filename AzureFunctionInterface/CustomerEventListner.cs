using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AzureFunctionInterface.Models;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionInterface
{
    public static class CustomerEventListner
    {

        [FunctionName("CustomerEventListner")]
        public static async Task Run([EventHubTrigger("%EventHubName%", Connection = "EventHub")] EventData[] events, ILogger log)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                foreach (EventData eventData in events)
                {
                    string value = Encoding.UTF8.GetString(eventData.Body);
                    CustomerSendEvent customer = new CustomerSendEvent
                    {
                        CustomerId = value,
                        AgentId = "EventHub"
                    };
                    var url = Environment.GetEnvironmentVariable("apiEventInvokeurl",EnvironmentVariableTarget.Process);
                    var content = JsonConvert.SerializeObject(customer);
                    if (null != content)
                    {
                        var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("apiAuthToken", EnvironmentVariableTarget.Process));
                        var result = await httpClient.PostAsync(url, stringContent);
                    }
                    else
                    {
                        throw new Exception("Failed to serialize object!");
                    }
                    log.LogInformation("New Customer with Id: "+value+" was added to cosmos DB.");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }
    }
}
