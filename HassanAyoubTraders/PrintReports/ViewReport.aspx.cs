using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Microsoft.Reporting.WebForms;
using HassanAyoubTraders.Controllers;
using Microsoft.Reporting.WebForms;
using System.Globalization;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Web;

namespace HassanAyoubTraders.PrintReports
{
    public partial class ViewReport : BaseUI
    {
        CultureInfo culture = new CultureInfo("en-US");

        #region

        private void LoadReport()
        {
            var reportName = Request.QueryString["ReportName"] == null ? "" : Request.QueryString["ReportName"].ToString();

            if (reportName == "SaleInvoiceSummary")
            {
                SaleInvoiceSummary();
            }
            else if (reportName == "CustomerLedger")
            {
                AccountLedger("CustomerLedger");
            }
            else if (reportName == "VendorLedger")
            {
                AccountLedger("VendorLedger");
            }
            else if (reportName == "CustomerWiseBalance")
            {
                CustomerWiseBalance();
            }
            else if (reportName == "ExecutionReport")
            {
                ExecutionReport();
            }
            else if (reportName == "SalesReport")
            {
                SalesReport();
            }
            else if (reportName == "AccountPayable")
            {
                AccountPayable();
            }
            else if (reportName == "AccountReceivable")
            {
                AccountReceivable();
            }




            else if (reportName == "InventoryOnHand")
            {
                InventoryOnHand();
            }
            else if (reportName == "AccountLedger")
            {
                AccountLedger("VendorLedger");
            }
            else if (reportName == "BankLedger")
            {
                BankLedger();
            }
            else if (reportName == "CashLedger")
            {
                CashLedger();
            }
            else if (reportName == "DailyCashReport")
            {
                DailyCashReport();
            }
            else if (reportName == "DailyLedgerStatement")
            {
                DailyLedgerStatement();
            }
            else if (reportName == "ItemActivity")
            {
                ItemActivity();
            }

            else if (reportName == "ProfitAndLoss")
            {
                 ProfitAndLoss();
            }
            else if (reportName == "CompanyPositionReport")
            {
                CompanyPositionReport();
            }
            else if (reportName == "PurchaseInvoice")
            {
                pnlrpt.Visible = false;
                //pnlHTML.Visible = true;
            }
            else if (reportName == "SalesInvoice")
            {
                pnlrpt.Visible = false;
            }
            else if (reportName == "SalesInvoicereturn")
            {
                pnlrpt.Visible = false;
            }
        }

        private void loadDropDown()
        {
            //var olist = dbContext.CompanyInformations.ToList();
            //ddlCompany.DataSource = olist;
            //ddlCompany.DataBind();
        }

        public void SaleInvoiceSummary()
        {

            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var soId = Request.QueryString["SaleOfficerID"] == null ? -1 : int.Parse(Request.QueryString["SaleOfficerID"]);
            var cusId = Request.QueryString["CustomerID"] == null ? -1 : int.Parse(Request.QueryString["CustomerID"]);
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetSaleInvoiceSummary(soId.ToString(), cusId.ToString(), stDate, edDate);
            var companyInfo = obj.GetCompanyInfo(userRole);
            var oAccount = obj.GetAccountByID(cusId);
            var title = "";
            if (oAccount != null)
                title = string.Format("For the Period From:{1} - To:{2}", oAccount.Name, st, ed);
            var reportDetail = GetReportData("Sale Invoice Summary", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptSaleInvoiceSummary.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }
        }

