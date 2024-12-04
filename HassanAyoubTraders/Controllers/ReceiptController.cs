using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
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
    public class ReceiptController : BaseController
    {
        //// GET: Receipt
        //public ActionResult ReceiptList()
        //{
        //    var vm = new ReceiptListViewModel();

        //    vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
        //    vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

        //    return View(vm);
        //}

        //[HttpPost]
        //public JsonResult GetPayments(string PaymentType, int? AccountsID, string StartDate, string EndDate)
        //{
        //    try
        //    {
        //        var vm = new PaymentListViewModel();

        //        if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
        //        {
        //            DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //            DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        //            Thread.Sleep(100);
        //            var list = _repository.BrowsePayments(PaymentType, AccountsID, stdt, eddt).Where(x => x.RoleId == GetCurrentUserRole()).ToList();

        //            vm.EndDate = eddt.ToString("dd/MM/yyyy");
        //            vm.StartDate = stdt.ToString("dd/MM/yyyy");

        //            return Json(new { Result = "ERROR", Data = list });
        //        }
        //        else
        //        {
        //            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
        //            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

        //            Thread.Sleep(100);
        //            var list = _repository.BrowsePayments(PaymentType, AccountsID, DateTime.Now.AddDays(-30), DateTime.Now).Where(x => x.RoleId == GetCurrentUserRole()).ToList();
        //            return Json(new { Result = "ERROR", Data = list });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message, Data = "" });
        //    }
        //}

        //public ActionResult Receipt()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public JsonResult InsertPayment(AddEditPaymentViewModel model)
        //{
        //    using (var context = new FKIndustryEntities())
        //    {
        //        using (var dbContextTransaction = context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                DateTime transactionDate = DateTime.ParseExact(model.TransactionDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //                if (model.PaymentMode == "Cash")
        //                {
        //                    var obj = new Payment();
        //                    obj.AccountID = model.AccountID;
        //                    obj.PaymentMode = model.PaymentMode;
        //                    obj.Amount = model.Amount;
        //                    obj.TransactionDate = transactionDate;
        //                    obj.Particulars = model.Particulars;
        //                    obj.VoucherType = model.VoucherType;
        //                    obj.BankID = model.BankID;
        //                    obj.CreatedDate = DateTime.Now;
        //                    obj.CreatedBy = GetCurrentUserName();
        //                    obj.RoleId = GetCurrentUserRole();
        //                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Added;
        //                    _repository.SaveChanges();

        //                    if (model.VoucherType == "Receipt Voucher")
        //                    {
        //                        //---------- Insert 1st Entry for (Debit)	"Cash"
        //                        var transID = InsertTransactionEntry("1080", transactionDate, Convert.ToDecimal(obj.Amount), 0, GetAccountTypeByAccountID(model.AccountID) + " Payment by Cash", obj.ID, GetAccountTypeByAccountID(model.AccountID) + " Receive");

        //                        //---------- Insert 2nd Entry for (Credit) "Accounts Receivable"		   
        //                        transID = InsertTransactionEntry("1210", transactionDate, 0, Convert.ToDecimal(obj.Amount), GetAccountTypeByAccountID(model.AccountID) + " Payment by Cash", obj.ID, GetAccountTypeByAccountID(model.AccountID) + " Receive");
        //                    }
        //                }
        //                else if (model.PaymentMode == "Bank")
        //                {
        //                    DateTime checkDate = DateTime.ParseExact(model.CheckDateOnlineDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        //                    var obj = new Payment();
        //                    obj.AccountID = model.AccountID;
        //                    obj.PaymentMode = model.PaymentMode;
        //                    obj.Amount = model.Amount;
        //                    obj.CheckNumberOrOnlineReciptNumber = model.CheckNumberOnlineReciptNumber;
        //                    obj.CheckDate = checkDate;
        //                    obj.TransactionDate = transactionDate;
        //                    obj.Particulars = model.Particulars;
        //                    obj.VoucherType = model.VoucherType;
        //                    obj.BankID = model.BankID;
        //                    obj.CreatedDate = DateTime.Now;
        //                    obj.CreatedBy = GetCurrentUserName();
        //                    obj.RoleId = GetCurrentUserRole();
        //                    obj.CheckDate = checkDate;
        //                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Added;
        //                    _repository.SaveChanges();

        //                    if (model.VoucherType == "Receipt Voucher")
        //                    {
        //                        //---------- Insert 1st Entry for (Debit)	"Bank"
        //                        var transID = InsertTransactionEntry("1090", transactionDate, Convert.ToDecimal(obj.Amount), 0, GetAccountTypeByAccountID(model.AccountID) + " Payment by Check/Online (" + model.CheckNumberOnlineReciptNumber + ")", obj.ID, GetAccountTypeByAccountID(model.AccountID) + " Receive");

        //                        //---------- Insert 2nd Entry for (Credit) "Accounts Receivable"		   
        //                        transID = InsertTransactionEntry("1210", transactionDate, 0, Convert.ToDecimal(obj.Amount), GetAccountTypeByAccountID(model.AccountID) + " Payment by Check/Online (" + model.CheckNumberOnlineReciptNumber + ")", obj.ID, GetAccountTypeByAccountID(model.AccountID) + " Receive");
        //                    }
        //                }
        //                dbContextTransaction.Commit();

        //                string message = "Payment has been saved!";
        //                return Json(new { status = "Success", message = "Error", detail = message, Result = "OK" }, JsonRequestBehavior.AllowGet);

        //            }
        //            catch (Exception ex)
        //            {
        //                dbContextTransaction.Rollback();
        //                return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //}

        //[HttpPost]
        //public JsonResult DeletePayment(int? PaymentId)
        //{
        //    var status = string.Empty;

        //    using (var context = new FKIndustryEntities())
        //    {
        //        using (var dbContextTransaction = context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                context.DeletePayment(PaymentId);
        //                dbContextTransaction.Commit();
        //                return Json(new { status = "Success", message = "Error", detail = "Payment has been removed!", Result = "OK" }, JsonRequestBehavior.AllowGet);
        //            }
        //            catch (Exception ex)
        //            {
        //                dbContextTransaction.Rollback();
        //                return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //}

    }
}