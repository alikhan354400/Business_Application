using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class CustomerWiseBalanceViewModel
    {
        public IEnumerable<SelectListItem> CityList { get; set; }
        public string CityID { get; set; }
        public IEnumerable<SelectListItem> CityAreaList { get; set; }
        public string CityAreaID { get; set; }
        public IEnumerable<SelectListItem> SalesOfficerList { get; set; }
        public string SalesOfficerID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReportName { get; set; }

        public IList<CustomerWiseBalanceModel> CustomerWiseBalanceDetail { get; set; }

        public CustomerWiseBalanceViewModel()
        {
            CityList = new List<SelectListItem>();
            CityAreaList = new List<SelectListItem>();
            SalesOfficerList = new List<SelectListItem>();

            CustomerWiseBalanceDetail = new List<CustomerWiseBalanceModel>();
        }
    }


    public class CustomerWiseBalanceModel
    {
        public int AccountID { get; set; }
        public string CustomerName { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int CityAreaID { get; set; }
        public string CityAreaName { get; set; }
        public string SalesOfficer { get; set; }
        public Nullable<decimal> OpeningBalance { get; set; }
        public Nullable<decimal> TotalSales { get; set; }
        public Nullable<decimal> TotalSalesReturn { get; set; }
        public Nullable<decimal> TotalAmountReceived { get; set; }
        public Nullable<decimal> Balance { get; set; }
    }
}