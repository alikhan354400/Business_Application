using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [Authorize]
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class SiteController : BaseController
    {
        [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

        public ContentResult ResetDatabase()
        {
            using (var db = new HassanAyoubDBEntities())
            {
                if (db.ResetDatabase() > 0)
                {
                    return Content("Database Reset Successfully", "text/plain", Encoding.UTF8);

                }
                else
                {
                    return Content("Database is not reset", "text/plain", Encoding.UTF8);
                }
            }

        }
    }
}