        public void AccountLedger(string type)
        {
            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var accId = Request.QueryString["AccountId"] == null ? 0 : int.Parse(Request.QueryString["AccountId"]);
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetAccountLedger(userRole, accId, stDate, edDate);
            var companyInfo = obj.GetCompanyInfo(userRole);
            var oAccount = obj.GetAccountByID(accId);
            var title = "";
            if (oAccount != null)
            {
                title = type == "CustomerLedger" ? string.Format("Customer: {0} {3}(From:{1} - To:{2})", oAccount.Name, st, ed, "\n") : string.Format("Vendor : {0} {3}(From:{1} - To:{2})", oAccount.Name, st, ed, "\n");
            }
            var reportDetail = type == "CustomerLedger" ? GetReportData("Customer Ledger", title) : GetReportData("Vendor Ledger", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptLedger.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }

        public void InventoryOnHand()
        {
            string userRole = GetCurrentUserRole();
            var obj = new InventoryController();
            var itmID = Request.QueryString["ItemID"] == null ? 0 : int.Parse(Request.QueryString["ItemID"]);
            var subitmID = Request.QueryString["SubItemId"] == null ? 0 : int.Parse(Request.QueryString["SubItemId"]);
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetInventoryOnHand(userRole, itmID, subitmID, stDate, edDate);
            var companyInfo = obj.GetCompanyInfo(userRole);
            var oItem = obj.GetItemByID(itmID);
            var title = "";
            if (oItem != null)
                title = string.Format("Item : {0}", oItem.ProductName);
            else if (oItem == null && itmID == 0)
                title = string.Format("All Items");

            var reportDetail = GetReportData("Inventory On Hand", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptInventoryOnHand.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();

                //ReportPrintDocument rp = new ReportPrintDocument();
                //rp.Print();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }
        }

        public void CustomerWiseBalance()
        {
            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var cityID = Request.QueryString["CityID"] == null ? -1 : int.Parse(Request.QueryString["CityID"]);
            var cityAreaID = Request.QueryString["CityAreaID"] == null ? -1 : int.Parse(Request.QueryString["CityAreaID"]);
            var soId = Request.QueryString["SalesOfficerID"] == null ? -1 : int.Parse(Request.QueryString["SalesOfficerID"]);
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetCustomerWiseBalance(cityID.ToString(), cityAreaID.ToString(), soId.ToString(), stDate, edDate);
            var companyInfo = obj.GetCompanyInfo(userRole);
            var oAccount = obj.GetAccountByID(soId);
            var title = "";
            if (oAccount != null)
                title = string.Format("Outstanding Report for the period From:{1} - To:{2}", oAccount.Name, st, ed);
            var reportDetail = GetReportData("Customer Wise Balance", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptCustomerWiseBalance.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }
        }

        public void ExecutionReport()
        {
            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var isSale = Request.QueryString["IsSale"] == null ? true : bool.Parse(Request.QueryString["IsSale"]);
            var vendorID = Request.QueryString["VendorID"] == null ? -1 : int.Parse(Request.QueryString["VendorID"]);
            var customerID = Request.QueryString["CustomerID"] == null ? -1 : int.Parse(Request.QueryString["CustomerID"]);
            var soId = Request.QueryString["SalesOfficerID"] == null ? -1 : int.Parse(Request.QueryString["SalesOfficerID"]);
            var cityID = Request.QueryString["CityID"] == null ? -1 : int.Parse(Request.QueryString["CityID"]);
            var cityAreaID = Request.QueryString["CityAreaID"] == null ? -1 : int.Parse(Request.QueryString["CityAreaID"]);
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetExecutionReport(isSale.ToString(), vendorID.ToString(), customerID.ToString(), soId.ToString(), cityID.ToString(), cityAreaID.ToString(), stDate, edDate);
            var totalrecovert = obj.GetTotalRecovery(isSale.ToString(), vendorID.ToString(), customerID.ToString(), soId.ToString(), cityID.ToString(), cityAreaID.ToString(), stDate, edDate);
            var dtRecovery = new DataTable();
            dtRecovery.Columns.Add("TotalAmount", typeof(decimal));
            dtRecovery.Rows.Add(totalrecovert);


            var companyInfo = obj.GetCompanyInfo(userRole);
            var oAccount = obj.GetAccountByID(soId);
            var title = "";
            if (oAccount != null)
                title = string.Format("Execution Report for the period From:{1} - To:{2}", oAccount.Name, st, ed);
            var reportDetail = GetReportData("Execution Report", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptExecutionReport.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                ReportDataSource datasource4 = new ReportDataSource("DataSet4", dtRecovery);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.DataSources.Add(datasource4);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }
        }

        public void SalesReport()
        {
            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var vendorID = Request.QueryString["VendorID"] == null ? -1 : int.Parse(Request.QueryString["VendorID"]);
            var ItemID = Request.QueryString["ItemID"] == null ? -1 : int.Parse(Request.QueryString["ItemID"]);
            var soId = Request.QueryString["SalesOfficerID"] == null ? -1 : int.Parse(Request.QueryString["SalesOfficerID"]);
            var cityID = Request.QueryString["CityID"] == null ? -1 : int.Parse(Request.QueryString["CityID"]);
            var cityAreaID = Request.QueryString["CityAreaID"] == null ? -1 : int.Parse(Request.QueryString["CityAreaID"]);
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetSalesReport(vendorID.ToString(), ItemID.ToString(), soId.ToString(), cityID.ToString(), cityAreaID.ToString(), stDate, edDate);
            var totalrecovert = obj.GetTotalRecoveryForSalesReport(vendorID.ToString(), ItemID.ToString(), soId.ToString(), cityID.ToString(), cityAreaID.ToString(), stDate, edDate);
            var dtRecovery = new DataTable();
            dtRecovery.Columns.Add("TotalAmount", typeof(decimal));
            dtRecovery.Rows.Add(totalrecovert);


            var companyInfo = obj.GetCompanyInfo(userRole);
            var oAccount = obj.GetAccountByID(soId);
            var title = "";
            if (oAccount != null)
                title = string.Format("Sales Report for the period From:{1} - To:{2}", oAccount.Name, st, ed);
            var reportDetail = GetReportData("Sales Report", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptSalesReport.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                ReportDataSource datasource4 = new ReportDataSource("DataSet4", dtRecovery);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.DataSources.Add(datasource4);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }
        }

        public void AccountPayable()
        {

            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetAccountPayableSearch(userRole, stDate).ToList();
            var companyInfo = obj.GetCompanyInfo(userRole);
            var title = string.Format("Account Payables (Date:{0})", st);
            var reportDetail = GetReportData("Account Payable", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptAccountPayable.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }

        public void AccountReceivable()
        {
            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetAccountRecievableSearch(userRole, stDate).ToList();
            var companyInfo = obj.GetCompanyInfo(userRole);
            var title = string.Format("Account Receivable (Date:{0})", st);
            var reportDetail = GetReportData("Account Receivable", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptAccountReceivable.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }



        public void BankLedger()
        {

            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var accId = Request.QueryString["AccountId"] == null ? 0 : int.Parse(Request.QueryString["AccountId"]);
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetBankLedger(userRole, accId, stDate, edDate);
            var companyInfo = obj.GetCompanyInfo(userRole);
            var oBank = obj.GetBankByID(accId);
            var title = "";
            if (oBank != null)
                title = string.Format("Bank : {0} ({1}) (From:{2} - To:{3})", oBank.BankName, oBank.AccountNumber, st, ed);
            var reportDetail = GetReportData("Bank Ledger", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptLedger.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();

            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }

        public void CashLedger()
        {

            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetCashLedger(userRole, stDate, edDate);
            var companyInfo = obj.GetCompanyInfo(userRole);
            var title = string.Format("Cash Ledger (From:{0} - To:{1})", st, ed);
            var reportDetail = GetReportData("Cash Ledger", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptLedger.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();

            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }

        public void DailyCashReport()
        {

            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetDailyCashReport(userRole, stDate);
            var companyInfo = obj.GetCompanyInfo(userRole);
            var title = string.Format("Daily Cash Report (Date:{0})", st);
            var reportDetail = GetReportData("Daily Cash Report", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptDailyCashReport.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();

            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }

        public void DailyLedgerStatement()
        {

            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var accId = Request.QueryString["AccountId"] == null ? 0 : int.Parse(Request.QueryString["AccountId"]);
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetDetailLedgerStatement(userRole, accId, stDate, edDate);
            var companyInfo = obj.GetCompanyInfo(userRole);
            var oAccount = obj.GetAccountByID(accId);
            var title = "";
            if (oAccount != null)
                title = string.Format("Account : {0} (From:{1} - To:{2})", oAccount.Name, st, ed);
            var reportDetail = GetReportData("Detail Ledger Statement", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptDailyLedgerStatement.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }

        public void ItemActivity()
        {

            string userRole = GetCurrentUserRole();
            var obj = new ReportController();
            var subItemid = Request.QueryString["SubItemID"] == null ? 0 : int.Parse(Request.QueryString["SubItemID"]);
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var ed = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime edDate = string.IsNullOrEmpty(ed) ? DateTime.Now : DateTime.ParseExact(ed, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var list = obj.GetItemActivitySearch(userRole, subItemid, stDate, edDate).ToList();
            var companyInfo = obj.GetCompanyInfo(userRole);
            var oItem = obj.GetSubItemByID(subItemid);
            var title = "";
            if (oItem != null)
                title = string.Format("Sub Item : {0} (From:{1} - To:{2})", oItem.SubItemName, st, ed);
            var reportDetail = GetReportData("Item Activity", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptItemActivity.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }


        public void ProfitAndLoss()
        {
            string userRole = GetCurrentUserRole();
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var et = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime etDate = string.IsNullOrEmpty(et) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(et, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var obj = new ReportController();
            var list = obj.GetProfitAndLossSearch(userRole, stDate, etDate).ToList();
            //var list = CreateProfitAndLossData(userRole, stDate,etDate);
            var companyInfo = obj.GetCompanyInfo(userRole);
            var title = string.Format("Profit & Loss (From:{0} - To:{1})", st, et);
            var reportDetail = GetReportData("Profit & Loss Report", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptProfitAndLoss.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }

        public void CompanyPositionReport()
        {
            string userRole = GetCurrentUserRole();
            var st = Request.QueryString["StartDate"] == null ? "" : Request.QueryString["StartDate"].ToString().Replace("|", "/");
            var et = Request.QueryString["EndDate"] == null ? "" : Request.QueryString["EndDate"].ToString().Replace("|", "/");

            DateTime stDate = string.IsNullOrEmpty(st) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(st, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime etDate = string.IsNullOrEmpty(et) ? DateTime.Now.AddMonths(-1) : DateTime.ParseExact(et, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var obj = new ReportController();
            var list = obj.GetCompanyPositionSearch(userRole, stDate, etDate).ToList();
            var companyInfo = obj.GetCompanyInfo(userRole);
            var title = string.Format("Company Position Report (From:{0} - To:{1})", st, et);
            var reportDetail = GetReportData("Company Position Report", title);

            if (list.ToList().Any())
            {
                var objList = list.ToList();
                rptViewer.Visible = true;
                lblSuccess.Visible = false;
                rptViewer.ProcessingMode = ProcessingMode.Local;
                rptViewer.LocalReport.ReportPath = Server.MapPath("~/PrintReports/rptCompanyPositionReport.rdlc");
                rptViewer.LocalReport.EnableExternalImages = true;

                ReportDataSource datasource = new ReportDataSource("DataSet1", objList);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyInfo);
                ReportDataSource datasource3 = new ReportDataSource("DataSet3", reportDetail);
                rptViewer.LocalReport.DataSources.Clear();
                rptViewer.LocalReport.DataSources.Add(datasource);
                rptViewer.LocalReport.DataSources.Add(datasource2);
                rptViewer.LocalReport.DataSources.Add(datasource3);
                rptViewer.LocalReport.Refresh();
            }
            else
            {
                rptViewer.Visible = false;
                lblSuccess.Visible = true;
                lblSuccess.Text = "Record Not Found!";
            }

        }

        #endregion

        #region Events 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //hdnGetWebsiteUrl.Value = ConfigurationManager.AppSettings["WebsiteUrl"].ToString();
                //loadDropDown();
                LoadReport();
            }
        }

        protected void btnPrint_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                FileStream fs = new FileStream(HttpContext.Current.Server.MapPath("output.pdf"),
                FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();

                //Open existing PDF
                Document document = new Document(PageSize.LETTER);
                PdfReader reader = new PdfReader(HttpContext.Current.Server.MapPath("output.pdf"));
                //Getting a instance of new PDF writer
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(
                   HttpContext.Current.Server.MapPath("Print.pdf"), FileMode.Create));
                document.Open();
                PdfContentByte cb = writer.DirectContent;

                int i = 0;
                int p = 0;
                int n = reader.NumberOfPages;
                Rectangle psize = reader.GetPageSize(1);

                float width = psize.Width;
                float height = psize.Height;

                //Add Page to new document
                while (i < n)
                {
                    document.NewPage();
                    p++;
                    i++;

                    PdfImportedPage page1 = writer.GetImportedPage(reader, i);
                    cb.AddTemplate(page1, 0, 0);
                }

                //Attach javascript to the document
                PdfAction jAction = PdfAction.JavaScript("this.print(true);\r", writer);
                writer.AddJavaScript(jAction);
                document.Close();

                //Attach pdf to the iframe
                //frmPrint.Attributes["src"] = "Print.pdf";

                //rptAccountPayable report1 = new reportname();
                //report1.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
                //report1.PrintOptions.PaperSize = PaperSize.PaperA4;
                //report1.PrintToPrinter(1, false, 0, 15);

                //--------------------------------
                //Warning[] warnings;
                //string[] streamids;
                //string mimeType;
                //string encoding;
                //string extension;

                //byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType,
                //               out encoding, out extension, out streamids, out warnings);

                //var path = "~/Document/Printing/";
                //if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))
                //    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));

                //var fileName = path + "output.pdf";
                //FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(fileName),
                //FileMode.Create);
                //fs.Write(bytes, 0, bytes.Length);
                //fs.Close();

                ////Open existing PDF
                //Document document = new Document(PageSize.LETTER);
                //PdfReader reader = new PdfReader(HttpContext.Current.Server.MapPath(fileName));
                ////Getting a instance of new PDF writer
                //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(
                //   HttpContext.Current.Server.MapPath("Print.pdf"), FileMode.Create));
                //document.Open();
                //PdfContentByte cb = writer.DirectContent;

                //int i = 0;
                //int p = 0;
                //int n = reader.NumberOfPages;
                //iTextSharp.text.Rectangle psize = reader.GetPageSize(1);

                //float width = psize.Width;
                //float height = psize.Height;

                ////Add Page to new document
                //while (i < n)
                //{
                //    document.NewPage();
                //    p++;
                //    i++;

                //    PdfImportedPage page1 = writer.GetImportedPage(reader, i);
                //    cb.AddTemplate(page1, 0, 0);
                //}

                ////Attach javascript to the document
                //PdfAction jAction = PdfAction.JavaScript("this.print(true);\r", writer);
                //writer.AddJavaScript(jAction);
                //document.Close();

                ////Attach pdf to the iframe
                //frmPrint.Attributes["src"] = "Print.pdf";
            }
            catch (Exception ex)
            {
            }

        }

        protected void lnkPrintHTML_Click(object sender, EventArgs e)
        {

        }

        #endregion
    }
}