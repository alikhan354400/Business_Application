using HassanAyoubTraders.Models.EF;
using HassanAyoubTraders.Models.EF;
using System.Collections.Generic;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class CompanyViewModel
    {
        public IList<BrowseAllCompanies_Result> CompanyList { get; set; }

        public CompanyViewModel()
        {
            CompanyList = new List<BrowseAllCompanies_Result>();
        }
    }
}