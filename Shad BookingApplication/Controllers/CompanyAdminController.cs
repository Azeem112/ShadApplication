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
    [Authorize(Roles = "Company_Admin")]

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

        [HttpPost]
        public ActionResult AddService(AspNetService aspNetService)
        {
            db.AspNetServices.Add(aspNetService);
            db.SaveChanges();

            var group = Request.Form["service_group_name"];

            var obj = new AspNetService_Group();
            obj.ServiceID = aspNetService.Id;
            obj.GroupID = Convert.ToInt16(group);

            db.AspNetService_Group.Add(obj);
            db.SaveChanges();

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
            addAgencyViewModel.BusinessSubCatageory = db.AspNetBusinessSubCatageories.ToList();
            ViewBag.AgencyAdmin = new SelectList(db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Company_Admin") || x.AspNetRoles.Select(y => y.Name).Contains("Agency_Manager")), "Id", "UserName");

            addAgencyViewModel.SMS = db.AspNetCustomerSMS.ToList();
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

            var user_id1 = User.Identity.GetUserId();
            var cus_id = db.AspNetCustomers.Where(x => x.UserID == user_id1).FirstOrDefault().Id;

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
                exp.Every = null;
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


        public ActionResult AgencyList()
        {
            var user_id1 = User.Identity.GetUserId();
            var cus_id = db.AspNetCustomers.Where(x => x.UserID == user_id1).Select(y => y.Id).FirstOrDefault();
            var data = db.AspNetAgencies.Where(x => x.HeadId == cus_id).ToList();
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
            return View();
        }


        public ActionResult ServiceList()
        {
            return View();
        }

        public ActionResult UserList()
        {
            var id = User.Identity.GetUserId();
            List<UserListViewModel_1> obj = new List<UserListViewModel_1>();
            var user = db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name).Contains("Agency_Manager") || x.AspNetRoles.Select(y => y.Name).Contains("Company_Admin") && x.HeadId == id);
            foreach (var item in user)
            {
                var user_id = item.Id;
                var agency_data = db.AspNetAgencies.Where(x => x.UserID == user_id).FirstOrDefault();
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
            return View(obj);
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
            user.Add(user1);
            List<userdata> data = new List<userdata>();
            foreach (var item in user)
            {
                var temp = new userdata();
                temp.FirstName = item.FirstName;
                temp.LastName = item.LastName;
                temp.UserName = item.UserName;
                temp.Email = item.Email;
                data.Add(temp);
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