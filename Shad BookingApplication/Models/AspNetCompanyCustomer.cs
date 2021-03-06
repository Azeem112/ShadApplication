//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shad_BookingApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetCompanyCustomer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetCompanyCustomer()
        {
            this.AspNetBookings = new HashSet<AspNetBooking>();
        }
    
        public int Id { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string TimeZone { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> BateofBirth { get; set; }
        public string RefferedBy { get; set; }
        public string Contradiction { get; set; }
        public Nullable<int> CompanyNotificationId { get; set; }
        public Nullable<int> AgencyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string SmsNumber { get; set; }
        public string Email { get; set; }
        public string Occupation { get; set; }
        public string Image { get; set; }
    
        public virtual AspNetAgency AspNetAgency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetBooking> AspNetBookings { get; set; }
        public virtual AspNetCompanyNotifination AspNetCompanyNotifination { get; set; }
    }
}
