using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_websub.ViewModels
{
    public class CustomerPaymentViewModel
    {
        public string UserName { get; set; }

        public string EmailId { get; set; }

        public string CardNumber { get; set; }

        public string Cvc { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public int Amount { get; set; }

        public string Currency { get; set; }

        public string PaymentMethodTypes { get; set; }

        public string subsctype { get; set; }

        public string massage { get; set; }

        public string cardtoken { get; set; }

    }

    public class User
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string StripeCustomerId { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }

        public bool HistoryView { get; set; }
    }
}
