using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class AddEditAccountViewModel
    {
        public int? CompanyId { get; set; }
        public int? AccountID { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTitle { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<decimal> OpeningBalance { get; set; }
        public string OpeningBalanceDate { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int CityAreaID { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public string AccountType { get; set; }
        public string RoleID { get; set; }
        public string HiringDate { get; set; }
        public string STR { get; set; }
        public string NTN { get; set; }
        public bool IsCashAccount { get; set; }
        public bool IsCashAccountExist { get; set; }
        public Nullable<decimal> Salary { get; set; }
        public int EmployeeID { get; set; }
        public string Designation { get; set; }

        public IEnumerable<SelectListItem> CompanyList{ get; set; }
        public IEnumerable<SelectListItem> AccountTypeList { get; set; }
        public IEnumerable<SelectListItem> CityList { get; set; }
        public IEnumerable<SelectListItem> CityAreaList { get; set; }
        public IEnumerable<SelectListItem> EmployeesList { get; set; }
        public IEnumerable<SelectListItem> DesignationList { get; set; }

        public AddEditAccountViewModel()
        {
            CompanyList = new List<SelectListItem>();
            AccountTypeList = new List<SelectListItem>();
            CityList = new List<SelectListItem>();
            CityAreaList = new List<SelectListItem>();
            EmployeesList = new List<SelectListItem>();
            DesignationList = new List<SelectListItem>();
        }
    }
}