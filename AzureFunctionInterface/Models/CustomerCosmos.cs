using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionInterface.Models
{
    public class CustomerCosmos
    {
        public string id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phonenumber { get; set; }
        public string TransactionID { get; set; }
        public string AgentID { get; set; }
    }
}
