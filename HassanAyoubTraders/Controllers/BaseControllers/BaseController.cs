using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Configuration;
using HassanAyoubTraders.Models.ViewModels;
using HassanAyoubTraders.Models.EF;

namespace HassanAyoubTraders.Controllers.BaseControllers
{
    public class BaseController : Controller
    {
        public HassanAyoubDBEntities  _repository = new HassanAyoubDBEntities();
        public ApplicationUserManager _userManager;

        [HttpPost]
        public string GetWebsiteUrl()
        {
            if (ConfigurationManager.AppSettings["WebsiteUrl"] == null)
                return "";
            else
                return ConfigurationManager.AppSettings["WebsiteUrl"].ToString();
        }

        [HttpPost]
        public string GetCurrentUserName()
        {
            return User.Identity.Name;
        }

        [HttpPost]
        public int GetCashAccountID()
        {
            string UserRole = GetCurrentUserRole();
            var acc = _repository.BrowseAccount().Where(x => x.RoleID == UserRole && x.IsCashAccount == true).FirstOrDefault();
            int id = 0;
            if (acc != null)
            {
                id = acc.ID;
            }
            return id;
        }

        [HttpPost]
        public string GetCashAccountName()
        {
            string UserRole = GetCurrentUserRole();
            var acc = _repository.BrowseAccount().Where(x => x.RoleID == UserRole && x.IsCashAccount == true).FirstOrDefault();
            string name = "";
            if (acc != null)
            {
                name = acc.Name;
            }
            return name;
        }

        [HttpPost]
        public string GetCurrentUserRole()
        {
            try
            {
                var user = _repository.AspNetUsers.FirstOrDefault(x => x.UserName.Equals(User.Identity.Name));
                if (user == null || user.Id == null)
                {
                    return "";
                }
                else
                {
                    _userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    var roles = _userManager.GetRoles(user.Id);
                    return roles[0];
                }
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        [HttpPost]
        public int InsertTransactionEntry(string accountCode, DateTime transDate, decimal dr, decimal cr, string remarks, int recordId, string recordType)
        {
            dbConnector objConn = new dbConnector();
            SqlConnection Conn = objConn.GetConnection;
            Conn.Open();
            int transId = 0;
            try
            {
                if (Conn.State != System.Data.ConnectionState.Open)
                    Conn.Open();

                SqlCommand objCommand = new SqlCommand("InsertAndDeleteLedgerEntry", Conn);
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Parameters.AddWithValue("@AccountCode", accountCode);
                objCommand.Parameters.AddWithValue("@TransDate", transDate);
                objCommand.Parameters.AddWithValue("@DR", dr);
                objCommand.Parameters.AddWithValue("@CR", cr);
                objCommand.Parameters.AddWithValue("@CreatedBy", "1");
                objCommand.Parameters.AddWithValue("@Remarks", remarks);
                objCommand.Parameters.AddWithValue("@RecordID", recordId);
                objCommand.Parameters.AddWithValue("@RecordType", recordType);
                objCommand.Parameters.Add("@TransactionID", SqlDbType.Int, 50).Direction = ParameterDirection.Output;
                //objCommand.Parameters["@TransactionID"].Direction = ParameterDirection.Output;
                objCommand.ExecuteNonQuery();
                transId = (int)objCommand.Parameters["@TransactionID"].Value;
                return transId;
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
        public string GetCodeValueByCodeID(int? codeId)
        {
            var code = _repository.Codes.Where(x => x.ID == codeId).FirstOrDefault();
            string codeValue = "";
            if (code != null)
            {
                codeValue = code.CodeValue;
            }
            return codeValue;
        }

        [HttpPost]
        public string GetAccountTypeByAccountID(int? accountId)
        {
            var account = _repository.BrowseAccountByID(accountId).FirstOrDefault();
            string accType = "";
            if (account != null)
            {
                accType = account.AccountType == null ? "" : account.AccountType;
            }
            return accType;
        }

        [HttpPost]
        public JsonResult GetPaymentTypes()
        {
            var continentals = _repository.Codes.Where(x => x.CodeType == "Payment Type").Select(c => new
            {
                Text = c.CodeValue,
                Value = c.ID
            }).OrderBy(s => s.Text);
            return Json(continentals);
        }

        [HttpPost]
        public JsonResult GetAccountsByPaymentType(int? id)
        {
            var listAccount = _repository.BrowseAccount().ToList();
            if (id == 7)                    // Payment Voucher
            {
                var ddl = listAccount.Where(x => x.AccountType.Equals("Supplier") && x.RoleID == GetCurrentUserRole()).Select(x => new SelectListItem
                {
                    Text = x.FullName,
                    Value = x.ID.ToString()
                }).ToList();
                return Json(ddl);
            }
            else if (id == 8)                // Receipt Voucher
            {
                var ddl = listAccount.Where(x => x.AccountType.Equals("Customer") && x.RoleID == GetCurrentUserRole()).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ID.ToString()
                }).ToList();
                return Json(ddl);
            }
            else
            {
                var ddl = listAccount.Where(x => x.IsDelete == false && x.IsActive == true && x.RoleID == GetCurrentUserRole()).Select(x => new SelectListItem
                {
                    Text = x.Name + " ( " + x.AccountType + " )",
                    Value = x.ID.ToString()
                }).ToList();
                return Json(ddl);
            }
        }

        [HttpPost]
        public JsonResult GetAccountsByTypeName(string type)
        {
            var listAccount = _repository.BrowseAccount().ToList();
            if (type == "Payment Voucher")
            {
                var ddl = listAccount.Where(x => x.AccountType.Equals("Supplier") && x.RoleID == GetCurrentUserRole()).Select(x => new SelectListItem
                {
                    Text = x.FullName,
                    Value = x.ID.ToString()
                }).ToList();
                return Json(ddl);
            }
            else if (type == "Receipt Voucher")
            {
                var ddl = listAccount.Where(x => x.AccountType.Equals("Customer") && x.RoleID == GetCurrentUserRole()).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ID.ToString()
                }).ToList();
                return Json(ddl);
            }
            else
            {
                var ddl = listAccount.Where(x => x.IsDelete == false && x.IsActive == true && x.RoleID == GetCurrentUserRole()).Select(x => new SelectListItem
                {
                    Text = x.Name + " ( " + x.AccountType + " )",
                    Value = x.ID.ToString()
                }).ToList();
                return Json(ddl);
            }
        }

