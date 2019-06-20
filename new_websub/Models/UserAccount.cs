using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace new_websub.Models
{
    public class UserAccount
    {
        private NewWebSubContext context;
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Please confirm your passsword")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        // make date time. do i need this or just make in data base ?
        [DataType(DataType.DateTime)]
        public DateTime Account_Created { get; set; }

        //make data time? 
        [DataType(DataType.DateTime)]
        public DateTime Account_Modified { get; set; }

        public List<AnAddress> AnAddresses { get; set; }
        //public List<Subscription> subscriptions { get; set; }

        public string toString()
        {
            return "Fist name =>" + this.FirstName + "\n" +
                "Last name =>" + this.LastName;
        }
    }
}
