using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Shad_BookingApplication.Models;
using System.Web.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Data.Entity;

namespace Shad_BookingApplication.Controllers
{
    [Authorize(Roles = "Company_Admin,Agency_Manager")]

    public class CompanyAdminController : Controller
    {

        private BookingModelEntities db = new BookingModelEntities();
        // GET: CompanyAdmin
        public ActionResult Index()
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

        public void Bind_ServiceToCompany(int customer_id, int servicegroup_Id)
        {
            var customer_service_group = new AspNetComanyService();
            customer_service_group.Service_GroupId = servicegroup_Id;
            customer_service_group.CustomerId = customer_id;

            db.AspNetComanyServices.Add(customer_service_group);
            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult AddService(AspNetService aspNetService)
        {
            var login_user_id=User.Identity.GetUserId();

            var head_id=db.AspNetUsers.Where(x => x.Id == login_user_id).FirstOrDefault().HeadId;
            if (head_id == null)
            {
                db.AspNetServices.Add(aspNetService);
                db.SaveChanges();

                var group_name = Request.Form["service_group_name"];

                var servicegroup = new AspNetService_Group();
                servicegroup.ServiceID = aspNetService.Id;
                servicegroup.GroupID = Convert.ToInt16(group_name);

                db.AspNetService_Group.Add(servicegroup);
                db.SaveChanges();

                var customer_id = db.AspNetCustomers.Where(x => x.UserID == login_user_id).FirstOrDefault().Id;
                Bind_ServiceToCompany(customer_id, servicegroup.Id);
            }
            else
            {
               

                db.AspNetServices.Add(aspNetService);
                db.SaveChanges();

                var group_name = Request.Form["service_group_name"];

                var servicegroup = new AspNetService_Group();
                servicegroup.ServiceID = aspNetService.Id;
                servicegroup.GroupID = Convert.ToInt16(servicegroup);

                db.AspNetService_Group.Add(servicegroup);
                db.SaveChanges();

                var user_id = db.AspNetUsers.Where(x => x.HeadId == head_id).FirstOrDefault().Id;
                var customer_id = db.AspNetCustomers.Where(x => x.UserID == user_id).FirstOrDefault().Id;
                Bind_ServiceToCompany(customer_id, servicegroup.Id);

            }

            return RedirectToAction("ServiceGroupList");
        }

        public ActionResult AddServiceGroup()
        {
            var ls = db.AspNetServices.ToList();
            ViewBag.service_list = ls;
            return View();
        }

        [HttpPost]
        public ActionResult AddServiceGroup(AspNetServiceGroup aspNetServiceGroup)
        {
            var login_user_id = User.Identity.GetUserId();

            var head_id = db.AspNetUsers.Where(x => x.Id == login_user_id).FirstOrDefault().HeadId;
            if (head_id == null)
            {
                db.AspNetServiceGroups.Add(aspNetServiceGroup);
                db.SaveChanges();

                var list = Request.Form["selected_services_name"].ToString();
                var selected_services = list.Split(',').ToList();

                foreach (var item in selected_services)
                {
                    if (item == "")
                        continue;

                    var obj = new AspNetService_Group();
                    obj.ServiceID = Convert.ToInt16(item);
                    obj.GroupID = aspNetServiceGroup.Id;

                    db.AspNetService_Group.Add(obj);
                    db.SaveChanges();

                    var customer_id = db.AspNetCustomers.Where(x => x.UserID == login_user_id).FirstOrDefault().Id;
                    Bind_ServiceToCompany(customer_id, obj.Id);
                }

               
            }
            else
            {

                db.AspNetServiceGroups.Add(aspNetServiceGroup);
                db.SaveChanges();

                var list = Request.Form["selected_services_name"].ToString();
                var selected_services = list.Split(',').ToList();

                foreach (var item in selected_services)
                {
                    if (item == "")
                        continue;

                    var obj = new AspNetService_Group();
                    obj.ServiceID = Convert.ToInt16(item);
                    obj.GroupID = aspNetServiceGroup.Id;

                    db.AspNetService_Group.Add(obj);
                    db.SaveChanges();
                }

                var user_id = db.AspNetUsers.Where(x => x.HeadId == head_id).FirstOrDefault().Id;
                var customer_id = db.AspNetCustomers.Where(x => x.UserID == user_id).FirstOrDefault().Id;
                Bind_ServiceToCompany(customer_id, aspNetServiceGroup.Id);

            }

            return RedirectToAction("ServiceGroupList");
        }


        public ActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser([Bind(Include = "Email,PasswordHash,UserName,PhoneNumber,Status")] AspNetUser aspnetuser, [Bind(Include = "Name")] AspNetRole role)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Context));

            try
            {

                var user = new ApplicationUser();
                user.UserName = aspnetuser.UserName;
                user.Email = aspnetuser.Email;
                string Pass = aspnetuser.PasswordHash;
                user.PhoneNumber = aspnetuser.PhoneNumber;

                var ChkUser = userManager.Create(user, Pass);
                if (ChkUser.Succeeded)
                {
                    var role_name = role.Name;
                    var result = userManager.AddToRole(user.Id, role_name);
                    var TemUser = db.AspNetUsers.Where(y => y.Email == user.Email).Select(x => x).FirstOrDefault();
                    TemUser.Status = aspnetuser.Status;
                    db.SaveChanges();
                }
                var id = User.Identity.GetUserId();
                
                var head = db.AspNetUsers.Where(y => y.Id == id).Select(x => x.HeadId).SingleOrDefault();

                if (head != null && head != "")
                {
                    var new_user = db.AspNetUsers.Where(x => x.UserName == aspnetuser.UserName).Select(y => y).SingleOrDefault();
                    new_user.HeadId = head;
                    db.SaveChanges();
                }
                else if (head == null || head == "")
                {
                    var new_user = db.AspNetUsers.Where(x => x.UserName == aspnetuser.UserName).Select(y => y).SingleOrDefault();
                    new_user.HeadId = id;
                    db.SaveChanges();
                }


                return RedirectToAction("UserList");
            }
            catch (Exception ex)
            {

            }


