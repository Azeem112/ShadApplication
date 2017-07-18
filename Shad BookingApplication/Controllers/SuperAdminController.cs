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

        public ActionResult UserList()
        {
            return View();
        }
        public ActionResult AddSuperAdmin()
        {
            return View();
        }

        public ActionResult AddItem()
        {
            return View();
        }

        public ActionResult LoginDetails()
        {
            return View();
        }

        public ActionResult OptionsOk()
        {
            return View();
        }

        public ActionResult EditCustomer()
        {
            return View();
        }

        public ActionResult AddCustomer()
        {
            return View();
        }

        public ActionResult Create_Invoice()
        {
            return View();
        }
        public ActionResult Create_Invoice2()
        {
            return View();
        }
        public ActionResult CustomersList()
        {
            return View();
        }
        public ActionResult InvoiceList()
        {
            return View();
        }
        public ActionResult InvoiceOption()
        {
            return View();
        }

        public ActionResult InvoiceTemplate()
        {
            return View();
        }

        public ActionResult ItemList()
        {
            return View();
        }

   

    }
}