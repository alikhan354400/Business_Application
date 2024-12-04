using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class AddEditBankAccountViewModel
    {
        public int? ID { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankBranchCity { get; set; }
        public string AccountTitle { get; set; }
        public string AccountNumber { get; set; }
        public Nullable<decimal> OpeningBalance { get; set; }
        public string OpeningBalanceDate { get; set; }
        public string ContactNo { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public AddEditBankAccountViewModel()
        {
        }
    }
}