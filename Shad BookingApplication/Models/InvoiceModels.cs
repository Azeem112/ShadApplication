using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class InvoiceModels
    {
    }

    public class tax_struct
    {
        public string id { get; set; }
        public string rate { get; set; }
        public string name { get; set; }
    }

    public class payment_struct
    {
        public string id { get; set; }
        public string name { get; set; }
    }


}