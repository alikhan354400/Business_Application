using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class PrintController : BaseController
    {
        // GET: Print --> PO (Purchase Invoice)
        public ActionResult PurchaseInvoice(int? id, int? CompanyID, bool? IsDispature)
        {
            return View(GetPrintPOData(id, CompanyID, IsDispature));
        }

        public PrintPurchaseReceiptViewModel GetPrintPOData(int? orderID, int? CompanyId, bool? IsDispature)
        {
            var vm = new PrintPurchaseReceiptViewModel();
            var roleId = GetCurrentUserRole();
            var oObj = _repository.vw_PurchaseOrder.FirstOrDefault(x => x.PurchaseOrderID == orderID);
            vm.CompanyInfo = GetCompanyInfo(oObj.CompanyId).FirstOrDefault();

            if (oObj != null)
            {
                try
                {
                    var dueAmnt = _repository.GetAccountDueAmountByIDandEndDate(oObj.AccountID, oObj.PurchaseOrderDate).FirstOrDefault();
                    var oObjDetail = _repository.BrowsePODetailBPOID(orderID).ToList();

                    vm.InvoiceNumber = "PO-" + oObj.PONumber.ToString();
                    vm.PODate = oObj.PurchaseOrderDate.HasValue ? oObj.PurchaseOrderDate.Value.ToString("dd/MM/yyyy") : "";
                    vm.VendorName = oObj.VendorName;
                    vm.VendorContactNo = oObj.ContactNo;
                    vm.VendorCNIC = oObj.VendorCNIC;
                    vm.VendorAddress = oObj.VendorAddress;
                    vm.VendorSTR = oObj.VendorSTR;
                    vm.VendorNTN = oObj.VendorNTN;
                    vm.VendorCityArea = oObj.VendorCityArea;
                    vm.SubTotal = oObj.SubTotal;
                    vm.Expense = oObj.Expense;
                    vm.Discount = oObj.Discount;
                    vm.SalesTax = oObj.SalesTax;
                    vm.PaidAmount = oObj.PaidAmount;
                    vm.TotalAmount = oObj.TotalAmount;
                    vm.Notes = oObj.Notes;
                    vm.DueAmount = dueAmnt ?? 0;
                    vm.EntryType = oObj.EntryType;
                    vm.IsDispature = IsDispature ?? false;
                    vm.Booker = oObj.BookerName;
                    vm.SalesOfficer = oObj.SupplierName;


                    var role = GetCurrentUserRole();
                    var itemDetail = oObjDetail.Select(x => new PrintPODetail()
                    {
                        ID = x.PurchaseOrderLineID,
                        ItemName = x.ItemName,
                        Quantity = x.Quantity ?? 0,
                        Price = x.PurchasePrice ?? 0,
                        TO = x.T_O ?? 0,
                        SubTotal = x.SubTotal ?? 0,
                        Disc = x.Discount ?? 0,
                        SalesTax = x.SalesTax ?? 0,
                        Total = x.TotalAmount ?? 0,

                    }).ToList();

                    vm.ItemDetail = itemDetail;
                }
                catch (Exception ex)
                {
                    //throw;
                }
            }
            return vm;
        }



        // GET: Print --> PO (Purchase Return Invoice)
        public ActionResult PurchaseReturnInvoice(int? id, int? CompanyID, bool? IsDispature)
        {
            return View(GetPrintPOReturnData(id, CompanyID, IsDispature));
        }

        public PrintPurchaseReturnReceiptViewModel GetPrintPOReturnData(int? orderID, int? CompanyId, bool? IsDispature)
        {
            var vm = new PrintPurchaseReturnReceiptViewModel();
            var roleId = GetCurrentUserRole();
            var oObj = _repository.vw_PurchaseOrderReturn.FirstOrDefault(x => x.PurchaseOrderReturnID == orderID);
            vm.CompanyInfo = GetCompanyInfo(oObj.CompanyId).FirstOrDefault();

            if (oObj != null)
            {
                try
                {
                    var dueAmnt = _repository.GetAccountDueAmountByIDandEndDate(oObj.AccountID, oObj.PurchaseOrderDate).FirstOrDefault();
                    var oObjDetail = _repository.GetPurhcaseOrderReturnItemById(orderID).ToList();

                    vm.InvoiceNumber = "POR-" + oObj.PurchaseOrderReturnID.ToString();
                    vm.PODate = oObj.PurchaseOrderDate.HasValue ? oObj.PurchaseOrderDate.Value.ToString("dd/MM/yyyy") : "";
                    vm.VendorName = oObj.VendorName;
                    vm.VendorContactNo = oObj.ContactNo;
                    vm.VendorCNIC = oObj.VendorCNIC;
                    vm.VendorAddress = oObj.VendorAddress;
                    vm.VendorSTR = oObj.VendorSTR;
                    vm.VendorNTN = oObj.VendorNTN;
                    vm.VendorCityArea = oObj.VendorCityArea;
                    vm.SubTotal = oObj.SubTotal;
                    vm.Expense = oObj.Expense;
                    vm.Discount = oObj.Discount;
                    vm.SalesTax = oObj.SalesTax;
                    vm.PaidAmount = oObj.PaidAmount;
                    vm.TotalAmount = oObj.TotalAmount;
                    vm.Notes = oObj.Notes;
                    vm.DueAmount = dueAmnt ?? 0;
                    vm.EntryType = oObj.EntryType;
                    vm.IsDispature = IsDispature ?? false;
                    vm.Booker = oObj.BookerName;
                    vm.SalesOfficer = oObj.SupplierName;


                    var role = GetCurrentUserRole();
                    var itemDetail = oObjDetail.Select(x => new PrintPOReturnDetail()
                    {
                        ID = x.PurchaseOrderReturnID ?? 0,
                        ItemName = x.ProductFullName,
                        Quantity = x.Quantity ?? 0,
                        Price = x.PurchasePrice ?? 0,
                        TO = x.T_O ?? 0,
                        SubTotal = x.SubTotal ?? 0,
                        Disc = x.Discount ?? 0,
                        SalesTax = x.SalesTax ?? 0,
                        Total = x.TotalAmount ?? 0,

                    }).ToList();

                    vm.ItemDetail = itemDetail;
                }
                catch (Exception ex)
                {
                    //throw;
                }
            }
            return vm;
        }




        // GET: Print --> SO (Sale Invoice)
        public ActionResult SalesInvoice(int? id, int? CompanyID, bool? IsDispature)
        {
            return View(GetPrintSOData(id, CompanyID, IsDispature));
        }

        public PrintSalesReceiptViewModel GetPrintSOData(int? id, int? CompanyID, bool? IsDispature)
        {
            var vm = new PrintSalesReceiptViewModel();
            var roleId = GetCurrentUserRole();
            //vm.CompanyInfo = _repository.CompanyInformations.FirstOrDefault(x => x.RoleId == roleId);
            var oObj = _repository.vw_SaleOrder.FirstOrDefault(x => x.SaleOrderID == id);
            vm.CompanyInfo = GetCompanyInfo(oObj.CompanyId).FirstOrDefault();

            if (oObj != null)
            {
                var dueAmnt = _repository.GetAccountDueAmountByIDandEndDate(oObj.AccountID, oObj.SaleOrderDate).FirstOrDefault();
                var oObjDetail = _repository.BrowseSODetailBSOID(id).ToList();

                vm.InvoiceNumber = "INV-" + oObj.SONumber.ToString();
                vm.SODate = oObj.SaleOrderDate.HasValue ? oObj.SaleOrderDate.Value.ToString("dd/MM/yyyy") : "";
                vm.CustomerName = oObj.CustomerName;
                vm.CustomerContactNo = oObj.ContactNo;
                vm.CustomerCNIC = oObj.CustomerCNIC;
                vm.CustomerAddress = oObj.CustomerAddress;
                vm.CustomerSTRN = oObj.CustomerSTRN;
                vm.CustomerNTN = oObj.CustomerNTN;
                vm.CustomerCityArea = oObj.CustomerCityArea;
                vm.SubTotal = oObj.SubTotal;
                vm.Expense = oObj.Expense;
                vm.Discount = oObj.Discount;
                vm.Discount2 = oObj.Discount2;
                vm.SalesTax = oObj.SalesTax;
                vm.PaidAmount = oObj.PaidAmount;
                vm.TotalAmount = oObj.TotalAmount;
                vm.Notes = oObj.Notes;
                vm.DueAmount = dueAmnt ?? 0;
                vm.EntryType = oObj.EntryType;
                vm.IsDispature = IsDispature ?? false;
                vm.SalesOfficer = oObj.SalesOfficer;
                vm.Booker = oObj.BookerName;

                var role = GetCurrentUserRole();
                var itemDetail = oObjDetail.Select(x => new PrintSODetail()
                {
                    ID = x.SaleOrderLineID,
                    ItemName = x.ItemName,
                    Price = x.SalePrice ?? 0,
                    Quantity = x.Quantity ?? 0,
                    TotalPrice = x.SubTotal ?? 0,
                    TO = x.T_O ?? 0,
                    SubTotal = x.SubTotal ?? 0,
                    Disc = x.Discount ?? 0,
                    SalesTax = x.SalesTax ?? 0,
                    Total = x.NetTotalAmount ?? 0
                }).ToList();

                vm.ItemDetail = itemDetail;
            }

            return vm;
        }




        // GET: Print --> SO Return (Sale Return Invoice)
        public ActionResult SalesInvoiceReturn(int? id, int? CompanyID, bool? IsDispature)
        {
            return View(GetPrintSOReturnData(id, CompanyID, IsDispature));
        }

        public PrintSalesReceiptViewModel GetPrintSOReturnData(int? id, int? CompanyID, bool? IsDispature)
        {
            var vm = new PrintSalesReceiptViewModel();
            var roleId = GetCurrentUserRole();
            //vm.CompanyInfo = _repository.CompanyInformations.FirstOrDefault(x => x.RoleId == roleId);
            var oObj = _repository.vw_SaleOrderReturn.FirstOrDefault(x => x.SaleOrderReturnID == id);
            vm.CompanyInfo = GetCompanyInfo(oObj.CompanyId).FirstOrDefault();

            if (oObj != null)
            {
                var dueAmnt = _repository.GetAccountDueAmountByIDandEndDate(oObj.AccountID, oObj.SaleOrderDate).FirstOrDefault();
                var oObjDetail = _repository.BrowseSOReturnDetailBSOID(id).ToList();

                vm.InvoiceNumber = "INV-" + oObj.SONumber.ToString();
                vm.SODate = oObj.SaleOrderDate.HasValue ? oObj.SaleOrderDate.Value.ToString("dd/MM/yyyy") : "";
                vm.CustomerName = oObj.CustomerName;
                vm.CustomerContactNo = oObj.ContactNo;
                vm.CustomerCNIC = oObj.CustomerCNIC;
                vm.CustomerAddress = oObj.CustomerAddress;
                vm.CustomerSTRN = oObj.CustomerSTRN;
                vm.CustomerNTN = oObj.CustomerNTN;
                vm.CustomerCityArea = oObj.CustomerCityArea;
                vm.SubTotal = oObj.SubTotal;
                vm.Expense = oObj.Expense;
                vm.Discount = oObj.Discount;
                vm.SalesTax = oObj.SalesTax;
                vm.PaidAmount = oObj.PaidAmount;
                vm.TotalAmount = oObj.TotalAmount;
                vm.Notes = oObj.Notes;
                vm.DueAmount = dueAmnt ?? 0;
                vm.EntryType = oObj.EntryType;
                vm.IsDispature = IsDispature ?? false;
                vm.SalesOfficer = oObj.SalesOfficer;
                vm.Booker = oObj.BookerName;
                vm.InvoiceNo = oObj.InvoiceNo;


                var role = GetCurrentUserRole();
                var itemDetail = oObjDetail.Select(x => new PrintSODetail()
                {
                    ID = x.SaleOrderReturnLineID,
                    ItemName = x.ItemName,
                    Price = x.SalePrice ?? 0,
                    Quantity = x.Quantity ?? 0,
                    TotalPrice = x.SubTotal ?? 0,
                    TO = x.T_O ?? 0,
                    SubTotal = x.SubTotal ?? 0,
                    Disc = x.Discount ?? 0,
                    SalesTax = x.SalesTaxRate ?? 0,
                    Total = x.NetTotalAmount ?? 0
                }).ToList();

                vm.ItemDetail = itemDetail;
            }

            return vm;
        }





        // GET: Print --> PS (Pay Slip)
        public ActionResult PrintPaySlip(int? id)
        {
            return View(GetPrintPaySlip(id));
        }

        public PrintPaymentReceiptViewModel GetPrintPaySlip(int? id)
        {
            var vm = new PrintPaymentReceiptViewModel();
            var roleId = GetCurrentUserRole();
            var oObj = _repository.vw_PaymentHeader.FirstOrDefault(x => x.ID == id);
            vm.CompanyInfo = GetCompanyInfo(oObj.CompanyId).FirstOrDefault();

            if (oObj != null)
            {
                var oObjDetail = _repository.vw_Payments.Where(x => x.PaymentHeaderID == id).ToList();

                vm.ReceiptNo = "PV-" + oObj.ReceiptNo.ToString();
                vm.PaymentDate = oObj.PaymentDate.HasValue ? oObj.PaymentDate.Value.ToString("dd/MM/yyyy") : "";
                vm.AccountID = oObj.EmployeeId;
                vm.AccountName = oObj.SalesOfficer;
                vm.ContactNo = oObj.ContactNo;
                vm.CNIC = oObj.CNIC;
                vm.Address = oObj.OfficerAddress;
                vm.STRN = oObj.STRN;
                vm.NTN = oObj.NTN;
                vm.CityAreaName = oObj.CityAreaName;
                vm.TotalAmount = oObj.TotalAmount;
                vm.Particular = oObj.Particular;
                vm.VoucharType = oObj.VoucharType;
                vm.PaymentDetail = oObj.PaymentDetail;

                vm.ItemDetail = oObjDetail;
            }

            return vm;
        }


        // GET: Print --> RP (Receipt Print)
        public ActionResult PrintReceipt(int? id)
        {
            return View(GetPrintReceipt(id));
        }

        public PrintPaymentReceiptViewModel GetPrintReceipt(int? id)
        {
            var vm = new PrintPaymentReceiptViewModel();
            var roleId = GetCurrentUserRole();
            var oObj = _repository.vw_PaymentHeader.FirstOrDefault(x => x.ID == id);
            vm.CompanyInfo = GetCompanyInfo(oObj.CompanyId).FirstOrDefault();

            if (oObj != null)
            {
                var oObjDetail = _repository.vw_Payments.Where(x => x.PaymentHeaderID == id).ToList();

                vm.ReceiptNo = "RV-" + oObj.ReceiptNo.ToString();
                vm.PaymentDate = oObj.PaymentDate.HasValue ? oObj.PaymentDate.Value.ToString("dd/MM/yyyy") : "";
                vm.AccountID = oObj.EmployeeId;
                vm.AccountName = oObj.SalesOfficer;
                vm.ContactNo = oObj.ContactNo;
                vm.CNIC = oObj.CNIC;
                vm.Address = oObj.OfficerAddress;
                vm.STRN = oObj.STRN;
                vm.NTN = oObj.NTN;
                vm.CityAreaName = oObj.CityAreaName;
                vm.TotalAmount = oObj.TotalAmount;
                vm.Particular = oObj.Particular;
                vm.VoucharType = oObj.VoucharType;
                vm.PaymentDetail = oObj.PaymentDetail;

                vm.ItemDetail = oObjDetail;
            }

            return vm;
        }



    }
}