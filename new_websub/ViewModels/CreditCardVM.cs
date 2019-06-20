using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace new_websub.ViewModels
{
    public class CreditCardVM
    {
        [Required(ErrorMessage = "Credit Card Number is required.")]
        public string creditCardNo { get; set; }

        [Required(ErrorMessage = "Expiration month is required.")]
        public string expDate_month { get; set; }

        [Required(ErrorMessage = "Expiration year is required.")]
        public string expDate_year { get; set; }

        [Required(ErrorMessage = "CVC Code is required.")]
        public string cvcCode { get; set; }

        [Required(ErrorMessage = "Subscription rate is required.")]
        public double rate { get; set; }
    }
}
