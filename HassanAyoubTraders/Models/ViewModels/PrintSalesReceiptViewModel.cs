using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class PrintSalesReceiptViewModel
    {
        public string InvoiceNumber { get; set; }
        public string SODate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContactNo { get; set; }
        public string CustomerCNIC { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerSTRN{ get; set; }
        public string CustomerNTN{ get; set; }
        public string CustomerCityArea { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Expense { get; set; }
        public decimal Discount { get; set; }
        public decimal Discount2 { get; set; }
        public decimal SalesTax { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public string EntryType { get; set; }
        public decimal DueAmount { get; set; }
        public bool IsDispature { get; set; }
        public string SalesOfficer { get; set; }
        public string Booker { get; set; }
        public string InvoiceNo{ get; set; }

        public IList<PrintSODetail> ItemDetail { get; set; }
        public CompanyInformation CompanyInfo { get; set; }

        public PrintSalesReceiptViewModel()
        {
            ItemDetail = new List<PrintSODetail>();
            CompanyInfo = new CompanyInformation();
        }
    }

    public class PrintSODetail
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TO { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Disc { get; set; }
        public decimal SalesTax { get; set; }
        public decimal Total { get; set; }
    }

}