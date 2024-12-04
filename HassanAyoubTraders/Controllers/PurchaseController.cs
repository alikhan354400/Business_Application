using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models;
using HassanAyoubTraders.Models.EF;
using HassanAyoubTraders.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class PurchaseController : BaseController
    {
        public ActionResult PurchaseList(string StartDate, string EndDate)
        {
            var vm = new PurchaseOrderViewModel();
            string UserRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
                vm.PurchaseOrderList = _repository.BrowsePurchaseOrders(stdt, eddt).ToList();

            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.PurchaseOrderList = _repository.BrowsePurchaseOrders(DateTime.Now.AddDays(-30), DateTime.Now).ToList();
            }

            return View(vm);
        }

        public ActionResult PurchaseOrder(int? id)
        {
            string UserRole = GetCurrentUserRole();

            var vm = new PurchaseOrderViewModel();
            vm.IsUpdate = false;
            vm.VendorList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Supplier").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.ItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.ExtraItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.EntryTypeList = _repository.EntryTypes.AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.Type), Value = x.ID.ToString() }).ToList();
            vm.StatusList = GetStatus().AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x), Value = x }).ToList();
            vm.BookerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.SupplierList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });

            if (id.HasValue)
            {
                var obj = _repository.PurchaseOrders.FirstOrDefault(o => o.PurchaseOrderID == id);
                if (obj != null)
                {
                    vm.PurchaseOrderID = obj.PurchaseOrderID;
                    vm.IsTaxInvoice = obj.IsTaxInvoice ?? false;
                    vm.PONumber = obj.PONumber ?? 0;
                    vm.RecordType = obj.RecordType;
                    vm.EntryTypeID = obj.EntryTypeID ?? 0;
                    vm.VendorID = obj.VendorID.Value;
                    vm.PoDate = obj.PODate.HasValue ? obj.PODate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                    vm.InvoiceNo = obj.InvoiceNo;
                    vm.Notes = obj.Notes;
                    vm.SubTotal = obj.SubTotal;
                    vm.Expense = obj.Expense;
                    vm.Discount = obj.Discount;
                    vm.SalesTax = obj.SalesTax;
                    vm.PaidAmount = obj.PaidAmount;
                    vm.TotalAmount = obj.TotalAmount;
                    vm.IsUpdate = true;
                    vm.Status = obj.Status ?? "";
                    vm.BookerID = obj.BookerID ?? 0;
                    vm.SupplierID = obj.SupplierID ?? 0;

                    vm.EditPurchaseOrderDetail = _repository.GetPurhcaseOrderItemById(id).ToList();
                }
            }
            else
            {
                vm.EntryTypeID = 2;
                vm.IsTaxInvoice = false;
                vm.PONumber = _repository.BrowseNewNumber("PO").FirstOrDefault().Value;
                vm.EntryTypeID = 2;
                vm.PoDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.VendorID = 0;
                vm.InvoiceNo = "";
                vm.Notes = "";
                vm.SubTotal = 0;
                vm.Expense = 0;
                vm.Discount = 0;
                vm.SalesTax = 0;
                vm.PaidAmount = 0;
                vm.TotalAmount = 0;
                vm.IsUpdate = false;
                vm.Status = "";
                vm.BookerID = 0;
                vm.SupplierID = 0;
            }

            return View(vm);
        }

        public JsonResult SavePurchaseOrder(PurchaseOrderViewModel vm)
        {
            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var vObj = context.BrowseAccount().Where(x => x.ID == vm.VendorID).FirstOrDefault();
                        var pObj = new PurchaseOrder();
                        if (vm.IsUpdate)
                            pObj = context.PurchaseOrders.FirstOrDefault(x => x.PurchaseOrderID == vm.PurchaseOrderID);

                        pObj.CompanyId = vObj.CompanyId;
                        pObj.IsTaxInvoice = vm.IsTaxInvoice;
                        pObj.PONumber = vm.PONumber;
                        pObj.RecordType = "PurchaseOrder";
                        pObj.EntryTypeID = vm.EntryTypeID;
                        pObj.VendorID = vm.VendorID;
                        pObj.PODate = vm.PoDate == string.Empty ? (DateTime?)null : DateTime.ParseExact(vm.PoDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        pObj.InvoiceNo = vm.InvoiceNo;
                        pObj.Notes = vm.Notes;
                        pObj.SubTotal = vm.SubTotal;
                        pObj.Expense = vm.Expense;
                        pObj.Discount = vm.Discount;
                        pObj.SalesTax = vm.SalesTax;
                        pObj.PaidAmount = vm.PaidAmount;
                        pObj.TotalAmount = vm.TotalAmount;
                        pObj.RoleID = GetCurrentUserRole();
                        pObj.Status = "Completed";
                        pObj.BookerID = vm.BookerID;
                        pObj.SupplierID = vm.SupplierID;

                        if (!vm.IsUpdate) // Add
                        {
                            #region Add 

                            pObj.PurchaseOrderID = -1;
                            pObj.CreatedBy = GetCurrentUserName();
                            pObj.CreatedDate = DateTime.Now;

                            context.Entry(pObj).State = System.Data.Entity.EntityState.Added;
                            if (context.SaveChanges() > 0)
                            {
                                var poId = pObj.PurchaseOrderID;
                                foreach (var item in vm.PurchaseOrderDetail)
                                {
                                    if (item.SubItemID.HasValue)
                                    {
                                        var obj = new PurchaseOrderLine();
                                        obj.PurchaseOrderID = poId;
                                        obj.SubItemID = item.SubItemID;
                                        obj.Quantity = item.Quantity;
                                        obj.PurchasePrice = item.PurchasePrice;
                                        obj.SalePrice = item.SalePrice;
                                        obj.T_O = item.T_O;
                                        obj.Discount = item.Discount;
                                        obj.SalesTax = item.SalesTax;
                                        obj.SubTotal = item.SubTotal;
                                        obj.TotalAmount = item.TotalAmount;
                                        obj.CreatedDate = DateTime.Now;
                                        obj.CreatedBy = GetCurrentUserName();

                                        context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                                        context.SaveChanges();
                                    }
                                }
                                dbContextTransaction.Commit();
                                context.AddUpdatePurchaseOrderTransactions(poId, pObj.PODate.Value, "I");

                                //---------------------------- Cast Type Inset Payment Entry --------------------------------------
                                //if (pObj.PaidAmount > 0) // Cash
                                //{
                                //    var ph = new PaymentHeader();
                                //    ph.EmployeeID = vm.BookerID;
                                //    ph.ReceiptNo = _repository.BrowseNewNumber("PP").FirstOrDefault().Value.ToString();
                                //    ph.TotalAmount = pObj.PaidAmount;
                                //    ph.Particulars = pObj.Notes;
                                //    ph.VoucherType = "PV";
                                //    ph.PaymentDate = pObj.PODate;
                                //    ph.CreatedBy = GetCurrentUserName();
                                //    ph.CreatedDate = DateTime.Now;
                                //    context.Entry(ph).State = System.Data.Entity.EntityState.Added;
                                //    if (context.SaveChanges() > 0)
                                //    {
                                //        var payId = context.InsertPayment(pObj.VendorID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.PODate, "Cash Payment against Order # " + pObj.PurchaseOrderID, "PP", 0, "PO", pObj.PurchaseOrderID, GetCurrentUserName(), GetCurrentUserRole(), ph.ID);
                                //    }
                                //}
                                //else
                                //{
                                //    context.DeleteCashPaymentEntry(pObj.PurchaseOrderID, "PO");
                                //}
                            }

                            string message = "Order has been saved!";
                            return Json(new { status = "Success", message = "Error", detail = message, Result = "OK", POID = pObj.PurchaseOrderID }, JsonRequestBehavior.AllowGet);

                            #endregion
                        }
                        else if (vm.IsUpdate == true && vm.PurchaseOrderID > 0) // Update
                        {
                            #region Update

                            pObj.PurchaseOrderID = vm.PurchaseOrderID;
                            pObj.ModifiedBy = GetCurrentUserName();
                            pObj.ModifiedDate = DateTime.Now;

                            context.Entry(pObj).State = System.Data.Entity.EntityState.Modified;
                            if (context.SaveChanges() > 0)
                            {
                                foreach (var item in vm.PurchaseOrderDetail)
                                {
                                    if (item.SubItemID.HasValue)
                                    {
                                        var obj = new PurchaseOrderLine();
                                        if (item.PurchaseOrderLineID > 0)
                                            obj = context.PurchaseOrderLines.FirstOrDefault(o => o.PurchaseOrderLineID == item.PurchaseOrderLineID);

                                        obj.PurchaseOrderID = pObj.PurchaseOrderID;
                                        obj.SubItemID = item.SubItemID;
                                        obj.Quantity = item.Quantity;
                                        obj.PurchasePrice = item.PurchasePrice;
                                        obj.SalePrice = item.SalePrice;
                                        obj.T_O = item.T_O;
                                        obj.Discount = item.Discount;
                                        obj.SalesTax = item.SalesTax;
                                        obj.SubTotal = item.SubTotal;
                                        obj.TotalAmount = item.TotalAmount;

                                        if (item.PurchaseOrderLineID > 0)
                                        {
                                            obj.ModifiedDate = DateTime.Now;
                                            obj.ModifiedBy = GetCurrentUserName();
                                            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                                        }
                                        else
                                        {
                                            obj.CreatedDate = DateTime.Now;
                                            obj.CreatedBy = GetCurrentUserName();
                                            context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                                        }
                                        context.SaveChanges();
                                    }
                                }

                                dbContextTransaction.Commit();
                                context.AddUpdatePurchaseOrderTransactions(pObj.PurchaseOrderID, pObj.PODate.Value, "U");

                                //---------------------------- Cast Type Inset Payment Entry --------------------------------------
                                //if (pObj.PaidAmount > 0) // Cash
                                //{
                                //    var isUpdate = false;
                                //    var ph = new PaymentHeader();
                                //    var pay = context.Payments.FirstOrDefault(x => x.RecordType == "PO" && x.RecordId == pObj.PurchaseOrderID);
                                //    if (pay != null)
                                //    {
                                //        var phid = pay.PaymentHeaderID;
                                //        ph = context.PaymentHeaders.FirstOrDefault(x => x.ID == pay.PaymentHeaderID);
                                //        if (ph == null)
                                //            ph = new PaymentHeader();
                                //        else
                                //            isUpdate = true;

                                //    }
                                //    ph.EmployeeID = vm.BookerID;
                                //    ph.ReceiptNo = _repository.BrowseNewNumber("PP").FirstOrDefault().Value.ToString();
                                //    ph.TotalAmount = pObj.PaidAmount;
                                //    ph.Particulars = pObj.Notes;
                                //    ph.VoucherType = "PV";
                                //    ph.PaymentDate = pObj.PODate;
                                //    ph.CreatedBy = GetCurrentUserName();
                                //    ph.CreatedDate = DateTime.Now;
                                //    if (isUpdate)
                                //        context.Entry(ph).State = System.Data.Entity.EntityState.Modified;
                                //    else
                                //        context.Entry(ph).State = System.Data.Entity.EntityState.Added;
                                //    if (context.SaveChanges() > 0)
                                //    {
                                //        if (pay == null)
                                //        {
                                //            var payId = context.InsertPayment(pObj.VendorID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.PODate, "Cash Payment against Order # " + pObj.PurchaseOrderID, "PP", 0, "PO", pObj.PurchaseOrderID, GetCurrentUserName(), GetCurrentUserRole(), ph.ID);
                                //        }
                                //        else
                                //        {
                                //            var payId = context.UpdatePayment(0, pObj.VendorID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.PODate, "Cash Payment against Order # " + pObj.PurchaseOrderID, "PP", 0, "PO", pObj.PurchaseOrderID, GetCurrentUserName(), GetCurrentUserRole());
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    context.DeleteCashPaymentEntry(pObj.PurchaseOrderID, "PO");
                                //}
                            }

                            string successMsg = "Order has been updated!";
                            return Json(new { status = "Success", message = "Error", detail = successMsg, Result = "OK", POID = pObj.PurchaseOrderID }, JsonRequestBehavior.AllowGet);

                            #endregion
                        }

                        string msg = "Server not responed!";
                        return Json(new { status = "Success", message = "Error", detail = msg, Result = "OK", POID = 0 }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL", POID = 0 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        public JsonResult DeletePOItem(int POLineID, int POID)
        {
            string message = string.Empty;
            try
            {
                message = _repository.DeletePOLine(POLineID, POID).FirstOrDefault();
                return Json(new { status = "Success", message = "Item has been deleted", Result = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { status = "Error", message = message, Result = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeletePurchaseOrder(int? ID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeletePurchaseOrder(ID);
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





        public ActionResult PurchaseReturnList(string StartDate, string EndDate)
        {
            var vm = new PurchaseOrderReturnViewModel();
            string UserRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.PurchaseOrderReturnList = _repository.BrowsePurchaseOrderReturn(stdt, eddt).ToList();
                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.PurchaseOrderReturnList = _repository.BrowsePurchaseOrderReturn(DateTime.Now.AddDays(-30), DateTime.Now).ToList();
            }
            return View(vm);
        }

        public ActionResult PurchaseOrderReturn(int? id)
        {
            string UserRole = GetCurrentUserRole();

            var vm = new PurchaseOrderReturnViewModel();
            vm.IsUpdate = false;
            vm.VendorList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Supplier").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.ItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.ExtraItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.EntryTypeList = _repository.EntryTypes.AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.Type), Value = x.ID.ToString() }).ToList();
            vm.StatusList = GetStatus().AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x), Value = x }).ToList();
            vm.BookerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.SupplierList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });

            if (id.HasValue)
            {
                var obj = _repository.PurchaseOrderReturns.FirstOrDefault(o => o.PurchaseOrderReturnID == id);
                if (obj != null)
                {
                    vm.PurchaseOrderReturnID = obj.PurchaseOrderReturnID;
                    vm.PONumber = obj.PONumber ?? 0;
                    vm.RecordType = obj.RecordType;
                    vm.EntryTypeID = obj.EntryTypeID ?? 0;
                    vm.VendorID = obj.VendorID.Value;
                    vm.PoDate = obj.PODate.HasValue ? obj.PODate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                    vm.InvoiceNo = obj.InvoiceNo;
                    vm.Notes = obj.Notes;
                    vm.SubTotal = obj.SubTotal;
                    vm.Expense = obj.Expense;
                    vm.Discount = obj.Discount;
                    vm.SalesTax = obj.SalesTax;
                    vm.PaidAmount = obj.PaidAmount;
                    vm.TotalAmount = obj.TotalAmount;
                    vm.IsUpdate = true;
                    vm.Status = obj.Status ?? "";
                    vm.BookerID = obj.BookerID ?? 0;
                    vm.SupplierID = obj.SupplierID ?? 0;

                    vm.EditPurchaseOrderReturnDetail = _repository.GetPurhcaseOrderReturnItemById(id).ToList();
                }
            }
            else
            {
                vm.EntryTypeID = 2;
                vm.PONumber = _repository.BrowseNewNumber("POR").FirstOrDefault().Value;
                vm.EntryTypeID = 2;
                vm.PoDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.VendorID = 0;
                vm.InvoiceNo = "";
                vm.Notes = "";
                vm.SubTotal = 0;
                vm.Expense = 0;
                vm.Discount = 0;
                vm.SalesTax = 0;
                vm.PaidAmount = 0;
                vm.TotalAmount = 0;
                vm.IsUpdate = false;
                vm.Status = "";
                vm.BookerID = 0;
                vm.SupplierID = 0;
            }

            return View(vm);
        }

        public JsonResult SavePurchaseOrderReturn(PurchaseOrderReturnViewModel vm)
        {
            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var pObj = new PurchaseOrderReturn();
                        if (vm.IsUpdate)
                            pObj = context.PurchaseOrderReturns.FirstOrDefault(x => x.PurchaseOrderReturnID == vm.PurchaseOrderReturnID);

                        pObj.PONumber = vm.PONumber;
                        pObj.RecordType = "PurchaseOrderReturn";
                        pObj.EntryTypeID = vm.EntryTypeID;
                        pObj.VendorID = vm.VendorID;
                        pObj.PODate = vm.PoDate == string.Empty ? (DateTime?)null : DateTime.ParseExact(vm.PoDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        pObj.InvoiceNo = vm.InvoiceNo;
                        pObj.Notes = vm.Notes;
                        pObj.SubTotal = vm.SubTotal;
                        pObj.Expense = vm.Expense;
                        pObj.Discount = vm.Discount;
                        pObj.SalesTax = vm.SalesTax;
                        pObj.PaidAmount = vm.PaidAmount;
                        pObj.TotalAmount = vm.TotalAmount;
                        pObj.RoleID = GetCurrentUserRole();
                        pObj.Status = "Completed";
                        pObj.BookerID = vm.BookerID;
                        pObj.SupplierID = vm.SupplierID;

                        if (!vm.IsUpdate) // Add
                        {
                            #region Add 

                            pObj.PurchaseOrderReturnID = -1;
                            pObj.CreatedBy = GetCurrentUserName();
                            pObj.CreatedDate = DateTime.Now;

                            context.Entry(pObj).State = System.Data.Entity.EntityState.Added;
                            if (context.SaveChanges() > 0)
                            {
                                var poId = pObj.PurchaseOrderReturnID;
                                foreach (var item in vm.PurchaseOrderReturnDetail)
                                {
                                    if (item.SubItemID.HasValue)
                                    {
                                        var obj = new PurchaseOrderReturnLine();
                                        obj.PurchaseOrderReturnID = poId;
                                        obj.SubItemID = item.SubItemID;
                                        obj.Quantity = item.Quantity;
                                        obj.PurchasePrice = item.PurchasePrice;
                                        obj.SalePrice = item.SalePrice;
                                        obj.T_O = item.T_O;
                                        obj.Discount = item.Discount;
                                        obj.SalesTax = item.SalesTax;
                                        obj.SubTotal = item.SubTotal;
                                        obj.TotalAmount = item.TotalAmount;
                                        obj.CreatedDate = DateTime.Now;
                                        obj.CreatedBy = GetCurrentUserName();

                                        context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                                        context.SaveChanges();
                                    }
                                }
                                dbContextTransaction.Commit();
                                context.AddUpdatePurchaseOrderReturnTransactions(poId, pObj.PODate.Value, "I");

                                //---------------------------- Cast Type Inset Payment Entry --------------------------------------
                                //if (pObj.EntryTypeID == 1 || pObj.PaidAmount > 0) // Cash
                                //{
                                //    var oven = context.BrowseAccount().FirstOrDefault(x => x.ID == pObj.VendorID);
                                //    var ph = new PaymentHeader();
                                //    ph.EmployeeID = oven == null ? 0 : oven.EmployeeID;
                                //    ph.ReceiptNo = _repository.BrowseNewNumber("RP").FirstOrDefault().Value.ToString();
                                //    ph.TotalAmount = pObj.PaidAmount;
                                //    ph.Particulars = pObj.Notes;
                                //    ph.VoucherType = "RV";
                                //    ph.PaymentDate = pObj.PODate;
                                //    ph.CreatedBy = GetCurrentUserName();
                                //    ph.CreatedDate = DateTime.Now;
                                //    context.Entry(pObj).State = System.Data.Entity.EntityState.Added;
                                //    if (context.SaveChanges() > 0)
                                //    {
                                //        var payId = context.InsertPayment(pObj.VendorID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.PODate, "Cash Payment against Order # " + pObj.PurchaseOrderReturnID, "RP", 0, "POR", pObj.PurchaseOrderReturnID, GetCurrentUserName(), GetCurrentUserRole(), ph.ID);
                                //    }
                                //}
                                //else
                                //{
                                //    context.DeleteCashPaymentEntry(pObj.PurchaseOrderReturnID, "POR");
                                //}
                            }

                            string message = "Order has been saved!";
                            return Json(new { status = "Success", message = "Error", detail = message, Result = "OK", POID = pObj.PurchaseOrderReturnID }, JsonRequestBehavior.AllowGet);

                            #endregion
                        }
                        else if (vm.IsUpdate == true && vm.PurchaseOrderReturnID > 0) // Update
                        {
                            #region Update

                            pObj.PurchaseOrderReturnID = vm.PurchaseOrderReturnID;
                            pObj.ModifiedBy = GetCurrentUserName();
                            pObj.ModifiedDate = DateTime.Now;

                            context.Entry(pObj).State = System.Data.Entity.EntityState.Modified;
                            if (context.SaveChanges() > 0)
                            {
                                foreach (var item in vm.PurchaseOrderReturnDetail)
                                {
                                    if (item.SubItemID.HasValue)
                                    {
                                        var obj = new PurchaseOrderReturnLine();
                                        if (item.PurchaseOrderReturnLineID > 0)
                                            obj = context.PurchaseOrderReturnLines.FirstOrDefault(o => o.PurchaseOrderReturnLineID == item.PurchaseOrderReturnLineID);

                                        obj.PurchaseOrderReturnID = pObj.PurchaseOrderReturnID;
                                        obj.SubItemID = item.SubItemID;
                                        obj.Quantity = item.Quantity;
                                        obj.PurchasePrice = item.PurchasePrice;
                                        obj.SalePrice = item.SalePrice;
                                        obj.T_O = item.T_O;
                                        obj.Discount = item.Discount;
                                        obj.SalesTax = item.SalesTax;
                                        obj.SubTotal = item.SubTotal;
                                        obj.TotalAmount = item.TotalAmount;

                                        if (item.PurchaseOrderReturnLineID > 0)
                                        {
                                            obj.ModifiedDate = DateTime.Now;
                                            obj.ModifiedBy = GetCurrentUserName();
                                            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                                        }
                                        else
                                        {
                                            obj.CreatedDate = DateTime.Now;
                                            obj.CreatedBy = GetCurrentUserName();
                                            context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                                        }
                                        context.SaveChanges();
                                    }
                                }

                                dbContextTransaction.Commit();
                                context.AddUpdatePurchaseOrderReturnTransactions(pObj.PurchaseOrderReturnID, pObj.PODate.Value, "U");

                                //---------------------------- Cast Type Inset Payment Entry --------------------------------------
                                //if (pObj.EntryTypeID == 1 || pObj.PaidAmount > 0) // Cash
                                //{
                                //    var payId = context.UpdatePayment(0, pObj.VendorID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.PODate, "Cash Payment against Order # " + pObj.PurchaseOrderReturnID, "RP", 0, "POR", pObj.PurchaseOrderReturnID, GetCurrentUserName(), GetCurrentUserRole());
                                //}
                                //else
                                //{
                                //    context.DeleteCashPaymentEntry(pObj.PurchaseOrderReturnID, "POR");
                                //}
                            }

                            string successMsg = "Order has been updated!";
                            return Json(new { status = "Success", message = "Error", detail = successMsg, Result = "OK", POID = pObj.PurchaseOrderReturnID }, JsonRequestBehavior.AllowGet);

                            #endregion
                        }

                        string msg = "Server not responed!";
                        return Json(new { status = "Success", message = "Error", detail = msg, Result = "OK", POID = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        public JsonResult DeletePOReturnItem(int POLineID, int POID)
        {
            string message = string.Empty;
            try
            {
                message = _repository.DeletePOReturnLine(POLineID, POID).FirstOrDefault();
                return Json(new { status = "Success", message = "Item has been deleted", Result = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { status = "Error", message = message, Result = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeletePurchaseOrderReturn(int? ID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeletePurchaseOrderReturn(ID);
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


        public JsonResult GetValueAutoComplate()
        {
            AutoCompleteView returnObj = new AutoCompleteView();
            using (var db = new HassanAyoubDBEntities())
            {
                returnObj.Items = new List<ItemView>();
                var queryPhone = from p in db.GetProductsForDdl()
                                 where p.RoleID.Equals(GetCurrentUserRole())
                                 select new ItemView()
                                 {
                                     Id = p.SubItemID,
                                     Name = p.ProductFullName,
                                     Description = p.SubItemName
                                 };
                returnObj.Items = queryPhone.ToList();
            }
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }

    }
}