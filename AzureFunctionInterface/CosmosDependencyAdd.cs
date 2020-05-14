using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using AzureFunctionInterface.Models;
using AzureFunctionInterface.Utilities;
using System.Collections.Generic;

namespace AzureFunctionInterface
{
    public class CosmosDependencyAdd
    {
        private CosmosClient _cosmosClient;

        public CosmosDependencyAdd(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        [FunctionName("CosmosDependencyAdd")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Customer Add begins");

            try
            {
                string customerBody = await new StreamReader(req.Body).ReadToEndAsync();
                var customer = JsonConvert.DeserializeObject<Dictionary<string,string>>(customerBody);
                var cosmosDbDatabaseName = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
                var cosmosDbContainerName = Environment.GetEnvironmentVariable("containerId", EnvironmentVariableTarget.Process);
                var cosmosDbPartitionKey = Environment.GetEnvironmentVariable("CustomerPartitionKey", EnvironmentVariableTarget.Process);
                //var customerContainer = CosmosUtilities.GetContainer(_cosmosClient, cosmosDbDatabaseName, cosmosDbContainerName, cosmosDbPartitionKey);
                //await customerContainer.CreateItemAsync(customer, customer.PartitionKey);
                if (CosmosUtilities.ValidateObject(customer, cosmosDbPartitionKey))
                {
                    var result = await CosmosUtilities.CreateItem(_cosmosClient, cosmosDbDatabaseName, cosmosDbContainerName, cosmosDbPartitionKey, customer);
                    return new StatusCodeResult((int)result.StatusCode);
                }
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch(Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
