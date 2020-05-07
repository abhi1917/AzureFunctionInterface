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
            var cosmosDbEndPoint = Environment.GetEnvironmentVariable("endpointUri", EnvironmentVariableTarget.Process);
            var cosmosDbAuthKey = Environment.GetEnvironmentVariable("customerPrimaryKey", EnvironmentVariableTarget.Process);
            var cosmosDbDatabaseName = Environment.GetEnvironmentVariable("databaseId", EnvironmentVariableTarget.Process);
            var cosmosDbContainerName = Environment.GetEnvironmentVariable("containerId", EnvironmentVariableTarget.Process);
            var cosmosDbPartitionKey = Environment.GetEnvironmentVariable("CustomerPartitionKey", EnvironmentVariableTarget.Process);

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
