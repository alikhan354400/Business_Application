using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Custom
{
    public class SiteAuthAttribute : AuthorizeAttribute
    {

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Controller.TempData["ErrorDetails"] = "You must be logged in to access this page";
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Controller.TempData["ErrorDetails"] = "You don't have access rights to this page";
                filterContext.Result = new RedirectResult("~/Site/Unauthorized");
                return;
            }
        }

    }
}