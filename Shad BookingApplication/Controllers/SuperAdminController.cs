using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Shad_BookingApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shad_BookingApplication.Controllers
{
    [Authorize(Roles = "Super_Admin")]   
    public class SuperAdminController : Controller
    {
        private BookingModelEntities db = new BookingModelEntities();

        // GET: SuperAdmin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddBusinessCategory()
        {
            return View();
        }

        public ActionResult AddBusinessSubCategory()
        {
            var category_ls = db.AspNetBusinessCatageories.ToList();
            ViewBag.category_list = category_ls;
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
            dynamic mymodel = new ExpandoObject(); 
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBusinessCategory([Bind(Include="Name")] AspNetBusinessCatageory aspNetBusinessCatageory)
        {
            db.AspNetBusinessCatageories.Add(aspNetBusinessCatageory);
            db.SaveChanges();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBusinessSubCategory([Bind(Include = "Name,BussinessCatageoryId")] AspNetBusinessSubCatageory aspNetBusinessSubCatageory)
        {
            db.AspNetBusinessSubCatageories.Add(aspNetBusinessSubCatageory);
            db.SaveChanges();
            return View();
        }


        [HttpPost]
        [Authorize]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem([Bind(Include = "Id,Name,Description,Vat,Price_W_O_Vat,Price_W__Vat,Status,IsSmsPackage")] AspNetItem aspNetItem)
        {
            if (ModelState.IsValid)
            {
                db.AspNetItems.Add(aspNetItem);
                db.SaveChanges();

                if (aspNetItem.IsSmsPackage.Equals("sms"))
                {
                    //var obj=db.AspNetItems.Single(x=>x== aspNetItem);
                    var smspackage = new AspNetCustomerSM();
                    smspackage.ItemID = aspNetItem.Id;
                    db.AspNetCustomerSMS.Add(smspackage);
                    db.SaveChanges();
                }
                return RedirectToAction("AddItem");
            }

            return View(aspNetItem);
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
            AddCustomerViewModel addCustomerViewModel = new AddCustomerViewModel();
            addCustomerViewModel.BusinessCatageory = db.AspNetBusinessCatageories.ToList();
            addCustomerViewModel.SMS = db.AspNetCustomerSMS.ToList();
            return View(addCustomerViewModel);
        }

        [HttpPost]
        
        public ActionResult AddCustomer(AddCustomerViewModel addCustomerViewModel)
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