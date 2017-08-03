using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class WorkingTimeViewModel
    { 
        public string Day { get; set; }
        public string isoff { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string LunchFrom { get; set; }
        public string LunchToo { get; set; }
    }
}