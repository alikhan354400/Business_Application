using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models.EF;
using HassanAyoubTraders.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class PaymentsController : BaseController
    {
        // GET: Payment
        public ActionResult PaymentList()
        {
            var vm = new PaymentListViewModel();
            string UserRole = GetCurrentUserRole();

            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType != "ExpenseAccount").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });

            return View(vm);
        }

        public ActionResult Payment(int? Id)
        {
            var vm = new AddEditPaymentViewModel();
            string UserRole = GetCurrentUserRole();

            if (Id.HasValue)
            {
                var expObj = _repository.Payments.FirstOrDefault(x => x.ID == Id);
                if (expObj != null)
                {
                    vm.PaymentID = expObj.ID;
                    vm.PaymentHeaderID = expObj.PaymentHeaderID ?? 0;
                    vm.IsUpdate = true;
                    vm.AccountID = expObj.AccountID;
                    vm.OrderNumber = expObj.OrderNumber == null ? 0 : expObj.OrderNumber.Value;
                    vm.Amount = expObj.Amount.Value;
                    vm.BankID = expObj.BankID;
                    vm.CheckDateOnlineDate = expObj.CheckDate.HasValue ? expObj.CheckDate.Value.ToString("dd/MM/yyyy") : null;
                    vm.CheckNumberOnlineReciptNumber = expObj.CheckNumberOrOnlineReciptNumber;
                    vm.Particulars = expObj.Particulars;
                    vm.PaymentMode = expObj.PaymentMode;
                    vm.TransactionDate = expObj.TransactionDate.Value.ToString("dd/MM/yyyy");
                    vm.VoucherType = expObj.VoucherType;
                }
            }

            //vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType != "ExpenseAccount" && (o.IsCashAccount == false || o.IsCashAccount == null)).Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType != "ExpenseAccount").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.PaymentModeList = _repository.Codes.Where(x => x.CodeType == "Payment Method").AsEnumerable().Select(x => new SelectListItem { Text = x.CodeValue, Value = x.CodeValue.ToString() });
            vm.BankList = _repository.Banks.Where(x => x.IsActive == true).AsEnumerable().Select(x => new SelectListItem { Text = x.BankName, Value = x.ID.ToString() });

            return View(vm);
        }

        [HttpPost]
        public JsonResult InsertPayment(AddEditPaymentViewModel model)
        {
            var context = _repository;
            try
            {
                var pDate = model.TransactionDate == string.Empty ? (DateTime?)null : DateTime.ParseExact(model.TransactionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var pObj = new PaymentHeader();
                if (model.IsUpdate)
                    pObj = _repository.PaymentHeaders.FirstOrDefault(x => x.ID == model.PaymentHeaderID);

                pObj.EmployeeID = model.AccountID;
                pObj.ReceiptNo = model.OrderNumber.ToString();
                pObj.TotalAmount = model.Amount;
                pObj.Particulars = model.Particulars;
                pObj.VoucherType = "PV";
                pObj.PaymentDate = pDate;

                if (!model.IsUpdate)
                {
                    pObj.ID = -1;
                    pObj.CreatedBy = GetCurrentUserName();
                    pObj.CreatedDate = DateTime.Now;

                    _repository.Entry(pObj).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    pObj.ID = pObj.ID;
                    pObj.ModifyBy = GetCurrentUserName();
                    pObj.ModifyDate = DateTime.Now;

                    _repository.Entry(pObj).State = System.Data.Entity.EntityState.Modified;
                }

                if (_repository.SaveChanges() > 0)
                {
                    DateTime transactionDate = DateTime.ParseExact(model.TransactionDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    var obj = new Payment();

                    if (model.PaymentMode == "Cash")
                    {
                        if (model.PaymentID > 0)
                            obj = context.Payments.FirstOrDefault(x => x.ID == model.PaymentID);

                        if (model.PaymentID == 0)
                        {
                            var payId = context.InsertPayment(model.AccountID, model.PaymentMode, model.OrderNumber, model.Amount ?? 0, "", null, transactionDate, model.Particulars, model.VoucherType, 0, "PP", model.PaymentID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID);
                        }
                        else
                        {
                            var payId = context.UpdatePayment(obj.ID, model.AccountID, model.PaymentMode, model.OrderNumber, model.Amount ?? 0, "", null, transactionDate, model.Particulars, model.VoucherType, 0, "PP", obj.ID, GetCurrentUserName(), GetCurrentUserRole());
                        }

                    }
                    else if (model.PaymentMode == "Bank")
                    {
                        DateTime checkDate = DateTime.ParseExact(model.CheckDateOnlineDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        if (model.PaymentID > 0)
                            obj = context.Payments.FirstOrDefault(x => x.ID == model.PaymentID);

                        if (model.PaymentID == 0)
                        {
                            var payId = context.InsertPayment(model.AccountID, model.PaymentMode, model.OrderNumber, model.Amount ?? 0, model.CheckNumberOnlineReciptNumber, checkDate, transactionDate, model.Particulars, model.VoucherType, model.BankID, "PP", model.PaymentID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID);
                        }
                        else
                        {
                            var payId = context.UpdatePayment(obj.ID, model.AccountID, model.PaymentMode, model.OrderNumber, model.Amount ?? 0, model.CheckNumberOnlineReciptNumber, checkDate, transactionDate, model.Particulars, model.VoucherType, model.BankID, "PP", model.PaymentID, GetCurrentUserName(), GetCurrentUserRole());
                        }
                    }
                }

                string message = "Payment has been saved!";
                return Json(new { status = "Success", message = "Error", detail = message, Result = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //dbContextTransaction.Rollback();
                return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetPayments(int? AccountsID, string StartDate, string EndDate)
        {
            try
            {
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

                var list = _repository.BrowsePayments("PV", AccountsID, stdt, eddt).ToList();

                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");

                return PartialView("_PaymentList", list);
            }
            catch (Exception ex)
            {
                return PartialView("_PaymentList", null);
            }
        }

        [HttpPost]
        public JsonResult DeletePayment(int? PaymentId)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeleteSinglePaymentTransaction("PP", PaymentId);
                        dbContextTransaction.Commit();
                        return Json(new { status = "Success", message = "Error", detail = "Payment has been Deleted!", Result = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }




        // GET: Receipt
        public ActionResult ReceiptList()
        {
            var vm = new PaymentListViewModel();
            string UserRole = GetCurrentUserRole();

            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType != "ExpenseAccount").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });

            return View(vm);
        }

        public ActionResult Receipt(int? Id)
        {
            var vm = new AddEditPaymentViewModel();
            string UserRole = GetCurrentUserRole();

            if (Id.HasValue)
            {
                var expObj = _repository.Payments.FirstOrDefault(x => x.ID == Id);
                if (expObj != null)
                {
                    vm.PaymentID = expObj.ID;
                    vm.AccountID = expObj.AccountID;
                    vm.OrderNumber = expObj.OrderNumber.Value;
                    vm.Amount = expObj.Amount.Value;
                    vm.BankID = expObj.BankID;
                    vm.CheckDateOnlineDate = expObj.CheckDate.HasValue ? expObj.CheckDate.Value.ToString("dd/MM/yyyy") : null;
                    vm.CheckNumberOnlineReciptNumber = expObj.CheckNumberOrOnlineReciptNumber;
                    vm.Particulars = expObj.Particulars;
                    vm.PaymentMode = expObj.PaymentMode;
                    vm.TransactionDate = expObj.TransactionDate.Value.ToString("dd/MM/yyyy");
                    vm.VoucherType = expObj.VoucherType;
                }
            }

            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType != "ExpenseAccount").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.PaymentModeList = _repository.Codes.Where(x => x.CodeType == "Payment Method").AsEnumerable().Select(x => new SelectListItem { Text = x.CodeValue, Value = x.CodeValue.ToString() });
            vm.BankList = _repository.Banks.Where(x => x.IsActive == true).AsEnumerable().Select(x => new SelectListItem { Text = x.BankName, Value = x.ID.ToString() });

            return View(vm);
        }

        [HttpPost]
        public JsonResult InsertReceipt(AddEditPaymentViewModel model)
        {
            var context = _repository;
            //using (var context = new FKIndustryEntities())
            //{
            //    using (var dbContextTransaction = context.Database.BeginTransaction())
            //    {
            try
            {
                var pDate = model.TransactionDate == string.Empty ? (DateTime?)null : DateTime.ParseExact(model.TransactionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var pObj = new PaymentHeader();
                if (model.IsUpdate)
                    pObj = _repository.PaymentHeaders.FirstOrDefault(x => x.ID == model.PaymentHeaderID);

                pObj.EmployeeID = model.AccountID;
                pObj.ReceiptNo = model.OrderNumber.ToString();
                pObj.TotalAmount = model.Amount;
                pObj.Particulars = model.Particulars;
                pObj.VoucherType = "PV";
                pObj.PaymentDate = pDate;

                if (!model.IsUpdate)
                {
                    pObj.ID = -1;
                    pObj.CreatedBy = GetCurrentUserName();
                    pObj.CreatedDate = DateTime.Now;

                    _repository.Entry(pObj).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    pObj.ID = pObj.ID;
                    pObj.ModifyBy = GetCurrentUserName();
                    pObj.ModifyDate = DateTime.Now;

                    _repository.Entry(pObj).State = System.Data.Entity.EntityState.Modified;
                }

                if (_repository.SaveChanges() > 0)
                {
                    DateTime transactionDate = DateTime.ParseExact(model.TransactionDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    var obj = new Payment();

                    if (model.PaymentMode == "Cash")
                    {
                        if (model.PaymentID > 0)
                            obj = context.Payments.FirstOrDefault(x => x.ID == model.PaymentID);

                        if (model.PaymentID == 0)
                        {
                            var payId = context.InsertPayment(model.AccountID, model.PaymentMode, model.OrderNumber, model.Amount ?? 0, "", null, transactionDate, model.Particulars, model.VoucherType, 0, "RP", model.PaymentID, GetCurrentUserName(), GetCurrentUserRole(), 0);
                        }
                        else
                        {
                            var payId = context.UpdatePayment(obj.ID, model.AccountID, model.PaymentMode, model.OrderNumber, model.Amount ?? 0, "", null, transactionDate, model.Particulars, model.VoucherType, 0, "RP", obj.ID, GetCurrentUserName(), GetCurrentUserRole());
                        }

                    }
                    else if (model.PaymentMode == "Bank")
                    {
                        DateTime checkDate = DateTime.ParseExact(model.CheckDateOnlineDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        if (model.PaymentID > 0)
                            obj = context.Payments.FirstOrDefault(x => x.ID == model.PaymentID);

                        if (model.PaymentID == 0)
                        {
                            var payId = context.InsertPayment(model.AccountID, model.PaymentMode, model.OrderNumber, model.Amount ?? 0, model.CheckNumberOnlineReciptNumber, checkDate, transactionDate, model.Particulars, model.VoucherType, model.BankID, "RP", model.PaymentID, GetCurrentUserName(), GetCurrentUserRole(), 0);
                        }
                        else
                        {
                            var payId = context.UpdatePayment(obj.ID, model.AccountID, model.PaymentMode, model.OrderNumber, model.Amount ?? 0, model.CheckNumberOnlineReciptNumber, checkDate, transactionDate, model.Particulars, model.VoucherType, model.BankID, "RP", model.PaymentID, GetCurrentUserName(), GetCurrentUserRole());
                        }
                    }
                }
                //dbContextTransaction.Commit();
                string message = "Payment has been saved!";
                return Json(new { status = "Success", message = "Error", detail = message, Result = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //dbContextTransaction.Rollback();
                return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
            }
            //    }
            //}
        }

        [HttpPost]
        public ActionResult GetReceipts(int? AccountsID, string StartDate, string EndDate)
        {
            try
            {
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

                var list = _repository.BrowsePayments("RV", AccountsID, stdt, eddt).ToList();

                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");

                return PartialView("_ReceiptList", list);
            }
            catch (Exception ex)
            {
                return PartialView("_ReceiptList", null);
            }
        }

        [HttpPost]
        public JsonResult DeleteReceipt(int? PaymentId)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeleteSinglePaymentTransaction("RP", PaymentId);
                        dbContextTransaction.Commit();
                        return Json(new { status = "Success", message = "Error", detail = "Payment has been Deleted!", Result = "OK" }, JsonRequestBehavior.AllowGet);
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