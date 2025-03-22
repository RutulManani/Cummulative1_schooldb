using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cummulative1_schooldb.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Controller responsible for handling the main page of the school database system.
        /// </summary>
        public ActionResult Index()
        {
            // Set the page title
            ViewBag.Title = "Welcome to School Management System";
            // Display the home page view
            return View();
        }
    }
}
