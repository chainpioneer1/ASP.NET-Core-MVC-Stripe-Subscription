using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace new_websub.ViewModels
{
    public class EmailFormVM
    {
        [Required(ErrorMessage = "Your email is required.")]
        public string toEmail { get; set; }
    }
}
