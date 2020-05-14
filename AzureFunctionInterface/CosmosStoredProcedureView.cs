using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using AzureFunctionInterface.Utilities;
using AzureFunctionInterface.StoredProcedures;

namespace AzureFunctionInterface
{
    public static class CosmosStoredProcedureView
    {
        [FunctionName("CosmosStoredProcedureView")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string documentJson = await new StreamReader(req.Body).ReadToEndAsync();
                var document = JsonConvert.DeserializeObject<Dictionary<string, string>>(documentJson);

                if (document.ContainsKey("DatabaseName") && document.ContainsKey("ContainerName") && document.ContainsKey("PartitionKey") && document.ContainsKey("PartitionValue"))
                {
                    var cosmosDbDatabaseName = document["DatabaseName"];
                    var cosmosDbContainerName = document["ContainerName"];
                    var cosmosDbPartitionKey = document["PartitionKey"];
                    var partitionValue= document["PartitionValue"];
                    var cosmosDbEndPoint = Environment.GetEnvironmentVariable("endpointUri", EnvironmentVariableTarget.Process);
                    var cosmosDbAuthKey = Environment.GetEnvironmentVariable("customerPrimaryKey", EnvironmentVariableTarget.Process);
                    var result=await DocumentUtilities.ReadDocumentsbyPartition(cosmosDbEndPoint, cosmosDbAuthKey, cosmosDbDatabaseName, cosmosDbContainerName, cosmosDbPartitionKey, StoredProcedureList.ReadByPartitionProcedure, partitionValue);
                    return new OkObjectResult(result);
                }
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
                //var cosmosDbDatabaseName = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
                //var cosmosDbContainerName = Environment.GetEnvironmentVariable("containerId", EnvironmentVariableTarget.Process);
                //var cosmosDbPartitionKey = Environment.GetEnvironmentVariable("CustomerPartitionKey", EnvironmentVariableTarget.Process);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
