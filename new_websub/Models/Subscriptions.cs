using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace new_websub.Models
{
    public class Subscriptions
    {
        private NewWebSubContext context;
        [Key]
        public int Subscription_id { get; set; }

     

        [Required(ErrorMessage = "Description is required.")]
        public string  Subscription_Description{ get; set; }


        public double Rate { get; set; }

        public bool is_valid { get; set; }


        //all date time automatic (how)
        [DataType(DataType.DateTime)]
        public DateTime Subscription_Started { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Subscription_Ended { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Subscription_Modified { get; set; }


        //link to Useraccount
        public virtual UserAccount UserAccount { get; set; }





    }
}
