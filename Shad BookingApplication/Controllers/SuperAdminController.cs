using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shad_BookingApplication.Controllers
{
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CheckLogin()
        {
            return View();
        }
    }
}