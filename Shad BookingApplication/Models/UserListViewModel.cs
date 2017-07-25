using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class UserListViewModel
    {
        public int id { get; set; }
        public string name { get; set; }

        public string role { get; set; }
        public string company { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string status { get; set; }

    }
}