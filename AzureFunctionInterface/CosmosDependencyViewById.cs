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
using System.Collections.Generic;
using AzureFunctionInterface.Models;
using System.Net.Http;
using System.Net;
using System.Text;

namespace AzureFunctionInterface
{
    public class CosmosDependencyViewById
    {
        private Container _customerContainer;

        public CosmosDependencyViewById(Container customerContainer)
        {
            _customerContainer = customerContainer;
        }

        [FunctionName("CosmosDependencyViewById")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            HttpResponseMessage response;
            try
            {
                IDictionary<string, string> queryParams = req.GetQueryParameterDictionary();
                CustomerDetail customer=null;
                if (queryParams.ContainsKey("lastName")&& queryParams.ContainsKey("id"))
                {
                    string lastName = queryParams["lastName"].Replace("\"", "");
                    string id= queryParams["id"].Replace("\"", "");
                    var returnResult = await _customerContainer.ReadItemAsync<CustomerCosmos>(id, new PartitionKey(lastName));
                    var result = returnResult.Resource;
                    if (result != null)
                    {
                        customer = new CustomerDetail
                        {
                            FirstName = result.FirstName,
                            LastName = result.LastName,
                            Address = result.Address,
                            Phonenumber = result.Phonenumber
                        };
                    }
                }
                if (null != customer)
                {
                    string jsonValue = JsonConvert.SerializeObject(customer);
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(jsonValue, UnicodeEncoding.UTF8, "application/json");
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    response.Content = new StringContent("Invalid Request!", UnicodeEncoding.UTF8, "application/text");

                }
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
