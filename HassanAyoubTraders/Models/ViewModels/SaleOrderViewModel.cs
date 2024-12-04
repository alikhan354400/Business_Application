using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class SaleOrderViewModel
    {
        public IEnumerable<SelectListItem> CustomerList { get; set; }
        public int CustomerID { get; set; }
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
        public string BookerName { get; set; }

        public IEnumerable<SelectListItem> SupplierList { get; set; }
        public int SupplierID { get; set; }


        public IList<SaleOrderLine> SaleOrderDetail { get; set; }
        public List<GetSalesOrderItemById_Result> EditSaleOrderDetail { get; set; }
        public IList<BrowseSaleOrders_Result> SaleOrderList { get; set; }

        public int SaleOrderID { get; set; }
        public int SONumber { get; set; }
        public string RecordType { get; set; }
        public string SODate { get; set; }
        public string InvoiceNo { get; set; }
        public string Notes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Expense { get; set; }
        public decimal Discount { get; set; }
        public decimal Discount2 { get; set; }
        public decimal SalesTax { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal NetTotalAmount { get; set; }
        public bool IsTaxInvoice { get; set; }
        public string RoleID { get; set; }
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public bool IsUpdate { get; set; }
        public List<string> DeleteItems { get; set; }
        public string UserRole { get; set; }
        public string Type { set; get; }
        public string Keyword { set; get; }

        public SaleOrderViewModel()
        {
            SaleOrderDetail = new List<SaleOrderLine>();
            SaleOrderList = new List<BrowseSaleOrders_Result>();
            EditSaleOrderDetail = new List<GetSalesOrderItemById_Result>();
        }
    }
}