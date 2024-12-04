using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class AddEditExpenseViewModel
    {
        public IEnumerable<SelectListItem> BankList { get; set; }
        public Nullable<int> BankID { get; set; }
        public IEnumerable<SelectListItem> PaymentModeList { get; set; }
        public Nullable<int> PaymentModeID { get; set; }
        public IEnumerable<SelectListItem> ExpenseTypeList { get; set; }
        public string AccountCode { get; set; }
        public IEnumerable<SelectListItem> AccountList { get; set; }
        public Nullable<int> AccountID { get; set; }
        public IEnumerable<SelectListItem> BranchesList { get; set; }
        public Nullable<int> BranchID { get; set; }



        public int ExpenseID { get; set; }
        public string PaymentMode { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string CheckNumberOnlineReciptNumber { get; set; }
        public string CheckDateOnlineDate { get; set; }
        public string TransactionDate { get; set; }
        public string Particulars { get; set; }
        public string VoucherType { get; set; }

        public AddEditExpenseViewModel()
        {

        }
    }
}