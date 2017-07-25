using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Shad_BookingApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
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
         /*   List<UserListViewModel> superadmin = new List<UserListViewModel>();

            //foreach (var item in db.AspNetUsers)
            //{
            //    var superadmin_item = new UserListViewModel();
            //    superadmin_item.id = item.Id;


            //    var companyname = db.AspNetCustomerDetails.Where(x => x.Id == Convert.ToInt32(item.Id)).Select(x => x.BussinessName).FirstOrDefault();
            //    superadmin.Add(superadmin_item);
            //}

            //List<UserListViewModel> Companyadmin = new List<UserListViewModel>();
            //foreach (var item in db.AspNetUsers)
            //{
            //    var Companyadmin_item = new UserListViewModel();
            //    Companyadmin_item.id = item.Id;




                Companyadmin.Add(Companyadmin_item);
            }
            */

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
                    var no_sms_package= Request.Form["no_remaining_sms"];
                    smspackage.RemainingSMS = Convert.ToInt16(no_sms_package);
                    smspackage.SmsPackageName = aspNetItem.Name;

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
        public ActionResult AddCustomer(AddCustomerViewModel addCustomerViewModel, HttpPostedFileBase[] files)
        {
                            // Adding Details
                 var list_of_subcat_ids=Get_SubCat_Id(addCustomerViewModel.Customer_SubCatageory);
                 addCustomerViewModel.Detail.SubCatageoryNo = list_of_subcat_ids.Count();

                 db.AspNetCustomerDetails.Add(addCustomerViewModel.Detail);
                 db.SaveChanges();

                             // Adding Location 
                 var loc_country_name = Request.Form["LocCountryName"].ToString();
                 addCustomerViewModel.Location.CountryName = loc_country_name;

                 db.AspNetCustomerLocations.Add(addCustomerViewModel.Location);
                 db.SaveChanges();

                             // Adding Contact
                 db.AspNetCustomerContacts.Add(addCustomerViewModel.Contact);
                 db.SaveChanges();

                             // Adding Region

                 var region_country = Request.Form["region_country"].ToString();
                 var region_timezone = Request.Form["region_timezone"];
                 var region_dateformat = Request.Form["region_dateformat"].ToString();
                 var region_timeformat = Request.Form["region_timeformat"].ToString();
                 var region_currency = Request.Form["region_currency"].ToString();


                 var region = new AspNetCustomerRegion();

                 region.CountryName = region_country;
                 region.TimeZoneName = region_timezone;
                 region.DateFormate = region_dateformat;
                 region.TimeFormate = region_timeformat;
                 region.CurrencyName = region_currency;

                 db.AspNetCustomerRegions.Add(region);
                 db.SaveChanges();


     
            // Adding Gallery 
            var gallary = new AspNetCustomerGallery();
            int i = 1;
            foreach (HttpPostedFileBase file1 in files)
            {
                //Checking file is available to save.  
                if (i > 5)
                    break;

                if (file1 != null)
                {
                    var fileName = Path.GetFileName(file1.FileName);
                    var path = Path.Combine(Server.MapPath("~/ServerFiles"), fileName);
                    file1.SaveAs(path);

                    if (i == 1)
                    {

                        gallary.Image1 = path;
                    }
                    else if (i == 2)
                    {
                        gallary.Image2 = path;
                    }
                    if (i == 3)
                    {
                        gallary.Image3 = path;
                    }
                    else if (i == 4)
                    {
                        gallary.Image4 = path;
                    }
                    if (i == 5)
                    {
                        gallary.Image5 = path;
                    }


                }
                i++;
            }

            db.AspNetCustomerGalleries.Add(gallary);
            db.SaveChanges();

                    // Adding Business Details
            var BusinessDetail = new AspNetCustomerBusinessDetail();
            HttpPostedFileBase file = Request.Files["business_logo_img"];
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/ServerFiles"), fileName);
                file.SaveAs(path);
                BusinessDetail.Logo = path;
                
            }
            else
            {
                BusinessDetail.Logo = "";
            }

            db.AspNetCustomerBusinessDetails.Add(BusinessDetail);
            db.SaveChanges();


            // Adding user Types
            addCustomerViewModel.UserType.CompanyName = addCustomerViewModel.Detail.BussinessName;
            db.AspNetCustomerTypes.Add(addCustomerViewModel.UserType);
            db.SaveChanges();
  
                    // Getting SMS ID
            var sms_package_id = addCustomerViewModel.SingleSms.ItemID;


                    // Adding Working time
            var time = Request.Form["custom_timepicker_name"].ToString();
            var id = time_string_parse(time);

            //  var businessDetails_textarea = Request.Form["editor1"];

                    // Adding Social
            db.AspNetSocials.Add(addCustomerViewModel.Social);
            db.SaveChanges();

            // Adding User
            var user_id=AddCustomerAccount(addCustomerViewModel.User);


            var customer = new AspNetCustomer();
            customer.BussinessID = BusinessDetail.Id;
            customer.RegionID = region.Id;
            customer.LocationId = addCustomerViewModel.Location.Id;
            customer.SmsID = sms_package_id;
            customer.TypeID = addCustomerViewModel.UserType.Id;
            customer.ContactId = addCustomerViewModel.Contact.Id;
            customer.DetailId = addCustomerViewModel.Detail.Id;
            customer.SocialID = addCustomerViewModel.Social.Id;
            customer.UserID = user_id;
            customer.GalleryID = gallary.Id;
            customer.WorkingID = id;

            db.AspNetCustomers.Add(customer);
            db.SaveChanges();

            foreach (var item in list_of_subcat_ids)
            {
                var obj = new AspNetCustomer_SubCatageory();
                obj.CustomerID = customer.Id;
                obj.SubCatageoryId = item;


                db.AspNetCustomer_SubCatageory.Add(obj);
                db.SaveChanges();
            }
            return RedirectToAction("Login", "Account");
        }


        public string AddCustomerAccount(AspNetUser aspNetUser)
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
                        var result1 = UserManager.AddToRole(user.Id, "Company_Admin");
                        var temp_user = db.AspNetUsers.Where(m => m.Email == user.Email).Select(x => x).FirstOrDefault();
                        temp_user.Status = aspNetUser.Status;
                        db.SaveChanges();
                        return temp_user.Id;
                    }


                   
                }
            }
            catch (Exception ex)
            {

            }

            return "";

        }

        public List<int> Get_SubCat_Id(string[] arr)
        {
            List<int> ls_id = new List<int>();

            foreach (var item in arr)
            {
                var id=db.AspNetBusinessSubCatageories.Where(x => x.Name == item).Select(x => x.Id).FirstOrDefault();
                ls_id.Add(id);
            }
            return ls_id;

        }


        private int time_string_parse(string time)
        {
            var mon_obj = new AspNetWorkingTime();
            var tue_obj = new AspNetWorkingTime();
            var wed_obj = new AspNetWorkingTime();
            var thus_obj = new AspNetWorkingTime();
            var fri_obj = new AspNetWorkingTime();
            var sat_obj = new AspNetWorkingTime();
            var sun_obj = new AspNetWorkingTime();

            var time_arr = time.Split('|');
            foreach (var item in time_arr)
            {
                if (item.Equals(""))
                    continue;
                if (item.Contains("Monday"))
                {
                    mon_obj.Day = "Monday";
                    mon_obj.isoff = false;

                    int index = item.IndexOf('-');
                    var str = item.Substring(index);
                    str = str.Replace('-', ' ').TrimStart(' ');
                    var str_arr = str.Split(',');
                    for (int i = 0; i < 4; i++)
                    {
                        var date_str = str_arr.ElementAt(i);
                        if (date_str.Equals(""))
                            continue;

                        if (i == 0)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            mon_obj.StartTime = date_val;
                        }
                        else if (i == 1)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            mon_obj.EndTime = date_val;
                        }
                        else if (i == 2)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            mon_obj.LunchFrom = date_val;
                        }
                        else if (i == 3)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            mon_obj.LunchToo = date_val;
                        }

                    }
                    db.AspNetWorkingTimes.Add(mon_obj);
                    db.SaveChanges();
                } //end if mon
                else if (item.Contains("Tuesday"))
                {
                    tue_obj.Day = "Tuesday";
                    tue_obj.isoff = false;

                    int index = item.IndexOf('-');
                    var str = item.Substring(index);
                    str = str.Replace('-', ' ').TrimStart(' ');
                    var str_arr = str.Split(',');
                    for (int i = 0; i < 4; i++)
                    {
                        var date_str = str_arr.ElementAt(i);
                        if (date_str.Equals(""))
                            continue;

                        if (i == 0)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            tue_obj.StartTime = date_val;
                        }
                        else if (i == 1)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            tue_obj.EndTime = date_val;
                        }
                        else if (i == 2)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            tue_obj.LunchFrom = date_val;
                        }
                        else if (i == 3)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            tue_obj.LunchToo = date_val;
                        }

                    }
                    db.AspNetWorkingTimes.Add(tue_obj);
                    db.SaveChanges();
                }// end if tue
                else if (item.Contains("Wednesday"))
                {
                    wed_obj.Day = "Wednesday";
                    wed_obj.isoff = false;

                    int index = item.IndexOf('-');
                    var str = item.Substring(index);
                    str = str.Replace('-', ' ').TrimStart(' ');
                    var str_arr = str.Split(',');
                    for (int i = 0; i < 4; i++)
                    {
                        var date_str = str_arr.ElementAt(i);
                        if (date_str.Equals(""))
                            continue;

                        if (i == 0)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            wed_obj.StartTime = date_val;
                        }
                        else if (i == 1)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            wed_obj.EndTime = date_val;
                        }
                        else if (i == 2)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            wed_obj.LunchFrom = date_val;
                        }
                        else if (i == 3)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            wed_obj.LunchToo = date_val;
                        }

                    }
                    db.AspNetWorkingTimes.Add(wed_obj);
                    db.SaveChanges();
                }// end if Wed
                else if (item.Contains("Thursday"))
                {
                    thus_obj.Day = "Thursday";
                    thus_obj.isoff = false;

                    int index = item.IndexOf('-');
                    var str = item.Substring(index);
                    str = str.Replace('-', ' ').TrimStart(' ');
                    var str_arr = str.Split(',');
                    for (int i = 0; i < 4; i++)
                    {
                        var date_str = str_arr.ElementAt(i);
                        if (date_str.Equals(""))
                            continue;

                        if (i == 0)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            thus_obj.StartTime = date_val;
                        }
                        else if (i == 1)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            thus_obj.EndTime = date_val;
                        }
                        else if (i == 2)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            thus_obj.LunchFrom = date_val;
                        }
                        else if (i == 3)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            thus_obj.LunchToo = date_val;
                        }

                    }
                    db.AspNetWorkingTimes.Add(thus_obj);
                    db.SaveChanges();
                }// end if thus
                else if (item.Contains("Friday"))
                {
                    fri_obj.Day = "Friday";
                    fri_obj.isoff = false;

                    int index = item.IndexOf('-');
                    var str = item.Substring(index);
                    str = str.Replace('-', ' ').TrimStart(' ');
                    var str_arr = str.Split(',');
                    for (int i = 0; i < 4; i++)
                    {
                        var date_str = str_arr.ElementAt(i);
                        if (date_str.Equals(""))
                            continue;

                        if (i == 0)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            fri_obj.StartTime = date_val;
                        }
                        else if (i == 1)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            fri_obj.EndTime = date_val;
                        }
                        else if (i == 2)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            fri_obj.LunchFrom = date_val;
                        }
                        else if (i == 3)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            fri_obj.LunchToo = date_val;
                        }

                    }
                    db.AspNetWorkingTimes.Add(fri_obj);
                    db.SaveChanges();
                }// end if fri
                else if (item.Contains("Saturday"))
                {
                    sat_obj.Day = "Saturday";
                    sat_obj.isoff = false;

                    int index = item.IndexOf('-');
                    var str = item.Substring(index);
                    str = str.Replace('-', ' ').TrimStart(' ');
                    var str_arr = str.Split(',');
                    for (int i = 0; i < 4; i++)
                    {
                        var date_str = str_arr.ElementAt(i);
                        if (date_str.Equals(""))
                            continue;

                        if (i == 0)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            sat_obj.StartTime = date_val;
                        }
                        else if (i == 1)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            sat_obj.EndTime = date_val;
                        }
                        else if (i == 2)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            sat_obj.LunchFrom = date_val;
                        }
                        else if (i == 3)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            sat_obj.LunchToo = date_val;
                        }

                    }
                    db.AspNetWorkingTimes.Add(sat_obj);
                    db.SaveChanges();
                }// end if sat
                else  if (item.Contains("Sunday"))
                {
                    sun_obj.Day = "Sunday";
                    sun_obj.isoff = false;
                    int index = item.IndexOf('-');
                    var str = item.Substring(index);
                    str = str.Replace('-', ' ').TrimStart(' ');
                    var str_arr = str.Split(',');
                    for (int i = 0; i < 4; i++)
                    {
                        var date_str = str_arr.ElementAt(i);
                        if (date_str.Equals(""))
                            continue;

                        if (i == 0)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            sun_obj.StartTime = date_val;
                        }
                        else if (i == 1)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            sun_obj.EndTime = date_val;
                        }
                        else if (i == 2)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            sun_obj.LunchFrom = date_val;
                        }
                        else if (i == 3)
                        {
                            var date_val = Convert.ToDateTime(date_str);
                            sun_obj.LunchToo = date_val;
                        }

                    }
                    db.AspNetWorkingTimes.Add(sun_obj);
                    db.SaveChanges();
                }// end if sat

            }// end foreach



            var timetable = new AspNetWorkingWeekTime();


           
            if (mon_obj.Id == 0)
            {
                mon_obj.Day = "Monday";
                mon_obj.isoff = true;
                db.AspNetWorkingTimes.Add(mon_obj);
                db.SaveChanges();
            }

            if (tue_obj.Id == 0)
            {
                tue_obj.Day = "Tuesday";
                tue_obj.isoff = true;
                db.AspNetWorkingTimes.Add(tue_obj);
                db.SaveChanges();
            }
           
                
            if (wed_obj.Id == 0)
            {
                wed_obj.Day = "Wednesday";
                wed_obj.isoff = true;
                db.AspNetWorkingTimes.Add(wed_obj);
                db.SaveChanges();
            }

            if (thus_obj.Id == 0)
            {
                thus_obj.Day = "Thursday";
                thus_obj.isoff = true;
                db.AspNetWorkingTimes.Add(thus_obj);
                db.SaveChanges();
            }
           
            if (fri_obj.Id == 0)
            {
                fri_obj.Day = "Friday";
                fri_obj.isoff = true;
                db.AspNetWorkingTimes.Add(fri_obj);
                db.SaveChanges();
            }
            
            if (sat_obj.Id == 0)
            {
                sat_obj.Day = "Saturday";
                sat_obj.isoff = true;
                db.AspNetWorkingTimes.Add(sat_obj);
                db.SaveChanges();
            }
            if (sun_obj.Id == 0)
            {
                sun_obj.Day = "Sunday";
                sun_obj.isoff = true;
                db.AspNetWorkingTimes.Add(sun_obj);
                db.SaveChanges();
            }

            timetable.MondayID = mon_obj.Id;
            timetable.TuesdayID = tue_obj.Id;
            timetable.WednesdayID = wed_obj.Id;
            timetable.ThursdayID = thus_obj.Id;
            timetable.FridayID = fri_obj.Id;
            timetable.SaturdayID = sat_obj.Id;
            timetable.SundayID = sun_obj.Id;

            db.AspNetWorkingWeekTimes.Add(timetable);
            db.SaveChanges();

            return timetable.Id;



        }

        public JsonResult Get_BusinessSubCatg(string id)
        {
            String ls = "";
            var ID = Convert.ToInt16(id);
            foreach (var item in db.AspNetBusinessSubCatageories)
            {
                if (item.BussinessCatageoryId == ID )
                {
                    ls = ls + item.Name + ",";
                }
            }
           
            return Json(ls ,JsonRequestBehavior.AllowGet);
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
            var customer = db.AspNetCustomers.ToList();         // cus id
            var users=db.AspNetUsers.ToList();                  //Name,email,phone_no
            var user_type = db.AspNetCustomerTypes.ToList();    //CompanyName, Status
            var sms = db.AspNetCustomerSMS.ToList();            // remaining sms
            var address = db.AspNetCustomerLocations.ToList();  // Address

            List<CustomerList_Data> list_data = new List<CustomerList_Data>();

            foreach (var item in customer)
            {
                CustomerList_Data obj = new CustomerList_Data();

                var customer_id = item.UserID;
                obj.customer_id = item.Id;

                var temp_user = users.Where(x => x.Id == customer_id).Select(x => x).FirstOrDefault();

                obj.email = temp_user.Email;
                obj.phone_num = temp_user.PhoneNumber;
                obj.user_name = temp_user.UserName;

                var user_type_id = item.TypeID;
                var temp_user_type = user_type.Where(x => x.Id == user_type_id).Select(x => x).FirstOrDefault();

                obj.company_name = temp_user_type.CompanyName;
                obj.status = temp_user_type.Status;

                var address_id = item.LocationId;
                var temp_address = address.Where(x => x.Id == address_id).Select(x => x).FirstOrDefault();

                obj.address = temp_address.Address;
                obj.city = temp_address.City;
                obj.zipcode =(int) temp_address.ZipCode;

                obj.country = temp_address.CountryName;
                obj.state = temp_address.State;


                var sms_id = item.SmsID;
                var temp_sms = sms.Where(x => x.Id == sms_id).Select(x => x).FirstOrDefault();

                obj.rem_sms = (int) temp_sms.RemainingSMS;

                list_data.Add(obj);
            }





            return View(list_data);
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