using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
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
    public class BankController : BaseController
    {
        // GET: Bank
        public ActionResult AccountsList()
        {
            var model = new BankAccountViewModel();
            model.AccountList = _repository.BrowseBankAccount().Where(x => x.RoleId == GetCurrentUserRole()).ToList();
            return View(model);
        }

        public ActionResult Account(int? id)
        {
            var vm = new AddEditBankAccountViewModel();
            if (id.HasValue) // edit
            {
                var obj = _repository.Banks.Where(x => x.ID == id).FirstOrDefault();
                if (obj != null)
                {
                    vm.ID = id;
                    vm.BankName = obj.BankName;
                    vm.BankBranch = obj.BankBranch;
                    vm.BankBranchCity = obj.BankBranchCity;
                    vm.AccountTitle = obj.AccountTitle;
                    vm.AccountNumber = obj.AccountNumber;
                    vm.OpeningBalance = obj.OpeningBalance;
                    vm.OpeningBalanceDate = obj.OpeningBalanceDate == null ? DateTime.Now.ToString("dd/MM/yyyy") : obj.OpeningBalanceDate.Value.ToString("dd/MM/yyyy");
                    vm.ContactNo = obj.ContactNo;
                    vm.IsActive = obj.IsActive;
                }
            }
            else // get
            {
                vm.ID = 0;
                vm.BankName = "";
                vm.BankBranch = "";
                vm.BankBranchCity = "";
                vm.AccountTitle = "";
                vm.AccountNumber = "";
                vm.OpeningBalance = 0;
                vm.OpeningBalanceDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.ContactNo = "";
                vm.IsActive = false;
            }
            return View(vm);
        }

        public JsonResult SaveAccount(AddEditBankAccountViewModel model)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        DateTime openingBalanceDate = string.IsNullOrEmpty(model.OpeningBalanceDate) ? DateTime.Now : DateTime.ParseExact(model.OpeningBalanceDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        var obj = new Bank();
                        obj.BankName = model.BankName;
                        obj.BankBranch = model.BankBranch;
                        obj.BankBranchCity = model.BankBranchCity;
                        obj.AccountTitle = model.AccountTitle;
                        obj.AccountNumber = model.AccountNumber;
                        obj.ContactNo = model.ContactNo;
                        obj.OpeningBalance = model.OpeningBalance ?? 0;
                        obj.OpeningBalanceDate = openingBalanceDate;

                        if (model.ID == null || model.ID == 0)
                        {
                            obj.IsActive = true;
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedBy = GetCurrentUserName();
                            obj.RoleId = GetCurrentUserRole();
                            context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                        }
                        else
                        {
                            //obj.IsDelete = model.IsDelete;
                            obj.IsActive = true;
                            obj.ModifyDate = DateTime.Now;
                            obj.ModifyBy = GetCurrentUserName();
                            obj.RoleId = GetCurrentUserRole();
                            obj.ID = model.ID ?? 0;
                            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                        }
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        return Json(new { status = "Success", message = "Error", detail = "Bank Account has been saved!", Result = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        public ActionResult AccountDelete(int Id)
        {
            var obj = _repository.Banks.Where(x => x.ID == Id).FirstOrDefault();
            if (obj != null)
            {
                obj.IsActive = false;
                _repository.SaveChanges();
            }
            var url = GetWebsiteUrl() + "/Bank/AccountsList";
            return Redirect(url);
        }

        public ActionResult BankTransferList()
        {
            var vm = new BankAccountViewModel();
            string UserRole = GetCurrentUserRole();

            vm.StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            vm.EndDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(vm);
        }

        [HttpPost]
        public ActionResult GetBankTransfer(string StartDate, string EndDate)
        {
            try
            {
                var vm = new BankAccountViewModel();

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

                var list = _repository.BrowseBankTransfers(stdt, eddt).ToList();

                vm.EndDate = eddt.ToString("dd/MM/yyyy");
                vm.StartDate = stdt.ToString("dd/MM/yyyy");

                return PartialView("_BankTransferList", list);
            }
            catch (Exception ex)
            {
                return PartialView("_BankTransferList", null);
            }
        }

        public ActionResult BankTransfer(int? id)
        {
            string UserRole = GetCurrentUserRole();

            var vm = new BankAccountViewModel();

            vm.BankList = _repository.Banks.Where(x => x.IsActive == true).AsEnumerable().Select(x => new SelectListItem { Text = x.BankName, Value = x.ID.ToString() });

            if (id.HasValue)
            {
                var obj = _repository.BankToBankTransfers.FirstOrDefault(x => x.ID == id.Value);
                if (obj != null)
                {
                    vm.Amount = obj.Amount;
                    vm.Description = obj.Description;
                    vm.FromBankID = obj.FromBankID.Value;
                    vm.OtherCharges = obj.ServiceCharges;
                    vm.ToBankID = obj.ToBankID.Value;
                    vm.TransferDate = obj.TransferDate == null ? "" : obj.TransferDate.Value.ToString("dd/MM/yyyy");
                    vm.BankTransferID = id.Value;
                }
            }
            else
            {
                vm.TransferDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.OtherCharges = 0;
            }

            return View(vm);
        }

        [HttpPost]
        public JsonResult InsertBankTransfer(BankAccountViewModel model)
        {
            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        DateTime transferDate = DateTime.ParseExact(model.TransferDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        string UserRole = GetCurrentUserRole();

                        var obj = new BankToBankTransfer();

                        if (model.BankTransferID > 0)
                            obj = context.BankToBankTransfers.FirstOrDefault(x => x.ID == model.BankTransferID);

                        obj.Amount = model.Amount;
                        obj.Description = model.Description;
                        obj.FromBankID = model.FromBankID;
                        obj.RoleID = UserRole;
                        obj.ServiceCharges = model.OtherCharges;
                        obj.ToBankID = model.ToBankID;
                        obj.TransferDate = transferDate;

                        if (model.BankTransferID == 0)
                        {
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedBy = User.Identity.Name;
                            context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                        }
                        else if (model.BankTransferID > 0)
                        {
                            obj.ModifiedDate = DateTime.Now;
                            obj.ModifiedBy = User.Identity.Name;
                            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                        }

                        context.SaveChanges();

                        if (model.BankTransferID == 0)
                            context.AddUpdateBankTransferTrans(obj.ID, obj.TransferDate, "I", obj.Amount, obj.ServiceCharges);
                        else if (model.BankTransferID > 0)
                            context.AddUpdateBankTransferTrans(obj.ID, obj.TransferDate, "U", obj.Amount, obj.ServiceCharges);


                        dbContextTransaction.Commit();

                        string message = "Record has been saved!";
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
        public JsonResult DeleteBankTransfer(int ID)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.DeleteBankTransfer(ID);
                        dbContextTransaction.Commit();
                        return Json(new { status = "Success", message = "Error", detail = "Record has been Deleted!", Result = "OK" }, JsonRequestBehavior.AllowGet);
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