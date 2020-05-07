using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using AzureFunctionInterface.Models;

namespace AzureFunctionInterface
{

    public static class CustomerCosmosAdd
    {
        [FunctionName("CustomerCosmosAdd")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "%databaseId%",
                collectionName: "%containerId%",
                ConnectionStringSetting = "AzureconnectionString",CreateIfNotExists = true,PartitionKey ="%CustomerPartitionKey%",Id ="id")]IAsyncCollector<object> customers,
            ILogger log)
        {
            log.LogInformation("Customer insertion begins!");
            try
            {
                string customerBody = await new StreamReader(req.Body).ReadToEndAsync();
                CustomerCosmos customer = JsonConvert.DeserializeObject<CustomerCosmos>(customerBody);
                await customers.AddAsync(customer);
                log.LogInformation("Customer insertion succesful!");
                return new OkObjectResult(customers);
            }
            catch(Exception ex)
            {
                log.LogError("Error:"+ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
