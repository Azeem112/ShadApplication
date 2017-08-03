﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class AddCustomerViewModel
    {
        public AspNetCustomerDetail Detail { get; set; }
        public AspNetCustomerLocation Location { get; set; }
        public AspNetCustomerContact Contact { get; set; }

        public AspNetCustomerRegion Region { get; set; }
        public AspNetCustomerGallery Gallery { get; set; }
        public AspNetCustomerBusinessDetail BusinessDetail { get; set; }

        public AspNetCustomerType UserType { get; set; }
        public AspNetWorkingTime WorkingTime { get; set; }
        public AspNetSocial Social { get; set; }

        public AspNetUser User { get; set; }
        public AspNetBusinessCatageory BusinessCatageorySingle { get; set; }

        public IEnumerable<AspNetCustomerSM> SMS { get; set; }
        public AspNetCustomerSM SingleSms { get; set; }
        public IEnumerable<AspNetBusinessCatageory> BusinessCatageory { get; set; }
        public IEnumerable<AspNetBusinessSubCatageory> BusinessSubCatageory { get; set; }

        public string[] Customer_SubCatageory { get; set; }
        public HttpPostedFileBase[] files { get; set; }

    }

    public class Region_Struct
    {
        public string CountryName { get; set; }
        public string TimeZoneName { get; set; }
        public string DateFormate { get; set; }

        public string TimeFormate { get; set; }
        public string CurrencyName { get; set; }
        public string Location_CountryName { get; set; }

        //LocCountryName
    }
}

