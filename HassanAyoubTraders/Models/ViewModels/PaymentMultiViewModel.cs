using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class PaymentMultiViewModel
    {
        public IEnumerable<SelectListItem> EmployeeList { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public IEnumerable<SelectListItem> AccountList { get; set; }
        public int AccountID { get; set; }
        public IEnumerable<SelectListItem> ModeList { get; set; }
        public string ModeID { get; set; }
        public IEnumerable<SelectListItem> BankList { get; set; }

        public Nullable<int> BankID { get; set; }

        public IEnumerable<SelectListItem> ExtraItemList { get; set; }
        public int ExtraItemID { get; set; }

        public int ID { get; set; }
        public int PaymentHeaderID { get; set; }
        public string ReceiptNo { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Particulars { get; set; }
        public string VoucherType { get; set; }
        public string PaymentDate { get; set; }
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public bool IsUpdate { get; set; }


        public IList<AddEditPaymentDetailViewModel> PaymentDetail { get; set; }
        public List<AddEditPaymentDetailViewModel> EditPaymentDetail { get; set; }
        public IList<BrowsePaymentHeader_Result> PaymentList { get; set; }

        public PaymentMultiViewModel()
        {
            PaymentDetail = new List<AddEditPaymentDetailViewModel>();
            EditPaymentDetail = new List<AddEditPaymentDetailViewModel>();
            PaymentList = new List<BrowsePaymentHeader_Result>();
        }
    }
}