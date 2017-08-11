using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class AddCompanyCustomerViewModel
    {
       
        public string Adress { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string TimeZone { get; set; }
        public string Gender { get; set; }
        public DateTime BateofBirth { get; set; }
        public string RefferedBy { get; set; }
        public string Contradiction { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string SmsNumber { get; set; }
        public string Email { get; set; }
        public string Occupation { get; set; }
        public string Image { get; set; }
        
        public AspNetAgency Agency { get; set; }
        public AspNetCompanyNotifination Notification {  get; set; }
    }
}