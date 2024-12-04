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
    public class EmployeeController : BaseController
    {
        // GET: Employee
        public ActionResult EmployeeList()
        {
            var vm = new AccountViewModel();
            vm.AccountList = _repository.BrowseAccount().Where(x => x.AccountType.Equals("Employee")).ToList();
            return View(vm);
        }

        public ActionResult AddEditEmployee(int? Id)
        {
            var vm = new AddEditAccountViewModel();
            vm.CompanyList = _repository.BrowseAllCompanies().Select(x => new SelectListItem { Text = x.CompanyName, Value = x.ID.ToString() });
            vm.CityList = _repository.Cities.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            vm.AccountTypeList = _repository.BrowseCodesByType("Account Type").Select(x => new SelectListItem { Text = x.CodeValue, Value = x.ID.ToString() });
            vm.DesignationList = _repository.Designations.Select(x => new SelectListItem { Text = x.Name, Value = x.Name.ToString() });

            if (Id.HasValue) // edit
            {
                var obj = _repository.Accounts.Where(x => x.ID == Id).FirstOrDefault();
                if (obj != null)
                {
                    var cityID = obj.City == null ? 0 : int.Parse(obj.City);
                    vm.CityAreaList = _repository.CityAreas.Where(x => x.CityId == cityID).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                    vm.AccountID = Id;
                    vm.CompanyId = obj.CompanyId;
                    vm.Name = obj.Name;
                    vm.MobileNo = obj.MobileNo;
                    vm.PhoneNo = obj.PhoneNo;
                    vm.OpeningBalance = obj.OpeningBalance;
                    vm.OpeningBalanceDate = obj.OpeningBalanceDate.HasValue ? obj.OpeningBalanceDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                    vm.CNIC = obj.CNIC;
                    vm.Address = obj.Address;
                    vm.City = obj.City;
                    vm.CityAreaID = obj.CityArea ?? 0;
                    vm.HiringDate = obj.HiringDate.HasValue ? obj.HiringDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                    vm.Salary = obj.Salary;
                    vm.IsDelete = obj.IsDelete ?? false;
                    vm.IsActive = obj.IsActive ?? false;
                    vm.AccountTypeID = obj.AccountTypeID ?? 3;
                    vm.Designation = obj.Designation;
                }
            }
            else // get
            {
                var cityID = 0;
                vm.CityAreaList = _repository.CityAreas.Where(x => x.CityId == cityID).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

                vm.AccountID = 0;
                vm.CompanyId = 0;
                vm.Name = "";
                vm.MobileNo = "";
                vm.PhoneNo = "";
                vm.OpeningBalance = 0;
                vm.OpeningBalanceDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.CNIC = "";
                vm.Address = "";
                vm.City = "";
                vm.CityAreaID = 0;
                vm.HiringDate = DateTime.Now.ToString("dd/MM/yyyy");
                vm.Salary = 0;
                vm.IsDelete = false;
                vm.IsActive = true;
                vm.Designation = "";
            }

            return View(vm);
        }

        public JsonResult EmpAddEdit(AddEditAccountViewModel model)
        {
            var status = string.Empty;
            var detail = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        DateTime HiringDate = string.IsNullOrEmpty(model.HiringDate) ? DateTime.Now : DateTime.ParseExact(model.HiringDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        var obj = (model.AccountID == null || model.AccountID == 0) ? new Account() : context.Accounts.FirstOrDefault(x => x.ID == model.AccountID);

                        obj.AccountTypeID = 3;
                        obj.CompanyId = model.CompanyId;
                        obj.Name = model.Name;
                        obj.AccountTite = model.Name;
                        obj.AccountTite = model.Name;
                        obj.MobileNo = model.MobileNo;
                        obj.PhoneNo = model.PhoneNo;
                        obj.OpeningBalance = model.OpeningBalance;
                        obj.HiringDate = HiringDate;
                        obj.Salary = model.Salary;
                        obj.CNIC = model.CNIC;
                        obj.Address = model.Address;
                        obj.City = model.City;
                        obj.CityArea = model.CityAreaID;
                        obj.Designation = model.Designation;

                        if (model.AccountID == null || model.AccountID == 0)
                        {
                            obj.IsDelete = false;
                            obj.IsActive = true;
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedBy = GetCurrentUserName();
                            obj.RoleID = GetCurrentUserRole();
                            context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                            detail = "Employee has been saved!";
                        }
                        else
                        {
                            obj.IsDelete = obj.IsDelete;
                            obj.IsActive = obj.IsActive;
                            obj.CreatedDate = obj.CreatedDate;
                            obj.CreatedBy = obj.CreatedBy;
                            obj.ModifyDate = DateTime.Now;
                            obj.ModifyBy = GetCurrentUserName();
                            obj.ID = model.AccountID.Value;
                            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                            detail = "Employee has been modified!";
                        }
                        context.SaveChanges();
                        dbContextTransaction.Commit();

                        return Json(new { status = "Success", message = "Error", detail = detail, Result = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        public ActionResult EmployeeDelete(int Id)
        {
            var obj = _repository.Accounts.Where(x => x.ID == Id).FirstOrDefault();
            if (obj != null)
            {
                obj.IsDelete = true;
                obj.IsActive = false;
                _repository.SaveChanges();
            }
            return Redirect(GetWebsiteUrl() + "/Employee/EmployeeList");
        }

    }
}