using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class AccountViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string AccountID { get; set; }
        public string Type { set; get; }
        public string Keyword { set; get; }
        public string AccountName { set; get; }
        public string Designation { set; get; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> CityArea { get; set; }
        public IEnumerable<SelectListItem> DesignationList { get; set; }
        public IList<BrowseAccount_Result> AccountList { get; set; }
        public AccountViewModel()
        {
            AccountList = new List<BrowseAccount_Result>();
            Accounts = new List<SelectListItem>();
            CityArea = new List<SelectListItem>();
            DesignationList = new List<SelectListItem>();
        }
    }
}