        [HttpPost]
        public JsonResult GetExpanseTypes()
        {
            var list = _repository.ChartofAccounts.Where(x => x.AccountTypeID == 6).ToList();
            var ddl = list.Select(x => new SelectListItem
            {
                Text = x.AccountName,
                Value = x.AccountCode.ToString()
            }).ToList();
            return Json(ddl);
        }

        [HttpPost]
        public JsonResult GetBanks()
        {
            var role = GetCurrentUserRole();
            var list = _repository.Banks.Where(x => x.IsActive == true && x.RoleId == role).ToList();
            var ddl = list.Select(x => new SelectListItem
            {
                Text = x.BankName + " (" + x.AccountTitle + " - " + x.AccountNumber + ") ",
                Value = x.ID.ToString()
            }).ToList();
            return Json(ddl);
        }

        [HttpPost]
        public JsonResult GetPaymentMode()
        {
            var lst = _repository.Codes.Where(x => x.CodeType == "Payment Method").Select(c => new
            {
                Text = c.CodeValue,
                Value = c.ID
            }).OrderBy(s => s.Text);
            return Json(lst);
        }

        public IEnumerable<CompanyInformation> GetCompanyInfo(string userRole)
        {
            return _repository.CompanyInformations.Where(x => x.RoleId == userRole).ToList();
        }

        public IEnumerable<CompanyInformation> GetCompanyInfo(int id)
        {
            return _repository.CompanyInformations.Where(x => x.ID == id).ToList();
        }

        public IEnumerable<string> GetStatus()
        {
            var oStatus = new List<string>();
            oStatus.Add("Completed");
            oStatus.Add("Hold");
            return oStatus;
        }


        [HttpPost]
        public Bank GetBankByID(int? Id)
        {
            var obj = _repository.Banks.FirstOrDefault(x => x.ID == Id);
            return obj;
        }

        [HttpPost]
        public Account GetAccountByID(int? Id)
        {
            var obj = _repository.Accounts.FirstOrDefault(x => x.ID == Id);
            return obj;
        }

        [HttpPost]
        public Item GetItemByID(int? Id)
        {
            var obj = _repository.Items.FirstOrDefault(x => x.ItemID == Id && x.IsScrap != true);
            return obj;
        }

