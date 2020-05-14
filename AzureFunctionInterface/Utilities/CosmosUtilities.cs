using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionInterface.Utilities
{
    public static class CosmosUtilities
    {
        public static Container GetContainer(CosmosClient cosmosClient,string database,string container,string partitionkey)
        {
            cosmosClient.CreateDatabaseIfNotExistsAsync(database).Wait();
            var databaseClient = cosmosClient.GetDatabase(database);
            databaseClient.CreateContainerIfNotExistsAsync(container, partitionkey).Wait();
            return databaseClient.GetContainer(container);
        }

        public static async Task<ItemResponse<Dictionary<string,string>>> CreateItem(CosmosClient cosmosClient, string database, string container, string partitionkey, Dictionary<string, string> customer)
        {
            try
            {
                var containerEntity = GetContainer(cosmosClient, database, container, partitionkey);
                var result = await containerEntity.CreateItemAsync(customer, new PartitionKey(customer[partitionkey.Substring(1)]));
                return result;
            }catch(Exception ex)
            {
                throw ex;
            }

        }

        public static bool ValidateObject(Dictionary<string,string> customer,string partitionKey)
        {
            return customer.ContainsKey(partitionKey.Substring(1)) && customer.ContainsKey("id");
        }

        public static async Task<IEnumerable<object>> ReadItems(CosmosClient cosmosClient, string database, string container, string partitionkey, string query)
        {
            try
            {
                var customerContainer = CosmosUtilities.GetContainer(cosmosClient, database, container, partitionkey);
                QueryDefinition queryDefinition = new QueryDefinition(query);
                FeedIterator<object> queryResultSetIterator = customerContainer.GetItemQueryIterator<object>(queryDefinition);
                while (queryResultSetIterator.HasMoreResults)
                {
                    var result = await queryResultSetIterator.ReadNextAsync();
                    return result.Resource;
                }
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        } 
    }
}
