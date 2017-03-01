using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ModernTricks.Controllers
{
    public class UnauthorisedController : Controller
    {
        // GET: Unauthorised
        public ActionResult Index()
        {
            Session.Abandon();
            return View();
        }

        public ActionResult Error(string _errorMsg)
        {
            ViewBag.ErrorMsg = _errorMsg;
            return View();
        }
    }
}