using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFunctionInterface.Constants;
using AzureFunctionInterface.StoredProcedures;
using AzureFunctionInterface.Utilities;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionInterface
{
    public static class TopicReaderA
    {
        [FunctionName("TopicReaderA")]
        public static async Task Run([ServiceBusTrigger("%ServiceBusTopic%", "%ServiceBusSubscription%", Connection = "ServiceBusConnectionString")]string mySbMsg,
                        [CosmosDB(
                databaseName: "%databaseId%",
                collectionName: "%EventContainer%",
                ConnectionStringSetting = "AzureconnectionString",CreateIfNotExists = true,PartitionKey ="%CosmosPartitionKey%",Id ="id")]IAsyncCollector<object> customers, ILogger log)
        {
            try
            {
                log.LogInformation("Topic Item read by reader A:" + mySbMsg);
                object myTopicObject = JsonConvert.DeserializeObject<object>(mySbMsg);
                await customers.AddAsync(myTopicObject);
                log.LogInformation("Succesfully updated details!");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }
    }
}
