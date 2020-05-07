using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionInterface.Models
{
    public class CustomerSendEvent
    {
        public string CustomerId { get; set; }
        public string AgentId { get; set; }
    }
}
