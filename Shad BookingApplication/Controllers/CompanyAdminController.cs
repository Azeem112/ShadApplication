using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Shad_BookingApplication.Models;
using System.Web.Mvc;
using System;
using System.Linq;

namespace Shad_BookingApplication.Controllers
{

    public class CompanyAdminController : Controller
    {
        private BookingModelEntities db = new BookingModelEntities();
        // GET: CompanyAdmin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser([Bind (Include = "Email,PasswordHash,UserName,PhoneNumber,Status")] AspNetUser aspnetuser,[Bind (Include="Name")] AspNetRole role)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Context));
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser();
                    user.UserName = aspnetuser.UserName;
                    user.Email = aspnetuser.Email;
                    string Pass = aspnetuser.PasswordHash;
                    user.PhoneNumber = aspnetuser.PhoneNumber;
                    var ChkUser = userManager.Create(user, Pass);
                    if(ChkUser.Succeeded)
                    {
                        var role_name=role.Name;
                        var result = userManager.AddToRole(user.Id, role_name );
                        var TemUser = db.AspNetUsers.Where(y => y.Email == user.Email).Select(x => x).FirstOrDefault();
                        TemUser.Status = aspnetuser.Status;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("AddUser");
            }
            catch( Exception ex)
            { }
            

            return View();
        }
        public ActionResult AddAgency()
        {
            return View();
        }
        public ActionResult AddCustomer()
        {
            return View();
        }

        public ActionResult AddEmployee()
        {
            return View();
        }

        public ActionResult AddItem()
        {
            return View();
        }

        public ActionResult AddService()
        {
            var ls = db.AspNetTaxes.ToList();
            ViewBag.tax_list = ls;
            ViewBag.service_group__list = db.AspNetServiceGroups.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult AddService(AspNetService aspNetService)
        {

            return View();
        }

        public ActionResult AddServiceGroup()
        {
            return View();
        }

        public ActionResult AddVoucher()
        {
            return View();
        }
        [HttpPost]
       
        public ActionResult AddVoucher(AspNetGiftVoucher voucher)
        {
            if(ModelState.IsValid)
            {
                AspNetGiftVoucher g_voucher = new AspNetGiftVoucher();
                db.AspNetGiftVouchers.Add(voucher);
                db.SaveChanges();
            }
            return RedirectToAction("VoucherList");
        }

        public ActionResult AgencyList()
        {
            return View();
        }

        public ActionResult CompanyProfile()
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

        public ActionResult CustomerDetails()
        {
            return View();
        }


        public ActionResult CustomerList()
        {
            return View();
        }

        public ActionResult EditCustomer()
        {
            return View();
        }


        public ActionResult EditService()
        {
            return View();
        }


        public ActionResult EditServiceGroup()
        {
            return View();
        }


        public ActionResult EmployeeList()
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


        public ActionResult Login()
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


        public ActionResult Report()
        {
            return View();
        }


        public ActionResult SalesDiscountList()
        {
            return View();
        }


        public ActionResult SalesDiscountListCara()
        {
            return View();
        }


        public ActionResult ServiceGroupList()
        {
            return View();
        }


        public ActionResult ServiceList()
        {
            return View();
        }


        public ActionResult UserList()
        {
            return View();
        }


        public ActionResult ViewService()
        {
            return View();
        }


        public ActionResult ViewServiceGroup()
        {
            return View();
        }
        public ActionResult VoucherList()
        {
            return View();
        }






    }
}