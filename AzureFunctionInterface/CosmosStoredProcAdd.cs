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
using Microsoft.Azure.Documents.Client;

namespace AzureFunctionInterface
{
    public static class CosmosStoredProcAdd
    {
        [FunctionName("CosmosStoredProcAdd")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Customer Add begins");

            try
            {
                string documentJson = await new StreamReader(req.Body).ReadToEndAsync();
                var document = JsonConvert.DeserializeObject<Dictionary<string, string>>(documentJson);
                
                if (document.ContainsKey("DatabaseName") && document.ContainsKey("ContainerName") && document.ContainsKey("PartitionKey"))
                {
                    var cosmosDbDatabaseName = document["DatabaseName"];
                    document.Remove("DatabaseName");
                    var cosmosDbContainerName = document["ContainerName"];
                    document.Remove("ContainerName");
                    var cosmosDbPartitionKey = document["PartitionKey"];
                    document.Remove("PartitionKey");
                    var cosmosDbEndPoint = Environment.GetEnvironmentVariable("endpointUri", EnvironmentVariableTarget.Process);
                    var cosmosDbAuthKey = Environment.GetEnvironmentVariable("customerPrimaryKey", EnvironmentVariableTarget.Process);
                    await DocumentUtilities.AddDocumentAsync(cosmosDbEndPoint, cosmosDbAuthKey, cosmosDbDatabaseName, cosmosDbContainerName, cosmosDbPartitionKey, StoredProcedureList.AddProcedure,document);
                    return new OkObjectResult(document);
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
