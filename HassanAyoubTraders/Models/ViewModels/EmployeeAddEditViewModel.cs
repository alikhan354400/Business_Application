using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class EmployeeAddEditViewModel
    {
        public int EmployeeID { get; set; }
        public string EmpName { get; set; }
        public string CNICNo { get; set; }
        public string MobileNo { get; set; }
        public string HiringDate { get; set; }
        public decimal? MonthlySalary { get; set; }
        public string Address { get; set; }

        public IEnumerable<SelectListItem> BranchList { get; set; }
        public int? BranchID { get; set; }

        public IEnumerable<SelectListItem> EmployeeTypeList { get; set; }
        public int? EmployeeTypeID { get; set; }


        public EmployeeAddEditViewModel()
        {

        }
    }
}