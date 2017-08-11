using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class AddEmployeeViewModel
    {
        public AspNetEmployee Employee { get; set; }
        public AspNetUser user { get; set; }
        public List<AspNetAgency> agency_list { get; set; }

        public List<group_struct> GroupServicesList { get; set; }

    }

    public class group_struct
    {
        public string group_name { get; set; }
        public int group_id { get; set; }

        public List<service_struct> service_grouplist { get; set; }
    }

    public class service_struct
    {
        public string service_name { get; set; }
        public int service_id { get; set; }
    }
}