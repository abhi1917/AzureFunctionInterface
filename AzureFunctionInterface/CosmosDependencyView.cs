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
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Net;
using AzureFunctionInterface.Utilities;

namespace AzureFunctionInterface
{
    public class CosmosDependencyView
    {
        private CosmosClient _cosmosClient;
        public CosmosDependencyView(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }
        [FunctionName("CosmosDependencyView")]
        public  async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            HttpResponseMessage response;
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                string sqlQueryText = "";
                IDictionary<string, string> queryParams = req.GetQueryParameterDictionary();
                if (queryParams.ContainsKey("lastName"))
                {
                    string lastName = queryParams["lastName"].Replace("\"", "");
                    sqlQueryText = "SELECT c.FirstName,c.LastName,c.Address,c.Phonenumber FROM c WHERE (lower(c.LastName) =lower('" + lastName + "'))";


                }
                else
                {
                    sqlQueryText = "SELECT c.FirstName,c.LastName,c.Address,c.Phonenumber FROM c";
                }
                var cosmosDbDatabaseName = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
                var cosmosDbContainerName = Environment.GetEnvironmentVariable("containerId", EnvironmentVariableTarget.Process);
                var cosmosDbPartitionKey = Environment.GetEnvironmentVariable("CustomerPartitionKey", EnvironmentVariableTarget.Process);
                var returnList = await CosmosUtilities.ReadItems(_cosmosClient, cosmosDbDatabaseName, cosmosDbContainerName, cosmosDbPartitionKey, sqlQueryText);
                string jsonValue = "";
                if (null != returnList)
                {
                    jsonValue = JsonConvert.SerializeObject(returnList);
                }
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(jsonValue, UnicodeEncoding.UTF8, "application/json");

            }
            catch(Exception ex)
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
