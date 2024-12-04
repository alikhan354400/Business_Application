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
using System.Web.Helpers;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class SaleController : BaseController
    {
        public ActionResult SaleList(string StartDate, string EndDate)
        {
            var vm = new SaleOrderViewModel();
            string UserRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.SaleOrderList = _repository.BrowseSaleOrders(stdt, eddt).Where(x => x.RoleID == UserRole).ToList();

                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

                vm.SaleOrderList = _repository.BrowseSaleOrders(DateTime.Now.AddDays(-30), DateTime.Now).Where(x => x.RoleID == UserRole).ToList();
            }
            return View(vm);
        }

        public ActionResult SaleOrder(int? id)
        {
            string UserRole = GetCurrentUserRole();
            var vm = new SaleOrderViewModel();
            //---------------------------- Get List for Bindings ----------------------------
            vm.CustomerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Customer").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.ItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.ExtraItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.EntryTypeList = _repository.EntryTypes.AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.Type), Value = x.ID.ToString() }).ToList();
            vm.BookerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.SupplierList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });

            //---------------------------- Get Values for Update Order ----------------------------
            if (id.HasValue)
            {
                var pObj = _repository.SaleOrders.FirstOrDefault(o => o.SaleOrderID == id);
                if (pObj != null)
                {
                    vm.SaleOrderID = pObj.SaleOrderID;
                    vm.IsTaxInvoice = pObj.IsTaxInvoice ?? false;
                    vm.SONumber = pObj.SONumber ?? 0;
                    vm.RecordType = pObj.RecordType;
                    vm.EntryTypeID = pObj.EntryTypeID ?? 0;
                    vm.CustomerID = pObj.CustomerID.Value;
                    vm.SODate = pObj.SODate.HasValue ? pObj.SODate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                    vm.InvoiceNo = pObj.InvoiceNo;
                    vm.Notes = pObj.Notes;
                    vm.SubTotal = pObj.SubTotal ?? 0;
                    vm.Expense = pObj.Expense ?? 0;
                    vm.Discount = pObj.Discount ?? 0;
                    vm.Discount2 = pObj.Discount2 ?? 0;
                    vm.SalesTax = pObj.SalesTax ?? 0;
                    vm.PaidAmount = pObj.PaidAmount ?? 0;
                    vm.NetTotalAmount = pObj.NetTotalAmount ?? 0;
                    vm.IsUpdate = true;
                    vm.BookerID = pObj.BookerID ?? 0;
                    vm.SupplierID = pObj.SupplierID ?? 0;

                    vm.EditSaleOrderDetail = _repository.GetSalesOrderItemById(id.Value).ToList();
                }
            }
            else
            {
                vm.SONumber = _repository.BrowseNewNumber("SO").FirstOrDefault().Value;
                vm.IsTaxInvoice = false;
                vm.EntryTypeID = 2;
                vm.CustomerID = 0;
                vm.SODate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.InvoiceNo = "";
                vm.Notes = "";
                vm.SubTotal = 0;
                vm.Expense = 0;
                vm.Discount = 0;
                vm.Discount2 = 0;
                vm.SalesTax = 0;
                vm.PaidAmount = 0;
                vm.NetTotalAmount = 0;
                vm.IsUpdate = false;
                vm.BookerID = 0;
                vm.SupplierID = 0;
            }

            return View(vm);
        }

        public JsonResult SaveSaleOrder(SaleOrderViewModel vm)
        {
            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var cObj = context.BrowseAccount().Where(x => x.ID == vm.CustomerID).FirstOrDefault();
                        var pObj = new SaleOrder();
                        if (vm.IsUpdate)
                            pObj = context.SaleOrders.FirstOrDefault(x => x.SaleOrderID == vm.SaleOrderID);

                        pObj.CompanyId = cObj.CompanyId;
                        pObj.IsTaxInvoice = vm.IsTaxInvoice;
                        pObj.SONumber = vm.SONumber;
                        pObj.RecordType = "SalesOrder";
                        pObj.EntryTypeID = vm.EntryTypeID;
                        pObj.CustomerID = vm.CustomerID;
                        pObj.SODate = vm.SODate == string.Empty ? (DateTime?)null : DateTime.ParseExact(vm.SODate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        pObj.InvoiceNo = vm.InvoiceNo;
                        pObj.Notes = vm.Notes;
                        pObj.SubTotal = vm.SubTotal;
                        pObj.Expense = vm.Expense;
                        pObj.Discount = vm.Discount;
                        pObj.Discount2 = vm.Discount2;
                        pObj.SalesTax = vm.SalesTax;
                        pObj.PaidAmount = vm.PaidAmount;
                        pObj.NetTotalAmount = vm.NetTotalAmount;
                        pObj.RoleID = GetCurrentUserRole();
                        pObj.Status = "Completed";
                        pObj.BookerID = vm.BookerID;
                        pObj.SupplierID = vm.SupplierID;


                        if (!vm.IsUpdate) // Add
                        {
                            pObj.SaleOrderID = -1;
                            pObj.CreatedBy = User.Identity.Name;
                            pObj.CreatedDate = DateTime.Now;

                            context.Entry(pObj).State = System.Data.Entity.EntityState.Added;
                            if (context.SaveChanges() > 0)
                            {
                                foreach (var item in vm.SaleOrderDetail)
                                {
                                    if (item.SubItemID.HasValue)
                                    {
                                        var pPrice = context.SubItems.Where(x => x.SubItemID == item.SubItemID).FirstOrDefault().PurchasePrice;

                                        var obj = new SaleOrderLine();
                                        obj.SaleOrderID = pObj.SaleOrderID;
                                        obj.SubItemID = item.SubItemID;
                                        obj.Quantity = item.Quantity;
                                        obj.SalePrice = item.SalePrice;
                                        obj.PurchasePrice = pPrice == null ? 0 : Convert.ToDecimal(pPrice);
                                        obj.T_O = item.T_O;
                                        obj.Discount = item.Discount;
                                        obj.SalesTax = item.SalesTax;
                                        obj.SubTotal = item.SubTotal;
                                        obj.NetTotalAmount = item.NetTotalAmount;
                                        obj.InventoryID = item.InventoryID;
                                        obj.CreatedDate = DateTime.Now;
                                        obj.CreatedBy = User.Identity.Name;
                                        context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                                        context.SaveChanges();
                                    }
                                }
                                dbContextTransaction.Commit();
                                _repository.AddUpdateSaleOrderTransactions(pObj.SaleOrderID, pObj.SODate.Value, "I");

                                //---------------------------- Cast Type Inset Payment Entry --------------------------------------
                                //if (pObj.PaidAmount > 0) // Cash
                                //{
                                //    var ph = new PaymentHeader();
                                //    ph.EmployeeID = pObj.BookerID;
                                //    ph.ReceiptNo = _repository.BrowseNewNumber("RP").FirstOrDefault().Value.ToString();
                                //    ph.TotalAmount = pObj.PaidAmount;
                                //    ph.Particulars = pObj.Notes;
                                //    ph.VoucherType = "RV";
                                //    ph.PaymentDate = pObj.SODate;
                                //    ph.CreatedBy = GetCurrentUserName();
                                //    ph.CreatedDate = DateTime.Now;
                                //    context.Entry(ph).State = System.Data.Entity.EntityState.Added;
                                //    if (context.SaveChanges() > 0)
                                //    {
                                //        var payId = context.InsertPayment(pObj.CustomerID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.SODate, "Cash Payment against Order # " + pObj.SaleOrderID, "RP", 0, "SO", pObj.SaleOrderID, GetCurrentUserName(), GetCurrentUserRole(), ph.ID);
                                //    }
                                //}
                                //else
                                //{
                                //    context.DeleteCashPaymentEntry(pObj.SaleOrderID, "SO");
                                //}
                            }

                            string message = "Order has been saved!";
                            return Json(new { status = "Success", message = "Error", detail = message, Result = "OK", SOID = pObj.SaleOrderID }, JsonRequestBehavior.AllowGet);
                        }
                        else if (vm.IsUpdate == true && vm.SaleOrderID > 0) // Update
                        {
                            pObj.SaleOrderID = vm.SaleOrderID;
                            pObj.ModifiedBy = User.Identity.Name;
                            pObj.ModifiedDate = DateTime.Now;

                            context.Entry(pObj).State = System.Data.Entity.EntityState.Modified;
                            if (context.SaveChanges() > 0)
                            {
                                foreach (var item in vm.SaleOrderDetail)
                                {
                                    if (item.SubItemID.HasValue)
                                    {
                                        var obj = new SaleOrderLine();
                                        if (item.SaleOrderLineID > 0)
                                            obj = context.SaleOrderLines.FirstOrDefault(o => o.SaleOrderLineID == item.SaleOrderLineID);

                                        var pPrice = context.SubItems.Where(x => x.SubItemID == item.SubItemID).FirstOrDefault().PurchasePrice;

                                        obj.SaleOrderID = pObj.SaleOrderID;
                                        obj.SubItemID = item.SubItemID;
                                        obj.Quantity = item.Quantity;
                                        obj.SalePrice = item.SalePrice;
                                        obj.PurchasePrice = pPrice == null ? 0 : Convert.ToDecimal(pPrice);
                                        obj.T_O = item.T_O;
                                        obj.Discount = item.Discount;
                                        obj.SalesTax = item.SalesTax;
                                        obj.SubTotal = item.SubTotal;
                                        obj.NetTotalAmount = item.NetTotalAmount;
                                        obj.InventoryID = item.InventoryID;

                                        if (item.SaleOrderLineID > 0)
                                        {
                                            obj.ModifiedDate = DateTime.Now;
                                            obj.ModifiedBy = User.Identity.Name;
                                            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                                        }
                                        else
                                        {
                                            obj.CreatedDate = DateTime.Now;
                                            obj.CreatedBy = User.Identity.Name;
                                            context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                                        }
                                        context.SaveChanges();
                                    }
                                }
                                if (vm.DeleteItems != null)
                                {
                                    foreach (var delItemId in vm.DeleteItems)
                                    {
                                        var itemId = Convert.ToInt32(delItemId);
                                        var employer = new SaleOrderLine { SaleOrderLineID = itemId };
                                        context.Entry(employer).State = System.Data.Entity.EntityState.Deleted;
                                        context.SaveChanges();
                                    }
                                }
                                dbContextTransaction.Commit();
                                _repository.AddUpdateSaleOrderTransactions(pObj.SaleOrderID, pObj.SODate.Value, "U");

                                //---------------------------- Cast Type Inset Payment Entry --------------------------------------
                                //if (pObj.PaidAmount > 0) // Cash
                                //{
                                //    var isUpdate = false;
                                //    var ph = new PaymentHeader();
                                //    var pay = context.Payments.FirstOrDefault(x => x.RecordType == "SO" && x.RecordId == pObj.SaleOrderID);
                                //    if (pay != null)
                                //    {
                                //        var phid = pay.PaymentHeaderID;
                                //        ph = context.PaymentHeaders.FirstOrDefault(x => x.ID == pay.PaymentHeaderID);
                                //        if (ph == null)
                                //            ph = new PaymentHeader();
                                //        else
                                //            isUpdate = true;

                                //    }
                                //    ph.EmployeeID = pObj.BookerID;
                                //    ph.ReceiptNo = _repository.BrowseNewNumber("RP").FirstOrDefault().Value.ToString();
                                //    ph.TotalAmount = pObj.PaidAmount;
                                //    ph.Particulars = pObj.Notes;
                                //    ph.VoucherType = "RV";
                                //    ph.PaymentDate = pObj.SODate;
                                //    ph.CreatedBy = GetCurrentUserName();
                                //    ph.CreatedDate = DateTime.Now;
                                //    context.Entry(ph).State = System.Data.Entity.EntityState.Added;
                                //    if (isUpdate)
                                //        context.Entry(ph).State = System.Data.Entity.EntityState.Modified;
                                //    else
                                //        context.Entry(ph).State = System.Data.Entity.EntityState.Added;
                                //    if (context.SaveChanges() > 0)
                                //    {
                                //        if (pay == null)
                                //        {
                                //            var payId = context.InsertPayment(pObj.CustomerID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.SODate, "Cash Payment against Order # " + pObj.SaleOrderID, "RP", 0, "SO", pObj.SaleOrderID, GetCurrentUserName(), GetCurrentUserRole(), ph.ID);
                                //        }
                                //        else
                                //        {
                                //            var payId = context.UpdatePayment(0, pObj.CustomerID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.SODate, "Cash Payment against Order # " + pObj.SaleOrderID, "RP", 0, "SO", pObj.SaleOrderID, GetCurrentUserName(), GetCurrentUserRole());
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    context.DeleteCashPaymentEntry(pObj.SaleOrderID, "SO");
                                //}
                            }
                            string message = "Order has been updated!";
                            return Json(new { status = "Success", message = "Error", detail = message, Result = "OK", SOID = pObj.SaleOrderID, IsUpdate = "True" }, JsonRequestBehavior.AllowGet);
                        }

                        string msg = "Server not responed!";
                        return Json(new { status = "Success", message = "Error", detail = msg, Result = "OK" }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        public JsonResult DeleteSOItem(int SaleOrderLineID, int SaleOrderID)
        {
            string message = string.Empty;
            try
            {
                message = _repository.DeleteSaleOrderLine(SaleOrderLineID, SaleOrderID).FirstOrDefault();
                return Json(new { status = "Success", message = "Item has been deleted", Result = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { status = "Error", message = message, Result = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteSalesOrder(int? ID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeleteSalesOrder(ID);
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




        public ActionResult SaleReturnList(string StartDate, string EndDate)
        {
            var vm = new SaleOrderReturnViewModel();
            string UserRole = GetCurrentUserRole();

            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                DateTime stdt = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eddt = DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                vm.SaleOrderReturnList = _repository.BrowseSaleOrdersReturn(stdt, eddt).Where(x => x.RoleID == UserRole).ToList();

                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");
            }
            else
            {
                vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

                vm.SaleOrderReturnList = _repository.BrowseSaleOrdersReturn(DateTime.Now.AddDays(-30), DateTime.Now).Where(x => x.RoleID == UserRole).ToList();
            }
            return View(vm);
        }

        public ActionResult SaleOrderReturn(int? id)
        {
            string UserRole = GetCurrentUserRole();
            var vm = new SaleOrderReturnViewModel();
            //---------------------------- Get List for Bindings ----------------------------
            vm.CustomerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Customer").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.ItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.ExtraItemList = _repository.GetProductsForDdl().Where(x => x.RoleID == UserRole).AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.ProductFullName), Value = x.SubItemID.ToString() }).ToList();
            vm.EntryTypeList = _repository.EntryTypes.AsEnumerable().Select(x => new SelectListItem { Text = string.Format("{0}", x.Type), Value = x.ID.ToString() }).ToList();
            vm.BookerList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });
            vm.SupplierList = _repository.BrowseAccount().Where(o => o.RoleID == UserRole && o.AccountType == "Employee").Select(x => new SelectListItem { Text = string.Format("{0}", x.FullName, x.AccountType), Value = x.ID.ToString() });

            //---------------------------- Get Values for Update Order ----------------------------
            if (id.HasValue)
            {
                var pObj = _repository.SaleOrderReturns.FirstOrDefault(o => o.SaleOrderReturnID == id);
                if (pObj != null)
                {
                    vm.SaleOrderReturnID = pObj.SaleOrderReturnID;
                    vm.SONumber = pObj.SONumber ?? 0;
                    vm.RecordType = pObj.RecordType;
                    vm.EntryTypeID = pObj.EntryTypeID ?? 0;
                    vm.CustomerID = pObj.CustomerID.Value;
                    vm.SODate = pObj.SODate.HasValue ? pObj.SODate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                    vm.InvoiceNo = pObj.InvoiceNo;
                    vm.Notes = pObj.Notes;
                    vm.SubTotal = pObj.SubTotal ?? 0;
                    vm.Expense = pObj.TotalExpense ?? 0;
                    vm.Discount = pObj.Discount ?? 0;
                    vm.SalesTax = pObj.SalesTax ?? 0;
                    vm.PaidAmount = pObj.PaidAmount ?? 0;
                    vm.NetTotalAmount = pObj.NetTotalAmount ?? 0;
                    vm.IsUpdate = true;
                    vm.BookerID = pObj.BookerID ?? 0;
                    vm.SupplierID = pObj.SupplierID ?? 0;

                    vm.EditSaleOrderReturnDetail = _repository.GetSalesOrderReturnItemById(id).ToList();
                    //vm.EditSaleOrderDetail = _repository.GetSalesOrderItemById(id.Value).ToList();
                }
            }
            else
            {
                vm.SONumber = _repository.BrowseNewNumber("SOR").FirstOrDefault().Value;
                vm.EntryTypeID = 2;
                vm.CustomerID = 0;
                vm.SODate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.InvoiceNo = "";
                vm.Notes = "";
                vm.SubTotal = 0;
                vm.Expense = 0;
                vm.Discount = 0;
                vm.SalesTax = 0;
                vm.PaidAmount = 0;
                vm.NetTotalAmount = 0;
                vm.IsUpdate = false;
                vm.BookerID = 0;
                vm.SupplierID = 0;
            }

            return View(vm);
        }

        public JsonResult SaveSaleOrderReturn(SaleOrderReturnViewModel vm)
        {
            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var cObj = context.BrowseAccount().Where(x => x.ID == vm.CustomerID).FirstOrDefault();
                        var pObj = new SaleOrderReturn();
                        if (vm.IsUpdate)
                            pObj = context.SaleOrderReturns.FirstOrDefault(x => x.SaleOrderReturnID == vm.SaleOrderReturnID);

                        pObj.CompanyId = cObj.CompanyId;
                        pObj.SONumber = vm.SONumber;
                        pObj.RecordType = "SalesOrderReturn";
                        pObj.EntryTypeID = vm.EntryTypeID;
                        pObj.CustomerID = vm.CustomerID;
                        pObj.SODate = vm.SODate == string.Empty ? (DateTime?)null : DateTime.ParseExact(vm.SODate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        pObj.InvoiceNo = vm.InvoiceNo;
                        pObj.Notes = vm.Notes;
                        pObj.SubTotal = vm.SubTotal;
                        pObj.TotalExpense = vm.Expense;
                        pObj.Discount = vm.Discount;
                        pObj.SalesTax = vm.SalesTax;
                        pObj.PaidAmount = vm.PaidAmount;
                        pObj.NetTotalAmount = vm.NetTotalAmount;
                        pObj.RoleID = GetCurrentUserRole();
                        //pObj..Status = "Completed";
                        pObj.BookerID = vm.BookerID;
                        pObj.SupplierID = vm.SupplierID;


                        if (!vm.IsUpdate) // Add
                        {
                            pObj.SaleOrderReturnID = -1;
                            pObj.CreatedBy = User.Identity.Name;
                            pObj.CreatedDate = DateTime.Now;

                            context.Entry(pObj).State = System.Data.Entity.EntityState.Added;
                            if (context.SaveChanges() > 0)
                            {
                                foreach (var item in vm.SaleOrderReturnDetail)
                                {
                                    if (item.SubItemID.HasValue)
                                    {
                                        var pPrice = context.SubItems.Where(x => x.SubItemID == item.SubItemID).FirstOrDefault().PurchasePrice;

                                        var obj = new SaleOrderReturnLine();
                                        obj.SaleOrderReturnID = pObj.SaleOrderReturnID;
                                        obj.SubItemID = item.SubItemID;
                                        obj.Quantity = item.Quantity;
                                        obj.SalePrice = item.SalePrice;
                                        obj.PurchasePrice = pPrice == null ? 0 : Convert.ToDecimal(pPrice);
                                        obj.T_O = item.T_O;
                                        obj.Discount = item.Discount;
                                        obj.SalesTaxRate = item.SalesTaxRate;
                                        obj.SubTotal = item.SubTotal;
                                        obj.NetTotalAmount = item.NetTotalAmount;
                                        obj.InventoryID = item.InventoryID;
                                        obj.CreatedDate = DateTime.Now;
                                        obj.CreatedBy = User.Identity.Name;
                                        context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                                        context.SaveChanges();
                                    }
                                }
                                dbContextTransaction.Commit();
                                _repository.AddUpdateSaleOrderReturnTransactions(pObj.SaleOrderReturnID, pObj.SODate.Value, "I");

                                //---------------------------- Cast Type Inset Payment Entry --------------------------------------
                                //if (pObj.PaidAmount > 0) // Cash
                                //{
                                //    var ph = new PaymentHeader();
                                //    ph.EmployeeID = vm.BookerID;
                                //    ph.ReceiptNo = _repository.BrowseNewNumber("PP").FirstOrDefault().Value.ToString();
                                //    ph.TotalAmount = pObj.PaidAmount;
                                //    ph.Particulars = pObj.Notes;
                                //    ph.VoucherType = "PV";
                                //    ph.PaymentDate = pObj.SODate;
                                //    ph.CreatedBy = GetCurrentUserName();
                                //    ph.CreatedDate = DateTime.Now;
                                //    context.Entry(ph).State = System.Data.Entity.EntityState.Added;
                                //    if (context.SaveChanges() > 0)
                                //    {
                                //        var payId = context.InsertPayment(pObj.CustomerID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.SODate, "Cash Payment against Order Retrun # " + pObj.SaleOrderReturnID, "PP", 0, "SO Return", pObj.SaleOrderReturnID, GetCurrentUserName(), GetCurrentUserRole(), ph.ID);
                                //    }
                                //}
                                //else
                                //{
                                //    context.DeleteCashPaymentEntry(pObj.SaleOrderReturnID, "SO");
                                //}
                            }

                            string message = "Order has been saved!";
                            return Json(new { status = "Success", message = "Error", detail = message, Result = "OK", SOID = pObj.SaleOrderReturnID }, JsonRequestBehavior.AllowGet);
                        }
                        else if (vm.IsUpdate == true && vm.SaleOrderReturnID > 0) // Update
                        {
                            pObj.SaleOrderReturnID = vm.SaleOrderReturnID;
                            pObj.ModifiedBy = User.Identity.Name;
                            pObj.ModifiedDate = DateTime.Now;

                            context.Entry(pObj).State = System.Data.Entity.EntityState.Modified;
                            if (context.SaveChanges() > 0)
                            {
                                foreach (var item in vm.SaleOrderReturnDetail)
                                {
                                    if (item.SubItemID.HasValue)
                                    {
                                        var obj = new SaleOrderReturnLine();
                                        if (item.SaleOrderReturnLineID > 0)
                                            obj = context.SaleOrderReturnLines.FirstOrDefault(o => o.SaleOrderReturnLineID == item.SaleOrderReturnLineID);

                                        var pPrice = context.SubItems.Where(x => x.SubItemID == item.SubItemID).FirstOrDefault().PurchasePrice;

                                        obj.SaleOrderReturnID = pObj.SaleOrderReturnID;
                                        obj.SubItemID = item.SubItemID;
                                        obj.Quantity = item.Quantity;
                                        obj.SalePrice = item.SalePrice;
                                        obj.PurchasePrice = pPrice == null ? 0 : Convert.ToDecimal(pPrice);
                                        obj.T_O = item.T_O;
                                        obj.Discount = item.Discount;
                                        obj.SalesTaxRate = item.SalesTaxRate;
                                        obj.SubTotal = item.SubTotal;
                                        obj.NetTotalAmount = item.NetTotalAmount;
                                        obj.InventoryID = item.InventoryID;

                                        if (item.SaleOrderReturnLineID > 0)
                                        {
                                            obj.ModifiedDate = DateTime.Now;
                                            obj.ModifiedBy = User.Identity.Name;
                                            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                                        }
                                        else
                                        {
                                            obj.CreatedDate = DateTime.Now;
                                            obj.CreatedBy = User.Identity.Name;
                                            context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                                        }
                                        context.SaveChanges();
                                    }
                                }
                                if (vm.DeleteItems != null)
                                {
                                    foreach (var delItemId in vm.DeleteItems)
                                    {
                                        var itemId = Convert.ToInt32(delItemId);
                                        var employer = new SaleOrderLine { SaleOrderLineID = itemId };
                                        context.Entry(employer).State = System.Data.Entity.EntityState.Deleted;
                                        context.SaveChanges();
                                    }
                                }
                                dbContextTransaction.Commit();
                                _repository.AddUpdateSaleOrderReturnTransactions(pObj.SaleOrderReturnID, pObj.SODate.Value, "U");

                                //---------------------------- Cast Type Inset Payment Entry --------------------------------------
                                //if (pObj.PaidAmount > 0) // Cash
                                //{
                                //    var isUpdate = false;
                                //    var ph = new PaymentHeader();
                                //    var pay = context.Payments.FirstOrDefault(x => x.RecordType == "SO Return" && x.RecordId == pObj.SaleOrderReturnID);
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
                                //    ph.PaymentDate = pObj.SODate;
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
                                //            var payId = context.InsertPayment(pObj.CustomerID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.SODate, "Cash Payment against Order Return # " + pObj.SaleOrderReturnID, "PP", 0, "SO Return", pObj.SaleOrderReturnID, GetCurrentUserName(), GetCurrentUserRole(), ph.ID);
                                //        }
                                //        else
                                //        {
                                //            var payId = context.UpdatePayment(0, pObj.CustomerID, "Cash", pObj.PaidAmount ?? 0, "", null, pObj.SODate, "Cash Payment against Order Return # " + pObj.SaleOrderReturnID, "PP", 0, "SO Return", pObj.SaleOrderReturnID, GetCurrentUserName(), GetCurrentUserRole());
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    context.DeleteCashPaymentEntry(pObj.SaleOrderReturnID, "SO");
                                //}

                            }
                            string message = "Order has been updated!";
                            return Json(new { status = "Success", message = "Error", detail = message, Result = "OK", SOID = pObj.SaleOrderReturnID, IsUpdate = "True" }, JsonRequestBehavior.AllowGet);
                        }

                        string msg = "Server not responed!";
                        return Json(new { status = "Success", message = "Error", detail = msg, Result = "OK" }, JsonRequestBehavior.AllowGet);

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
        public JsonResult DeleteSalesOrderReturn(int? ID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeleteSalesOrderReturn(ID);
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

        public JsonResult DeleteSOReturnItem(int SaleOrderReturnLineID, int SaleOrderReturnID)
        {
            string message = string.Empty;
            try
            {
                message = _repository.DeleteSaleOrderReturnLine(SaleOrderReturnLineID, SaleOrderReturnID).FirstOrDefault();
                return Json(new { status = "Success", message = "Item has been deleted", Result = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { status = "Error", message = message, Result = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

    }

    public class Select2Model
    {
        public string id { get; set; }
        public string text { get; set; }
    }
}