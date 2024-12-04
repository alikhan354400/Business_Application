using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class BankAccountViewModel
    {
        public int BankTransferID { get; set; }

        public IList<BrowseBankAccount_Result> AccountList { get; set; }
        public IList<BrowseBankTransfers_Result> BankTransferList { get; set; }

        public IEnumerable<SelectListItem> BankList { get; set; }
        public int FromBankID { get; set; }
        public int ToBankID { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> OtherCharges { get; set; }
        public string Description { get; set; }
        public string TransferDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReportName { get; set; }

        public BankAccountViewModel()
        {
            AccountList = new List<BrowseBankAccount_Result>();
            BankTransferList = new List<BrowseBankTransfers_Result>();
        }
    }
}