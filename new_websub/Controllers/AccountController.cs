using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using new_websub.Models;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using new_websub.ViewModels;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using Stripe;
using Microsoft.Extensions.Options;

namespace new_websub.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOptions<StripeSettings> _stripeSettings;
        private IOptions<EmailSettings> _emailSettings;

        private Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public AccountController(IOptions<StripeSettings> stripeSettings, IOptions<EmailSettings> emailSettings)
        {
            _stripeSettings = stripeSettings;
            this._emailSettings = emailSettings;
        }
        public string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for(i = 0; i<arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        // my account page
        public ActionResult Index()
        {
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            string email = HttpContext.Session.GetString("User");
            if(ViewBag.isLoggedIn != 1)
            {
                return RedirectToAction(nameof(Login));
            }
            StripeConfiguration.SetApiKey(_stripeSettings.Value.SecretKey);

            var service = new SubscriptionService();
            StripeList<Subscription> response = service.List(new SubscriptionListOptions
            {
                Limit = 50,
                Status = "all"
            });
            List<SubscriptionList> subscriptionLists = new List<SubscriptionList>();
            SubscriptionList subscriptionList = new SubscriptionList();

            string userEmail = HttpContext.Session.GetString("User");
            NewWebSubContext context = HttpContext.RequestServices.GetService(typeof(new_websub.NewWebSubContext)) as NewWebSubContext;
            MySqlConnection conn = context.GetConnection();
            conn.Open();
            string query = "select * from subscriptions where Email=@Email";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlParameter param = new MySqlParameter("@Email", userEmail);
            param.MySqlDbType = MySqlDbType.VarChar;
            cmd.Parameters.Add(param);
            MySqlDataReader reader =  cmd.ExecuteReader();

            List<string> customerIds = new List<string>();
            while (reader.Read())
            {
                customerIds.Add(reader["CustomerId"].ToString());
            }
            reader.Close();

            foreach (var item in response)
            {
                if(customerIds.IndexOf(item.CustomerId) < 0)
                {
                    continue;
                }
                subscriptionList = new SubscriptionList();
                subscriptionList.CustomerId = item.CustomerId;
                subscriptionList.DefaultTaxRates = item.DefaultTaxRates;
                subscriptionList.Discount = item.Discount;
                subscriptionList.EndedAt = item.EndedAt;
                subscriptionList.Items = item.Items;
                subscriptionList.LatestInvoiceId = item.LatestInvoiceId;
                subscriptionList.LatestInvoice = item.LatestInvoice;
                subscriptionList.Livemode = item.Livemode;
                subscriptionList.Metadata = item.Metadata;
                subscriptionList.Plan = item.Plan;
                subscriptionList.Quantity = item.Quantity;
                subscriptionList.StartDate = item.StartDate;
                subscriptionList.Status = item.Status;
                subscriptionList.TransferData = item.TransferData;
                subscriptionList.DefaultSource = item.DefaultSource;
                subscriptionList.TrialEnd = item.TrialEnd;
                subscriptionList.DefaultSourceId = item.DefaultSourceId;
                subscriptionList.DefaultPaymentMethodId = item.DefaultPaymentMethodId;
                subscriptionList.Id = item.Id;
                subscriptionList.Object = item.Object;
                subscriptionList.ApplicationFeePercent = item.ApplicationFeePercent;
                subscriptionList.Billing = item.Billing;
                subscriptionList.BillingCycleAnchor = item.BillingCycleAnchor;
                subscriptionList.BillingThresholds = item.BillingThresholds;
                subscriptionList.CancelAt = item.CancelAt;
                subscriptionList.CancelAtPeriodEnd = item.CancelAtPeriodEnd;
                subscriptionList.CanceledAt = item.CanceledAt;
                subscriptionList.Created = item.Created;
                subscriptionList.CurrentPeriodEnd = item.CurrentPeriodEnd;
                subscriptionList.CurrentPeriodStart = item.CurrentPeriodStart;
                subscriptionList.CustomerId = item.CustomerId;
                subscriptionList.Customer = item.Customer;
                subscriptionList.DaysUntilDue = item.DaysUntilDue;
                subscriptionList.DefaultPaymentMethod = item.DefaultPaymentMethod;
                subscriptionList.TrialStart = item.TrialStart;
                subscriptionLists.Add(subscriptionList);
            }
            return View(subscriptionLists);

        }
        //registration method
        public ActionResult Register()
        {
            return View();// shows the register page
        }

        [HttpPost]// take the info from the page and puts it in the database
        public ActionResult Register(RegisterFormVM registerFormVM)
        {
            UserAccount account = registerFormVM.userAccount;
            AnAddress address = registerFormVM.address;
            Debug.Write(account.toString());
            MySqlParameter param;
            ViewBag.Success = false;

            NewWebSubContext context = HttpContext.RequestServices.GetService(typeof(new_websub.NewWebSubContext)) as NewWebSubContext;
            using (MySqlConnection conn = context.GetConnection())
            {
                try
                {
                    conn.Open();
                    // check if the email and username is unique
                    string query = "select * from useraccounts where UserName=@username or Email=@email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // username
                    param = new MySqlParameter("@username", account.UserName);
                    param.MySqlDbType = MySqlDbType.VarChar;
                    cmd.Parameters.Add(param);

                    // email
                    param = new MySqlParameter("@email", account.Email);
                    param.MySqlDbType = MySqlDbType.VarChar;
                    cmd.Parameters.Add(param);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ViewBag.Message = account.UserName + " or " + account.Email + " is already exist.";
                    }
                    else
                    {

                        // success
                        ViewBag.Success = true;

                    }
                    reader.Close();

                    if (ViewBag.Success)
                    {
                        // insert into address

                        string query1 = "Insert into address(Address1, Address2, City, State, " +
                            "Zipcode, Country, Address_Created, Address_Modified) values(@Address1, @Address2, @City, @State, " +
                            "@Zipcode, @Country, @Address_Created, @Address_Modified)";

                        MySqlCommand cmd1 = new MySqlCommand(query1, conn);
                        MySqlParameter param1;
                        // First name
                        param1 = new MySqlParameter("@Address1", address.Address1);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        param1 = new MySqlParameter("@Address2", address.Address2);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        // Last name
                        param1 = new MySqlParameter("@City", address.City);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        // User name
                        param1 = new MySqlParameter("@State", address.State);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        // Email
                        param1 = new MySqlParameter("@Zipcode", address.Zipcode);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        param1 = new MySqlParameter("@Country", address.Country);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);


                        param1 = new MySqlParameter("@Address_Created", DateTime.Now);
                        param1.MySqlDbType = MySqlDbType.DateTime;
                        cmd1.Parameters.Add(param1);

                        param1 = new MySqlParameter("@Address_Modified", DateTime.Now);
                        param1.MySqlDbType = MySqlDbType.DateTime;
                        cmd1.Parameters.Add(param1);

                        cmd1.ExecuteNonQuery();
                        query1 = "select * from address";
                        cmd1 = new MySqlCommand(query1, conn);
                        reader = cmd1.ExecuteReader();
                        int addressKey = 0;
                        while (reader.Read())
                        {
                            addressKey = Convert.ToInt32(reader["addresskey"]);
                        }
                        // get address id
                        reader.Close();

                        // insert into useraccounts;

                        query1 = "Insert into useraccounts(FirstName, LastName, UserName, " +
                            "Email, Password, CompanyName, PhoneNumber, Account_Created, Account_Modified, AddressId) values(@FirstName, @lname, @uname, " +
                            "@email, @pwd, @company, @phone, @account_created, @account_modified, @AddressId)";

                        cmd1 = new MySqlCommand(query1, conn);
                        
                        // First name
                        param1 = new MySqlParameter("@FirstName", account.FirstName);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        // Last name
                        param1 = new MySqlParameter("@lname", account.LastName);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        // User name
                        param1 = new MySqlParameter("@uname", account.UserName);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        // Email
                        param1 = new MySqlParameter("@email", account.Email);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        byte[] tmpPwd = ASCIIEncoding.ASCII.GetBytes(account.Password);
                        byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpPwd);


                        param1 = new MySqlParameter("@pwd", ByteArrayToString(tmpHash));
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        param1 = new MySqlParameter("@company", account.CompanyName);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        param1 = new MySqlParameter("@phone", account.PhoneNumber);
                        param1.MySqlDbType = MySqlDbType.VarChar;
                        cmd1.Parameters.Add(param1);

                        param1 = new MySqlParameter("@account_created", DateTime.Now);
                        param1.MySqlDbType = MySqlDbType.DateTime;
                        cmd1.Parameters.Add(param1);

                        param1 = new MySqlParameter("@account_modified", DateTime.Now);
                        param1.MySqlDbType = MySqlDbType.DateTime;
                        cmd1.Parameters.Add(param1);

                        param1 = new MySqlParameter("@AddressId", addressKey);
                        param1.MySqlDbType = MySqlDbType.Int32;
                        cmd1.Parameters.Add(param1);

                        cmd1.ExecuteNonQuery();
                        ViewBag.Message = "A new member " + account.FirstName + " is added successfully";

                        HttpContext.Session.SetInt32("isLoggedIn", 1);
                        HttpContext.Session.SetString("User", account.Email);

                        return RedirectToAction(nameof(Index));
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.ToString();
                }
            }

            return View();
        }


        //login method
        public ActionResult Login()
        {
            return View();// show the login page
        }

        [HttpPost]//get the login info 
        public ActionResult Login(UserAccount user)
        {
            if (ModelState["UserName"].Errors.Any() || ModelState["Password"].Errors.Any())// check the Username and password are entered
            {
                ModelState.AddModelError("UserName", "UserName and Password are required");
            }
            else
            {
                NewWebSubContext context = HttpContext.RequestServices.GetService(typeof(new_websub.NewWebSubContext)) as NewWebSubContext;
                using (MySqlConnection conn = context.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        // check username and password
                        string query = "select * from useraccounts where UserName=@username";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        MySqlParameter param = new MySqlParameter("@username", user.UserName);
                        param.MySqlDbType = MySqlDbType.VarChar;
                        cmd.Parameters.Add(param);

                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            string dbHashedPwd = reader["Password"].ToString();
                            byte[] tmpPwd = ASCIIEncoding.ASCII.GetBytes(user.Password);
                            byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpPwd);
                            string newHashedPwd = ByteArrayToString(tmpHash);
                            if (newHashedPwd.Equals(dbHashedPwd))
                            {
                                ViewBag.Message = "Login Successfully";
                                HttpContext.Session.SetInt32("isLoggedIn", 1);
                                HttpContext.Session.SetString("User", reader["Email"].ToString());
                                
                                return Redirect("/account");
                            }
                            else
                            {
                                ViewBag.Message = "Login failed, password is incorrect.";
                            }
                        }
                        else
                        {
                            ViewBag.Message = "No account for this User name";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = ex.ToString();
                        return View();
                    }
                }
            }
            return View();
        }

       

        public ActionResult LoggedIn()
        {
            if (HttpContext.Session.GetInt32("userId") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet] // get type of method follows this 
        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]// post type of method follows this 
        public async Task<ActionResult> ForgotPassword(EmailFormVM emailFormVM)
        {
            try
            {
                HttpRequest request = HttpContext.Request;
                string token = this.RandomString(10);

                byte[] tmpToken = ASCIIEncoding.ASCII.GetBytes(token);
                byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpToken);
                string newHashedToken = ByteArrayToString(tmpHash);
                NewWebSubContext context = HttpContext.RequestServices.GetService(typeof(new_websub.NewWebSubContext)) as NewWebSubContext;
                using (MySqlConnection conn = context.GetConnection())
                {
                    conn.Open();
                    string query = "select * from useraccounts where Email=@Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlParameter param = new MySqlParameter("@Email", emailFormVM.toEmail);
                    param.MySqlDbType = MySqlDbType.VarChar;
                    cmd.Parameters.Add(param);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        ViewBag.message = "This email is not exist.";
                        return View();
                        
                    }
                   
                    reader.Close();

                    // insert hashed_token inside the database
                    try
                    {
                        query = "Update useraccounts set Hashed_Token=@token WHERE Email=@Email";
                        cmd = new MySqlCommand(query, conn);
                        param = new MySqlParameter("@token", newHashedToken);
                        param.MySqlDbType = MySqlDbType.VarChar;
                        cmd.Parameters.Add(param);

                        param = new MySqlParameter("@Email", emailFormVM.toEmail);
                        param.MySqlDbType = MySqlDbType.VarChar;
                        cmd.Parameters.Add(param);

                        cmd.ExecuteNonQuery();

                        // mail sent

                        string redirectLinnk = request.Host.ToString() + "/Account/ResetPassword?email=" + emailFormVM.toEmail + "&token=" + token;
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(_emailSettings.Value.PrimaryEmail);
                        mail.Subject = "Reset password";
                        mail.IsBodyHtml = true;
                        mail.Body = "<p>Please click following url to reset your password <a href='" + redirectLinnk + "'>" +redirectLinnk +"</a></p>";
                        mail.Sender = new MailAddress(_emailSettings.Value.PrimaryEmail);
                        mail.IsBodyHtml = true;
                        mail.To.Add(emailFormVM.toEmail);
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = _emailSettings.Value.PrimaryDomain; //Or Your SMTP Server Address
                        smtp.Port = _emailSettings.Value.PrimaryPort;
                        smtp.UseDefaultCredentials = false;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Credentials = new System.Net.NetworkCredential(_emailSettings.Value.PrimaryEmail, _emailSettings.Value.PrimaryPassword);
                        //Or your Smtp Email ID and Password
                        smtp.EnableSsl = _emailSettings.Value.EnableSsl;

                        smtp.Send(mail); // should be removed for local testing

                        ViewBag.message = "Recovery Email has been sent, please check your mail box.";
                        // ViewBag.message = redirectLinnk; // for only testing in local

                    }
                    catch (Exception ex1)
                    {
                        ViewBag.message = ex1.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetpasswordForm model)
        {
            if(model.NewPassword == null)
            {
                ViewBag.message = "Password is required";
                return View();
            }
            if(!model.NewPassword.Equals(model.NewPasswordConfirm))
            {
                ViewBag.message = "Password is mismatch.";
                return View();
            }
            string email = model.Email;
            NewWebSubContext context = HttpContext.RequestServices.GetService(typeof(new_websub.NewWebSubContext)) as NewWebSubContext;
            using (MySqlConnection conn = context.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "select * from useraccounts where Email=@Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlParameter param = new MySqlParameter("@Email", email);
                    param.MySqlDbType = MySqlDbType.VarChar;
                    cmd.Parameters.Add(param);
                    
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        ViewBag.message = "This email is not valid.";
                        return View();

                    }
                    string hashedToken = reader["Hashed_Token"].ToString();
                    reader.Close();
                    byte[] tmpToken = ASCIIEncoding.ASCII.GetBytes(model.token);
                    byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpToken);
                    string newHashedToken = ByteArrayToString(tmpHash);
                    if (!newHashedToken.Equals(hashedToken))
                    {
                        ViewBag.message = "Token is not valid";
                        return View();
                    }

                    // reset new password
                    try
                    {
                        byte[] tmpPwd = ASCIIEncoding.ASCII.GetBytes(model.NewPassword);
                        byte[] tmpPwdHash = new MD5CryptoServiceProvider().ComputeHash(tmpPwd);
                        string newHashedPwd = ByteArrayToString(tmpPwdHash);

                        query = "update useraccounts set Password=@Password where Email=@Email";
                        cmd = new MySqlCommand(query, conn);
                        param = new MySqlParameter("@Password", newHashedPwd);
                        param.MySqlDbType = MySqlDbType.VarChar;
                        cmd.Parameters.Add(param);

                        param = new MySqlParameter("@Email", email);
                        param.MySqlDbType = MySqlDbType.VarChar;
                        cmd.Parameters.Add(param);
                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Login));
                    }
                    catch(Exception ex2)
                    {
                        ViewBag.message = ex2.Message;
                        return View();
                    }
                    
                }
                catch (Exception ex)
                {
                    ViewBag.message = ex.Message;
                    return View();
                }
            }
            
        }
        public ActionResult ResetPassword(string email, string token)
        {
            ResetpasswordForm resetpasswordForm = new ResetpasswordForm();
            resetpasswordForm.Email = email;
            resetpasswordForm.token = token;
            return View(resetpasswordForm);
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult Subscription()
        {
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            if (ViewBag.isLoggedIn != 1)
            {
                return RedirectToAction(nameof(Login));
            }
           
            CustomerPaymentViewModel payment = new CustomerPaymentViewModel();
            payment.massage = "";
            return View(payment);
        }

        [HttpPost]
        public IActionResult Subscription(CustomerPaymentViewModel payment)
        {
            string email = HttpContext.Session.GetString("User");
            ViewBag.isLoggedIn = 1;
            
            try
            {
                NewWebSubContext context = HttpContext.RequestServices.GetService(typeof(new_websub.NewWebSubContext)) as NewWebSubContext;

                string query = "select * from useraccounts u inner join address a on u.AddressId=a.addresskey where u.Email=@Email";
                MySqlConnection conn = context.GetConnection();

                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlParameter param = new MySqlParameter("@Email", email);
                param.MySqlDbType = MySqlDbType.VarChar;
                cmd.Parameters.Add(param);
                MySqlDataReader reader = cmd.ExecuteReader();
                
                if (reader.Read())
                {
                    User user = new User();
                    user.Email = email;
                    user.Id = reader["UserID"].ToString();
                    user.FullName = reader["UserName"].ToString();
                    user.StripeCustomerId = "";
                    user.AddressLine1 = reader["Address1"].ToString();
                    user.AddressLine2 = reader["Address2"].ToString();
                    user.City = reader["City"].ToString();
                    user.State = reader["State"].ToString();
                    user.Zip = reader["Zipcode"].ToString();
                    user.Country = reader["Country"].ToString();
                    user.HistoryView = true;

                    StripeConfiguration.SetApiKey(_stripeSettings.Value.SecretKey);

                    var tokenoptions = new TokenCreateOptions
                    {
                        Card = new CreditCardOptions
                        {
                            Number = payment.CardNumber,
                            ExpYear = payment.ExpiryYear,
                            ExpMonth = payment.ExpiryMonth,
                            Cvc = payment.Cvc
                        }
                    };

                    var tokenservice = new TokenService();
                    Token stripeToken = tokenservice.Create(tokenoptions);
                    payment.cardtoken = stripeToken.Id;
                    CustomerCreateOptions customerCreateOptions = GetCustomerCreateOptions(payment, user);
                    var cusservice = new CustomerService();
                    var customers = cusservice.Create(customerCreateOptions);
                    Subscription subscription;
                    var plservice = new PlanService();
                    try
                    {
                        var plplan = plservice.Get(payment.subsctype);

                        var items = new List<SubscriptionItemOption> {
                            new SubscriptionItemOption {
                                PlanId = plplan.Id
                            }
                        };
                        var suboptions = new SubscriptionCreateOptions
                        {
                            CustomerId = customers.Id,
                            Items = items
                        };

                        var subservice = new SubscriptionService();
                        subscription = subservice.Create(suboptions);
                    }
                    catch
                    {
                        var options = new PlanCreateOptions
                        {
                            Product = new PlanProductCreateOptions
                            {
                                Id = payment.subsctype,
                                Name = payment.subsctype
                            },
                            Amount = payment.Amount,
                            Currency = payment.Currency,
                            Interval = payment.subsctype,
                            Id = payment.subsctype
                        };

                        var service = new PlanService();
                        Plan plan = service.Create(options);
                        var items = new List<SubscriptionItemOption> {
                            new SubscriptionItemOption {
                                PlanId = plan.Id
                            }
                        };
                        var suboptions = new SubscriptionCreateOptions
                        {
                            CustomerId = customers.Id,
                            Items = items
                        };

                        var subservice = new SubscriptionService();
                        subscription = subservice.Create(suboptions);
                    }

                    reader.Close();
                    // insert into subscriptions table

                    query = "insert into subscriptions(Email, CustomerId, SubscriptionId, Subscription_Started, Subscription_Ended) values(@Email," +
                        "@CustomerId, @SubscriptionId, @Subscription_Started, @Subscription_Ended)";
                    MySqlCommand cmd1 = new MySqlCommand(query, conn);
                    MySqlParameter param1 = new MySqlParameter("@Email", user.Email);
                    param1.MySqlDbType = MySqlDbType.VarChar;
                    cmd1.Parameters.Add(param1);

                    param1 = new MySqlParameter("@CustomerId", subscription.CustomerId);
                    param1.MySqlDbType = MySqlDbType.VarChar;
                    cmd1.Parameters.Add(param1);

                    param1 = new MySqlParameter("@SubscriptionId", subscription.Id);
                    param1.MySqlDbType = MySqlDbType.VarChar;
                    cmd1.Parameters.Add(param1);

                    param1 = new MySqlParameter("@Subscription_Started", subscription.StartDate);
                    param1.MySqlDbType = MySqlDbType.DateTime;
                    cmd1.Parameters.Add(param1);

                    param1 = new MySqlParameter("@Subscription_Ended", subscription.EndedAt);
                    param1.MySqlDbType = MySqlDbType.DateTime;
                    cmd1.Parameters.Add(param1);

                    cmd1.ExecuteNonQuery();

                    HttpContext.Session.SetInt32("isLoggedIn", 1);
                    payment.massage = "Payment created successfully";

                    //return View("Success"); // render Success.cshtml
                    return View(payment);
                }
                else
                {
                    return RedirectToAction(nameof(Login));
                }

                
            }
            catch (Exception ex)
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_emailSettings.Value.PrimaryEmail);
                mail.Subject = "Subscription Fail";
                mail.IsBodyHtml = true;
                mail.Body = ex.Message;
                mail.Sender = new MailAddress(_emailSettings.Value.PrimaryEmail);
                mail.To.Add(email);
                SmtpClient smtp = new SmtpClient();
                smtp.Host = _emailSettings.Value.PrimaryDomain; //Or Your SMTP Server Address
                smtp.Port = _emailSettings.Value.PrimaryPort;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(_emailSettings.Value.PrimaryEmail, _emailSettings.Value.PrimaryPassword);
                //Or your Smtp Email ID and Password
                smtp.EnableSsl = _emailSettings.Value.EnableSsl;
                smtp.Send(mail);
                payment.massage = ex.Message;
                return View(payment);
            }
        }


        private static CustomerCreateOptions GetCustomerCreateOptions(CustomerPaymentViewModel payment, User user)
        {
            return new CustomerCreateOptions
            {
                Email = user.Email,
                Description = $"{user.Email} {user.Id}",
                Source = payment.cardtoken
            };
        }

        public IActionResult Cancel(string id)
        {
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            if (ViewBag.isLoggedIn != 1)
            {
                return RedirectToAction(nameof(Login));
            }
            StripeConfiguration.SetApiKey(_stripeSettings.Value.SecretKey);

            var service = new SubscriptionService();
            var subscription = service.Cancel(id, null);
            return RedirectToAction(nameof(Index));
        }

    }
}