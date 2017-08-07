using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class AddCompanyCustomerViewModel
    {
        public AspNetCompanyCustomer Customer { set; get; }
        public AspNetCompanyNotifination Notification { set; get; }
    }
}