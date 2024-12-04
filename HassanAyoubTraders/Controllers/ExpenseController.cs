using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models;
using HassanAyoubTraders.Models.EF;
using HassanAyoubTraders.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class ExpenseController : BaseController
    {
        // GET: Expanse
        public ActionResult ExpenseList()
        {
            var vm = new ExpanseListViewModel();
            string UserRole = GetCurrentUserRole();

            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountTypeID == 10).Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetExpasnes(string PaymentType, int? AccountsID, string StartDate, string EndDate)
        {
            try
            {
                string UserRole = GetCurrentUserRole();
                var vm = new ExpanseListViewModel();

                DateTime stdt;
                DateTime eddt;
                if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
                {
                    stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    stdt = DateTime.Now.AddDays(-30);
                    eddt = DateTime.Now;
                }

                var list = _repository.BrowseExpenses(PaymentType, AccountsID, stdt, eddt).Where(x => x.RoleID == UserRole).ToList();

                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");

                return PartialView("_ExpenseList", list);
            }
            catch (Exception ex)
            {
                return PartialView("_ExpenseList", null);
            }
        }

        public ActionResult Expense(int? Id)
        {
            var vm = new AddEditExpenseViewModel();
            string UserRole = GetCurrentUserRole();

            if (Id.HasValue)
            {
                var expObj = _repository.Expenses.FirstOrDefault(x => x.ExpenseID == Id);
                if (expObj != null)
                {
                    vm.AccountCode = expObj.ExpenseTypeID.ToString();
                    vm.AccountID = expObj.AccountID;
                    vm.Amount = expObj.Amount.Value;
                    vm.BankID = expObj.BankID;
                    vm.CheckDateOnlineDate = expObj.CheckDate.HasValue ? expObj.CheckDate.Value.ToString("dd/MM/yyyy") : null;
                    vm.CheckNumberOnlineReciptNumber = expObj.CheckNumberOrOnlineReciptNumber;
                    vm.ExpenseID = Id.Value;
                    vm.Particulars = expObj.Particulars;
                    vm.PaymentModeID = expObj.PaymentModeID;
                    vm.TransactionDate = expObj.PaymentDate.Value.ToString("dd/MM/yyyy");
                    vm.VoucherType = expObj.VoucherType;
                }
            }

            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountTypeID == 10).Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.ExpenseTypeList = _repository.ChartofAccounts.Where(x => x.AccountTypeID == 6).AsEnumerable().Select(x => new SelectListItem { Text = x.AccountName, Value = x.AccountCode.ToString() });
            vm.PaymentModeList = _repository.Codes.Where(x => x.CodeType == "Payment Method").AsEnumerable().Select(x => new SelectListItem { Text = x.CodeValue, Value = x.ID.ToString() });
            vm.BankList = _repository.Banks.Where(x => x.IsActive == true).AsEnumerable().Select(x => new SelectListItem { Text = x.BankName, Value = x.ID.ToString() });

            return View(vm);
        }

        [HttpPost]
        public JsonResult InsertExpanse(AddEditExpenseViewModel model)
        {
            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        DateTime transactionDate = DateTime.ParseExact(model.TransactionDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        if (model.PaymentMode == "Cash")
                        {
                            var obj = new Expens();
                            if (model.ExpenseID > 0)
                                obj = context.Expenses.FirstOrDefault(x => x.ExpenseID == model.ExpenseID);

                            obj.AccountID = model.AccountID;
                            obj.PaymentModeID = 5;  // model.PaymentModeID;
                            obj.Amount = model.Amount;
                            obj.PaymentDate = transactionDate;
                            obj.Particulars = model.Particulars;
                            obj.VoucherType = model.VoucherType;
                            obj.BankID = model.BankID;
                            obj.BranchID = model.BranchID;
                            obj.ExpenseTypeID = Convert.ToInt32(model.AccountCode);
                            obj.RoleID = GetCurrentUserRole();

                            if (model.ExpenseID == 0)
                            {
                                obj.CreatedDate = DateTime.Now;
                                obj.CreatedBy = User.Identity.Name;
                                context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                            }
                            else
                            {
                                obj.ModifyDate = DateTime.Now;
                                obj.ModifyBy = User.Identity.Name;
                                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                            }

                            context.SaveChanges();

                        }
                        else if (model.PaymentMode == "Bank")
                        {
                            DateTime checkDate = DateTime.ParseExact(model.CheckDateOnlineDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                            var obj = new Expens();
                            if (model.ExpenseID > 0)
                                obj = context.Expenses.FirstOrDefault(x => x.ExpenseID == model.ExpenseID);

                            obj.AccountID = model.AccountID;
                            obj.PaymentModeID = 6;      // model.PaymentModeID;
                            obj.Amount = model.Amount;
                            obj.CheckNumberOrOnlineReciptNumber = model.CheckNumberOnlineReciptNumber;
                            obj.CheckDate = checkDate;
                            obj.PaymentDate = transactionDate;
                            obj.Particulars = model.Particulars;
                            obj.VoucherType = model.VoucherType;
                            obj.BankID = model.BankID;
                            obj.BranchID = model.BranchID;
                            obj.ExpenseTypeID = Convert.ToInt32(model.AccountCode);
                            obj.CheckDate = checkDate;
                            obj.RoleID = GetCurrentUserRole();

                            if (model.ExpenseID == 0)
                            {
                                obj.CreatedDate = DateTime.Now;
                                obj.CreatedBy = User.Identity.Name;
                                context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                            }
                            else
                            {
                                obj.ModifyDate = DateTime.Now;
                                obj.ModifyBy = User.Identity.Name;
                                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                            }

                            context.SaveChanges();
                        }
                        dbContextTransaction.Commit();

                        string message = "Payment has been saved!";
                        return Json(new { status = "Success", message = "Error", detail = message, Result = "OK" }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteExpense(int? ExpenseID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeleteExpense(ExpenseID);
                        dbContextTransaction.Commit();
                        return Json(new { status = "Success", message = "Error", detail = "Expese has been Deleted!", Result = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
    }
}