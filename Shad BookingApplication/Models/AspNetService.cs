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
    
    public partial class AspNetService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetService()
        {
            this.AspNetBookingServices = new HashSet<AspNetBookingService>();
            this.AspNetService_Group = new HashSet<AspNetService_Group>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<int> TaxId { get; set; }
        public string Duration { get; set; }
        public string PaddingBefore { get; set; }
        public string PaddingAfter { get; set; }
        public string TotalMinutes { get; set; }
        public Nullable<bool> SeveralAppointments { get; set; }
        public Nullable<int> MaxAppointments { get; set; }
        public string ServiceColor { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsBookOnline { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetBookingService> AspNetBookingServices { get; set; }
        public virtual AspNetTax AspNetTax { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetService_Group> AspNetService_Group { get; set; }
    }
}
