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
    
    public partial class AspNetItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetItem()
        {
            this.AspNetCustomerSMS = new HashSet<AspNetCustomerSM>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> Vat { get; set; }
        public Nullable<double> Price_W_O_Vat { get; set; }
        public Nullable<double> Price_W__Vat { get; set; }
        public string Status { get; set; }
        public string IsSmsPackage { get; set; }
        public Nullable<int> RemainingSMS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetCustomerSM> AspNetCustomerSMS { get; set; }
        public virtual AspNetTax AspNetTax { get; set; }
    }
}
