using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Shad_BookingApplication.Models
{
    public class BookingDbContext: DbContext
    {
        public DbSet<AspNetUser> Users { get; set; }
    }
}