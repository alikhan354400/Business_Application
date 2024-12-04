using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class PurchaseOrderViewModel
    {
        public IEnumerable<SelectListItem> VendorList { get; set; }
        public int VendorID { get; set; }
        public IEnumerable<SelectListItem> ItemList { get; set; }
        public int ItemID { get; set; }
        public IEnumerable<SelectListItem> ExtraItemList { get; set; }
        public int ExtraItemID { get; set; }
        public IEnumerable<SelectListItem> EntryTypeList { get; set; }
        public int EntryTypeID { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> BookerList { get; set; }
        public int BookerID { get; set; }
        public IEnumerable<SelectListItem> SupplierList { get; set; }
        public int SupplierID { get; set; }

        public IList<PurchaseOrderLine> PurchaseOrderDetail { get; set; }
        public List<GetPurhcaseOrderItemById_Result> EditPurchaseOrderDetail { get; set; }
        public IList<BrowsePurchaseOrders_Result> PurchaseOrderList { get; set; }

        public int PurchaseOrderID { get; set; }
        public int PONumber { get; set; }
        public string RecordType { get; set; }
        public string PoDate { get; set; }
        public string InvoiceNo { get; set; }
        public string Notes { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Expense { get; set; }
        public decimal? Discount { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool IsTaxInvoice { get; set; }
        public string RoleID { get; set; }
         
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public bool IsUpdate { get; set; }
        public List<string> DeleteItems { get; set; }

        public PurchaseOrderViewModel()
        {
            PurchaseOrderDetail = new List<PurchaseOrderLine>();
            EditPurchaseOrderDetail = new List<GetPurhcaseOrderItemById_Result>();
            PurchaseOrderList = new List<BrowsePurchaseOrders_Result>();
        }
    }
}