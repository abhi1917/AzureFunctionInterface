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

namespace AzureFunctionInterface
{
    public class CosmosDependencyAdd
    {
        private Container _customerContainer;

        public CosmosDependencyAdd(Container customerContainer)
        {
            _customerContainer = customerContainer;
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
                CustomerCosmos customer = JsonConvert.DeserializeObject<CustomerCosmos>(customerBody);
                await _customerContainer.CreateItemAsync(customer, customer.PartitionKey);

                return new OkObjectResult("Customer:"+customer.FirstName+" "+customer.LastName+"has been added!");
            }
            catch(Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