            return View();
        }
        public ActionResult AddAgency()
        {

            AddAgencyViewModel addAgencyViewModel = new AddAgencyViewModel();
            
            
            var id = User.Identity.GetUserId();
            var h_id = db.AspNetUsers.Where(x => x.Id == id).Select(y => y.HeadId).SingleOrDefault();
            var cus_data = new AspNetCustomer();

            if (h_id == null)

            {

                cus_data = db.AspNetCustomers.Where(x => x.UserID == id).SingleOrDefault();

            }

            else

            {

                cus_data = db.AspNetCustomers.Where(x => x.UserID == h_id).SingleOrDefault();

            }


            //sub category dropdown in agency 
            var cus =db.AspNetCustomer_SubCatageory.Where(x => x.CustomerID == cus_data.Id).ToList();
            List<AspNetBusinessSubCatageory> sub_data = new List<AspNetBusinessSubCatageory>();
            foreach (var item in cus)
            {
                var sub=db.AspNetBusinessSubCatageories.Where(x => x.Id == item.SubCatageoryId).SingleOrDefault();
                var data = new AspNetBusinessSubCatageory();
                data.Id = sub.Id;
                data.Name = sub.Name;
                data.BussinessCatageoryId = sub.BussinessCatageoryId;
                sub_data.Add(data);
                
            }
        
            
             ViewBag.BusinessSubCatageory = sub_data;





            //agency admin dropdown list
            if (cus_data != null)
            {
                ViewBag.AgencyAdmin = new SelectList(db.AspNetUsers.Where(x =>( x.AspNetRoles.Select(y => y.Name).Contains("Company_Admin") || x.AspNetRoles.Select(y => y.Name).Contains("Agency_Manager")) && ((x.HeadId == cus_data.UserID) || x.Id == cus_data.UserID)), "Id", "UserName");
            }



            addAgencyViewModel.SMS = db.AspNetCustomerSMS.Where(x=>x.Id==cus_data.SmsID).ToList();




            return View(addAgencyViewModel);
        }




        [HttpPost]
        public ActionResult AddAgency(AddAgencyViewModel addAgencyViewModel, HttpPostedFileBase[] files)
        {

            // Adding Details
            
            /* 
            if(!ModelState.IsValid)
             {
                 var age= new AddAgencyViewModel();
                 age = addCustomerViewModel;
                 age.BusinessSubCatageory = db.AspNetBusinessSubCatageories.ToList();
                 ViewBag.AgencyAdmin = new SelectList(db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Company_Admin") || x.AspNetRoles.Select(y => y.Name).Contains("Agency_Manager")), "Id", "UserName");

                 age.SMS = db.AspNetCustomerSMS.ToList();
                 return View("AddAgency",age);
             }*/

            db.AspNetCustomerDetails.Add(addAgencyViewModel.Detail);
            db.SaveChanges();
            // Adding Location 
            var loc_country_name = Request.Form["LocCountryName"].ToString();
            addAgencyViewModel.Location.CountryName = loc_country_name;


            db.AspNetCustomerLocations.Add(addAgencyViewModel.Location);
            db.SaveChanges();

            // Adding Contact

            db.AspNetCustomerContacts.Add(addAgencyViewModel.Contact);
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

            BusinessDetail.APE = addAgencyViewModel.BusinessDetail.APE;
            BusinessDetail.Description = Request.Form["editor1"];
            BusinessDetail.SRIET = addAgencyViewModel.BusinessDetail.SRIET;
            BusinessDetail.VatNumber = addAgencyViewModel.BusinessDetail.VatNumber;

            db.AspNetCustomerBusinessDetails.Add(BusinessDetail);
            db.SaveChanges();


            // Adding user Types
            addAgencyViewModel.UserType.CompanyName = addAgencyViewModel.Detail.BussinessName;
            
            db.AspNetCustomerTypes.Add(addAgencyViewModel.UserType);
            db.SaveChanges();

            // Getting SMS ID
            var sms_package_id = addAgencyViewModel.SingleSms.ItemID;


            // Adding Working time
            var time = Request.Form["custom_timepicker_name"].ToString();
            var id = time_string_parse(time);

            //  var businessDetails_textarea = Request.Form["editor1"];

            // Adding Social
            db.AspNetSocials.Add(addAgencyViewModel.Social);
            db.SaveChanges();

            // Adding User
            //  var user_id = AddCustomerAccount(addCustomerViewModel.User);
            var user_id = Request.Form["AgencyAdmin"];

            var user = db.AspNetUsers.Where(x => x.Id == user_id).SingleOrDefault();
            //user.FirstName = addAgencyViewModel.User.FirstName;
            //user.LastName = addAgencyViewModel.User.LastName;
            //  user.Status = Request.Form["UserType_Status"];
            // db.SaveChanges();
            var agency = new AspNetAgency();

            agency.BussinessID = BusinessDetail.Id;
            agency.RegionID = region.Id;
            agency.LocationId = addAgencyViewModel.Location.Id;
            agency.SmsID = sms_package_id;
            agency.TypeID = addAgencyViewModel.UserType.Id;
            agency.ContactId = addAgencyViewModel.Contact.Id;
            agency.DetailId = addAgencyViewModel.Detail.Id;
            agency.SocialID = addAgencyViewModel.Social.Id;
            agency.UserID = user_id;
            agency.GalleryID = gallary.Id;
            agency.WorkingID = id;

            var u_id = User.Identity.GetUserId();
            var h_id = db.AspNetUsers.Where(x => x.Id == u_id).Select(y => y.HeadId).SingleOrDefault();
            var cus_data = new AspNetCustomer();

            if (h_id == null)

            {

                cus_data = db.AspNetCustomers.Where(x => x.UserID == u_id).SingleOrDefault();

            }

            else

            {

                cus_data = db.AspNetCustomers.Where(x => x.UserID == h_id).SingleOrDefault();

            }

            var cus_id = cus_data.Id;

            agency.HeadId = cus_id;


            db.AspNetAgencies.Add(agency);
            db.SaveChanges();

            return RedirectToAction("AgencyList", "CompanyAdmin");


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
        private int time_string_parse(string time)
        {
            var mon = new AspNetWorkingTime();
            var tue = new AspNetWorkingTime();
            var wed = new AspNetWorkingTime();
            var thu = new AspNetWorkingTime();
            var fri = new AspNetWorkingTime();
            var sat = new AspNetWorkingTime();
            var sun = new AspNetWorkingTime();


            var mon_obj = new AspNetWorkingTime();
            var tue_obj = new AspNetWorkingTime();
            var wed_obj = new AspNetWorkingTime();
            var thus_obj = new AspNetWorkingTime();
            var fri_obj = new AspNetWorkingTime();
            var sat_obj = new AspNetWorkingTime();
            var sun_obj = new AspNetWorkingTime();


            if (ViewBag.workingid != null)
            {
                short working_id = Convert.ToInt16(ViewBag.workingid);
                var work_data = db.AspNetWorkingWeekTimes.Where(x => x.Id == working_id).SingleOrDefault();
                mon = db.AspNetWorkingTimes.Where(x => x.Id == work_data.MondayID).SingleOrDefault();
                tue = db.AspNetWorkingTimes.Where(x => x.Id == work_data.TuesdayID).SingleOrDefault();
                wed = db.AspNetWorkingTimes.Where(x => x.Id == work_data.WednesdayID).SingleOrDefault();
                thu = db.AspNetWorkingTimes.Where(x => x.Id == work_data.ThursdayID).SingleOrDefault();
                fri = db.AspNetWorkingTimes.Where(x => x.Id == work_data.FridayID).SingleOrDefault();
                sat = db.AspNetWorkingTimes.Where(x => x.Id == work_data.SaturdayID).SingleOrDefault();
                sun = db.AspNetWorkingTimes.Where(x => x.Id == work_data.SundayID).SingleOrDefault();
            }



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
                    if (ViewBag.check == null)
                    {
                        db.AspNetWorkingTimes.Add(mon_obj);
                    }
                    else if (ViewBag.check == 1)
                    {
                        if (mon_obj.isoff != true)
                        {
                            mon.StartTime = mon_obj.StartTime;
                            mon.EndTime = mon_obj.EndTime;
                            mon.LunchFrom = mon_obj.LunchFrom;
                            mon.LunchToo = mon.LunchToo;
                            mon.Day = mon_obj.Day;
                            mon.isoff = mon_obj.isoff;
                            mon_obj.Id = mon.Id;
                        }
                    }
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
                    if (ViewBag.check == null)
                    {
                        db.AspNetWorkingTimes.Add(tue_obj);
                    }
                    else if (ViewBag.check == 1)
                    {
                        if (tue_obj.isoff != true)
                        {
                            tue.StartTime = tue_obj.StartTime;
                            tue.EndTime = tue_obj.EndTime;
                            tue.LunchFrom = tue_obj.LunchFrom;
                            tue.LunchToo = tue.LunchToo;
                            tue.Day = tue_obj.Day;
                            tue.isoff = tue_obj.isoff;
                            tue_obj.Id = tue.Id;
                        }
                    }
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
                    if (ViewBag.check == null)
                    {
                        db.AspNetWorkingTimes.Add(wed_obj);
                    }
                    else if (ViewBag.check == 1)
                    {
                        if (wed_obj.isoff != true)
                        {
                            wed.StartTime = wed_obj.StartTime;
                            wed.EndTime = wed_obj.EndTime;
                            wed.LunchFrom = wed_obj.LunchFrom;
                            wed.LunchToo = wed.LunchToo;
                            wed.Day = wed_obj.Day;
                            wed.isoff = wed_obj.isoff;
                            wed_obj.Id = wed.Id;
                        }
                    }

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
                    if (ViewBag.check == null)
                    {
                        db.AspNetWorkingTimes.Add(thus_obj);
                    }
                    else if (ViewBag.check == 1)
                    {
                        if (thus_obj.isoff != true)
                        {
                            thu.StartTime = thus_obj.StartTime;
                            thu.EndTime = thus_obj.EndTime;
                            thu.LunchFrom = thus_obj.LunchFrom;
                            thu.LunchToo = thus_obj.LunchToo;
                            thu.Day = thus_obj.Day;
                            thu.isoff = thus_obj.isoff;
                            thus_obj.Id = thu.Id;
                        }
                    }
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
                    if (ViewBag.check == null)
                    {
                        db.AspNetWorkingTimes.Add(fri_obj);
                    }
                    else if (ViewBag.check == 1)
                    {
                        if (fri_obj.isoff != true)
                        {
                            fri.StartTime = fri_obj.StartTime;
                            fri.EndTime = fri_obj.EndTime;
                            fri.LunchFrom = fri_obj.LunchFrom;
                            fri.LunchToo = fri_obj.LunchToo;
                            fri.Day = fri_obj.Day;
                            fri.isoff = fri_obj.isoff;
                            fri_obj.Id = fri.Id;
                        }
                    }
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
                    if (ViewBag.check == null)
                    {
                        db.AspNetWorkingTimes.Add(sat_obj);
                    }
                    else if (ViewBag.check == 1)
                    {
                        if (sat_obj.isoff != true)
                        {
                            sat.StartTime = sat_obj.StartTime;
                            sat.EndTime = sat_obj.EndTime;
                            sat.LunchFrom = sat_obj.LunchFrom;
                            sat.LunchToo = sat_obj.LunchToo;
                            sat.Day = sat_obj.Day;
                            sat.isoff = sat_obj.isoff;
                            sat_obj.Id = sat.Id;
                        }
                    }
                    db.SaveChanges();
                }// end if sat
                else if (item.Contains("Sunday"))
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
                    if (ViewBag.check == null)
                    {
                        db.AspNetWorkingTimes.Add(sun_obj);
                    }
                    else if (ViewBag.check == 1)
                    {
                        if (sun_obj.isoff != true)
                        {
                            sun.StartTime = sun_obj.StartTime;
                            sun.EndTime = sun_obj.EndTime;
                            sun.LunchFrom = sun_obj.LunchFrom;
                            sun.LunchToo = sun_obj.LunchToo;
                            sun.Day = sun_obj.Day;
                            sun.isoff = sun_obj.isoff;
                            sun_obj.Id = sun.Id;
                        }
                    }

                    db.SaveChanges();
                }// end if sat

            }// end foreach



            var timetable = new AspNetWorkingWeekTime();



            if (mon_obj.Id == 0)
            {
                mon_obj.Day = "Monday";
                mon_obj.isoff = true;
                if (ViewBag.check == null)
                {
                    db.AspNetWorkingTimes.Add(mon_obj);
                }
                else if (ViewBag.check == 1)
                {
                    mon.Day = mon_obj.Day;
                    mon.isoff = mon_obj.isoff;
                    mon_obj.Id = mon.Id;
                }
                db.SaveChanges();
            }

            if (tue_obj.Id == 0)
            {
                tue_obj.Day = "Tuesday";
                tue_obj.isoff = true;
                if (ViewBag.check == null)
                {
                    db.AspNetWorkingTimes.Add(tue_obj);
                }
                else if (ViewBag.check == 1)
                {
                    tue.Day = tue_obj.Day;
                    tue.isoff = tue_obj.isoff;
                    tue_obj.Id = tue.Id;
                }
                db.SaveChanges();
            }


            if (wed_obj.Id == 0)
            {
                wed_obj.Day = "Wednesday";
                wed_obj.isoff = true;
                if (ViewBag.check == null)
                {
                    db.AspNetWorkingTimes.Add(wed_obj);
                }
                else if (ViewBag.check == 1)
                {
                    wed.Day = wed_obj.Day;
                    wed.isoff = wed_obj.isoff;
                    wed_obj.Id = wed.Id;
                }
                db.SaveChanges();
            }

            if (thus_obj.Id == 0)
            {
                thus_obj.Day = "Thursday";
                thus_obj.isoff = true;
                if (ViewBag.check == null)
                {
                    db.AspNetWorkingTimes.Add(thus_obj);
                }
                else if (ViewBag.check == 1)
                {
                    thu.Day = thus_obj.Day;
                    thu.isoff = thus_obj.isoff;
                    thus_obj.Id = thu.Id;
                }
                db.SaveChanges();
            }

            if (fri_obj.Id == 0)
            {
                fri_obj.Day = "Friday";
                fri_obj.isoff = true;
                if (ViewBag.check == null)
                {
                    db.AspNetWorkingTimes.Add(fri_obj);
                }
                else if (ViewBag.check == 1)
                {
                    fri.Day = fri_obj.Day;
                    fri.isoff = fri_obj.isoff;
                    fri_obj.Id = fri.Id;
                }
                db.SaveChanges();
            }

            if (sat_obj.Id == 0)
            {
                sat_obj.Day = "Saturday";
                sat_obj.isoff = true;
                if (ViewBag.check == null)
                {
                    db.AspNetWorkingTimes.Add(sat_obj);
                }
                else if (ViewBag.check == 1)
                {
                    sat.Day = sat_obj.Day;
                    sat.isoff = sat_obj.isoff;
                    sat_obj.Id = sat.Id;
                }
                db.SaveChanges();
            }
            if (sun_obj.Id == 0)
            {
                sun_obj.Day = "Sunday";
                sun_obj.isoff = true;
                if (ViewBag.check == null)
                {
                    db.AspNetWorkingTimes.Add(sun_obj);
                }
                else if (ViewBag.check == 1)
                {
                    sun.Day = sun_obj.Day;
                    sun.isoff = sun_obj.isoff;
                    sun_obj.Id = sun.Id;
                }
                db.SaveChanges();
            }

            timetable.MondayID = mon_obj.Id;
            timetable.TuesdayID = tue_obj.Id;
            timetable.WednesdayID = wed_obj.Id;
            timetable.ThursdayID = thus_obj.Id;
            timetable.FridayID = fri_obj.Id;
            timetable.SaturdayID = sat_obj.Id;
            timetable.SundayID = sun_obj.Id;
            if (ViewBag.check == null)
            {
                db.AspNetWorkingWeekTimes.Add(timetable);
            }
            db.SaveChanges();

            return timetable.Id;



        }

        public ActionResult EditAgency(int id)
        {
            AddAgencyViewModel editAgencyViewModel = new AddAgencyViewModel();
            editAgencyViewModel.BusinessSubCatageory = db.AspNetBusinessSubCatageories.ToList();
            ViewBag.AgencyAdmin = new SelectList(db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Company_Admin") || x.AspNetRoles.Select(y => y.Name).Contains("Agency_Manager")), "Id", "UserName");

            editAgencyViewModel.SMS = db.AspNetCustomerSMS.ToList();
            var agency = db.AspNetAgencies.Where(x => x.Id == id).SingleOrDefault();
            editAgencyViewModel.Detail = db.AspNetCustomerDetails.Where(x => x.Id == agency.DetailId).SingleOrDefault();
            editAgencyViewModel.Location = db.AspNetCustomerLocations.Where(x => x.Id == agency.LocationId).SingleOrDefault();
            editAgencyViewModel.BusinessDetail = db.AspNetCustomerBusinessDetails.Where(y => y.Id == agency.BussinessID).SingleOrDefault();
            editAgencyViewModel.Contact = db.AspNetCustomerContacts.Where(x => x.Id == agency.ContactId).SingleOrDefault();
            editAgencyViewModel.Social = db.AspNetSocials.Where(x => x.Id == agency.SocialID).SingleOrDefault();
            editAgencyViewModel.Region = db.AspNetCustomerRegions.Where(x => x.Id == agency.RegionID).SingleOrDefault();
            editAgencyViewModel.User = db.AspNetUsers.Where(x => x.Id == agency.UserID).SingleOrDefault();
            var user = db.AspNetUsers.Where(x => x.Id == agency.UserID).SingleOrDefault();
            editAgencyViewModel.Gallery = db.AspNetCustomerGalleries.Where(x => x.Id == agency.GalleryID).SingleOrDefault();
            editAgencyViewModel.WorkingTime = db.AspNetWorkingTimes.Where(x => x.Id == agency.WorkingID).SingleOrDefault();
            editAgencyViewModel.UserType = db.AspNetCustomerTypes.Where(x => x.Id == agency.TypeID).SingleOrDefault();
            ViewBag.sub = db.AspNetBusinessSubCatageories.Where(x => x.Id == agency.AspNetCustomerDetail.SubCatageoryNo).Select(y => y.Name).SingleOrDefault();
            ViewBag.sms = db.AspNetCustomerSMS.Where(x => x.Id == agency.SmsID).Select(y => y.SmsPackageName).SingleOrDefault();
            ViewBag.id = id;
            //ViewBag.act = user.Status;
            ViewBag.agencyAdminId = agency.UserID;
            return View(editAgencyViewModel);
        }

        public ActionResult SaveEditAgency(AddAgencyViewModel agency, HttpPostedFileBase[] files)
        {
            var agencyId = Convert.ToInt16(Request.Form["agency_id"]);
            var agency_data = db.AspNetAgencies.Where(x => x.Id == agencyId).SingleOrDefault();

            var detail = db.AspNetCustomerDetails.Where(x => x.Id == agency_data.DetailId).SingleOrDefault();
            var location = db.AspNetCustomerLocations.Where(x => x.Id == agency_data.LocationId).SingleOrDefault();
            var businessdetail = db.AspNetCustomerBusinessDetails.Where(y => y.Id == agency_data.BussinessID).SingleOrDefault();
            var contact = db.AspNetCustomerContacts.Where(x => x.Id == agency_data.ContactId).SingleOrDefault();
            var social = db.AspNetSocials.Where(x => x.Id == agency_data.SocialID).SingleOrDefault();
            var region = db.AspNetCustomerRegions.Where(x => x.Id == agency_data.RegionID).SingleOrDefault();
            var user = db.AspNetUsers.Where(x => x.Id == agency_data.UserID).SingleOrDefault();
            var usertype = db.AspNetCustomerTypes.Where(x => x.Id == agency_data.TypeID).SingleOrDefault();
            var gallery = db.AspNetCustomerGalleries.Where(x => x.Id == agency_data.GalleryID).SingleOrDefault();
            // var workingTime = db.AspNetWorkingTimes.Where(x => x.Id == agency_data.WorkingID).SingleOrDefault();
            var userType = db.AspNetCustomerTypes.Where(x => x.Id == agency_data.TypeID).SingleOrDefault();
            var sms = db.AspNetCustomerSMS.Where(x => x.Id == agency.SingleSms.Id).SingleOrDefault();




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

            db.SaveChanges();


            //customerDetail
            detail.BussinessName = agency.Detail.BussinessName;
            detail.BussinessWebsite = agency.Detail.BussinessWebsite;
            detail.SubCatageoryNo = agency.Detail.SubCatageoryNo;
            db.SaveChanges();

            //customerlocation
            location.Address = agency.Location.Address;
            location.City = agency.Location.City;
            location.CountryName = Request.Form["LocCountryName"];
            location.Latitude = agency.Location.Latitude;
            location.Longitude = agency.Location.Longitude;
            location.Radius = agency.Location.Radius;
            location.State = agency.Location.State;
            location.ZipCode = agency.Location.ZipCode;
            location.IntervensionZone = agency.Location.IntervensionZone;
            db.SaveChanges();

            //customerbusinessDetail
            HttpPostedFileBase file = Request.Files["business_logo_img"];
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/ServerFiles"), fileName);
                file.SaveAs(path);
                businessdetail.Logo = path;

            }
            else
            {
                businessdetail.Logo = "";
            }

            businessdetail.APE = agency.BusinessDetail.APE;

            businessdetail.SRIET = agency.BusinessDetail.SRIET;
            businessdetail.VatNumber = agency.BusinessDetail.VatNumber;
            businessdetail.Description = Request.Form["editor1"];
            db.SaveChanges();

            //customerContact
            contact.BussinessEmail = agency.Contact.BussinessEmail;
            contact.Mobile = agency.Contact.Mobile;
            contact.Telephone1 = agency.Contact.Telephone1;
            contact.Telephone2 = agency.Contact.Telephone2;
            contact.Fax = agency.Contact.Fax;
            db.SaveChanges();


            //social

            social.Twitter = agency.Social.Twitter;
            social.Facebook = agency.Social.Facebook;
            db.SaveChanges();

            //region
            region.CountryName = Request.Form["region_country"].ToString();
            region.TimeZoneName = Request.Form["region_timezone"];
            region.DateFormate = Request.Form["region_dateformat"].ToString();
            region.TimeFormate = Request.Form["region_timeformat"].ToString();
            region.CurrencyName = Request.Form["region_currency"].ToString();


            db.SaveChanges();

            //user
            // user.FirstName = agency.User.FirstName;
            //user.LastName = agency.User.LastName;
            // user.Status = Request.Form["UserType.Status"];
            //db.SaveChanges();

            //usertype
            usertype.CompanyName = agency.Detail.BussinessName;
            usertype.SingleorMulti = agency.UserType.SingleorMulti;
            usertype.Status = agency.UserType.Status;
            db.SaveChanges();
            //sms
            //if(agency.SingleSms.ItemID==2)
            //{
            //    agency_data.SmsID = null;
            //}
            //else
            //{
            //    sms.SmsPackageName = agency.SingleSms.SmsPackageName;

            //}
            //db.SaveChanges();

            // Adding Working time
            ViewBag.check = 1;
            ViewBag.workingid = agency_data.WorkingID;
            var time = Request.Form["custom_timepicker_name"].ToString();
            time_string_parse(time);
            return RedirectToAction("AgencyList", "CompanyAdmin");



        }
        public JsonResult Get_RegionData(int region_id)
        {
            var id = region_id;
            var region_obj = db.AspNetCustomerRegions.Where(x => x.Id == id).FirstOrDefault();

            RegionViewModel obj = new RegionViewModel();
            obj.CountryName = region_obj.CountryName;
            obj.CurrencyName = region_obj.CurrencyName;
            obj.DateFormate = region_obj.DateFormate;
            obj.TimeFormate = region_obj.TimeFormate;
            obj.TimeZoneName = region_obj.TimeZoneName;
            var location_id = db.AspNetAgencies.Where(x => x.RegionID == id).Select(y => y.LocationId).FirstOrDefault();
            var loc = db.AspNetCustomerRegions.Where(x => x.Id == location_id).Select(y => y.CountryName).FirstOrDefault();
            obj.Location_CountryName = loc;

            return Json(obj, JsonRequestBehavior.AllowGet);
        }



        public ActionResult AddCustomer()
        {
            AddCompanyCustomerViewModel obj = new AddCompanyCustomerViewModel();
            
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomer(AddCompanyCustomerViewModel customer , HttpPostedFileBase[] files)
        {
            var id = User.Identity.GetUserId();
            var agency = db.AspNetAgencies.Where(x => x.UserID == id).FirstOrDefault();
           var agency_id =agency.Id;
            
            customer.Gender=Request.Form["Gender"];
            customer.TimeZone=Request.Form["timezone"];

            HttpPostedFileBase file = Request.Files["upload"];
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/ServerFiles"), fileName);
                file.SaveAs(path);
                customer.Image = path;

            }
            else
            {
                customer.Image = "";
            }

            var notification = new AspNetCompanyNotifination();
            //AspNetCompanyNotifination customer.Notification;
            notification.SendAppointmentMessage = false;
            notification.DifferentSmsInterval = false;
            notification.DifferentEmailInterval = false;
            notification.DefaultSmsRemainder = false;
            notification.DefaultEmailRemainder = false;
            if (Request.Form["appointment_message"] == "on")
            {

                notification.SendAppointmentMessage = true;
            }
            

            if (Request.Form["use_different_sms_interval"] == "on")
            {

                notification.DifferentSmsInterval = true;
            }
            

            if (Request.Form["email_interval"] == "on")
            {

                notification.DifferentEmailInterval = true;
            }
            

            if (Request.Form["default_sms_reminder"] == "on")
            {

                notification.DefaultSmsRemainder = true;
            }
            
            if (Request.Form["default_email_reminder"] == "on")
            {

                notification.DefaultEmailRemainder = true;
            }

            notification.SmsBeforeArrive =Request.Form["sms_before_arrive"];
            notification.EmailBeforeArrive =Request.Form["before_arrive"] ;
            db.AspNetCompanyNotifinations.Add(notification);
            db.SaveChanges();
            var company_customer = new AspNetCompanyCustomer();
            company_customer.AgencyId = agency_id;
            company_customer.CompanyNotificationId = notification.Id;
            company_customer.FirstName =customer.FirstName;
            company_customer.LastName =customer.LastName;
            company_customer.Occupation =customer.Occupation;
            company_customer.BateofBirth = customer.BateofBirth;
            company_customer.City = customer.City;
            company_customer.Contradiction = customer.Contradiction;
            company_customer.Email = customer.Email;
            company_customer.Gender =customer.Gender;
            company_customer.Image =customer.Image;
            company_customer.PostCode =customer.PostCode;
            company_customer.RefferedBy =customer.RefferedBy;
            company_customer.SmsNumber = customer.SmsNumber;
            company_customer.Telephone = customer.Telephone;
            company_customer.TimeZone = customer.TimeZone;
            company_customer.Adress = customer.Adress;
            db.AspNetCompanyCustomers.Add(company_customer);
            db.SaveChanges();
            

            return RedirectToAction("CustomerList");
        }



        public ActionResult AddEmployee()
        {
            AddEmployeeViewModel employeeViewModel = new AddEmployeeViewModel();

            employeeViewModel.GroupServicesList = Get_CompanyServicesOfLoginUser();

            return View(employeeViewModel);
        }

        public ActionResult AddItem()
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

            voucher.Type = Request.Form["Type"];
            if (Request.Form["Voided"] == "on")
            {
                voucher.Voided = true;
            }
            else
            {
                voucher.Voided = false;
            }

            if (Request.Form["RedmeeOnline"] == "on")
            {
                voucher.RedmeeOnline = true;
            }
            else
            {
                voucher.RedmeeOnline = false;
            }


            AspNetExpire exp = new AspNetExpire();
            if (Request.Form["ExpireId"] == "Fixed")
            {
                exp.From = Convert.ToDateTime(Request.Form["Fixed_From"]);
                exp.To = Convert.ToDateTime(Request.Form["Fixed_To"]);
                exp.Date = Convert.ToDateTime(Request.Form["Fixed_Date"]);
                exp.Every = null;
                exp.Name = Request.Form["ExpireId"];

            }
            else if (Request.Form["ExpireId"] == "Period")
            {
                exp.From = Convert.ToDateTime(Request.Form["Period_From"]);
                exp.To = Convert.ToDateTime(Request.Form["Period_To"]);
                exp.Every = null;
                exp.Name = Request.Form["ExpireId"];
                exp.Date = null;


            }
            else if (Request.Form["ExpireId"] == "Recuring")
            {
                exp.From = Convert.ToDateTime(Request.Form["Recuring_From"]);
                exp.To = Convert.ToDateTime(Request.Form["Recuring_To"]);
                exp.Every = Request.Form["Recuring_Every"].ToString();
                exp.Name = Request.Form["ExpireId"];
                exp.Date = null;


            }
            else if (Request.Form["ExpireId"] == null)
            {
                exp.From = null; ;
                exp.To = null; ;
                exp.Every = null;
                exp.Name = null; ;
                exp.Date = null;


            }

            db.AspNetExpires.Add(exp);
            db.SaveChanges();
            //select id from table
            voucher.ExpireId = db.AspNetExpires.Select(x => x.Id).Max();

            if (true)
            {

                db.AspNetGiftVouchers.Add(voucher);
                db.SaveChanges();
            }
            return RedirectToAction("VoucherList");
        }

        public ActionResult EditVoucher(int id)
        {
            var data = new AspNetGiftVoucher();
            data = db.AspNetGiftVouchers.Where(x => x.Id == id).SingleOrDefault();
            ViewBag.fix = false;
            ViewBag.per = false;
            ViewBag.recu = false;
            ViewBag.redmeeOnline = false;
            ViewBag.voided = false;
            if (data.AspNetExpire.Name=="Fixed")
            {
                ViewBag.fix = true;
            }
            if (data.AspNetExpire.Name== "Period")
            {
                ViewBag.per = true;
            }

            if (data.AspNetExpire.Name == "Recuring")
            {
                ViewBag.recu = true;
            }
            
            if(data.RedmeeOnline==true)
            {
                ViewBag.redmeeOnline=true;
            }
            if (data.Voided == true)
            {
                ViewBag.voided = true;
            }
            return View(data);
        }
        [HttpPost]
        public ActionResult UpdateVoucher(AspNetGiftVoucher voucher)
        {
            var ex_id=Request.Form["expire_id"];
            var vou_id= Convert.ToInt16(Request.Form["voucher_id"]);
            var he=Request.Form["RedmeeOnline"];
            var type = Request.Form["Type"];
            
            var db_data=db.AspNetGiftVouchers.Where(x => x.Id == vou_id).SingleOrDefault();
            if(voucher.Name=="Fixed")
            {
                db_data.AspNetExpire.From = Convert.ToDateTime(Request.Form["Fixed_From"]);
                db_data.AspNetExpire.To = Convert.ToDateTime(Request.Form["Fixed_To"]);
                db_data.AspNetExpire.Date = Convert.ToDateTime(Request.Form["Fixed_date"]);
            }
            if(voucher.Name=="Period")
            {
                db_data.AspNetExpire.From = Convert.ToDateTime(Request.Form["Period_From"]);
                db_data.AspNetExpire.To = Convert.ToDateTime(Request.Form["Period_To"]);
            }
            
            if (voucher.Name == "Recuring")
            {
                db_data.AspNetExpire.Every =Request.Form["Recuring_Every"];
                db_data.AspNetExpire.From = Convert.ToDateTime(Request.Form["Recuring_From"]);
                db_data.AspNetExpire.To = Convert.ToDateTime(Request.Form["Recuring_To"]);
            }
            if (Request.Form["Voided"] == "on")
            {
                db_data.Voided = true;
            }
            else
            {
                db_data.Voided = false;
            }

            if (Request.Form["RedmeeOnline"] == "on")
            {
                db_data.RedmeeOnline = true;
            }
            else
            {
                db_data.RedmeeOnline = false;
            }



            db_data.Name = voucher.Name;
          
            db_data.Sku = voucher.Sku;
            db_data.Type = type;
            db_data.Discount = voucher.Discount;
            db_data.Description = voucher.Description;
            db.SaveChanges();
            return RedirectToAction("VoucherList","CompanyAdmin");
        }
        public ActionResult AgencyList()
        {
            var user_id1 = User.Identity.GetUserId();
            var user_data=db.AspNetUsers.Where(x => x.Id == user_id1).SingleOrDefault();
            var data = new List<AspNetAgency>();
            if (user_data.HeadId == null)
            {
                var cus_id = db.AspNetCustomers.Where(x => x.UserID == user_id1).Select(y => y.Id).FirstOrDefault();
                data = db.AspNetAgencies.Where(x => x.HeadId == cus_id).ToList();
            }
            else
            {
                var cus_id = db.AspNetCustomers.Where(x => x.UserID == user_data.HeadId).Select(y => y.Id).FirstOrDefault();
                data = db.AspNetAgencies.Where(x => x.HeadId == cus_id).ToList();
            }
                return View(data);
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
            var data = new List<AspNetCompanyCustomer>();
            var id=User.Identity.GetUserId();
            var cus_data=db.AspNetCustomers.Where(x => x.UserID == id).SingleOrDefault();
            
            if (cus_data!=null)
            {
                 var agency_data = db.AspNetAgencies.Where(x => x.HeadId == cus_data.Id);
                foreach(var item in agency_data)
                {
                    var comp_cus = db.AspNetCompanyCustomers.Where(x => x.AgencyId == item.Id);
                    foreach(var item1 in comp_cus)
                    {
                        data.Add(item1);
                    }
                }
            }
            
            if (cus_data == null)
            {
                var Name=db.AspNetUsers.Where(x=>x.Id==id).FirstOrDefault().AspNetRoles.FirstOrDefault();
                db.AspNetUsers.Where(x => (x.AspNetRoles.Select(y => y.Name).Contains("Company_Admin") || x.AspNetRoles.Select(y => y.Name).Contains("Agency_Manager")) && (x.Id == id));
                if (Name.Name == "Agency_Manager")
                {
                    var agency = db.AspNetAgencies.Where(x => x.UserID == id).SingleOrDefault();
                    if (agency != null)
                    {
                        data = db.AspNetCompanyCustomers.Where(x => x.AgencyId == agency.Id).ToList();
                        return View(data);
                    }
                }
                if(Name.Name=="Company_Admin")
                {
                    var user=db.AspNetUsers.Where(x => x.Id == id).SingleOrDefault();
                   var cus= db.AspNetCustomers.Where(y => y.UserID == user.HeadId).SingleOrDefault();
                    var agency_data = db.AspNetAgencies.Where(x => x.HeadId == cus.Id);
                    foreach (var item in agency_data)
                    {
                        var comp_cus = db.AspNetCompanyCustomers.Where(x => x.AgencyId == item.Id);
                        foreach (var item1 in comp_cus)
                        {
                            data.Add(item1);
                        }
                    }
                }
            }
             
            return View(data);
        }

        public ActionResult EditCustomer(int id )
        {
            var data = db.AspNetCompanyCustomers.Where(x => x.Id == id).SingleOrDefault();
            ViewBag.appointmentMessage = data.AspNetCompanyNotifination.SendAppointmentMessage;
            ViewBag.smsBeforeArrive = data.AspNetCompanyNotifination.SmsBeforeArrive;
            ViewBag.emailReminder = data.AspNetCompanyNotifination.DefaultEmailRemainder;
            ViewBag.smsReminder = data.AspNetCompanyNotifination.DefaultSmsRemainder;
            ViewBag.emailInterval = data.AspNetCompanyNotifination.DifferentEmailInterval;
            ViewBag.differentSmsInterval = data.AspNetCompanyNotifination.DifferentSmsInterval;
            ViewBag.emailBeforeArrive = data.AspNetCompanyNotifination.EmailBeforeArrive;
            return View(data);
        }
        [HttpPost]
        public ActionResult UpdateCustomer( AddCompanyCustomerViewModel customer, HttpPostedFileBase[] files)
        {
            var id = User.Identity.GetUserId();
            
            var agency_id = Convert.ToInt16( Request.Form["agency_id"]);
            var cus = db.AspNetCompanyCustomers.Where(x => x.AgencyId == agency_id).SingleOrDefault();
            
            customer.Gender = Request.Form["Gender"];
            customer.TimeZone = Request.Form["timezone"];
            
            HttpPostedFileBase file = Request.Files["upload"];
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/ServerFiles"), fileName);
                file.SaveAs(path);
                customer.Image = path;

            }
            else
            {
                customer.Image = "";
            }
            


            var notification = db.AspNetCompanyNotifinations.Where(x=>x.Id==cus.CompanyNotificationId).SingleOrDefault();
            //AspNetCompanyNotifination customer.Notification;
            notification.SendAppointmentMessage = false;
            notification.DifferentSmsInterval = false;
            notification.DifferentEmailInterval = false;
            notification.DefaultSmsRemainder = false;
            notification.DefaultEmailRemainder = false;
            var hr=Request.Form["appointment_message"];

            if (Request.Form["appointment_message"] == "on")
            {

                notification.SendAppointmentMessage = true;
            }


            if (Request.Form["use_different_sms_interval"] == "on")
            {

                notification.DifferentSmsInterval = true;
            }


            if (Request.Form["email_interval"] == "on")
            {

                notification.DifferentEmailInterval = true;
            }


            if (Request.Form["default_sms_reminder"] == "on")
            {

                notification.DefaultSmsRemainder = true;
            }

            if (Request.Form["default_email_reminder"] == "on")
            {

                notification.DefaultEmailRemainder = true;
            }
            notification.SmsBeforeArrive = Request.Form["sms_before_arrive"];
            notification.EmailBeforeArrive = Request.Form["before_arrive"];
            
            db.SaveChanges();
            var company_customer = cus;
            company_customer.AgencyId = agency_id;
            company_customer.CompanyNotificationId = notification.Id;
            company_customer.FirstName = customer.FirstName;
            company_customer.LastName = customer.LastName;
            company_customer.Occupation = customer.Occupation;
            company_customer.BateofBirth = customer.BateofBirth;
            company_customer.City = customer.City;
            company_customer.Contradiction = customer.Contradiction;
            company_customer.Email = customer.Email;
            company_customer.Gender = customer.Gender;
            company_customer.Image = customer.Image;
            company_customer.PostCode = customer.PostCode;
            company_customer.RefferedBy = customer.RefferedBy;
            company_customer.SmsNumber = customer.SmsNumber;
            company_customer.Telephone = customer.Telephone;
            company_customer.TimeZone = customer.TimeZone;
            company_customer.Adress = customer.Adress;
            db.SaveChanges();
            return RedirectToAction("CustomerList", "CompanyAdmin");

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
            // Get user id of currently logged in user
            var loggedInUserId = User.Identity.GetUserId();
            // Find the user from the db set
            var loggedInUser = db.AspNetUsers.Find(loggedInUserId);
            // Check if the user has a status
            bool hasStatus = false;
            if (loggedInUser.Status != null)
            {
                hasStatus = true;
                ViewBag.UserStatus = loggedInUser.Status;
            }
            ViewBag.HasStatus = hasStatus;
            return View(loggedInUser);
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
            var user_id = User.Identity.GetUserId();
            var customer = db.AspNetCustomers.Where(x => x.UserID == user_id).FirstOrDefault();

            if (customer == null)
            {
                var head_id = db.AspNetUsers.Where(x => x.Id == user_id).FirstOrDefault().HeadId;
                customer = db.AspNetCustomers.Where(x => x.UserID == head_id).FirstOrDefault();
            }

            var company_services = db.AspNetComanyServices.Where(x => x.CustomerId == customer.Id).ToList();
            List<AspNetServiceGroup> ser = new List<AspNetServiceGroup>();

            List<string> temp_group_ids = new List<string>();

            foreach (var item in company_services)
            {
                var service_group = db.AspNetService_Group.Where(x => x.Id == item.Service_GroupId).FirstOrDefault();
                var group = db.AspNetServiceGroups.Where(x => x.Id == service_group.GroupID).FirstOrDefault();

                if (!temp_group_ids.Contains(group.Name))
                {
                    ser.Add(group);
                    temp_group_ids.Add(group.Name);
                }
            }
            return View(ser);
        }


        public ActionResult ServiceList()
        {
            var user_id = User.Identity.GetUserId();
            var customer = db.AspNetCustomers.Where(x => x.UserID == user_id).FirstOrDefault();

            if (customer==null)
            {
                var head_id= db.AspNetUsers.Where(x => x.Id == user_id).FirstOrDefault().HeadId;
                customer= db.AspNetCustomers.Where(x => x.UserID == head_id).FirstOrDefault();
            }

            var company_services = db.AspNetComanyServices.Where(x => x.CustomerId == customer.Id).ToList();
            List<AspNetService> ser = new List<AspNetService>();
           
            List<string> temp_group_ids = new List<string>();

            foreach (var item in company_services)
            {
                var service_group = db.AspNetService_Group.Where(x => x.Id == item.Service_GroupId).FirstOrDefault();
                var group = db.AspNetServiceGroups.Where(x => x.Id == service_group.GroupID).FirstOrDefault();

                if (!temp_group_ids.Contains(group.Name))
                {
                    var services_list = db.AspNetService_Group.Where(x => x.GroupID == group.Id).Select(y => y.ServiceID).ToList();
                   

                    foreach (var item1 in services_list)
                    {
                        service_struct temp = new service_struct();
                        var service_obj = db.AspNetServices.Where(x => x.Id == item1).FirstOrDefault();
                        ser.Add(service_obj);
                    }

                    temp_group_ids.Add(group.Name);
                }
            }
            return View(ser);
        }

        public ActionResult UserList()
        {
            var id = User.Identity.GetUserId();
            var h_id = db.AspNetUsers.Where(x => x.Id == id).Select(y => y.HeadId).SingleOrDefault();
            var cus_data = new AspNetCustomer();
            List<UserListViewModel_1> obj = new List<UserListViewModel_1>();
            if (h_id == null)

            {

                cus_data = db.AspNetCustomers.Where(x => x.UserID == id).SingleOrDefault();

            }

            else

            {

                cus_data = db.AspNetCustomers.Where(x => x.UserID == h_id).SingleOrDefault();

            }
            
            var user = db.AspNetUsers.Where(x => (x.AspNetRoles.Select(y => y.Name).Contains("Agency_Manager") || x.AspNetRoles.Select(y => y.Name).Contains("Company_Admin") )&& (( x.HeadId == cus_data.UserID)||x.Id==cus_data.UserID));
            if (user != null)
            {
                foreach (var item in user)
                {
                    var user_id = item.Id;
                    var agency_data = db.AspNetAgencies.Where(x => x.UserID == user_id && x.HeadId == cus_data.Id).FirstOrDefault();
                    if (agency_data != null)
                    {
                        var list = new UserListViewModel_1();
                        list.Name = item.UserName;

                        list.Agency_Name = agency_data.AspNetCustomerDetail.BussinessName;
                        list.emial = item.Email;
                        var val = db.AspNetUsers.Where(x => x.Id == user_id).Select(x => x.AspNetRoles.Select(y => y.Name));
                        var ok = db.AspNetUsers.Where(x => x.Id == user_id).Select(x => x.AspNetRoles.Select(y => y.Name).FirstOrDefault()).FirstOrDefault();
                        list.Role = ok;
                        list.status = item.Status;
                        list.mobile = item.PhoneNumber;
                        obj.Add(list);


                      
                    }
                    else
                    {
                        var list = new UserListViewModel_1();
                        list.Name = item.UserName;

                        list.Agency_Name = "---";
                        list.emial = item.Email;
                        var ok = db.AspNetUsers.Where(x => x.Id == user_id).Select(x => x.AspNetRoles.Select(y => y.Name).FirstOrDefault()).FirstOrDefault();
                        list.Role = ok;
                        list.status = item.Status;
                        list.mobile = item.PhoneNumber;
                        obj.Add(list);


                    }
                }
            }


            return View(obj);
        }


        public ActionResult ViewService()
        {
            var ser = db.AspNetServices.ToList();
            return View(ser);
        }


        public ActionResult ViewServiceGroup()
        {
            return View();
        }
        public ActionResult VoucherList()
        {
            var data = db.AspNetGiftVouchers.ToList();
            return View(data);
        }

        public JsonResult Get_WorkingTime(string region_id)
        {
            var id = Convert.ToInt16(region_id);
            var working_id = db.AspNetAgencies.Where(x => x.RegionID == id).Select(y => y.WorkingID).FirstOrDefault();
            var obj = db.AspNetWorkingWeekTimes.Where(x => x.Id == working_id).FirstOrDefault();
            List<AspNetWorkingTime> list_time = new List<AspNetWorkingTime>();
            var mon = db.AspNetWorkingTimes.Where(x => x.Id == obj.MondayID).FirstOrDefault();
            var tues = db.AspNetWorkingTimes.Where(x => x.Id == obj.TuesdayID).FirstOrDefault();
            var wed = db.AspNetWorkingTimes.Where(x => x.Id == obj.WednesdayID).FirstOrDefault();
            var thus = db.AspNetWorkingTimes.Where(x => x.Id == obj.ThursdayID).FirstOrDefault();
            var fri = db.AspNetWorkingTimes.Where(x => x.Id == obj.FridayID).FirstOrDefault();
            var sat = db.AspNetWorkingTimes.Where(x => x.Id == obj.SaturdayID).FirstOrDefault();
            var sun = db.AspNetWorkingTimes.Where(x => x.Id == obj.SundayID).FirstOrDefault();

            list_time.Add(mon);
            list_time.Add(tues);
            list_time.Add(wed);
            list_time.Add(thus);
            list_time.Add(fri);
            list_time.Add(sat);
            list_time.Add(sun);
            List<WorkingTimeViewModel> list_WorkingTimeViewModel = new List<WorkingTimeViewModel>();
            foreach (var item in list_time)
            {
                var temp = new WorkingTimeViewModel();
                temp.Day = item.Day;
                temp.StartTime = item.StartTime.ToString();
                temp.EndTime = item.EndTime.ToString();
                temp.LunchToo = item.LunchToo.ToString();
                temp.LunchFrom = item.LunchFrom.ToString();
                temp.isoff = item.isoff.ToString();
                list_WorkingTimeViewModel.Add(temp);
            }
            return Json(list_WorkingTimeViewModel, JsonRequestBehavior.AllowGet);

        }
        public JsonResult Get_UserData(string agencyAdmin_id)
        {
            List<AspNetUser> user = new List<AspNetUser>();
            var user1 = db.AspNetUsers.Where(x => x.Id == agencyAdmin_id).SingleOrDefault();
            if (user1 != null)
            {
                user.Add(user1);
            }
            List<userdata> data = new List<userdata>();
            if (user!=null)
            {
                foreach (var item in user)
                {
                    var temp = new userdata();
                    temp.FirstName = item.FirstName;
                    temp.LastName = item.LastName;
                    temp.UserName = item.UserName;
                    temp.Email = item.Email;
                    data.Add(temp);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword(string oldPassword, string newPassword)
        {
            var loggedInUser = db.AspNetUsers.Find(User.Identity.GetUserId());
            // Get the password hash for the logged in user
            string hash = loggedInUser.PasswordHash;
            // now verify the password using hasher
            PasswordHasher hasher = new PasswordHasher();
            PasswordVerificationResult result = hasher.VerifyHashedPassword(hash, oldPassword);
            if (result == PasswordVerificationResult.Success)
            {
                string newHash = hasher.HashPassword(newPassword);
                loggedInUser.PasswordHash = newHash;
                db.Entry(loggedInUser).State = EntityState.Modified;
                db.SaveChanges();
                return Content("Password has been changed successfully.");
            }
            else
            {
                throw new Exception("Illegal password entered");
            }
        }


        public ActionResult EditUser(string id)
        {
            var user = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(AspNetUser user)
        {
            //var pass = db.AspNetUsers.Where(x => x.Id == user.Id).FirstOrDefault().PasswordHash;
            //user.PasswordHash = pass;

            user.Id = User.Identity.GetUserId();

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("UserList");
        }



    }
}