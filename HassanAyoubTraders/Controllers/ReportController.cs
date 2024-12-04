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
    public class ReportController : BaseController
    {

        public ActionResult SaleInvoiceSummaryReport()
        {
            var vm = new SaleInvoiceSummaryViewModel();
            string UserRole = GetCurrentUserRole();

            vm.CustomerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Customer").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.SalesOfficerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetSaleInvoiceSummarySearch(string SaleOfficerID, string CustomerID, string StartDate, string EndDate)
        {
            var vm = new SaleInvoiceSummaryViewModel();

            if (!string.IsNullOrEmpty(SaleOfficerID) && !string.IsNullOrEmpty(CustomerID) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.SaleInvocieSummaryDetail = GetSaleInvoiceSummary(SaleOfficerID, CustomerID, stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.ReportName = "Account Ledger";
                vm.SaleOfficerID = SaleOfficerID;
                vm.CustomerID = CustomerID;
                vm.ReportName = "Sale Invoice Summary";
                //if (vm.LedgerDetail.Count > 1)
                //    vm.AccountName = vm.LedgerDetail[1].AccountTitle;
                //else
                //    vm.AccountName = "";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

                vm.SaleInvocieSummaryDetail = GetSaleInvoiceSummary("-1", "-1", DateTime.Now.AddDays(-30), DateTime.Now).ToList();
                vm.ReportName = "Sale Invoice Summary";
                //vm.ReportName = "";
                //vm.AccountName = "";
            }

            return PartialView("_SaleInvoiceSummary", vm);
        }

        [HttpPost]
        public IEnumerable<SaleInvoiceSummaryModel> GetSaleInvoiceSummary(string SaleOfficerID, string CustomerID, DateTime startDate, DateTime endDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<SaleInvoiceSummaryModel> list = new List<SaleInvoiceSummaryModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                SqlCommand objCommand = new SqlCommand("BrowseSalesInvoiceSummary", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@SalesOfficerID", SaleOfficerID);
                objCommand.Parameters.AddWithValue("@CustomerID", CustomerID);
                objCommand.Parameters.AddWithValue("@StartDate", startDate);
                objCommand.Parameters.AddWithValue("@EndDate", endDate);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new SaleInvoiceSummaryModel()
                    {
                        SaleOrderID = Convert.ToInt32(x["SaleOrderID"] == DBNull.Value ? "0" : x["SaleOrderID"]),
                        SONumber = Convert.ToInt32(x["SONumber"] == DBNull.Value ? "0" : x["SONumber"]),
                        InvoiceNo = Convert.ToInt32(x["InvoiceNo"] == DBNull.Value ? "0" : x["InvoiceNo"]),
                        SaleOrderDate = (x["SaleOrderDate"] == DBNull.Value ? "" : Convert.ToDateTime(x["SaleOrderDate"]).ToString("dd/MM/yyyy")),
                        CustomerName = x["CustomerName"] == DBNull.Value ? "" : x["CustomerName"].ToString(),
                        PaidAmount = Convert.ToDecimal(x["PaidAmount"] == DBNull.Value ? "0" : x["PaidAmount"]),
                        TotalAmount = Convert.ToDecimal(x["TotalAmount"] == DBNull.Value ? "0" : x["TotalAmount"]),
                        SalesOfficer = x["SalesOfficer"] == DBNull.Value ? "" : x["SalesOfficer"].ToString(),
                        AccountID = Convert.ToInt32(x["AccountID"] == DBNull.Value ? "0" : x["AccountID"]),
                        SupplierID = Convert.ToInt32(x["SupplierID"] == DBNull.Value ? "0" : x["SupplierID"]),
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
        }



        public ActionResult CustomerLedger()
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Customer").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        public ActionResult VendorLedger()
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Supplier").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        public ActionResult AccountLedger()
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole).Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetAccountLedgerSearch(string AccountId, string StartDate, string EndDate)
        {
            var vm = new LedgerViewModel();
            string userRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(AccountId) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.LedgerDetail = GetAccountLedger(userRole, int.Parse(AccountId.ToString()), stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.ReportName = "Account Ledger";
                vm.AccountID = AccountId;
                if (vm.LedgerDetail.Count > 1)
                    vm.AccountName = vm.LedgerDetail[1].AccountTitle;
                else
                    vm.AccountName = "";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

                vm.LedgerDetail = GetAccountLedger(userRole, -1, DateTime.Now.AddDays(-30), DateTime.Now).ToList();
                vm.ReportName = "";
                vm.AccountName = "";
            }

            return PartialView("_AccountLedgerDetail", vm);
        }

        [HttpPost]
        public IEnumerable<LedgerModel> GetAccountLedger(string userRole, int accountId, DateTime startDate, DateTime endDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<LedgerModel> list = new List<LedgerModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                SqlCommand objCommand = new SqlCommand("BrowseAccountLedger_New", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@StartDate", startDate);
                objCommand.Parameters.AddWithValue("@EndDate", endDate);
                objCommand.Parameters.AddWithValue("@AccountID", accountId);
                objCommand.Parameters.AddWithValue("@RoleID", userRole);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new LedgerModel()
                    {
                        TransID = Convert.ToInt32(x["TransID"] == DBNull.Value ? "0" : x["TransID"]),
                        AccountCode = Convert.ToInt32(x["AccountCode"] == DBNull.Value ? "0" : x["AccountCode"]),
                        TransDate = (x["TransDate"] == DBNull.Value ? "" : Convert.ToDateTime(x["TransDate"]).ToString("dd/MM/yyyy")),
                        RecordID = Convert.ToInt32(x["RecordID"] == DBNull.Value ? "0" : x["RecordID"]),
                        RecordType = x["RecordType"] == DBNull.Value ? "" : x["RecordType"].ToString(),
                        Remarks = x["Remarks"] == DBNull.Value ? "" : x["Remarks"].ToString(),
                        Dr = Convert.ToDecimal(x["DR"] == DBNull.Value ? "0" : x["DR"]),
                        Cr = Convert.ToDecimal(x["CR"] == DBNull.Value ? "0" : x["CR"]),
                        Balance = Convert.ToDecimal(x["Balance"] == DBNull.Value ? "0" : x["Balance"].ToString()),
                        AccountTitle = x["AccountName"] == DBNull.Value ? "" : x["AccountName"].ToString(),
                        ValueType = x["ValueType"] == DBNull.Value ? "" : x["ValueType"].ToString()
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
        }



        public ActionResult CustomerWiseBalanceReport()
        {
            var vm = new CustomerWiseBalanceViewModel();
            string UserRole = GetCurrentUserRole();

            vm.CityList = _repository.Cities.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            var cityID = 0;
            vm.CityAreaList = _repository.CityAreas.Where(x => x.CityId == cityID).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            vm.SalesOfficerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetCustomerWiseBalanceSearch(string CityID, string CityAreaID, string SaleOfficerID, string StartDate, string EndDate)
        {
            var vm = new CustomerWiseBalanceViewModel();

            if (!string.IsNullOrEmpty(CityID) && !string.IsNullOrEmpty(CityAreaID) && !string.IsNullOrEmpty(SaleOfficerID) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.CustomerWiseBalanceDetail = GetCustomerWiseBalance(CityID, CityAreaID, SaleOfficerID, stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.CityID = CityID;
                vm.CityAreaID = CityAreaID;
                vm.SalesOfficerID = SaleOfficerID;
                vm.ReportName = "Customer Wise Balance";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

                vm.CustomerWiseBalanceDetail = GetCustomerWiseBalance("-1", "-1", "-1", DateTime.Now.AddDays(-30), DateTime.Now).ToList();
                vm.ReportName = "Customer Wise Balance";
                //vm.ReportName = "";
                //vm.AccountName = "";
            }

            return PartialView("_CustomerWiseBalanceReport", vm);
        }

        [HttpPost]
        public IEnumerable<CustomerWiseBalanceModel> GetCustomerWiseBalance(string CityID, string CityAreaID, string SaleOfficerID, DateTime StartDate, DateTime EndDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<CustomerWiseBalanceModel> list = new List<CustomerWiseBalanceModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                SqlCommand objCommand = new SqlCommand("BrowseCustomerWiseBalanceReport", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@CityID", CityID);
                objCommand.Parameters.AddWithValue("@CityAreaID", CityAreaID);
                objCommand.Parameters.AddWithValue("@SalesOfficerID", SaleOfficerID);
                objCommand.Parameters.AddWithValue("@StartDate", StartDate);
                objCommand.Parameters.AddWithValue("@EndDate", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new CustomerWiseBalanceModel()
                    {
                        AccountID = Convert.ToInt32(x["AccountID"] == DBNull.Value ? "0" : x["AccountID"]),
                        CustomerName = x["CustomerName"].ToString(),
                        CityID = Convert.ToInt32(x["CityID"] == DBNull.Value ? "0" : x["CityID"]),
                        CityName = x["CityName"] == DBNull.Value ? "" : x["CityName"].ToString(),
                        CityAreaID = Convert.ToInt32(x["CityAreaID"] == DBNull.Value ? "0" : x["CityAreaID"]),
                        CityAreaName = x["CityAreaName"] == DBNull.Value ? "" : x["CityAreaName"].ToString(),
                        SalesOfficer = x["SalesOfficer"] == DBNull.Value ? "" : x["SalesOfficer"].ToString(),
                        OpeningBalance = Convert.ToDecimal(x["OpeningBalance"] == DBNull.Value ? "0" : x["OpeningBalance"]),
                        TotalSales = Convert.ToDecimal(x["TotalSales"] == DBNull.Value ? "0" : x["TotalSales"]),
                        TotalSalesReturn = Convert.ToDecimal(x["TotalSalesReturn"] == DBNull.Value ? "0" : x["TotalSalesReturn"]),
                        TotalAmountReceived = Convert.ToDecimal(x["TotalAmountReceived"] == DBNull.Value ? "0" : x["TotalAmountReceived"]),
                        Balance = Convert.ToDecimal(x["Balance"] == DBNull.Value ? "0" : x["Balance"])
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
        }



        public ActionResult ExecutionReport()
        {
            var vm = new ExecutionReportViewModel();
            string UserRole = GetCurrentUserRole();

            vm.VendorList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Supplier").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.CustomerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Customer").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.SalesOfficerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.CityList = _repository.Cities.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            var cityID = 0;
            vm.CityAreaList = _repository.CityAreas.Where(x => x.CityId == cityID).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetExecutionReportSearch(string IsSale, string VendorID, string CustomerID, string SaleOfficerID, string CityID, string CityAreaID, string StartDate, string EndDate)
        {
            var vm = new ExecutionReportViewModel();

            if (!string.IsNullOrEmpty(IsSale) && !string.IsNullOrEmpty(VendorID) && !string.IsNullOrEmpty(CustomerID) && !string.IsNullOrEmpty(SaleOfficerID) && !string.IsNullOrEmpty(CityID) && !string.IsNullOrEmpty(CityAreaID) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.ExecutionReportDetail = GetExecutionReport(IsSale, VendorID, CustomerID, SaleOfficerID, CityID, CityAreaID, stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.VendorID = VendorID;
                vm.CustomerID = CustomerID;
                vm.SalesOfficerID = SaleOfficerID;
                vm.CityID = CityID;
                vm.CityAreaID = CityAreaID;
                vm.ReportName = "Execution Report";
                vm.TotalRecovery = GetTotalRecovery(IsSale, VendorID, CustomerID, SaleOfficerID, CityID, CityAreaID, stdt, eddt);
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.ExecutionReportDetail = GetExecutionReport("True", "-1", "-1", "-1", "-1", "-1", DateTime.Now.AddDays(-30), DateTime.Now).ToList();
                vm.ReportName = "Execution Report";
                vm.TotalRecovery = GetTotalRecovery("True", "-1", "-1", "-1", "-1", "-1", DateTime.Now.AddDays(-30), DateTime.Now);
                //vm.ReportName = "";
                //vm.AccountName = "";
            }

            return PartialView("_ExecutionReport", vm);
        }

        [HttpPost]
        public IEnumerable<ExecutionReportModel> GetExecutionReport(string IsSale, string VendorID, string CustomerID, string SaleOfficerID, string CityID, string CityAreaID, DateTime StartDate, DateTime EndDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<ExecutionReportModel> list = new List<ExecutionReportModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                //-----------------------Get Execution Report------------------------------------------------
                SqlCommand objCommand = new SqlCommand("BrowseExceutionReport_New", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@IsSale", IsSale);
                objCommand.Parameters.AddWithValue("@VendorID", VendorID);
                objCommand.Parameters.AddWithValue("@CustomerID", CustomerID);
                objCommand.Parameters.AddWithValue("@SalesOfficerID", SaleOfficerID);
                objCommand.Parameters.AddWithValue("@CityID", CityID);
                objCommand.Parameters.AddWithValue("@CityAreaID", CityAreaID);
                objCommand.Parameters.AddWithValue("@StartDate", StartDate);
                objCommand.Parameters.AddWithValue("@EndDate", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new ExecutionReportModel()
                    {
                        SubItemID = Convert.ToInt32(x["SubItemID"] == DBNull.Value ? "0" : x["SubItemID"]),
                        SubItemName = x["SubItemName"] == DBNull.Value ? "" : x["SubItemName"].ToString(),
                        ItemID = Convert.ToInt32(x["ItemID"] == DBNull.Value ? "0" : x["ItemID"]),
                        ItemName = x["ItemName"] == DBNull.Value ? "" : x["ItemName"].ToString(),
                        TotalSQ = Convert.ToDecimal(x["TotalSQ"] == DBNull.Value ? "0" : x["TotalSQ"]),
                        TotalSP = Convert.ToDecimal(x["TotalSP"] == DBNull.Value ? "0" : x["TotalSP"]),
                        TotalRQ = Convert.ToDecimal(x["TotalRQ"] == DBNull.Value ? "0" : x["TotalRQ"]),
                        TotalRP = Convert.ToDecimal(x["TotalRP"] == DBNull.Value ? "0" : x["TotalRP"]),
                        RemainingQty = Convert.ToDecimal(x["RemainingQty"] == DBNull.Value ? "0" : x["RemainingQty"]),
                        RemainingPrice = Convert.ToDecimal(x["RemainingPrice"] == DBNull.Value ? "0" : x["RemainingPrice"]),
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
        }

        [HttpPost]
        public decimal GetTotalRecovery(string IsSale, string VendorID, string CustomerID, string SaleOfficerID, string CityID, string CityAreaID, DateTime StartDate, DateTime EndDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                decimal remainingQty = 0;
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                //-----------------------Get Execution Report------------------------------------------------
                SqlCommand objCommand = new SqlCommand("GetTotalRecovery", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@IsSale", IsSale);
                objCommand.Parameters.AddWithValue("@VendorID", VendorID);
                objCommand.Parameters.AddWithValue("@CustomerID", CustomerID);
                objCommand.Parameters.AddWithValue("@SalesOfficerID", SaleOfficerID);
                objCommand.Parameters.AddWithValue("@CityID", CityID);
                objCommand.Parameters.AddWithValue("@CityAreaID", CityAreaID);
                objCommand.Parameters.AddWithValue("@StartDate", StartDate);
                objCommand.Parameters.AddWithValue("@EndDate", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];
                    remainingQty = Convert.ToDecimal(dr["TotalRecovery"] == DBNull.Value ? 0 : decimal.Parse(dr["TotalRecovery"].ToString()));
                }

                return remainingQty;
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
        }



        public ActionResult SalesReport()
        {
            var vm = new SalesReportViewModel();
            string UserRole = GetCurrentUserRole();

            vm.VendorList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Supplier").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.ItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.SalesOfficerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.CityList = _repository.Cities.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            var cityID = 0;
            vm.CityAreaList = _repository.CityAreas.Where(x => x.CityId == cityID).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetSalesReportSearch(string VendorID, string ItemId, string SaleOfficerID, string CityID, string CityAreaID, string StartDate, string EndDate)
        {
            var vm = new SalesReportViewModel();

            if (!string.IsNullOrEmpty(VendorID) && !string.IsNullOrEmpty(ItemId) && !string.IsNullOrEmpty(SaleOfficerID) && !string.IsNullOrEmpty(CityID) && !string.IsNullOrEmpty(CityAreaID) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.SalesReportDetail = GetSalesReport(VendorID, ItemId, SaleOfficerID, CityID, CityAreaID, stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.VendorID = VendorID;
                vm.ItemID = ItemId;
                vm.SalesOfficerID = SaleOfficerID;
                vm.CityID = CityID;
                vm.CityAreaID = CityAreaID;
                vm.ReportName = "Sales Report";
                vm.TotalRecovery = GetTotalRecoveryForSalesReport(VendorID, ItemId, SaleOfficerID, CityID, CityAreaID, stdt, eddt);
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.SalesReportDetail = GetSalesReport("-1", "-1", "-1", "-1", "-1", DateTime.Now.AddDays(-30), DateTime.Now).ToList();
                vm.ReportName = "Execution Report";
                vm.TotalRecovery = GetTotalRecoveryForSalesReport("-1", "-1", "-1", "-1", "-1", DateTime.Now.AddDays(-30), DateTime.Now);
                //vm.ReportName = "";
                //vm.AccountName = "";
            }

            return PartialView("_SalesReport", vm);
        }

        [HttpPost]
        public IEnumerable<SalesReportModel> GetSalesReport(string VendorID, string ItemId, string SaleOfficerID, string CityID, string CityAreaID, DateTime StartDate, DateTime EndDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<SalesReportModel> list = new List<SalesReportModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                //-----------------------Get Execution Report------------------------------------------------
                SqlCommand objCommand = new SqlCommand("BrowseSalesReport", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@VendorID", VendorID);
                objCommand.Parameters.AddWithValue("@ItemID", ItemId);
                objCommand.Parameters.AddWithValue("@SalesOfficerID", SaleOfficerID);
                objCommand.Parameters.AddWithValue("@CityID", CityID);
                objCommand.Parameters.AddWithValue("@CityAreaID", CityAreaID);
                objCommand.Parameters.AddWithValue("@StartDate", StartDate);
                objCommand.Parameters.AddWithValue("@EndDate", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new SalesReportModel()
                    {
                        SubItemID = Convert.ToInt32(x["SubItemID"] == DBNull.Value ? "0" : x["SubItemID"]),
                        SubItemName = x["SubItemName"] == DBNull.Value ? "" : x["SubItemName"].ToString(),
                        ItemID = Convert.ToInt32(x["ItemID"] == DBNull.Value ? "0" : x["ItemID"]),
                        ItemName = x["ItemName"] == DBNull.Value ? "" : x["ItemName"].ToString(),
                        TotalSQ = Convert.ToDecimal(x["TotalSQ"] == DBNull.Value ? "0" : x["TotalSQ"]),
                        TotalSP = Convert.ToDecimal(x["TotalSP"] == DBNull.Value ? "0" : x["TotalSP"]),
                        TotalRQ = Convert.ToDecimal(x["TotalRQ"] == DBNull.Value ? "0" : x["TotalRQ"]),
                        TotalRP = Convert.ToDecimal(x["TotalRP"] == DBNull.Value ? "0" : x["TotalRP"]),
                        RemainingQty = Convert.ToDecimal(x["RemainingQty"] == DBNull.Value ? "0" : x["RemainingQty"]),
                        RemainingPrice = Convert.ToDecimal(x["RemainingPrice"] == DBNull.Value ? "0" : x["RemainingPrice"]),
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
        }


        [HttpPost]
        public decimal GetTotalRecoveryForSalesReport(string VendorID, string ItemID, string SaleOfficerID, string CityID, string CityAreaID, DateTime StartDate, DateTime EndDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                decimal remainingQty = 0;
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                //-----------------------Get Execution Report------------------------------------------------
                SqlCommand objCommand = new SqlCommand("GetTotalRecoveryForSalesReport", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@VendorID", VendorID);
                objCommand.Parameters.AddWithValue("@ItemID", ItemID);
                objCommand.Parameters.AddWithValue("@SalesOfficerID", SaleOfficerID);
                objCommand.Parameters.AddWithValue("@CityID", CityID);
                objCommand.Parameters.AddWithValue("@CityAreaID", CityAreaID);
                objCommand.Parameters.AddWithValue("@StartDate", StartDate);
                objCommand.Parameters.AddWithValue("@EndDate", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];
                    remainingQty = Convert.ToDecimal(dr["TotalRecovery"] == DBNull.Value ? 0 : decimal.Parse(dr["TotalRecovery"].ToString()));
                }

                return remainingQty;
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
        }



        public ActionResult AccountPayable()
        {
            var vm = new LedgerViewModel();

            vm.StartDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetAccountPayable(string StartDate)
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //vm.AccountPayableList = _repository.BrowseAccountPayableReport(stdt, UserRole).ToList();
            vm.AccountPayableList = GetAccountPayableSearch(UserRole, stdt).ToList();
            vm.ReportName = "AccountPayable";

            return PartialView("_AccountPayable", vm);
        }

        [HttpPost]
        public IEnumerable<BrowseAccountPayableReport_Result> GetAccountPayableSearch(string userRole, DateTime startDate)
        {
            try
            {
                IEnumerable<BrowseAccountPayableReport_Result> list = new List<BrowseAccountPayableReport_Result>();
                list = _repository.BrowseAccountPayableReport(startDate, userRole);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }


        public ActionResult AccountRecievable()
        {
            var vm = new LedgerViewModel();

            vm.StartDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetAccountRecievable(string StartDate)
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //vm.AccountRecievableList = _repository.BrowseAccountRecievableReport(stdt, UserRole).ToList();
            vm.AccountRecievableList = GetAccountRecievableSearch(UserRole, stdt).ToList();
            vm.ReportName = "AccountReceivable";

            return PartialView("_AccountRecievable", vm);
        }

        [HttpPost]
        public IEnumerable<BrowseAccountRecievableReport_Result> GetAccountRecievableSearch(string userRole, DateTime startDate)
        {
            try
            {
                IEnumerable<BrowseAccountRecievableReport_Result> list = new List<BrowseAccountRecievableReport_Result>();
                list = _repository.BrowseAccountRecievableReport(startDate, userRole);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }







        public ActionResult BankLedger()
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            vm.AccountList = _repository.BrowseBankAccount().Where(o => o.RoleId == UserRole).Select(x => new SelectListItem { Text = string.Format("{0}", x.BankName, x.AccountNumber), Value = x.ID.ToString() });
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);

        }

        [HttpPost]
        public ActionResult GetBankLedgerSearch(string AccountId, string StartDate, string EndDate)
        {
            var vm = new LedgerViewModel();
            string userRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(AccountId) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.LedgerDetail = GetBankLedger(userRole, int.Parse(AccountId.ToString()), stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.AccountID = AccountId;
                vm.ReportName = "Bank Ledger";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.LedgerDetail = null;
                vm.ReportName = "";
            }

            return PartialView("_LedgerDetail", vm);
        }

        [HttpPost]
        public IEnumerable<LedgerModel> GetBankLedger(string userRole, int accountId, DateTime startDate, DateTime endDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<LedgerModel> list = new List<LedgerModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                SqlCommand objCommand = new SqlCommand("GetBankLedger", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@StartDate", startDate);
                objCommand.Parameters.AddWithValue("@EndDate", endDate);
                objCommand.Parameters.AddWithValue("@BankID", accountId);
                objCommand.Parameters.AddWithValue("@RoleID", userRole);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new LedgerModel()
                    {
                        TransID = Convert.ToInt32(x["TransID"] == DBNull.Value ? "0" : x["TransID"]),
                        AccountCode = Convert.ToInt32(x["AccountCode"] == DBNull.Value ? "0" : x["AccountCode"]),
                        TransDate = (x["TransDate"] == DBNull.Value ? "" : Convert.ToDateTime(x["TransDate"]).ToString("dd/MM/yyyy")),
                        RecordID = Convert.ToInt32(x["RecordID"] == DBNull.Value ? "0" : x["RecordID"]),
                        RecordType = x["RecordType"] == DBNull.Value ? "" : x["RecordType"].ToString(),
                        Remarks = x["Remarks"] == DBNull.Value ? "" : x["Remarks"].ToString(),
                        Dr = Convert.ToDecimal(x["Dr"] == DBNull.Value ? "0" : x["Dr"]),
                        Cr = Convert.ToDecimal(x["Cr"] == DBNull.Value ? "0" : x["Cr"]),
                        Balance = Convert.ToDecimal(x["Balance"] == DBNull.Value ? "0" : x["Balance"].ToString()),
                        ValueType = x["ValueType"] == DBNull.Value ? "" : x["ValueType"].ToString()
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
        }

        public ActionResult CashLedger()
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);

        }

        [HttpPost]
        public ActionResult GetCashLedgerSearch(string StartDate, string EndDate)
        {
            var vm = new LedgerViewModel();
            string userRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.LedgerDetail = GetCashLedger(userRole, stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.ReportName = "Cash Ledger";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.LedgerDetail = null;
                vm.ReportName = "";
            }

            return PartialView("_LedgerDetail", vm);
        }

        [HttpPost]
        public IEnumerable<LedgerModel> GetCashLedger(string userRole, DateTime startDate, DateTime endDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<LedgerModel> list = new List<LedgerModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                SqlCommand objCommand = new SqlCommand("GetCashLedger", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@StartDate", startDate);
                objCommand.Parameters.AddWithValue("@EndDate", endDate);
                objCommand.Parameters.AddWithValue("@RoleID", userRole);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new LedgerModel()
                    {
                        TransID = Convert.ToInt32(x["TransID"] == DBNull.Value ? "0" : x["TransID"]),
                        AccountCode = Convert.ToInt32(x["AccountCode"] == DBNull.Value ? "0" : x["AccountCode"]),
                        TransDate = (x["TransDate"] == DBNull.Value ? "" : Convert.ToDateTime(x["TransDate"]).ToString("dd/MM/yyyy")),
                        RecordID = Convert.ToInt32(x["RecordID"] == DBNull.Value ? "0" : x["RecordID"]),
                        RecordType = x["RecordType"] == DBNull.Value ? "" : x["RecordType"].ToString(),
                        Remarks = x["Remarks"] == DBNull.Value ? "" : x["Remarks"].ToString(),
                        Dr = Convert.ToDecimal(x["Dr"] == DBNull.Value ? "0" : x["Dr"]),
                        Cr = Convert.ToDecimal(x["Cr"] == DBNull.Value ? "0" : x["Cr"]),
                        Balance = Convert.ToDecimal(x["Balance"] == DBNull.Value ? "0" : x["Balance"].ToString()),
                        ValueType = x["ValueType"] == DBNull.Value ? "" : x["ValueType"].ToString(),
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
        }


        public ActionResult ProfitAndLoss()
        {
            var vm = new LedgerViewModel();

            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetProfitAndLoss(string StartDate, string EndDate)
        {
            string userRole = GetCurrentUserRole();
            var vm = new LedgerViewModel();

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                //vm.ProfitAndLostReportList = _repository.GetProfitAndLossReport(stdt, eddt, UserRole).ToList();
                vm.ProfitAndLostReportList = GetProfitAndLossSearch(userRole, stdt, eddt).ToList();

                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.ReportName = "Profit & Loss Statement";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

                vm.LedgerDetail = null;
                vm.ReportName = "";
            }

            return PartialView("_ProfitAndLoss", vm);
        }

        [HttpPost]
        public IEnumerable<GetProfitAndLossReport_New_Result> GetProfitAndLossSearch(string userRole, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var dt = new DataTable();
                IEnumerable<GetProfitAndLossReport_New_Result> list = new List<GetProfitAndLossReport_New_Result>();
                list = _repository.GetProfitAndLossReport_New(StartDate, EndDate, userRole);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }



        public ActionResult ProfitAndLostSummary()
        {
            var vm = new ProfitAndLostSummaryViewModel();
            string UserRole = GetCurrentUserRole();

            vm.VendorList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Supplier").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.ItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.SalesOfficerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.BookerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.CityList = _repository.Cities.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            var cityID = 0;
            vm.CityAreaList = _repository.CityAreas.Where(x => x.CityId == cityID).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            vm.SONumber = "";
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult ProfitAndLostSummarySearch(string SONumber, string VendorId, string ItemId, string SaleOfficerId, string BookerId, string CityId, string CityAreaId, string StartDate, string EndDate)
        {
            var vm = new ProfitAndLostSummaryViewModel();

            if (!string.IsNullOrEmpty(VendorId) && !string.IsNullOrEmpty(ItemId) && !string.IsNullOrEmpty(SaleOfficerId) && !string.IsNullOrEmpty(CityId) && !string.IsNullOrEmpty(CityAreaId) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.ProfitAndLostSummaryDetail = GetProfitAndLostSummaryReport(SONumber, VendorId, ItemId, SaleOfficerId, BookerId, CityId, CityAreaId, stdt, eddt).ToList();
                vm.SONumber = SONumber;
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.VendorID = VendorId;
                vm.ItemID = ItemId;
                vm.SalesOfficerID = SaleOfficerId;
                vm.BookerID = BookerId;
                vm.CityID = CityId;
                vm.CityAreaID = CityAreaId;
                vm.ReportName = "Profit And Lost Summary Report";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.ProfitAndLostSummaryDetail = GetProfitAndLostSummaryReport("-1", "-1", "-1", "-1", "-1", "-1", "-1", DateTime.Now.AddDays(-30), DateTime.Now).ToList();
                vm.ReportName = "Execution Report";
                //vm.ReportName = "";
                //vm.AccountName = "";
            }

            return PartialView("_ProfitAndLostSummary", vm);
        }

        [HttpPost]
        public IEnumerable<ProfitAndLostSummaryModel> GetProfitAndLostSummaryReport(string SoNumber, string VendorID, string ItemId, string SaleOfficerID, string BookerID, string CityID, string CityAreaID, DateTime StartDate, DateTime EndDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<ProfitAndLostSummaryModel> list = new List<ProfitAndLostSummaryModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                //-----------------------Get Execution Report------------------------------------------------
                SqlCommand objCommand = new SqlCommand("sp_ProfitAndLostSummary", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@SONumber", SoNumber);
                objCommand.Parameters.AddWithValue("@VendorID", VendorID);
                objCommand.Parameters.AddWithValue("@ItemID", ItemId);
                objCommand.Parameters.AddWithValue("@SalesOfficerID", SaleOfficerID);
                objCommand.Parameters.AddWithValue("@BookerId", BookerID);
                objCommand.Parameters.AddWithValue("@CityID", CityID);
                objCommand.Parameters.AddWithValue("@CityAreaID", CityAreaID);
                objCommand.Parameters.AddWithValue("@StartDate", StartDate);
                objCommand.Parameters.AddWithValue("@EndDate", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new ProfitAndLostSummaryModel()
                    {
                        RecordType = x["RecordType"] == DBNull.Value ? "" : x["RecordType"].ToString(),
                        SONumber = Convert.ToInt32(x["SONumber"] == DBNull.Value ? "0" : x["SONumber"]),
                        SODate = (x["SODate"] == DBNull.Value ? "" : Convert.ToDateTime(x["SODate"]).ToString("dd/MM/yyyy")),
                        CustomerName = x["CustomerName"] == DBNull.Value ? "" : x["CustomerName"].ToString(),
                        ProductName = x["ProductName"] == DBNull.Value ? "" : x["ProductName"].ToString(),
                        Quantity = Convert.ToInt32(x["Quantity"] == DBNull.Value ? "0" : x["Quantity"]),
                        PurchasePrice = Convert.ToDecimal(x["PurchasePrice"] == DBNull.Value ? "0" : x["PurchasePrice"].ToString()),
                        SalesPrice = Convert.ToDecimal(x["SalesPrice"] == DBNull.Value ? "0" : x["SalesPrice"].ToString()),
                        TotalPurchase = Convert.ToDecimal(x["TotalPurchase"] == DBNull.Value ? "0" : x["TotalPurchase"].ToString()),
                        TotalSale = Convert.ToDecimal(x["TotalSale"] == DBNull.Value ? "0" : x["TotalSale"].ToString()),
                        TotalProfit = Convert.ToDecimal(x["TotalProfit"] == DBNull.Value ? "0" : x["TotalProfit"].ToString())
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
        }




        public ActionResult CompanyPositionReport()
        {
            var vm = new LedgerViewModel();

            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetCompanyPosition(string StartDate, string EndDate)
        {
            string userRole = GetCurrentUserRole();
            var vm = new LedgerViewModel();

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.CompanyPositionReportList = GetCompanyPositionSearch(userRole, stdt, eddt).ToList();

                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.ReportName = "Company Position Report";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

                vm.LedgerDetail = null;
                vm.ReportName = "";
            }

            return PartialView("_CompanyPositionReport", vm);
        }

        [HttpPost]
        public IEnumerable<GetCompanyPositionReport_Result> GetCompanyPositionSearch(string userRole, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var dt = new DataTable();
                IEnumerable<GetCompanyPositionReport_Result> list = new List<GetCompanyPositionReport_Result>();
                list = _repository.GetCompanyPositionReport(StartDate, EndDate, userRole);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }


        public ActionResult DailyCashReport()
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            vm.StartDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);

        }

        [HttpPost]
        public ActionResult GetDailyCashReportSearch(string StartDate)
        {
            var vm = new LedgerViewModel();
            string userRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.LedgerDetail = GetDailyCashReport(userRole, stdt).ToList();
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.TotalDr = (vm.LedgerDetail.Count > 1 ? vm.LedgerDetail.Sum(x => x.Dr) : 0) ?? 0;
                vm.TotalCr = (vm.LedgerDetail.Count > 1 ? vm.LedgerDetail.Sum(x => x.Cr) : 0) ?? 0;
                vm.ClosingBalance = (vm.LedgerDetail.Count > 1 ? vm.LedgerDetail[0].Balance + (vm.TotalDr - vm.TotalCr) : vm.LedgerDetail[0].Balance) ?? 0;
                vm.ReportName = "Daily Cash/Bank Report";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.LedgerDetail = null;
                vm.ReportName = "";
            }

            return PartialView("_DailyCashReport", vm);
        }

        [HttpPost]
        public IEnumerable<LedgerModel> GetDailyCashReport(string userRole, DateTime startDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<LedgerModel> list = new List<LedgerModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                SqlCommand objCommand = new SqlCommand("GetDailCashReport", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@ReportDate", startDate);
                objCommand.Parameters.AddWithValue("@RoleID", userRole);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new LedgerModel()
                    {
                        TransID = Convert.ToInt32(x["TransID"] == DBNull.Value ? "0" : x["TransID"]),
                        TransDate = (x["TransDate"] == DBNull.Value ? "" : Convert.ToDateTime(x["TransDate"]).ToString("dd/MM/yyyy")),
                        AccountCode = Convert.ToInt32(x["AccountCode"] == DBNull.Value ? "0" : x["AccountCode"]),
                        ChartOfAccountName = x["ChartOfAccountName"] == DBNull.Value ? "" : x["ChartOfAccountName"].ToString(),
                        Particular = x["Particular"] == DBNull.Value ? "" : x["Particular"].ToString(),
                        Remarks = x["Particular"] == DBNull.Value ? "" : x["Particular"].ToString(),
                        RecordID = Convert.ToInt32(x["AccountID"] == DBNull.Value ? "0" : x["AccountID"]),
                        AccountTitle = x["AccountName"] == DBNull.Value ? "" : x["AccountName"].ToString(),
                        Dr = Convert.ToDecimal(x["Dr"] == DBNull.Value ? "0" : x["Dr"]),
                        Cr = Convert.ToDecimal(x["Cr"] == DBNull.Value ? "0" : x["Cr"]),
                        Balance = Convert.ToDecimal(x["Balance"] == DBNull.Value ? "0" : x["Balance"].ToString())
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
        }

        public ActionResult DetailLedgerStatement()
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole).Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetDetailLedgerStatementSearch(string AccountId, string StartDate, string EndDate)
        {
            var vm = new LedgerViewModel();
            string userRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(AccountId) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.DetailLedgerStatement = GetDetailLedgerStatement(userRole, int.Parse(AccountId.ToString()), stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.AccountID = AccountId;
                vm.ReportName = "Detail Ledger Statement";
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.DetailLedgerStatement = null;
                vm.ReportName = "";
            }

            return PartialView("_DetailLedgerStatement", vm);
        }

        [HttpPost]
        public IEnumerable<DetailLedgerStatementModel> GetDetailLedgerStatement(string userRole, int accountId, DateTime startDate, DateTime endDate)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            try
            {
                var dt = new DataTable();
                IEnumerable<DetailLedgerStatementModel> list = new List<DetailLedgerStatementModel>();

                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                SqlCommand objCommand = new SqlCommand("BrowseDetailLedgerStatement", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@StartDate", startDate);
                objCommand.Parameters.AddWithValue("@EndDate", endDate);
                objCommand.Parameters.AddWithValue("@AccountID", accountId);
                objCommand.Parameters.AddWithValue("@RoleID", userRole);
                SqlDataAdapter da = new SqlDataAdapter(objCommand);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    list = dt.AsEnumerable().Select(x => new DetailLedgerStatementModel()
                    {
                        AccountId = Convert.ToInt32(x["AccountId"] == DBNull.Value ? "0" : x["AccountId"]),
                        AccountName = x["AccountName"] == DBNull.Value ? "" : x["AccountName"].ToString(),
                        TransDate = (x["TransDate"] == DBNull.Value ? "" : Convert.ToDateTime(x["TransDate"]).ToString("dd/MM/yyyy")),
                        Descriptions = x["Descriptions"] == DBNull.Value ? "" : x["Descriptions"].ToString(),
                        InQty = Convert.ToInt32(x["InQty"] == DBNull.Value ? "0" : x["InQty"]),
                        OutQty = Convert.ToInt32(x["OutQty"] == DBNull.Value ? "0" : x["OutQty"]),
                        Rate = Convert.ToDecimal(x["Rate"] == DBNull.Value ? "0" : x["Rate"].ToString()),
                        Dr = Convert.ToDecimal(x["DR"] == DBNull.Value ? "0" : x["DR"]),
                        Cr = Convert.ToDecimal(x["CR"] == DBNull.Value ? "0" : x["CR"]),
                        Balance = Convert.ToDecimal(x["Balance"] == DBNull.Value ? "0" : x["Balance"].ToString()),
                        Remarks = x["Remarks"] == DBNull.Value ? "" : x["Remarks"].ToString(),
                        RecordId = Convert.ToInt32(x["RecordId"] == DBNull.Value ? "0" : x["RecordId"]),
                        RoleID = x["RoleID"] == DBNull.Value ? "" : x["RoleID"].ToString(),
                        ValueType = x["ValueType"] == DBNull.Value ? "" : x["ValueType"].ToString()
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
        }

        public ActionResult ItemActivity()
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
            vm.ItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.SubItemName), Value = x.SubItemID.ToString() }).ToList();

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetItemActivity(int? SubItemID, string StartDate, string EndDate)
        {
            var vm = new LedgerViewModel();
            string UserRole = GetCurrentUserRole();

            DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //vm.ItemActivityList = _repository.BrowseItemActivityReport(stdt, eddt, SubItemID.Value, UserRole).ToList();
            vm.ItemActivityList = GetItemActivitySearch(UserRole, SubItemID.Value, stdt, eddt).ToList();
            vm.ReportName = "Item Activity Report";

            return PartialView("_ItemActivity", vm);
        }

        [HttpPost]
        public IEnumerable<BrowseItemActivityReport_Result> GetItemActivitySearch(string userRole, int subItemId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var dt = new DataTable();
                IEnumerable<BrowseItemActivityReport_Result> list = new List<BrowseItemActivityReport_Result>();
                list = _repository.BrowseItemActivityReport(startDate, endDate, subItemId, userRole);
                return list;
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