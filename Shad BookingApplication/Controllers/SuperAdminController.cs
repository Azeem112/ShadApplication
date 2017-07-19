using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Shad_BookingApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shad_BookingApplication.Controllers
{
    public class SuperAdminController : Controller
    {
        private BookingModelEntities db = new BookingModelEntities();

        // GET: SuperAdmin
        public ActionResult Index()
        {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSuperAdmin([Bind(Include = "Email,PasswordHash,UserName,PhoneNumber,Status")] AspNetUser aspNetUser)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser();
                    user.UserName = aspNetUser.UserName;
                    user.Email = aspNetUser.Email;
                    user.PhoneNumber = aspNetUser.PhoneNumber;
                    string userPWD = aspNetUser.PasswordHash;
                    var chkUser = UserManager.Create(user, userPWD);


                    //Add default User to Role Admin   
                    if (chkUser.Succeeded)
                    {
                        var result1 = UserManager.AddToRole(user.Id, "Super_Admin");
                        var temp_user = db.AspNetUsers.Where(m => m.Email == user.Email).Select(x => x).FirstOrDefault();
                        temp_user.Status = aspNetUser.Status;
                        db.SaveChanges();
                    }


                    return RedirectToAction("AddSuperAdmin");
                }
            }
            catch(Exception ex)
            {

            }
            
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