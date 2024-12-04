using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models.EF;
using HassanAyoubTraders.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class CustomerController : BaseController
    {
        // GET: Customer

        public ActionResult CustomerList()
        {
            var vm = new AccountViewModel();
            vm.AccountList = _repository.BrowseAccount().Where(x => x.AccountType.Equals("Customer") && x.RoleID.Equals(GetCurrentUserRole())).ToList();
            return View(vm);
        }

        public ActionResult CustomerAddEdit(int? id)
        {
            var vm = new AddEditAccountViewModel();
            vm.CompanyList = _repository.BrowseAllCompanies().Select(x => new SelectListItem { Text = x.CompanyName, Value = x.ID.ToString() });
            vm.CityList = _repository.Cities.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            vm.EmployeesList = _repository.BrowseAccount().Where(x => x.AccountTypeID == 3).Select(x => new SelectListItem { Text = x.FullName, Value = x.ID.ToString() });
            vm.AccountTypeList = _repository.BrowseCodesByType("Account Type").Select(x => new SelectListItem { Text = x.CodeValue, Value = x.ID.ToString() });
            var acc = _repository.BrowseAccount().Where(x => x.RoleID.Equals(GetCurrentUserRole()) && x.IsCashAccount == true).FirstOrDefault();
            vm.IsCashAccountExist = acc == null ? false : true;

            if (id.HasValue) // edit
            {
                var obj = _repository.Accounts.Where(x => x.ID == id).FirstOrDefault();
                if (obj != null)
                {
                    var cityID = obj.City == null ? 0 : int.Parse(obj.City);
                    vm.CityAreaList = _repository.CityAreas.Where(x => x.CityId == cityID).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

                    vm.AccountID = id;
                    vm.CompanyId = obj.CompanyId;
                    vm.Name = obj.Name;
                    vm.AccountTitle = obj.AccountTite;
                    vm.MobileNo = obj.MobileNo;
                    vm.PhoneNo = obj.PhoneNo;
                    vm.OpeningBalance = obj.OpeningBalance;
                    vm.OpeningBalanceDate = obj.OpeningBalanceDate.HasValue ? obj.OpeningBalanceDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                    vm.CNIC = obj.CNIC;
                    vm.Address = obj.Address;
                    vm.City = obj.City;
                    vm.CityAreaID = obj.CityArea ?? 0;
                    vm.STR = obj.STR;
                    vm.NTN = obj.NTN;
                    vm.IsDelete = obj.IsDelete ?? false;
                    vm.IsActive = obj.IsActive ?? false;
                    vm.IsCashAccount = obj.IsCashAccount ?? false;
                    vm.AccountTypeID = obj.AccountTypeID ?? 0;
                    vm.EmployeeID = obj.EmployeeID ?? 0;
                }
            }
            else // get
            {
                var cityID = 0;
                vm.CityAreaList = _repository.CityAreas.Where(x => x.CityId == cityID).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

                vm.AccountID = 0;
                vm.CompanyId = 0;
                vm.Name = "";
                vm.AccountTitle = "";
                vm.MobileNo = "";
                vm.PhoneNo = "";
                vm.OpeningBalance = 0;
                vm.OpeningBalanceDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.CNIC = "";
                vm.Address = "";
                vm.City = "";
                vm.CityAreaID = 0;
                vm.STR = "";
                vm.NTN = "";
                vm.IsDelete = false;
                vm.IsActive = true;
                vm.IsCashAccount = false;
                vm.EmployeeID = 0;
            }

            return View(vm);
        }

        public JsonResult SaveAccount(AddEditAccountViewModel model)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        DateTime OpeningBalanceDate = string.IsNullOrEmpty(model.OpeningBalanceDate) ? DateTime.Now : DateTime.ParseExact(model.OpeningBalanceDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        var obj = (model.AccountID == null || model.AccountID == 0) ? new Account() : context.Accounts.FirstOrDefault(x => x.ID == model.AccountID);

                        obj.AccountTypeID = 2;
                        obj.CompanyId = model.CompanyId;
                        obj.Name = model.Name;
                        obj.AccountTite = model.AccountTitle;
                        obj.MobileNo = model.MobileNo;
                        obj.PhoneNo = model.PhoneNo;
                        obj.OpeningBalance = model.OpeningBalance;
                        obj.OpeningBalanceDate = OpeningBalanceDate;
                        obj.CNIC = model.CNIC;
                        obj.Address = model.Address;
                        obj.City = model.City;
                        obj.CityArea = model.CityAreaID;
                        obj.STR = model.STR;
                        obj.NTN = model.NTN;
                        obj.EmployeeID = model.EmployeeID;

                        if (model.AccountID == null || model.AccountID == 0)
                        {
                            obj.IsDelete = false;
                            obj.IsActive = true;
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedBy = GetCurrentUserName();
                            obj.RoleID = GetCurrentUserRole();
                            obj.IsCashAccount = model.IsCashAccount;
                            context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                        }
                        else
                        {
                            obj.IsDelete = obj.IsDelete;
                            obj.IsActive = obj.IsActive;
                            obj.CreatedDate = obj.CreatedDate;
                            obj.CreatedBy = obj.CreatedBy;

                            obj.ModifyDate = DateTime.Now;
                            obj.ModifyBy = GetCurrentUserName();
                            obj.RoleID = GetCurrentUserRole();
                            obj.ID = model.AccountID.Value;
                            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                        }
                        context.SaveChanges();
                        dbContextTransaction.Commit();

                        return Json(new { status = "Success", message = "Error", detail = "Account has been saved!", Result = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        public ActionResult CustomerDelete(int Id)
        {
            var obj = _repository.Accounts.Where(x => x.ID == Id).FirstOrDefault();
            if (obj != null)
            {
                obj.IsDelete = true;
                obj.IsActive = false;
                _repository.SaveChanges();
            }
            return Redirect(GetWebsiteUrl() + "/Customer/CustomerList");
        }

    }
}