using Shad_BookingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shad_BookingApplication.Controllers
{
    public class SuperAdminController : Controller
    {
        private BookingDbContext dbcontext = new BookingDbContext();
        // GET: SuperAdmin
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult CheckLogin(string email_id, string pass)
        {

            var obj = dbcontext.Users.Where(x => x.Email.Equals(""));
            if (obj.Count() != 0)
            {

            }

            //String email_id,String pass
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
    }
}