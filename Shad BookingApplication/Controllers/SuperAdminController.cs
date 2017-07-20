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
            List<UserListViewModel> superadmin = new List<UserListViewModel>();

            foreach (var item in db.AspNetUsers)
            {
                var superadmin_item = new UserListViewModel();
                superadmin_item.id = item.Id;


                var companyname = db.AspNetCustomerDetails.Where(x => x.Id == Convert.ToInt32(item.Id)).Select(x => x.BussinessName).FirstOrDefault();
                superadmin.Add(superadmin_item);
            }

            List<UserListViewModel> Companyadmin = new List<UserListViewModel>();
            foreach (var item in db.AspNetUsers)
            {
                var Companyadmin_item = new UserListViewModel();
                Companyadmin_item.id = item.Id;




                Companyadmin.Add(Companyadmin_item);
            }


                //var user = db.AspNetUsers.Select(x => new {
                //    x.LastName,
                //    x.Id,
                //    x.FirstName,
                //    x.UserName,
                //    x.Status

                //}).ToList();

                //List<string> Roles = new List<string>();

                //for (int i=0;i< user.Count();i++)
                //{
                //    var item = user.ElementAt(i);
                //    var role = db.AspNetUsers.Where(x=> x.Id == item.Id).Select(x => x.AspNetRoles.Select(y => y.Name)).FirstOrDefault().ToString();
                //    Roles.Add(role);
                //}
                //List<string> Comp = new List<string>();
                //for (int i = 0; i < user.Count(); i++)
                //{
                //    var item = user.ElementAt(i);
                //    var comp = db.AspNetUsers.Where(x => x.Id == item.Id).Select(x => x.AspNetCustomerDetail.Select(y => y.Company)).FirstOrDefault().ToString();
                //    Comp.Add(comp);
                //}



                //ViewBag.company = Comp;
                //ViewBag.userdata = user;
                //ViewBag.Roles = Roles;
                return View();
        }
        public ActionResult AddSuperAdmin()
        {
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
            var items = db.AspNetItems.ToList();
            return View(items);
        }

   

    }
}