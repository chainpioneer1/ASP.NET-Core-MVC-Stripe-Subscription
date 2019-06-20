using new_websub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_websub.ViewModels
{
    public class RegisterFormVM
    {
        public AnAddress address { get; set; }
        public UserAccount userAccount { get; set; }
    }
}
