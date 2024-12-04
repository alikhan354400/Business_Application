using HassanAyoubTraders.Models.EF;
using System.Collections.Generic;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public IList<BrowseAllEmployees_Result> EmpList { get; set; }

        public EmployeeViewModel()
        {
            EmpList = new List<BrowseAllEmployees_Result>();
        }
    }
}