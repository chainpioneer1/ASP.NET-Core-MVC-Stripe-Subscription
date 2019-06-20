using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace new_websub.Models
{
    public class AnAddress
    {
        private NewWebSubContext context;
        [Key]
        public int addresskey { get; set; }

        [Required(ErrorMessage = "Address1 is required.")]
        public string Address1 { get; set; }

        
        public string Address2 { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zipcode is required.")]
        public string Zipcode { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        // make date time. do i need this or just make in data base ?
        [DataType (DataType.DateTime)]
        public DateTime Address_Created { get; set; }

        //make data time? 
        [DataType(DataType.DateTime)]
        public DateTime Address_Modified { get; set; }

        //link to Useraccount
        public virtual UserAccount UserAccount { get; set; }

    }
}
