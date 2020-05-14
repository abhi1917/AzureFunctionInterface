using AzureFunctionInterface.Constants;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionInterface.Utilities
{
    public static class DocumentOperations
    {
        public static async Task AddDocumentAsync(DocumentClient client, string database, string container, string partitionKey, string storedProc, Dictionary<string, string> customer)
        {
            await CreateStoredProcedureIfNotExists(client, database, container, storedProc, partitionKey);
            await ExecuteAddStoredProcedure(client, storedProc, database, container, partitionKey, customer);
        }


        public static async Task<StoredProcedure> CreateStoredProcedureIfNotExists(DocumentClient client, string database, string container, string storedProcedureId, string partitionKey)
        {
            ResourceResponse<StoredProcedure> response = null;
            try
            {
                try
                {
                    Uri storedprocuri = UriFactory.CreateStoredProcedureUri(database, container, storedProcedureId);
                    RequestOptions options = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };
                    response = await client.ReadStoredProcedureAsync(storedprocuri, options);
                }
                catch (DocumentClientException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        var path = Path.GetFullPath(Constant.StoredProcRelativePath + storedProcedureId + Constant.javaScriptExtension);
                        StoredProcedure newStoredProcedure = new StoredProcedure
                        {
                            Id = storedProcedureId,
                            Body = File.ReadAllText(path)
                        };
                        Uri containerUri = UriFactory.CreateDocumentCollectionUri(database, container);
                        response = await client.CreateStoredProcedureAsync(containerUri, newStoredProcedure);
                    }
                }
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static async Task ExecuteAddStoredProcedure(DocumentClient client, string storedProcedure, string database, string container, string partitionKey, Dictionary<string, string> customer)
        {
            try
            {
                Uri uri = UriFactory.CreateStoredProcedureUri(database, container, storedProcedure);
                RequestOptions options = new RequestOptions { PartitionKey = new PartitionKey(customer[partitionKey.Substring(1)]) };
                var result = await client.ExecuteStoredProcedureAsync<string>(uri, options, customer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> ReadDocumentsbyPartition(DocumentClient client, string database, string container, string partitionKey, string storedProc, string partitionValue)
        {
            await CreateStoredProcedureIfNotExists(client, database, container, storedProc, partitionKey);
            return await ExecuteReadDocumentsStoredProcedure(client, storedProc, database, container, partitionValue);
        }

        public static async Task<string> ExecuteReadDocumentsStoredProcedure(DocumentClient client, string storedProcedure, string database, string container, string partitionValue)
        {
            try
            {
                Uri uri = UriFactory.CreateStoredProcedureUri(database, container, storedProcedure);
                RequestOptions options = new RequestOptions { PartitionKey = new PartitionKey(partitionValue) };
                var result = await client.ExecuteStoredProcedureAsync<string>(uri, options);
                return result.Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
