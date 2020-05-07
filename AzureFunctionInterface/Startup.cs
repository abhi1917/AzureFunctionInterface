using AzureFunctionInterface;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AzureFunctionInterface
{
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<Container>(GetContainer);
        }

        private Container GetContainer(IServiceProvider options)
        {
            var cosmosDbEndPoint = Environment.GetEnvironmentVariable("endpointUri");
            var cosmosDbAuthKey = Environment.GetEnvironmentVariable("customerPrimaryKey");
            var cosmosDbDatabaseName = Environment.GetEnvironmentVariable("databaseId");
            var cosmosDbContainerName = Environment.GetEnvironmentVariable("containerId");
            var cosmosDbPartitionKey = Environment.GetEnvironmentVariable("CustomerPartitionKey");

            var client = new CosmosClient(cosmosDbEndPoint, cosmosDbAuthKey, new CosmosClientOptions {
                ConnectionMode = ConnectionMode.Direct
            });
            client.CreateDatabaseIfNotExistsAsync(cosmosDbDatabaseName).Wait();
            var database = client.GetDatabase(cosmosDbDatabaseName);
            database.CreateContainerIfNotExistsAsync(cosmosDbContainerName, cosmosDbPartitionKey).Wait();
            return database.GetContainer(cosmosDbContainerName);
        }
    }
}
