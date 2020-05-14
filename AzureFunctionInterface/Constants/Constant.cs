using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionInterface.Constants
{
    public static class Constant
    {
        public static string StoredProcRelativePath = $@"..\..\..\StoredProcedures\";
        public static string javaScriptExtension = ".js";
        public static string Database = "databaseId";
        public static string Container = "EventContainer";
        public static string PartitionKey = "CosmosPartitionKey";
    }
}
