using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class AddEditPaymentDetailViewModel
    {
        public int ID { get; set; }
        public Nullable<int> AccountID { get; set; }
        public string PaymentMode { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string CheckNumberOrOnlineReciptNumber { get; set; }
        public string CheckDate { get; set; }
        public string TransactionDate { get; set; }
        public string Particulars { get; set; }
        public string VoucherType { get; set; }
        public string RecordType { get; set; }
        public Nullable<int> RecordId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<int> BankID { get; set; }
        public string BankName { get; set; }
        public string RoleId { get; set; }
        public Nullable<int> PaymentHeaderID { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string FullName { get; set; }

    }
}