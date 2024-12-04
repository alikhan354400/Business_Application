using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class ProfitAndLostSummaryViewModel
    {
        public IEnumerable<SelectListItem> VendorList { get; set; }
        public string VendorID { get; set; }
        public IEnumerable<SelectListItem> ItemList { get; set; }
        public string ItemID { get; set; }
        public IEnumerable<SelectListItem> SalesOfficerList { get; set; }
        public string SalesOfficerID { get; set; }
        public IEnumerable<SelectListItem> BookerList { get; set; }
        public string BookerID { get; set; }
        public IEnumerable<SelectListItem> CityList { get; set; }
        public string CityID { get; set; }
        public IEnumerable<SelectListItem> CityAreaList { get; set; }
        public string CityAreaID { get; set; }

        public string SONumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReportName { get; set; }
        public IList<ProfitAndLostSummaryModel> ProfitAndLostSummaryDetail { get; set; }

        public ProfitAndLostSummaryViewModel()
        {
            VendorList = new List<SelectListItem>();
            ItemList = new List<SelectListItem>();
            SalesOfficerList = new List<SelectListItem>();
            BookerList = new List<SelectListItem>();
            CityList = new List<SelectListItem>();
            CityAreaList = new List<SelectListItem>();

            ProfitAndLostSummaryDetail = new List<ProfitAndLostSummaryModel>();
        }
    }

    public class ProfitAndLostSummaryModel
    {
        public string RecordType { get; set; }
        public int SONumber { get; set; }
        public string SODate { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<decimal> SalesPrice { get; set; }
        public Nullable<decimal> TotalPurchase { get; set; }
        public Nullable<decimal> TotalSale { get; set; }
        public Nullable<decimal> TotalProfit { get; set; }
    }
}