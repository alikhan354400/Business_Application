using HassanAyoubTraders.Models.EF;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class ItemAddEditListViewModel
    {
        public int? CompanyId { get; set; }
        public int? VendorId { get; set; }
        public int ItemID { get; set; }
        public int? UnitTypeID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Description { get; set; }

        public IList<BrowseAllItmes_Result> ItemsList { get; set; }

        public IEnumerable<SelectListItem> VendorList{ get; set; }
        public IEnumerable<SelectListItem> CompanyList{ get; set; }
        public IEnumerable<SelectListItem> UnitTypeList { get; set; }

        public ItemAddEditListViewModel()
        {
            VendorList = new List<SelectListItem>();
            CompanyList = new List<SelectListItem>();
            ItemsList = new List<BrowseAllItmes_Result>();
        }
    }
}