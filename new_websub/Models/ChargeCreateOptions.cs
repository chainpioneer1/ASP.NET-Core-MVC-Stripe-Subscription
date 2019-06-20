using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_websub.Models
{
    public class ChargeCreateOptions
    {
        public string Amount { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public string SourceId { get; set; }

        public string ReceiptEmail { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}
