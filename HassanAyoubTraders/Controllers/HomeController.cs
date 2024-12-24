using HassanAyoubTraders.Custom;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
           

            return View();
        }

    }
}