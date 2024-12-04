using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class PrintPurchaseReturnReceiptViewModel
    {
        public string InvoiceNumber { get; set; }
        public string PODate { get; set; }
        public string VendorName { get; set; }
        public string VendorContactNo { get; set; }
        public string VendorCNIC { get; set; }
        public string VendorAddress { get; set; }
        public string VendorSTR { get; set; }
        public string VendorNTN { get; set; }
        public string VendorCityArea { get; set; }
        public decimal Expense { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesTax { get; set; }
        public decimal SubTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public decimal DueAmount { get; set; }
        public string EntryType { get; set; }
        public bool IsDispature { get; set; }
        public string Booker { get; set; }
        public string SalesOfficer { get; set; }


        public IList<PrintPOReturnDetail> ItemDetail { get; set; }
        public CompanyInformation CompanyInfo { get; set; }

        public PrintPurchaseReturnReceiptViewModel()
        {
            ItemDetail = new List<PrintPOReturnDetail>();
            CompanyInfo = new CompanyInformation();
        }
    }

    public class PrintPOReturnDetail
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TO { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Disc { get; set; }
        public decimal SalesTax { get; set; }
        public decimal Total { get; set; }
    } 
}