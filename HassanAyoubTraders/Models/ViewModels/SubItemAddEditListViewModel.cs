using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class SubItemAddEditListViewModel
    {
     
        public int? CompanyId { get; set; }
        public int SubItemID { get; set; }
        public string SubItemCode { get; set; }
        public string SubItemName { get; set; }
        public int? ItemID { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? RPCharges { get; set; }
        public int? OpeningInventoryId { get; set; }
        public int? OpeningInventoryQty { get; set; }
        public decimal? PicesPerCorton{ get; set; }

        public IList<BrowseAllSubItems_Result> SubItemList { get; set; }
        public IList<SubItem> AddSubItemList { get; set; }

        public IEnumerable<SelectListItem> CompanyList { get; set; }

        public IEnumerable<SelectListItem> ItemsList { get; set; }

        public string UserRole { get; set; }

        public SubItemAddEditListViewModel()
        {
            CompanyList = new List<SelectListItem>();
            SubItemList = new List<BrowseAllSubItems_Result>();
            AddSubItemList = new List<SubItem>();
            ItemsList = new SelectList(new List<string>());
        }
    }
}