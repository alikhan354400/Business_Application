using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class SalesReportViewModel
    {
        public IEnumerable<SelectListItem> VendorList { get; set; }
        public string VendorID { get; set; }
        public IEnumerable<SelectListItem> ItemList { get; set; }
        public string ItemID { get; set; }
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

        public IList<SalesReportModel> SalesReportDetail { get; set; }

        public SalesReportViewModel()
        {
            CityList = new List<SelectListItem>();
            CityAreaList = new List<SelectListItem>();
            SalesOfficerList = new List<SelectListItem>();
            SalesReportDetail = new List<SalesReportModel>();
        }
    }


    public class SalesReportModel
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