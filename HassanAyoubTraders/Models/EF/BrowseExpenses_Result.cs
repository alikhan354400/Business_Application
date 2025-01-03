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
    
    public partial class BrowseExpenses_Result
    {
        public int ExpenseID { get; set; }
        public Nullable<int> AccountID { get; set; }
        public Nullable<int> PaymentModeID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string CheckNumberOrOnlineReciptNumber { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string Particulars { get; set; }
        public string VoucherType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<int> BankID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> ExpenseTypeID { get; set; }
        public string RoleID { get; set; }
        public string AccountName { get; set; }
        public string PaymentMethod { get; set; }
        public string BankName { get; set; }
        public string ExpenseType { get; set; }
    }
}
