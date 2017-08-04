﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Shad_BookingApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
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

        public string Get_Role(string id)
        {
            var val = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.AspNetRoles.Select(y => y.Name));
           var ok=  db.AspNetUsers.Where(x => x.Id == id).Select(x => x.AspNetRoles.Select(y => y.Name).FirstOrDefault()).FirstOrDefault();
                return ok;
        }

        public string Get_Company(string id)
        {
            var user_type_id = db.AspNetCustomers.Where(x => x.UserID == id).Select(y => y.TypeID).FirstOrDefault();
            if (user_type_id != null)
               return db.AspNetCustomerTypes.Where(x => x.Id == user_type_id).Select(y => y.CompanyName).FirstOrDefault();
            else
                return "-";
        }


        public ActionResult EditUser(string id)
        {
            var user=db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(AspNetUser user)
        {
            //var pass = db.AspNetUsers.Where(x => x.Id == user.Id).FirstOrDefault().PasswordHash;
            //user.PasswordHash = pass;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("UserList");
        }

        public ActionResult UserList()
        {
            List<UserListViewModel> list_data = new List<UserListViewModel>();
          
            var users = db.AspNetUsers.ToList();                  //Name,email,phone_no
            int i = 1;
            foreach (var item in users)
            {
                var obj = new UserListViewModel();
                obj.id = i;
                i++;
                obj.email = item.Email;
                obj.mobile = item.PhoneNumber;
                obj.name = item.UserName;
                obj.status = item.Status;
                obj.role = Get_Role(item.Id);
                obj.company = Get_Company(item.Id);
                obj.user_id = item.Id;

                list_data.Add(obj);
            }
            return View(list_data);

         
        }
        public ActionResult AddSuperAdmin()
        {
          //  dynamic mymodel = new ExpandoObject(); 
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBusinessCategory([Bind(Include="Name")] AspNetBusinessCatageory aspNetBusinessCatageory)
        {
            db.AspNetBusinessCatageories.Add(aspNetBusinessCatageory);
            db.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBusinessSubCategory([Bind(Include = "Name,BussinessCatageoryId")] AspNetBusinessSubCatageory aspNetBusinessSubCatageory)
        {
            db.AspNetBusinessSubCatageories.Add(aspNetBusinessSubCatageory);
            db.SaveChanges();
            return RedirectToAction("Dashboard");
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


                    return RedirectToAction("UserList");
                }
            }
            catch(Exception ex)
            {

            }
            
            return RedirectToAction("UserList");
        }
        //EditItem

        public ActionResult EditItem(int ? id)
        {
            var ls = db.AspNetTaxes.ToList();
            ViewBag.tax_list = ls;

            var item = db.AspNetItems.Where(x => x.Id == id).FirstOrDefault();
            return View(item);
        }

        [HttpPost]
        public ActionResult EditItem(AspNetItem aspNetItem)
        {
            db.Entry(aspNetItem).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ItemList");
        }

        public ActionResult AddItem()
        {
            var ls = db.AspNetTaxes.ToList();
            ViewBag.tax_list = ls;
         
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem([Bind(Include = "Id,Name,Description,Vat,Price_W_O_Vat,Price_W__Vat,Status,IsSmsPackage")] AspNetItem aspNetItem)
        {
            if (ModelState.IsValid)
            {
                if (aspNetItem.IsSmsPackage.Equals("sms"))
                {
                    var no_sms_package = Request.Form["no_remaining_sms"];
                    aspNetItem.RemainingSMS = Convert.ToInt16(no_sms_package);
                    db.AspNetItems.Add(aspNetItem);
                    db.SaveChanges();
                }
                else
                {
                    db.AspNetItems.Add(aspNetItem);
                    db.SaveChanges();
                }
                return RedirectToAction("ItemList");
            }

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


        public ActionResult AddCustomer()
        {
            AddCustomerViewModel addCustomerViewModel = new AddCustomerViewModel();
            addCustomerViewModel.BusinessCatageory = db.AspNetBusinessCatageories.ToList();
            var sms = db.AspNetItems.Where(x => x.IsSmsPackage == "sms").ToList();
            addCustomerViewModel.sms_items = sms;
            return View(addCustomerViewModel);
        }

        

        [HttpPost]
        public ActionResult AddCustomer(AddCustomerViewModel addCustomerViewModel, HttpPostedFileBase[] files)
        {
            // Adding Details
            var list_of_subcat_ids = Get_SubCat_Id(addCustomerViewModel.Customer_SubCatageory);
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

            BusinessDetail.Description = addCustomerViewModel.BusinessDetail.Description;
            BusinessDetail.SRIET = addCustomerViewModel.BusinessDetail.SRIET;
            BusinessDetail.VatNumber = addCustomerViewModel.BusinessDetail.VatNumber;
            BusinessDetail.APE = addCustomerViewModel.BusinessDetail.APE;

            db.AspNetCustomerBusinessDetails.Add(BusinessDetail);
            db.SaveChanges();


            // Adding user Types
            addCustomerViewModel.UserType.CompanyName = addCustomerViewModel.Detail.BussinessName;
            db.AspNetCustomerTypes.Add(addCustomerViewModel.UserType);
            db.SaveChanges();

            // Getting SMS ID
            var sms_package_id = addCustomerViewModel.SingleSms.ItemID;
            var sms_package = new AspNetCustomerSM();

            // Saving Customer SMS Package
            var item = db.AspNetItems.Where(x => x.Id == sms_package_id).FirstOrDefault();
            sms_package.ItemID = sms_package_id;
            sms_package.SmsPackageName = item.Name;
            sms_package.RemainingSMS = item.RemainingSMS;

            db.AspNetCustomerSMS.Add(sms_package);
            db.SaveChanges();


            // Adding Working time
            var time = Request.Form["custom_timepicker_name"].ToString();
            var id = time_string_parse(time);

            //  var businessDetails_textarea = Request.Form["editor1"];

            // Adding Social
            db.AspNetSocials.Add(addCustomerViewModel.Social);
            db.SaveChanges();

            // Adding User
            var user_id=AddCustomerAccount(addCustomerViewModel.User, addCustomerViewModel.Contact.Mobile);


            var customer = new AspNetCustomer();
            customer.BussinessID = BusinessDetail.Id;
            customer.RegionID = region.Id;
            customer.LocationId = addCustomerViewModel.Location.Id;
            customer.SmsID = sms_package.Id;
            customer.TypeID = addCustomerViewModel.UserType.Id;
            customer.ContactId = addCustomerViewModel.Contact.Id;
            customer.DetailId = addCustomerViewModel.Detail.Id;
            customer.SocialID = addCustomerViewModel.Social.Id;
            customer.UserID = user_id;
            customer.GalleryID = gallary.Id;
            customer.WorkingID = id;

            db.AspNetCustomers.Add(customer);
            db.SaveChanges();

            foreach (var item1 in list_of_subcat_ids)
            {
                var obj = new AspNetCustomer_SubCatageory();
                obj.CustomerID = customer.Id;
                obj.SubCatageoryId = item1;


                db.AspNetCustomer_SubCatageory.Add(obj);
                db.SaveChanges();
            }
            return RedirectToAction("CustomersList");
        }

        public ActionResult EditCustomer(int? id)
        {
            var customer_list = db.AspNetCustomers.ToList();
            var customer = customer_list.Where(x => x.Id == id).Select(x => x).FirstOrDefault();

            var users = db.AspNetUsers.Where(x=>x.Id== customer.UserID).Select(x => x).FirstOrDefault();
            var user_type = db.AspNetCustomerTypes.Where(x => x.Id == customer.TypeID).Select(x => x).FirstOrDefault();
            var address = db.AspNetCustomerLocations.Where(x => x.Id == customer.LocationId).Select(x => x).FirstOrDefault();
            var detail=db.AspNetCustomerDetails.Where(x=>x.Id==customer.DetailId).Select(x => x).FirstOrDefault();

            var bus_detail=db.AspNetCustomerBusinessDetails.Where(x=>x.Id==customer.BussinessID).Select(y => y).FirstOrDefault();
            var contact = db.AspNetCustomerContacts.Where(x => x.Id == customer.ContactId).Select(x => x).FirstOrDefault();
            var social = db.AspNetSocials.Where(x => x.Id == customer.SocialID).Select(x => x).FirstOrDefault();
                                              
            var detail_id = db.AspNetCustomers.Where(x => x.Id == id).FirstOrDefault().DetailId;
            var bus_cat_id = db.AspNetCustomerDetails.Where(x => x.Id == detail_id).FirstOrDefault().BusinessCatageoryId;
            var bus_cat_name = db.AspNetBusinessCatageories.Where(x => x.Id == bus_cat_id).FirstOrDefault().Name;
            var bus_subcat_list = db.AspNetBusinessSubCatageories.Where(x => x.BussinessCatageoryId == bus_cat_id).ToList();

            var user_type_id=db.AspNetCustomers.Where(x => x.Id == id).FirstOrDefault().TypeID;
            var SingleorMulti = db.AspNetCustomerTypes.Where(x => x.Id == user_type_id).FirstOrDefault().SingleorMulti;
            var MultiNumber = db.AspNetCustomerTypes.Where(x => x.Id == user_type_id).FirstOrDefault().MultiNumber;
            var status = db.AspNetCustomerTypes.Where(x => x.Id == user_type_id).FirstOrDefault().Status;
            var sms_id = db.AspNetCustomers.Where(x => x.Id == id).FirstOrDefault().SmsID;
            var sms_package_name = db.AspNetCustomerSMS.Where(x => x.Id == sms_id).FirstOrDefault().SmsPackageName;


            // Getting bus Sub cat id's and then convert in to name

            var selected_subCatlist = db.AspNetCustomer_SubCatageory.Where(x => x.CustomerID == id).ToList();
            List<string> selected_subCatlist_name = new List<string>();
            foreach (var item in selected_subCatlist)
            {
                var name=db.AspNetBusinessSubCatageories.Where(x => x.Id == item.SubCatageoryId).FirstOrDefault().Name;
                selected_subCatlist_name.Add(name);
            }


            var region_id = db.AspNetCustomers.Where(x => x.Id == id).FirstOrDefault().RegionID;
            var region_obj = db.AspNetCustomerRegions.Where(x => x.Id == region_id).FirstOrDefault();




            if (MultiNumber != null && MultiNumber != 0)
                ViewBag.MultiNumber = MultiNumber;
            else
                ViewBag.MultiNumber = 0;


            ViewBag.BusinessCatageory = db.AspNetBusinessCatageories.ToList();
            ViewBag.selected_BusinessCat = bus_cat_name;
            ViewBag.selected_BusinessSubCat = bus_subcat_list;
            ViewBag.SingleorMulti = SingleorMulti;
            ViewBag.UserType_Status = status;
            ViewBag.SMSPackage= db.AspNetCustomerSMS.ToList();
            ViewBag.sms_package_name = sms_package_name;
            ViewBag.selected_CurrencyName = region_obj.CurrencyName;
            ViewBag.selected_subCatlist_name = selected_subCatlist_name;



         

            AddCustomerViewModel viewmodel = new AddCustomerViewModel();
            viewmodel.User = users;
            viewmodel.Location = address;
            viewmodel.UserType = user_type;
            viewmodel.Detail = detail;
            viewmodel.BusinessDetail = bus_detail;
            viewmodel.Contact = contact;
            viewmodel.Social = social;
            viewmodel.Region = region_obj;

            return View(viewmodel);
        }


        public void Update_SubCatagory(List<int> sub_CatIdlist, int region_id)
        {
            var cus_id = db.AspNetCustomers.Where(x => x.RegionID == region_id).FirstOrDefault().Id;
            var ls_subCat = db.AspNetCustomer_SubCatageory.Where(x => x.CustomerID == cus_id).ToList();

            // db.AspNetCustomer_SubCatageor


            //ctx.Entry(employer).State = EntityState.Deleted;
            //ctx.SaveChanges();

            foreach (var item in ls_subCat)
            {
                var obj = db.AspNetCustomer_SubCatageory.Where(x => x.SubCatageoryId == item.SubCatageoryId && x.CustomerID == cus_id).FirstOrDefault();
                db.Entry(obj).State = EntityState.Deleted;
                db.SaveChanges();
            }

            foreach (var item in sub_CatIdlist)
            {
                var obj = new AspNetCustomer_SubCatageory();
                obj.SubCatageoryId = item;
                obj.CustomerID = cus_id;
                db.AspNetCustomer_SubCatageory.Add(obj);
                db.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult EditCustomer(AddCustomerViewModel addCustomerViewModel)
        {

            var list_of_subcat_ids = new List<int>();
            foreach (var item in addCustomerViewModel.Customer_SubCatageory)            // Converting string => int 
            {
                var value= Convert.ToInt16(item);
                list_of_subcat_ids.Add(value);
            }

            addCustomerViewModel.Detail.SubCatageoryNo = list_of_subcat_ids.Count();

            Update_SubCatagory(list_of_subcat_ids, addCustomerViewModel.Region.Id);

           // addCustomerViewModel.Detail.BusinessCatageoryId


            // adding location missing value from models
            addCustomerViewModel.Location.CountryName= Request.Form["LocCountryName"].ToString();

            //Contact.Mobile
            addCustomerViewModel.User.PhoneNumber = addCustomerViewModel.Contact.Mobile;

            //company name
            addCustomerViewModel.UserType.CompanyName= addCustomerViewModel.Detail.BussinessName;


            var region_country = Request.Form["region_country"].ToString();
            var region_timezone = Request.Form["region_timezone"];
            var region_dateformat = Request.Form["region_dateformat"].ToString();
            var region_timeformat = Request.Form["region_timeformat"].ToString();
            var region_currency = Request.Form["region_currency"].ToString();


            var region = new AspNetCustomerRegion();

            region.Id = addCustomerViewModel.Region.Id;
            region.CountryName = region_country;
            region.TimeZoneName = region_timezone;
            region.DateFormate = region_dateformat;
            region.TimeFormate = region_timeformat;
            region.CurrencyName = region_currency;


            // updating time week
            var time = Request.Form["custom_timepicker_name"].ToString();
            var work_id =(int) db.AspNetCustomers.Where(x => x.RegionID == addCustomerViewModel.Region.Id).FirstOrDefault().WorkingID;
            time_string_parse(time, work_id);


          //  var bb=db.AspNetCustomer_SubCatageory.Where()


            try
                {
                db.Entry(addCustomerViewModel.Location).State = EntityState.Modified;
                db.SaveChanges();

                db.Entry(region).State = EntityState.Modified;
                db.SaveChanges();

                //db.Entry(addCustomerViewModel.Gallery).State = EntityState.Modified;
                //db.SaveChanges();

                db.Entry(addCustomerViewModel.BusinessDetail).State = EntityState.Modified;
                db.SaveChanges();

                db.Entry(addCustomerViewModel.Detail).State = EntityState.Modified;
                db.SaveChanges();

                db.Entry(addCustomerViewModel.Contact).State = EntityState.Modified;
                db.SaveChanges();

                db.Entry(addCustomerViewModel.UserType).State = EntityState.Modified;
                db.SaveChanges();

                db.Entry(addCustomerViewModel.Social).State = EntityState.Modified;
                db.SaveChanges();

                db.Entry(addCustomerViewModel.User).State = EntityState.Modified;
                db.SaveChanges();

                }
                catch (Exception ex)
                {
                   // db.Refresh(RefreshMode.ClientWins, db.Articles);
                   // context.SaveChanges();
                }
                
                
                return RedirectToAction("CustomersList");
            
           // return View();
        }

        public string AddCustomerAccount(AspNetUser aspNetUser,string phone_no)
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
                        temp_user.FirstName = aspNetUser.FirstName;
                        temp_user.LastName = aspNetUser.LastName;
                        temp_user.PhoneNumber = phone_no;
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


        private void time_string_parse(string time,int work_id)
        {
            var mon_obj = new AspNetWorkingTime();
            var tue_obj = new AspNetWorkingTime();
            var wed_obj = new AspNetWorkingTime();
            var thus_obj = new AspNetWorkingTime();
            var fri_obj = new AspNetWorkingTime();
            var sat_obj = new AspNetWorkingTime();
            var sun_obj = new AspNetWorkingTime();

            var a =(int) db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().MondayID;
            var b = (int)db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().TuesdayID;
            var c = (int)db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().WednesdayID;
            var d = (int)db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().ThursdayID;
            var e = (int)db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().FridayID;
            var f = (int)db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().SaturdayID;
            var g = (int)db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().SundayID;

            mon_obj.Id = a;
            tue_obj.Id = b;
            wed_obj.Id = c;
            thus_obj.Id = d;
            fri_obj.Id = e;
            sat_obj.Id = f;
            sun_obj.Id = g;


            mon_obj.isoff = true;
            tue_obj.isoff = true;
            wed_obj.isoff = true;
            thus_obj.isoff = true;

            fri_obj.isoff = true;
            sat_obj.isoff = true;
            sun_obj.isoff = true;


            db.Entry(mon_obj).State = EntityState.Modified;
            db.Entry(tue_obj).State = EntityState.Modified;
            db.Entry(wed_obj).State = EntityState.Modified;
            db.Entry(thus_obj).State = EntityState.Modified;
            db.Entry(fri_obj).State = EntityState.Modified;
            db.Entry(sat_obj).State = EntityState.Modified;
            db.Entry(sun_obj).State = EntityState.Modified;

            db.SaveChanges();


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

                    var monid = db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().MondayID;
                    mon_obj.Id =(int) monid;
                    db.Entry(mon_obj).State= EntityState.Modified;
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
                    var tueid = db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().TuesdayID;
                    tue_obj.Id = (int)tueid;
                    db.Entry(tue_obj).State = EntityState.Modified;
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
                    var wedid = db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().WednesdayID;
                    wed_obj.Id = (int)wedid;
                    db.Entry(wed_obj).State = EntityState.Modified;
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

                    var thusid = db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().ThursdayID;
                    thus_obj.Id = (int)thusid;
                    db.Entry(thus_obj).State = EntityState.Modified;
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

                    var friid = db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().FridayID;
                    fri_obj.Id = (int)friid;
                    db.Entry(fri_obj).State = EntityState.Modified;
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

                    var satid = db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().SaturdayID;
                    sat_obj.Id = (int)satid;
                    db.Entry(sat_obj).State = EntityState.Modified;
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
                    var sunid = db.AspNetWorkingWeekTimes.Where(x => x.Id == work_id).FirstOrDefault().SundayID;
                    sun_obj.Id = (int)sunid;
                    db.Entry(sun_obj).State = EntityState.Modified;
                    db.SaveChanges();

                }// end if sat

            }// end foreach
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

        public JsonResult Get_RegionData(string region_id)
        {
            var id = Convert.ToInt16(region_id);
            var region_obj = db.AspNetCustomerRegions.Where(x => x.Id == id).FirstOrDefault();
           
            Region_Struct obj = new Region_Struct();
            obj.CountryName = region_obj.CountryName;
            obj.CurrencyName = region_obj.CurrencyName;
            obj.DateFormate = region_obj.DateFormate;
            obj.TimeFormate = region_obj.TimeFormate;
            obj.TimeZoneName = region_obj.TimeZoneName;

            var location_id = db.AspNetCustomers.Where(x => x.RegionID == id).FirstOrDefault().LocationId;
            var loc = db.AspNetCustomerLocations.Where(x => x.Id == location_id).FirstOrDefault().CountryName;
            obj.Location_CountryName = loc;


            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Get_WorkingTime(string region_id)
        {
            var id = Convert.ToInt16(region_id);

            var working_id = db.AspNetCustomers.Where(x => x.RegionID==id).FirstOrDefault().WorkingID;
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



        public SortedDictionary<string, string> Get_CustomerName()
        {
            var ls = db.AspNetCustomers.ToList();

            SortedDictionary<string, string> customer_list = new SortedDictionary<string, string>();
            foreach (var item in ls)
            {
                var id = item.UserID;
                var name = db.AspNetUsers.Where(x => x.Id == id).Select(y => y.UserName).FirstOrDefault();
                customer_list.Add(name, item.Id.ToString());
            }
            return customer_list;
        }

        public JsonResult Get_SuperAdminList()
        {
            List< super_admin_struct > ls = new List<super_admin_struct>();
            var super_admin_list = db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name == "Super_Admin").FirstOrDefault()).ToList();
            foreach (var item in super_admin_list)
            {
                var obj = new super_admin_struct();
                obj.name = item.UserName;
                obj.id = item.Id;

                ls.Add(obj);
            }
            return Json(ls, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get_tax(string item_id)
        {
            var id=Convert.ToInt16(item_id);
            var tax = db.AspNetItems.Where(x => x.Id == id).Select(y => y.Vat).FirstOrDefault();
            return Json(tax, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get_TaxRate(string item_id)
        {
            var id = Convert.ToInt16(item_id);
            var tax = db.AspNetTaxes.Where(x => x.Id == id).Select(y => y.Rate).FirstOrDefault().ToString();
            return Json(tax, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Get_ItemList()
        {
            List<item_struct> ls = new List<item_struct>();
            var item_list = db.AspNetItems.ToList();
            foreach (var item in item_list)
            {
                var obj = new item_struct();
                obj.name = item.Name;
                obj.id = item.Id.ToString();
                obj.tax = item.Vat.ToString();
                ls.Add(obj);
            }
            return Json(ls, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create_Invoice()
        {
            var customer_list=db.AspNetCustomers.ToList();
            //var super_admin_list = db.AspNetUsers.Where(x => x.AspNetRoles.Select(y => y.Name == "Super_Admin").FirstOrDefault()).ToList();
            //var item_list = db.AspNetItems.ToList();
            var discount_list = db.AspNetDiscounts.ToList();

 
            ViewBag.customer_list = Get_CustomerName();
            ViewBag.discount_list = discount_list;
            return View();


        }

        [HttpPost]
        public ActionResult Create_Invoice(InvoiceViewModel invoiceViewModel)
        {
            var totel = Request.Form["totel_ammount_final_name"];
            var in_date = Request.Form["in_date_name"];
            var due_date = Request.Form["due_date_name"];
            var Customer_key = Request.Form["Customer_Name"];

            var invoice = new AspNetInvoice();
            invoice.Status = "UnPaid";
            invoice.CustomerID =Convert.ToInt16(Customer_key);
            invoice.InvoiceDate = Convert.ToDateTime(in_date);
            invoice.DueDate = Convert.ToDateTime(due_date);
            invoice.Ammount = Convert.ToDouble(totel);
            invoice.Tax = 0;

            db.AspNetInvoices.Add(invoice);
            db.SaveChanges();

            return RedirectToAction("Create_Invoice2");
        }


        public ActionResult Create_Invoice2()
        {
            var max_id=db.AspNetInvoices.Max(x => x.Id);
            var obj=db.AspNetInvoices.Where(x => x.Id == max_id).FirstOrDefault();
            return View(obj);
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

        public ActionResult AddDiscount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDiscount(AspNetDiscount aspNetDiscount)
        {
            if (ModelState.IsValid)
            {
                db.AspNetDiscounts.Add(aspNetDiscount);
                db.SaveChanges();
            }
            return View();
        }

        public List<customer_struct> Get_InvoiceCustomer()
        {
            List<customer_struct> cus_ls = new List<customer_struct>();
            var ls= db.AspNetInvoices.ToList();
            foreach (var item in ls)
            {
                var obj = new customer_struct();
                var user_id = db.AspNetCustomers.Where(x => x.Id == item.CustomerID).FirstOrDefault().UserID;
                var user = db.AspNetUsers.Where(x => x.Id == user_id).FirstOrDefault();

                obj.email = user.Email;
                obj.name = user.UserName;
                obj.phone_no = user.PhoneNumber;

                cus_ls.Add(obj);
            }
            return cus_ls;
        }

        public ActionResult InvoiceList()
        {
            InvoiceViewModel viewmodel = new InvoiceViewModel();
            viewmodel.invoice_list = db.AspNetInvoices.ToList();
            viewmodel.customer_list = Get_InvoiceCustomer();
            return View(viewmodel);
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

        public JsonResult Insert_tax(string name, string rate)
        {
            var obj = new AspNetTax();

            try
            {
                obj.Name = name;
                obj.Rate = Convert.ToDouble(rate);
                db.AspNetTaxes.Add(obj);
                db.SaveChanges();
                var id=obj.Id.ToString();
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.InnerException.ToString(), JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult GetTax_ForInvoiceOptions()
        {
            List<tax_struct> ls = new List<tax_struct>();
            var list_taxes=db.AspNetTaxes.ToList();
            
            foreach (var item in list_taxes)
            {
                var obj = new tax_struct();
                obj.id = item.Id.ToString();
                obj.name = item.Name;
                obj.rate = item.Rate.ToString();

                ls.Add(obj);

            }
            return Json(ls, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete_tax(string id)
        {
            var tax_id = Convert.ToInt16(id);
            var tax_obj = db.AspNetTaxes.Where(x=>x.Id == tax_id).FirstOrDefault();
            db.AspNetTaxes.Remove(tax_obj);
            db.SaveChanges();

            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }

        public JsonResult Update_tax(string id,string name,string rate)
        {
            var tax_id = Convert.ToInt16(id);
            var tax_rate = Convert.ToDouble(rate);

            var obj = new AspNetTax();
            obj.Id = tax_id;
            obj.Name = name;
            obj.Rate = tax_rate;

            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();

            return Json("Updated", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InvoiceOption(AspNetInvoiceOption aspNetInvoiceOption)
        {
            if (aspNetInvoiceOption.HideName == null)
                aspNetInvoiceOption.HideName = false;

            if (aspNetInvoiceOption.HidePaymentHistory == null)
                aspNetInvoiceOption.HidePaymentHistory = false;

            db.AspNetInvoiceOptions.Add(aspNetInvoiceOption);
            db.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public JsonResult Insert_Payment(string name)
        {
            var obj = new AspNetBookingPaymentType();

            try
            {
                obj.Name = name;
                db.AspNetBookingPaymentTypes.Add(obj);
                db.SaveChanges();
                var id = obj.Id.ToString();
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.InnerException.ToString(), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult Update_Payment(string id, string name)
        {
            var tax_id = Convert.ToInt16(id);
            
            var obj = new AspNetBookingPaymentType();
            obj.Id = tax_id;
            obj.Name = name;

            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();

            return Json("Updated", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPayment_ForInvoiceOptions()
        {
            List<payment_struct> ls = new List<payment_struct>();
            var list_taxes = db.AspNetBookingPaymentTypes.ToList();

            foreach (var item in list_taxes)
            {
                var obj = new payment_struct();
                obj.id = item.Id.ToString();
                obj.name = item.Name;

                ls.Add(obj);

            }
            return Json(ls, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete_Payment(string id)
        {
            var pay_id = Convert.ToInt16(id);
            var pay_obj = db.AspNetBookingPaymentTypes.Where(x => x.Id == pay_id).FirstOrDefault();
            db.AspNetBookingPaymentTypes.Remove(pay_obj);
            db.SaveChanges();

            return Json("Deleted", JsonRequestBehavior.AllowGet);
        }
        //Delete_Payment
    }
}