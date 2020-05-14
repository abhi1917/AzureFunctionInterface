using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionInterface.StoredProcedures
{
    public static class StoredProcedureList
    {
        public static string AddProcedure = "AddItem";
        public static string ReadByPartitionProcedure = "ReadItemsByPartition";
    }
}
