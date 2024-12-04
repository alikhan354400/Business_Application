using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models.EF;
using HassanAyoubTraders.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class InventoryController : BaseController
    {
        // GET: Inventory
        public ActionResult InventoryOnHand()
        {
            var vm = new InventoryOnHandViewModel();
            string UserRole = GetCurrentUserRole();

            vm.ItemList = _repository.BrowseAllItmes().Select(x => new SelectListItem { Text = x.ProductName, Value = x.ItemID.ToString() });
            vm.SubItemList = _repository.GetProductsForDdlWithoutDetail().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.SubItemName), Value = x.SubItemID.ToString() }).ToList();
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult InventoryOnHandSearch(int ItemId, int SubItemId, string StartDate, string EndDate)
        {
            string userRole = GetCurrentUserRole();
            var vm = new InventoryOnHandViewModel();

            if (ItemId != 0 && SubItemId != 0 && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.InventoryOnHandList = GetInventoryOnHand(userRole, ItemId, SubItemId, stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.ReportName = "Inventory On Hand";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.InventoryOnHandList = GetInventoryOnHand(userRole, -1, -1, DateTime.Now.AddDays(-30), DateTime.Now).ToList();
                vm.ReportName = "Inventory On Hand";
            }

            return PartialView("_InventoryOnHandList", vm);
        }

        [HttpPost]
        public IEnumerable<GetInventoryItemOnHand_New_Result> GetInventoryOnHand(string userRole, int ItemId, int SubItemId, DateTime startDate, DateTime endDate)
        {
            try
            {
                IEnumerable<GetInventoryItemOnHand_New_Result> oInventoryOnHandList = new List<GetInventoryItemOnHand_New_Result>();
                dbConnector objConn = new dbConnector();
                SqlConnection Conn = objConn.GetConnection;
                Conn.Open();
                try
                {
                    var dt = new DataTable();
                    IEnumerable<GetInventoryItemOnHand_New_Result> list = new List<GetInventoryItemOnHand_New_Result>();

                    if (Conn.State != System.Data.ConnectionState.Open)
                        Conn.Open();

                    SqlCommand objCommand = new SqlCommand("GetInventoryItemOnHand_New", Conn);
                    objCommand.CommandType = CommandType.StoredProcedure;
                    objCommand.Parameters.AddWithValue("@ItemId", ItemId);
                    objCommand.Parameters.AddWithValue("@SubItemId", SubItemId);
                    objCommand.Parameters.AddWithValue("@StartDate", startDate);
                    objCommand.Parameters.AddWithValue("@EndDate", endDate);
                    SqlDataAdapter da = new SqlDataAdapter(objCommand);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        list = dt.AsEnumerable().Select(x => new GetInventoryItemOnHand_New_Result()
                        {
                            SubItemID = Convert.ToInt32(x["SubItemID"] == DBNull.Value ? "0" : x["SubItemID"]),
                            ItemID = Convert.ToInt32(x["ItemID"] == DBNull.Value ? "0" : x["ItemID"]),
                            SubItemName = x["SubItemName"] == DBNull.Value ? "" : x["SubItemName"].ToString(),
                            PurchasePrice = Convert.ToDecimal(x["PurchasePrice"] == DBNull.Value ? "0" : x["PurchasePrice"]),
                            SalePrice = Convert.ToDecimal(x["SalePrice"] == DBNull.Value ? "0" : x["SalePrice"]),
                            AvailableStock = Convert.ToInt32(x["AvailableStock"] == DBNull.Value ? "0" : x["AvailableStock"]),
                            PurchaseQuantity = Convert.ToInt32(x["PurchaseQuantity"] == DBNull.Value ? "0" : x["PurchaseQuantity"]),
                            SaleQuantity = Convert.ToInt32(x["SaleQuantity"] == DBNull.Value ? "0" : x["SaleQuantity"]),
                            TotalPOReturnQty = Convert.ToInt32(x["TotalPOReturnQty"] == DBNull.Value ? "0" : x["TotalPOReturnQty"]),
                            TotalSOReturnQty = Convert.ToInt32(x["AvailableStock"] == DBNull.Value ? "0" : x["TotalSOReturnQty"]),
                            RoleID = x["RoleID"] == DBNull.Value ? "" : x["RoleID"].ToString(),
                            Descriptions = x["Descriptions"] == DBNull.Value ? "" : x["Descriptions"].ToString(),
                            DescriptionsForDdl = x["DescriptionsForDdl"] == DBNull.Value ? "" : x["DescriptionsForDdl"].ToString(),
                            ProductName = x["ProductName"] == DBNull.Value ? "" : x["ProductName"].ToString(),
                            OpeningInventoryQty = Convert.ToInt32(x["OpeningInventoryQty"] == DBNull.Value ? "0" : x["OpeningInventoryQty"]),
                        });
                    }
                    return list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (Conn != null)
                    {
                        if (Conn.State == ConnectionState.Open)
                        {
                            Conn.Close();
                            Conn.Dispose();
                        }
                    }
                }

                //if (ItemId > 0 && SubItemId == 0)
                //    oInventoryOnHandList = _repository.GetInventoryItemOnHand().Where(x => x.RoleID == userRole && x.ItemID == ItemId).ToList();
                //if (ItemId > 0 && SubItemId > 0)
                //    oInventoryOnHandList = _repository.GetInventoryItemOnHand().Where(x => x.RoleID == userRole && x.ItemID == ItemId && x.SubItemID == SubItemId).ToList();
                //if (ItemId == 0 && SubItemId > 0)
                //    oInventoryOnHandList = _repository.GetInventoryItemOnHand().Where(x => x.RoleID == userRole && x.SubItemID == SubItemId).ToList();
                //if (ItemId == 0 && SubItemId == 0)
                //    oInventoryOnHandList = _repository.GetInventoryItemOnHand().Where(x => x.RoleID == userRole).ToList();

                //return oInventoryOnHandList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

    }
}