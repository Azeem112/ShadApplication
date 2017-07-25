using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class CustomerList_Data
    {
        public int customer_id { get; set; }
        public string user_name { get; set; }
        public string email { get; set; }
        public string phone_num { get; set; }
        public string company_name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public int zipcode { get; set; }
        public string country { get; set; }
        public int rem_sms { get; set; }
        public string status { get; set; }


    }
}