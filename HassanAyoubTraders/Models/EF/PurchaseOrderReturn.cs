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
    
    public partial class PurchaseOrderReturn
    {
        public int PurchaseOrderReturnID { get; set; }
        public Nullable<int> PONumber { get; set; }
        public string RecordType { get; set; }
        public Nullable<int> EntryTypeID { get; set; }
        public Nullable<int> VendorID { get; set; }
        public Nullable<System.DateTime> PODate { get; set; }
        public string InvoiceNo { get; set; }
        public string Notes { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> Expense { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> SalesTax { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string RoleID { get; set; }
        public string Status { get; set; }
        public Nullable<int> SalesOrderID { get; set; }
        public Nullable<int> BookerID { get; set; }
        public Nullable<int> SupplierID { get; set; }
    }
}