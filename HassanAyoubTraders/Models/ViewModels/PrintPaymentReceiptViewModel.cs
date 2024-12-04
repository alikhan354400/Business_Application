using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class PrintPaymentReceiptViewModel
    {
        public string ReceiptNo { get; set; }
        public string PaymentDate { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ContactNo { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public string STRN { get; set; }
        public string NTN { get; set; }
        public string CityAreaName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Particular { get; set; }
        public string VoucharType { get; set; }
        public string PaymentDetail { get; set; }

        public IList<vw_Payments> ItemDetail { get; set; }
        public CompanyInformation CompanyInfo { get; set; }

        public PrintPaymentReceiptViewModel()
        {
            ItemDetail = new List<vw_Payments>();
            CompanyInfo = new  CompanyInformation();
        }
    }
}