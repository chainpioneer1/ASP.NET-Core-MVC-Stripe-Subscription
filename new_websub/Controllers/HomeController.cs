using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using new_websub.Models;

namespace new_websub.Controllers
{
    public class HomeController : Controller
    {

        public List<AnAddress> GetAllAddress(NewWebSubContext context)
        {
            List<AnAddress> list = new List<AnAddress>();

            using (MySqlConnection conn = context.GetConnection())
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from address", conn);
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            list.Add(new AnAddress()
                            {
                                addresskey = Convert.ToInt32(reader["addresskey"]),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString(),
                                Zipcode = reader["Zipcode"].ToString()
                            });
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

            }
            return list;
        }

        public IActionResult Index()// henry (observing) comes here first because of routing on startup.cs
        {
            //NewWebSubContext context = HttpContext.RequestServices.GetService(typeof(new_websub.NewWebSubContext)) as NewWebSubContext;
            //return View(GetAllAddress(context)); //shows the index view page
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            return View();
        }

        public IActionResult About()// comes here because of the action on the button in layout page navbar
        {
            ViewData["Message"] = "Your application description page.";
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
