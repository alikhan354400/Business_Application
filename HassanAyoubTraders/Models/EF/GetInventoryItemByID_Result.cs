//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HassanAyoubTraders.Models.EF
{
    using System;
    
    public partial class GetInventoryItemByID_Result
    {
        public int InventoryID { get; set; }
        public int SubItemID { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<int> AvailableStock { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int TransferedTimeStock { get; set; }
        public int SaleQuantity { get; set; }
        public int TotalPOReturnQty { get; set; }
        public int TotalSOReturnQty { get; set; }
        public string RoleID { get; set; }
        public string Descriptions { get; set; }
        public string DescriptionsForDdl { get; set; }
        public string SubItemName { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> ItemID { get; set; }
    }
}