using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineAbit2013.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string lang)
        {
            if (Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1 && Util.bUseRedirection &&
                Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
                return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

            Guid g;
            if (!Util.CheckAuthCookies(Request.Cookies, out g))
                return RedirectToAction("LogOn", "Account");
            else
                return RedirectToAction("Main", "AbiturientNew");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult AboutRussianCitizens()
        {
            Util.SetThreadCultureByCookies(Request.Cookies);
            return View();
        }
    }
}
