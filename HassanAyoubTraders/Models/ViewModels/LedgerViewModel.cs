using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class LedgerViewModel
    {
        public IList<GetCompanyPositionReport_Result> CompanyPositionReportList { get; set; }
        public IList<GetProfitAndLossReport_New_Result> ProfitAndLostReportList { get; set; }
        public IList<LedgerModel> LedgerDetail { get; set; }
        public IList<DetailLedgerStatementModel> DetailLedgerStatement { get; set; }
        public IEnumerable<SelectListItem> AccountList { get; set; }
        public string ReportName { get; set; }
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal TotalDr { get; set; }
        public decimal TotalCr { get; set; }
        public decimal ClosingBalance { get; set; }

        public IList<BrowseItemActivityReport_Result> ItemActivityList { get; set; }
        public IList<BrowseAccountPayableReport_Result> AccountPayableList { get; set; }
        public IList<BrowseAccountRecievableReport_Result> AccountRecievableList { get; set; }

        public IEnumerable<SelectListItem> ItemList { get; set; }
        public string SubItemID { get; set; }

        public LedgerViewModel()
        {
            LedgerDetail = new List<LedgerModel>();
            DetailLedgerStatement = new List<DetailLedgerStatementModel>();
            AccountList = new List<SelectListItem>();
            ProfitAndLostReportList = new List<GetProfitAndLossReport_New_Result>();
            ItemActivityList = new List<BrowseItemActivityReport_Result>();
            AccountPayableList = new List<BrowseAccountPayableReport_Result>();
            AccountRecievableList = new List<BrowseAccountRecievableReport_Result>();
            CompanyPositionReportList = new List<GetCompanyPositionReport_Result>();
        }
    }

    public class LedgerModel
    {
        public int TransID { get; set; }
        public Nullable<int> AccountCode { get; set; }
        public string AccountTitle { get; set; }
        public string TransDate { get; set; }
        public Nullable<int> RecordID { get; set; }
        public string RecordType { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> Dr { get; set; }
        public Nullable<decimal> Cr { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public string ChartOfAccountName { get; set; }
        public string Particular { get; set; }
        public string ValueType { get; set; }
    }

    public class DetailLedgerStatementModel
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string TransDate { get; set; }
        public string Descriptions { get; set; }
        public Nullable<int> InQty { get; set; }
        public Nullable<int> OutQty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Dr { get; set; }
        public Nullable<decimal> Cr { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> RecordId { get; set; }
        public string RoleID { get; set; }
        public string ValueType { get; set; }
    }

}