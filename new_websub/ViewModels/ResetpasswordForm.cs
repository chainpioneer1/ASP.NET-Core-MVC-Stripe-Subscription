using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_websub.ViewModels
{
    public class ResetpasswordForm
    {
        public string Email { get; set; }
        public string token { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}
