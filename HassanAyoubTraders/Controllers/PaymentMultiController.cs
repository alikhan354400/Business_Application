using HassanAyoubTraders.Models.EF;
using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class PaymentMultiController : BaseController
    {
        public ActionResult PaymentList(string StartDate, string EndDate)
        {
            var vm = new PaymentMultiViewModel();
            string UserRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.PaymentList = _repository.BrowsePaymentHeader(stdt, eddt).Where(x => x.VoucharType == "PV").ToList();

            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.PaymentList = _repository.BrowsePaymentHeader(DateTime.Now.AddDays(-30), DateTime.Now).Where(x => x.VoucharType == "PV").ToList();
            }

            return View(vm);
        }

        public ActionResult AddEditPayment(int? id)
        {
            string UserRole = GetCurrentUserRole();
            var vm = new PaymentMultiViewModel();
            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Supplier").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() }).ToList();
            vm.EmployeeList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountTypeID == 3).Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() }).ToList();
            vm.ExtraItemList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Supplier").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() }).ToList();
            var mode = new List<SelectListItem>()
            {
             new SelectListItem() {  Text ="Cash",Value ="Cash",Selected =true },
             new SelectListItem() {  Text ="Check",Value ="Check" }
            };
            vm.ModeList = mode;

            if (id.HasValue)
            {
                var obj = _repository.PaymentHeaders.FirstOrDefault(o => o.ID == id);
                if (obj != null)
                {
                    vm.ID = obj.ID;
                    vm.EmployeeID = obj.EmployeeID ?? 0;
                    vm.ReceiptNo = obj.ReceiptNo;
                    vm.Amount = obj.TotalAmount ?? 0;
                    vm.VoucherType = obj.VoucherType;
                    vm.Particulars = obj.Particulars;
                    vm.PaymentDate = obj.PaymentDate.HasValue ? obj.PaymentDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                    vm.IsUpdate = true;

                    vm.EditPaymentDetail = _repository.GetPaymentsByPaymentHeaderId(obj.ID).Select(x => new AddEditPaymentDetailViewModel()
                    {
                        ID = x.ID,
                        AccountID = x.AccountID,
                        PaymentMode = x.PaymentMode,
                        Amount = x.Amount,
                        CheckNumberOrOnlineReciptNumber = x.CheckNumberOrOnlineReciptNumber,
                        CheckDate = x.CheckDate == null ? "" : x.CheckDate.Value.ToString("dd/MM/yyyy"),
                        TransactionDate = x.TransactionDate == null ? "" : x.TransactionDate.Value.ToString("dd/MM/yyyy"),
                        Particulars = x.Particulars,
                        VoucherType = x.VoucherType,
                        RecordType = x.RecordType,
                        RecordId = x.RecordId,
                        CreatedDate = x.CreatedDate,
                        CreatedBy = x.CreatedBy,
                        ModifyDate = x.ModifyDate,
                        ModifyBy = x.ModifyBy,
                        BankID = x.BankID,
                        BankName = x.BankName,
                        RoleId = x.RoleId,
                        PaymentHeaderID = x.PaymentHeaderID,
                        OrderNumber = x.OrderNumber,
                        FullName = x.FullName,
                    }).ToList();
                }
            }
            else
            {
                vm.ID = 0;
                vm.EmployeeID = 00;
                vm.ReceiptNo = _repository.BrowseNewNumber("PP").FirstOrDefault().Value.ToString();
                vm.Amount = 0;
                vm.VoucherType = "";
                vm.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.IsUpdate = false;
            }
            return View(vm);
        }

        public JsonResult SavePayment(PaymentMultiViewModel vm)
        {
            //using (var context = new NadeemAndCoEntities())
            //{
            //    using (var dbContextTransaction = context.Database.BeginTransaction())
            //    {
            try
            {
                var pDate = vm.PaymentDate == string.Empty ? (DateTime?)null : DateTime.ParseExact(vm.PaymentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var pObj = new PaymentHeader();
                if (vm.IsUpdate)
                    pObj = _repository.PaymentHeaders.FirstOrDefault(x => x.ID == vm.PaymentHeaderID);

                pObj.EmployeeID = vm.EmployeeID;
                pObj.ReceiptNo = vm.ReceiptNo;
                pObj.TotalAmount = vm.Amount;
                pObj.Particulars = vm.Particulars;
                pObj.VoucherType = "PV";
                pObj.PaymentDate = pDate;

                if (!vm.IsUpdate) // Add
                {
                    #region Add 

                    pObj.ID = -1;
                    pObj.CreatedBy = GetCurrentUserName();
                    pObj.CreatedDate = DateTime.Now;

                    _repository.Entry(pObj).State = System.Data.Entity.EntityState.Added;
                    if (_repository.SaveChanges() > 0)
                    {
                        var poId = pObj.ID;
                        foreach (var item in vm.PaymentDetail)
                        {
                            if (item.ID == 0)
                            {
                                if (item.PaymentMode == "Cash")
                                {
                                    var payId = _repository.InsertPayment(item.AccountID, item.PaymentMode, 0, item.Amount ?? 0, "", null, pDate, item.Particulars, pObj.VoucherType, 0, "PP", item.ID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID).ToList();
                                }
                                else if (item.PaymentMode == "Check")
                                {
                                    var payId = _repository.InsertPayment(item.AccountID, item.PaymentMode, 0, item.Amount ?? 0, "", pDate, pDate, item.Particulars, pObj.VoucherType, item.BankID, "PP", item.ID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID).ToList();
                                }
                            }
                        }
                        //dbContextTransaction.Commit();
                        //context.AddUpdatePurchaseOrderTransactions(poId, pObj.PODate.Value, "I");
                    }

                    string message = "Payment has been saved!";
                    return Json(new { status = "Success", message = "Error", detail = message, Result = "OK", PaymentHeaderID = pObj.ID }, JsonRequestBehavior.AllowGet);

                    #endregion
                }
                else if (vm.IsUpdate == true && vm.ID > 0) // Update
                {
                    #region Update

                    pObj.ID = vm.ID;
                    pObj.ModifyBy = GetCurrentUserName();
                    pObj.ModifyDate = DateTime.Now;

                    _repository.Entry(pObj).State = System.Data.Entity.EntityState.Modified;
                    if (_repository.SaveChanges() > 0)
                    {
                        var obj = new Payment();
                        foreach (var item in vm.PaymentDetail)
                        {
                            if (item.PaymentMode == "Cash")
                            {
                                if (item.ID > 0)
                                    obj = _repository.Payments.FirstOrDefault(x => x.ID == item.ID);

                                if (item.ID == 0)
                                {
                                    var payId = _repository.InsertPayment(item.AccountID, item.PaymentMode, 0, item.Amount ?? 0, "", null, pDate, item.Particulars, pObj.VoucherType, 0, "PP", item.ID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID);
                                }
                                else
                                {
                                    var payId = _repository.UpdatePayment(obj.ID, item.AccountID, item.PaymentMode, 0, item.Amount ?? 0, "", null, pDate, item.Particulars, pObj.VoucherType, 0, "PP", obj.ID, GetCurrentUserName(), GetCurrentUserRole());
                                }
                            }
                            else if (item.PaymentMode == "Check")
                            {
                                if (item.ID > 0)
                                    obj = _repository.Payments.FirstOrDefault(x => x.ID == item.ID);

                                if (item.ID == 0)
                                {
                                    var payId = _repository.InsertPayment(item.AccountID, item.PaymentMode, 0, item.Amount ?? 0, "", pDate, pDate, item.Particulars, item.VoucherType, item.BankID, "PP", item.ID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID);
                                }
                                else
                                {
                                    var payId = _repository.UpdatePayment(obj.ID, item.AccountID, item.PaymentMode, 0, item.Amount ?? 0, "", pDate, pDate, item.Particulars, item.VoucherType, item.BankID, "PP", item.ID, GetCurrentUserName(), GetCurrentUserRole());
                                }
                            }
                        }
                    }

                    string successMsg = "Payment has been updated!";
                    return Json(new { status = "Success", message = "Error", detail = successMsg, Result = "OK", PaymentHeaderID = pObj.ID }, JsonRequestBehavior.AllowGet);

                    #endregion
                }

                string msg = "Server not responed!";
                return Json(new { status = "Success", message = "Error", detail = msg, Result = "OK", POID = 0 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //dbContextTransaction.Rollback();
                return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
            }
            //}
            //}
        }

        public JsonResult DeletePaymentDetail(int HeaderID, int ItemID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeletePaymentTransaction("PP", ItemID);
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

        [HttpPost]
        public JsonResult DeletePaymentHeader(int? ID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var obj = context.PaymentHeaders.FirstOrDefault(o => o.ID == ID);
                        if (obj != null)
                        {
                            context.PaymentHeaders.Remove(obj);
                            var res = context.SaveChanges();
                            if (res > 0)
                            {
                                var objItem = context.Payments.Where(o => o.PaymentHeaderID == ID).ToList();
                                foreach (var item in objItem)
                                {
                                    context.DeletePaymentTransaction("PP", item.ID);
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                        return Json(new { status = "Success", message = "Error", detail = "Order has been Deleted!", Result = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }




        public ActionResult ReceiptList(string StartDate, string EndDate)
        {
            var vm = new PaymentMultiViewModel();
            string UserRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.PaymentList = _repository.BrowsePaymentHeader(stdt, eddt).Where(x => x.VoucharType == "RV").ToList();

            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.PaymentList = _repository.BrowsePaymentHeader(DateTime.Now.AddDays(-30), DateTime.Now).Where(x => x.VoucharType == "RV").ToList();
            }

            return View(vm);
        }

        public ActionResult AddEditReceipt(int? id)
        {
            string UserRole = GetCurrentUserRole();
            var vm = new PaymentMultiViewModel();
            vm.AccountList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Customer").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() }).ToList();
            vm.EmployeeList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountTypeID == 3).Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() }).ToList();
            vm.ExtraItemList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Customer").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() }).ToList();
            vm.BankList = _repository.Banks.Where(x => x.IsActive == true).AsEnumerable().Select(x => new SelectListItem { Text = x.BankName, Value = x.ID.ToString() });
            var mode = new List<SelectListItem>()
            {
             new SelectListItem() {  Text ="Cash",Value ="Cash",Selected =true },
             new SelectListItem() {  Text ="Check",Value ="Check" }
            };
            vm.ModeList = mode;

            if (id.HasValue)
            {
                var obj = _repository.PaymentHeaders.FirstOrDefault(o => o.ID == id);
                if (obj != null)
                {
                    vm.ID = obj.ID;
                    vm.EmployeeID = obj.EmployeeID ?? 0;
                    vm.ReceiptNo = obj.ReceiptNo;
                    vm.Amount = obj.TotalAmount ?? 0;
                    vm.VoucherType = obj.VoucherType;
                    vm.Particulars = obj.Particulars;
                    vm.PaymentDate = obj.PaymentDate.HasValue ? obj.PaymentDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                    vm.IsUpdate = true;

                    vm.EditPaymentDetail = _repository.GetPaymentsByPaymentHeaderId(obj.ID).Select(x => new AddEditPaymentDetailViewModel()
                    {
                        ID = x.ID,
                        AccountID = x.AccountID,
                        PaymentMode = x.PaymentMode,
                        Amount = x.Amount,
                        CheckNumberOrOnlineReciptNumber = x.CheckNumberOrOnlineReciptNumber,
                        CheckDate = x.CheckDate == null ? "" : x.CheckDate.Value.ToString("dd/MM/yyyy"),
                        TransactionDate = x.TransactionDate == null ? "" : x.TransactionDate.Value.ToString("dd/MM/yyyy"),
                        Particulars = x.Particulars,
                        VoucherType = x.VoucherType,
                        RecordType = x.RecordType,
                        RecordId = x.RecordId,
                        CreatedDate = x.CreatedDate,
                        CreatedBy = x.CreatedBy,
                        ModifyDate = x.ModifyDate,
                        ModifyBy = x.ModifyBy,
                        BankID = x.BankID,
                        BankName = x.BankName,
                        RoleId = x.RoleId,
                        PaymentHeaderID = x.PaymentHeaderID,
                        OrderNumber = x.OrderNumber,
                        FullName = x.FullName,
                    }).ToList();
                }
            }
            else
            {
                vm.ID = 0;
                vm.EmployeeID = 00;
                vm.ReceiptNo = _repository.BrowseNewNumber("RP").FirstOrDefault().Value.ToString();
                vm.Amount = 0;
                vm.VoucherType = "";
                vm.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.IsUpdate = false;
            }
            return View(vm);
        }

        public JsonResult SaveReceipt(PaymentMultiViewModel vm)
        {
            //using (var context = new NadeemAndCoEntities())
            //{
            //    using (var dbContextTransaction = context.Database.BeginTransaction())
            //    {
            try
            {
                var pDate = vm.PaymentDate == string.Empty ? (DateTime?)null : DateTime.ParseExact(vm.PaymentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var pObj = new PaymentHeader();
                if (vm.IsUpdate)
                    pObj = _repository.PaymentHeaders.FirstOrDefault(x => x.ID == vm.ID);

                pObj.EmployeeID = vm.EmployeeID;
                pObj.ReceiptNo = vm.ReceiptNo;
                pObj.TotalAmount = vm.Amount;
                pObj.Particulars = vm.Particulars;
                pObj.VoucherType = "RV";
                pObj.PaymentDate = pDate;

                if (!vm.IsUpdate) // Add
                {
                    #region Add 

                    pObj.ID = -1;
                    pObj.CreatedBy = GetCurrentUserName();
                    pObj.CreatedDate = DateTime.Now;

                    _repository.Entry(pObj).State = System.Data.Entity.EntityState.Added;
                    if (_repository.SaveChanges() > 0)
                    {
                        var poId = pObj.ID;
                        foreach (var item in vm.PaymentDetail)
                        {
                            if (item.ID == 0)
                            {
                                var chkDate = string.IsNullOrEmpty(item.CheckDate) ? (DateTime?)null : DateTime.ParseExact(item.CheckDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                if (item.PaymentMode == "Cash")
                                {
                                    var payId = _repository.InsertPayment(item.AccountID, item.PaymentMode, item.OrderNumber, item.Amount ?? 0, item.CheckNumberOrOnlineReciptNumber, chkDate, pDate, item.Particulars, pObj.VoucherType, 0, "RP", item.ID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID).ToList();
                                }
                                else if (item.PaymentMode == "Check")
                                {
                                    var payId = _repository.InsertPayment(item.AccountID, item.PaymentMode, item.OrderNumber, item.Amount ?? 0, item.CheckNumberOrOnlineReciptNumber, chkDate, pDate, item.Particulars, pObj.VoucherType, item.BankID, "RP", item.ID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID).ToList();
                                }
                            }
                        }
                        //dbContextTransaction.Commit();
                        //context.AddUpdatePurchaseOrderTransactions(poId, pObj.PODate.Value, "I");
                    }

                    string message = "Payment has been saved!";
                    return Json(new { status = "Success", message = "Error", detail = message, Result = "OK", PaymentHeaderID = pObj.ID }, JsonRequestBehavior.AllowGet);

                    #endregion
                }
                else if (vm.IsUpdate == true && vm.ID > 0) // Update
                {
                    #region Update

                    pObj.ID = vm.ID;
                    pObj.ModifyBy = GetCurrentUserName();
                    pObj.ModifyDate = DateTime.Now;

                    _repository.Entry(pObj).State = System.Data.Entity.EntityState.Modified;
                    if (_repository.SaveChanges() > 0)
                    {
                        var obj = new Payment();
                        foreach (var item in vm.PaymentDetail)
                        {
                            var chkDate = string.IsNullOrEmpty(item.CheckDate) ? (DateTime?)null : DateTime.ParseExact(item.CheckDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            if (item.PaymentMode == "Cash")
                            {
                                if (item.ID > 0)
                                    obj = _repository.Payments.FirstOrDefault(x => x.ID == item.ID);

                                if (item.ID == 0)
                                {
                                    var payId = _repository.InsertPayment(item.AccountID, item.PaymentMode, item.OrderNumber, item.Amount ?? 0, item.CheckNumberOrOnlineReciptNumber, chkDate, pDate, item.Particulars, pObj.VoucherType, item.BankID, "RP", item.ID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID);
                                }
                                else
                                {
                                    var payId = _repository.UpdatePayment(obj.ID, item.AccountID, item.PaymentMode, item.OrderNumber, item.Amount ?? 0, item.CheckNumberOrOnlineReciptNumber, chkDate, pDate, item.Particulars, pObj.VoucherType, item.BankID, "RP", obj.ID, GetCurrentUserName(), GetCurrentUserRole());
                                }
                            }
                            else if (item.PaymentMode == "Check")
                            {
                                if (item.ID > 0)
                                    obj = _repository.Payments.FirstOrDefault(x => x.ID == item.ID);

                                if (item.ID == 0)
                                {
                                    var payId = _repository.InsertPayment(item.AccountID, item.PaymentMode, item.OrderNumber, item.Amount ?? 0, item.CheckNumberOrOnlineReciptNumber, chkDate, pDate, item.Particulars, item.VoucherType, item.BankID, "RP", item.ID, GetCurrentUserName(), GetCurrentUserRole(), pObj.ID);
                                }
                                else
                                {
                                    var payId = _repository.UpdatePayment(obj.ID, item.AccountID, item.PaymentMode, item.OrderNumber, item.Amount ?? 0, item.CheckNumberOrOnlineReciptNumber, chkDate, pDate, item.Particulars, item.VoucherType, item.BankID, "RP", item.ID, GetCurrentUserName(), GetCurrentUserRole());
                                }
                            }
                        }
                    }

                    string successMsg = "Payment has been updated!";
                    return Json(new { status = "Success", message = "Error", detail = successMsg, Result = "OK", PaymentHeaderID = pObj.ID }, JsonRequestBehavior.AllowGet);

                    #endregion
                }

                string msg = "Server not responed!";
                return Json(new { status = "Success", message = "Error", detail = msg, Result = "OK", POID = 0 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //dbContextTransaction.Rollback();
                return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
            }
            //}
            //}
        }

        public JsonResult DeleteReceiptDetail(int HeaderID, int ItemID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeletePaymentTransaction("RP", ItemID);
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

        [HttpPost]
        public JsonResult DeleteReceiptHeader(int? ID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var obj = context.PaymentHeaders.FirstOrDefault(o => o.ID == ID);
                        if (obj != null)
                        {
                            context.PaymentHeaders.Remove(obj);
                            var res = context.SaveChanges();
                            if (res > 0)
                            {
                                var objItem = context.Payments.Where(o => o.PaymentHeaderID == ID).ToList();
                                foreach (var item in objItem)
                                {
                                    context.DeletePaymentTransaction("RP", item.ID);
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                        return Json(new { status = "Success", message = "Error", detail = "Order has been Deleted!", Result = "OK" }, JsonRequestBehavior.AllowGet);
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