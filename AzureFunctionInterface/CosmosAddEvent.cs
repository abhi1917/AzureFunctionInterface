using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionInterface
{
    public static class CosmosAddEvent
    {
        [FunctionName("CosmosAddEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "%databaseId%",
                collectionName: "%EventContainer%",
                ConnectionStringSetting = "AzureconnectionString",CreateIfNotExists = true,PartitionKey ="%CosmosPartitionKey%",Id ="id")]IAsyncCollector<object> customers,
            ILogger log)
        {
            log.LogInformation("Customer insertion begins!");
            try
            {
                string customerBody = await new StreamReader(req.Body).ReadToEndAsync();
                object customer = JsonConvert.DeserializeObject<object>(customerBody);
                await customers.AddAsync(customer);
                log.LogInformation("Customer insertion succesful!");
                return new OkObjectResult(customers);
            }
            catch (Exception ex)
            {
                log.LogError("Error:" + ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
