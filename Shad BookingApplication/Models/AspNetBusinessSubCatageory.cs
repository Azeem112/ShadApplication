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
    
    public partial class AspNetBusinessSubCatageory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetBusinessSubCatageory()
        {
            this.AspNetCustomer_SubCatageory = new HashSet<AspNetCustomer_SubCatageory>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> BussinessCatageoryId { get; set; }
    
        public virtual AspNetBusinessCatageory AspNetBusinessCatageory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetCustomer_SubCatageory> AspNetCustomer_SubCatageory { get; set; }
    }
}
