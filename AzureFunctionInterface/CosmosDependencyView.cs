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

namespace AzureFunctionInterface
{
    public class CosmosDependencyView
    {
        private Container _customerContainer;
        public CosmosDependencyView(Container customerContainer)
        {
            _customerContainer = customerContainer;
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
                    sqlQueryText = "SELECT * FROM c WHERE (lower(c.LastName) =lower('" + lastName + "'))";


                }
                else
                {
                    sqlQueryText = "SELECT * FROM c";
                }
                QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
                FeedIterator<CustomerCosmos> queryResultSetIterator = _customerContainer.GetItemQueryIterator<CustomerCosmos>(queryDefinition);
                List<CustomerDetail> returnCustomerList = new List<CustomerDetail>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<CustomerCosmos> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (CustomerCosmos customer in currentResultSet)
                    {
                        returnCustomerList.Add(new CustomerDetail
                        {
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Address = customer.Address,
                            Phonenumber = customer.Phonenumber
                        });
                    }
                }
                string jsonValue = JsonConvert.SerializeObject(returnCustomerList);
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
