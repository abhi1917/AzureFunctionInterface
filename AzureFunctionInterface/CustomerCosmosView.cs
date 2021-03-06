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
using System.Net.Http;
using System.Net;
using System.Linq;
using System.Text;
using AzureFunctionInterface.Models;

namespace AzureFunctionInterface
{
    public static class CustomerCosmosView
    {
        [FunctionName("CustomerCosmosView")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "%databaseId%",
                collectionName: "%containerId%",
                ConnectionStringSetting = "AzureconnectionString",
                SqlQuery ="SELECT * FROM c")] IEnumerable<CustomerDetail> customers,
            ILogger log)
        {
            log.LogInformation("Customer View begins");
            string jsonValue = "";
            HttpResponseMessage response;
            try
            {
                List<CustomerDetail> returnList;
                IDictionary<string, string> queryParams = req.GetQueryParameterDictionary();
                if (queryParams.ContainsKey("lastName"))
                {
                    string lastName = queryParams["lastName"].Replace("\"", "");
                    returnList = customers.Where(s => s.LastName == lastName).ToList();

                }
                else
                {
                    returnList = customers.ToList();
                }
                jsonValue = JsonConvert.SerializeObject(returnList);
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(jsonValue, UnicodeEncoding.UTF8, "application/json");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogError(ex.InnerException.Message);
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.Message, UnicodeEncoding.UTF8, "application/text");
            }
            return response;
        }
    }
}
