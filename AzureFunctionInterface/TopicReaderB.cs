using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunctionInterface
{
    public static class TopicReaderB
    {
        [FunctionName("TopicReaderB")]
        public static void Run([ServiceBusTrigger("%ServiceBusTopic%", "%ServiceBusSubscriptionB%", Connection = "ServiceBusConnectionString")]string mySbMsg, ILogger log)
        {
            log.LogInformation($"Message read by reader B: {mySbMsg}");
        }
    }
}
