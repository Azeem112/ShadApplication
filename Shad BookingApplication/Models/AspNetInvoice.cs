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
    
    public partial class AspNetInvoice
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<double> Ammount { get; set; }
        public string Status { get; set; }
        public Nullable<double> Tax { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> CompanyCustomerID { get; set; }
    
        public virtual AspNetCompanyCustomer AspNetCompanyCustomer { get; set; }
        public virtual AspNetCustomer AspNetCustomer { get; set; }
    }
}
