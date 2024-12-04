using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class InventoryOnHandViewModel
    {
        public int? ItemId { get; set; }
        public int? SubItemId { get; set; }
        public string ReportName{ get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }


        public IList<GetInventoryItemOnHand_New_Result> InventoryOnHandList { get; set; }
        public IEnumerable<SelectListItem> ItemList { get; set; }
        public IEnumerable<SelectListItem> SubItemList { get; set; }

        public InventoryOnHandViewModel()
        {
            InventoryOnHandList = new List<GetInventoryItemOnHand_New_Result>();
        }
    }

    public class InventoryOnHandModel
    {
        public int InventoryID { get; set; }
        public int SubItemID { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public int AvailableStock { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TransferedTimeStock { get; set; }
        public int SaleQuantity { get; set; }
        public string Descriptions { get; set; }
        public string SubItemName { get; set; }
        public string ProductName { get; set; }
        public int ItemID { get; set; }
    }
}