using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_websub.Models
{
    public class EmailSettings
    {
        public string PrimaryEmail { get; set; }
        public string PrimaryPassword { get; set; }
        public int PrimaryPort { get; set; }
        public string PrimaryDomain { get; set; }
        public bool EnableSsl { get; set; }
        public string EmailUserName { get; set; }
    }
}
