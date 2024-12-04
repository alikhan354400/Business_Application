using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class PurchaseOrderReturnViewModel
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

        public IList<PurchaseOrderReturnLine> PurchaseOrderReturnDetail { get; set; }
        public List<GetPurhcaseOrderReturnItemById_Result> EditPurchaseOrderReturnDetail { get; set; }
        public IList<BrowsePurchaseOrderReturn_Result> PurchaseOrderReturnList { get; set; }

        public int PurchaseOrderReturnID { get; set; }
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
        public string RoleID { get; set; }

        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public bool IsUpdate { get; set; }
        public List<string> DeleteItems { get; set; }

        public PurchaseOrderReturnViewModel()
        {
            PurchaseOrderReturnDetail = new List<PurchaseOrderReturnLine>();
            EditPurchaseOrderReturnDetail = new List<GetPurhcaseOrderReturnItemById_Result>();
            PurchaseOrderReturnList = new List<BrowsePurchaseOrderReturn_Result>();
        }

        //    public IEnumerable<SelectListItem> AccountList { get; set; }
        //    public int AccountID { get; set; }
        //    public IEnumerable<SelectListItem> ItemList { get; set; }
        //    public int ItemID { get; set; }
        //    public IEnumerable<SelectListItem> ExtraItemList { get; set; }
        //    public int ExtraItemID { get; set; }
        //    public IEnumerable<SelectListItem> EntryTypeList { get; set; }
        //    public int EntryTypeID { get; set; }

        //    public IList<PurchaseOrderReturnLine> PurchaseOrderReturnDetail { get; set; }
        //    public List<BrowseEditPOReturnLine_Result> EditPurchaseOrderReturnDetail { get; set; }
        //    public IList<BrowsePurchaseOrderReturn_Result> PurchaseOrderReturnList { get; set; }

        //    public int PurchaseOrderReturnID { get; set; }
        //    public string PODate { get; set; }
        //    public string Notes { get; set; }
        //    public decimal? Discount { get; set; }
        //    public string InvoiceNo { get; set; }
        //    public decimal? TotalAmount { get; set; }
        //    public string RoleID { get; set; }
        //    public decimal? TotalExpense { get; set; }
        //    public decimal? NetTotalAmount { get; set; }
        //    public decimal? PaidAmount { get; set; }
        //    public string EndDate { get; set; }
        //    public string StartDate { get; set; }
        //    public bool IsUpdate { get; set; }
        //    public List<string> DeleteItems { get; set; }
        //    public string UserRole { get; set; }
        //    public string Type { set; get; }
        //    public string Keyword { set; get; }

        //    public PurchaseOrderReturnViewModel()
        //    {
        //        PurchaseOrderReturnDetail = new List<PurchaseOrderReturnLine>();
        //        PurchaseOrderReturnList = new List<BrowsePurchaseOrderReturn_Result>();
        //        EditPurchaseOrderReturnDetail = new List<BrowseEditPOReturnLine_Result>();
        //    }

    }
}