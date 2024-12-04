using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class PaymentListViewModel
    {
        public string EndDate { get; set; }
        public string StartDate { get; set; }

        public IEnumerable<SelectListItem> AccountList { get; set; }
        public int? AccountID { get; set; }
    }
}