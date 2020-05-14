using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunctionInterface
{
    public static class ChangeFeedListnerParallel
    {
        [FunctionName("ChangeFeedListnerParallel")]
        public static void Run([CosmosDBTrigger(
            databaseName: "%databaseId%",
            collectionName: "%containerId%",
            ConnectionStringSetting = "AzureconnectionString",
            LeaseCollectionName = "leasesTwo",CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> documents, ILogger log)
        {
            try
            {
                if (documents != null && documents.Count > 0)
                {
                    log.LogInformation(documents[0].ToString());
                    log.LogInformation("Documents modified " + documents.Count);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }
    }
}
