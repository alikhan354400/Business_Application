using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class ExecutionReportViewModel
    {
        public IEnumerable<SelectListItem> VendorList { get; set; }
        public string VendorID { get; set; }
        public IEnumerable<SelectListItem> CustomerList { get; set; }
        public string CustomerID { get; set; }
        public IEnumerable<SelectListItem> SalesOfficerList { get; set; }
        public string SalesOfficerID { get; set; }
        public IEnumerable<SelectListItem> CityList { get; set; }
        public string CityID { get; set; }
        public IEnumerable<SelectListItem> CityAreaList { get; set; }
        public string CityAreaID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string IsSale { get; set; }
        public string ReportName { get; set; }
        public Nullable<decimal> TotalRecovery { get; set; }

        public IList<ExecutionReportModel> ExecutionReportDetail { get; set; }

        public ExecutionReportViewModel()
        {
            CityList = new List<SelectListItem>();
            CityAreaList = new List<SelectListItem>();
            SalesOfficerList = new List<SelectListItem>();

            ExecutionReportDetail = new List<ExecutionReportModel>();
        }
    }


    public class ExecutionReportModel
    {
        public int SubItemID { get; set; }
        public string SubItemName { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public Nullable<decimal> TotalSQ { get; set; }
        public Nullable<decimal> TotalSP { get; set; }
        public Nullable<decimal> TotalRQ { get; set; }
        public Nullable<decimal> TotalRP { get; set; }
        public Nullable<decimal> RemainingQty { get; set; }
        public Nullable<decimal> RemainingPrice { get; set; }
    }
}