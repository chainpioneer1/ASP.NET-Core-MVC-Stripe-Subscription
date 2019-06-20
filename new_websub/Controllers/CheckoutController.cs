using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Configuration;

namespace new_websub.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {   
            ViewBag.PaymentAmount = 10;
            return View();
        }

        [HttpPost]
        public ActionResult Charge(string stripeEmail, string stripeToken)
        {
            //string secretKey = ConfigurationManager.AppSettings["Stripe:secretKey"];
            string secretKey = "sk_test_e3ssfSIQhhvg7DtlMpHLxMKm";
            StripeConfiguration.SetApiKey(secretKey);

            Stripe.CustomerCreateOptions myCustomer = new Stripe.CustomerCreateOptions();
            myCustomer.Email = stripeEmail;
            myCustomer.Source = stripeToken;
            var customerService = new Stripe.CustomerService();
            Stripe.Customer stripeCustomer = customerService.Create(myCustomer);

            var options = new Stripe.ChargeCreateOptions
            {
                Amount = 1000,
                Currency = "USD",
                Description = "Buying 10 rubber ducks",
                Source = stripeToken,
                ReceiptEmail = stripeEmail
            };
            //and Create Method of this object is doing the payment execution.  
            var service = new Stripe.ChargeService();
            //Stripe.Charge charge = service.Create(options);

            return RedirectToAction(nameof(Index));
        }
    }
}