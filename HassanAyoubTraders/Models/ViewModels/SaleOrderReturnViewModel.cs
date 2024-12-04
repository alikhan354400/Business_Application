using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class SaleOrderReturnViewModel
    {
        public IEnumerable<SelectListItem> CustomerList { get; set; }
        public int CustomerID { get; set; }
        public IEnumerable<SelectListItem> ItemList { get; set; }
        public int ItemID { get; set; }
        public IEnumerable<SelectListItem> ExtraItemList { get; set; }
        public int ExtraItemID { get; set; }
        public IEnumerable<SelectListItem> EntryTypeList { get; set; }
        public int EntryTypeID { get; set; }
        public IEnumerable<SelectListItem> BookerList { get; set; }
        public int BookerID { get; set; }
        public IEnumerable<SelectListItem> SupplierList { get; set; }
        public int SupplierID { get; set; }


        public IList<GetSalesOrderReturnItemById_Result> SaleOrderReturnDetail { get; set; }
        public List<GetSalesOrderReturnItemById_Result> EditSaleOrderReturnDetail { get; set; }
        public IList<BrowseSaleOrdersReturn_Result> SaleOrderReturnList { get; set; }


        public int SaleOrderReturnID { get; set; }
        public int SONumber { get; set; }
        public string RecordType { get; set; }
        public string SODate { get; set; }
        public string InvoiceNo { get; set; }
        public string Notes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Expense { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesTax { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal NetTotalAmount { get; set; }
        public string RoleID { get; set; }
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public bool IsUpdate { get; set; }
        public string UserRole { get; set; }
        public string Type { set; get; }
        public string Keyword { set; get; }

        public List<string> DeleteItems { get; set; }

        public SaleOrderReturnViewModel()
        {
            SaleOrderReturnDetail = new List<GetSalesOrderReturnItemById_Result>();
            SaleOrderReturnList = new List<BrowseSaleOrdersReturn_Result>();
            EditSaleOrderReturnDetail = new List<GetSalesOrderReturnItemById_Result>();
        }
    }

    public class SaleOrderReturnLineModel
    {
        public int SaleOrderReturnLineID { get; set; }
        public Nullable<int> SaleOrderReturnID { get; set; }
        public Nullable<int> SubItemID { get; set; }
        public string SubItemName { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> SalesTaxRate { get; set; }
        public Nullable<decimal> NetTotalAmount { get; set; }
        public Nullable<int> InventoryID { get; set; }


    }

}