        [HttpPost]
        public SubItem GetSubItemByID(int? Id)
        {
            var obj = _repository.SubItems.FirstOrDefault(x => x.SubItemID == Id && x.IsScrap != true);
            return obj;
        }

        public int GetNewNumber(string type)
        {
            int num = 1;
            if (type == "PO")
            {

            }
            else if (type == "SO")
            {

            }
            else if (type == "PP")
            {

            }
            else if (type == "RP")
            {

            }

            return num;
        }

        [HttpPost]
        public JsonResult GetCityAreaByCityID(int? Id)
        {
            AddEditAccountViewModel model = new AddEditAccountViewModel();
            model.CityAreaList = _repository.CityAreas.Where(x => x.CityId == Id).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            return Json(model);
        }

        [HttpPost]
        public JsonResult GetBookerIDByCustomerID(int? Id)
        {
            SaleOrderViewModel model = new SaleOrderViewModel();
            var mdl = _repository.BrowseAccount().FirstOrDefault(x => x.ID == Id);
            if (mdl == null)
            {
                model.BookerID = 0;// mdl.EmployeeID ?? 0;
                model.BookerName = "Select";// string.Format("{0} ({1})", mdl.EmployeeName, "Employee");
            }
            else
            {
                model.BookerID = 0;
                model.BookerName = string.Format("Select");
            }
            return Json(model);
        }

        [HttpPost]
        public JsonResult IsValidSONo(int? SONo)
        {
            SaleOrderViewModel model = new SaleOrderViewModel();
            var pObj = _repository.SaleOrders.FirstOrDefault(o => o.SONumber == SONo);
            if (pObj != null)
                return Json(new { SOID = pObj.SaleOrderID, Result = "OK" });
            else
                return Json(new { SOID = 0, Result = "Failed" });
        }

        public JsonResult GetInventoryItem(int ItemID)
        {
            string message = string.Empty;
            try
            {
                //var itemList = _repository.GetInventoryItemByID(ItemID).FirstOrDefault();
                var itemList = _repository.GetProductsForDdl().FirstOrDefault(x => x.SubItemID == ItemID);
                message = "";
                return Json(new { status = "Success", message = "Inventory", data = itemList, detail = message, Result = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { status = "Success", message = "Error", data = "", detail = message, Result = "OK" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetItemsList(string q)
        {
            string UserRole = GetCurrentUserRole();

            var list = new List<Select2Model>();

            list = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new Select2Model { text = string.Format("{0}", x.ProductFullName), id = x.SubItemID.ToString() }).ToList();

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list = list.Where(x => x.text.ToLower().Contains(q.ToLower())).ToList();
            }
            return Json(new { items = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendorListSearch(string q)
        {
            string UserRole = GetCurrentUserRole();

            var list = new List<Select2Model>();
            list = _repository.BrowseAccount().Where(x => x.RoleID == UserRole && x.AccountType == "Supplier").AsEnumerable().Select(x => new Select2Model { text = string.Format("{0}", x.FullName), id = x.ID.ToString() }).ToList();
            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list = list.Where(x => x.text.ToLower().Contains(q.ToLower())).ToList();
            }
            return Json(new { items = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerListSearch(string q)
        {
            string UserRole = GetCurrentUserRole();

            var list = new List<Select2Model>();
            list = _repository.BrowseAccount().Where(x => x.RoleID == UserRole && x.AccountType == "Customer").AsEnumerable().Select(x => new Select2Model { text = string.Format("{0}", x.FullName), id = x.ID.ToString() }).ToList();
            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list = list.Where(x => x.text.ToLower().Contains(q.ToLower())).ToList();
            }
            return Json(new { items = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankListSearch(string q)
        {
            string UserRole = GetCurrentUserRole();

            var list = new List<Select2Model>();
            list = _repository.Banks.Where(x => x.IsActive == true).AsEnumerable().Select(x => new Select2Model { text = string.Format("{0}", x.BankName), id = x.ID.ToString() }).ToList();
            //list = _repository.BrowseAccount().Where(x => x.RoleID == UserRole && x.AccountType == "Customer").AsEnumerable().Select(x => new Select2Model { text = string.Format("{0}", x.FullName), id = x.ID.ToString() }).ToList();
            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list = list.Where(x => x.text.ToLower().Contains(q.ToLower())).ToList();
            }
            return Json(new { items = list }, JsonRequestBehavior.AllowGet);
        }

    }
}