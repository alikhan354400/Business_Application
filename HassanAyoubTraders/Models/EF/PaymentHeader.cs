//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HassanAyoubTraders.Models.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaymentHeader
    {
        public int ID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public string ReceiptNo { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string Particulars { get; set; }
        public string VoucherType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
    }
}
