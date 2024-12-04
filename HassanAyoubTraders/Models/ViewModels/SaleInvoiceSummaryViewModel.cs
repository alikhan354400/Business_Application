using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class SaleInvoiceSummaryViewModel
    {
        public IEnumerable<SelectListItem> SalesOfficerList { get; set; }
        public string SaleOfficerID { get; set; }
        public IEnumerable<SelectListItem> CustomerList { get; set; }
        public string CustomerID{ get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReportName { get; set; }

        public IList<SaleInvoiceSummaryModel> SaleInvocieSummaryDetail { get; set; }
        public SaleInvoiceSummaryViewModel()
        {
            SalesOfficerList = new List<SelectListItem>();
            CustomerList = new List<SelectListItem>();

            SaleInvocieSummaryDetail = new List<SaleInvoiceSummaryModel>();
        }
    }


    public class SaleInvoiceSummaryModel
    {
        public int SaleOrderID { get; set; }
        public int SONumber { get; set; }
        public int InvoiceNo { get; set; }
        public string SaleOrderDate { get; set; }
        public string CustomerName { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string SalesOfficer { get; set; }
        public int AccountID { get; set; }
        public int SupplierID { get; set; }
    }
}