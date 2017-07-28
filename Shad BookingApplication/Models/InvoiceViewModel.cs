using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class InvoiceViewModel
    {
        public AspNetInvoice invoice { get; set; }
        public List<AspNetDiscount> discount_list { get; set; }

        public List<AspNetInvoice> invoice_list { get; set; }
        public List<customer_struct> customer_list { get; set; }
    }

    public class super_admin_struct
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class item_struct
    {
        public string name { get; set; }
        public string id { get; set; }
        public string tax { get; set; }
    }

    public class customer_struct
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone_no { get; set; }
    }
}