using HassanAyoubTraders.Models.EF;
using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class ItemsController : BaseController
    {
        public ActionResult ItemList()
        {
            string UserRole = GetCurrentUserRole();
            var vm = new ItemAddEditListViewModel
            {
                ItemsList = _repository.BrowseAllItmes().Where(x => x.RoleID != null && x.RoleID.Equals(UserRole)).ToList(),
            };

            return View(vm);
        }

        public ActionResult ItemAddEdit(int? Id)
        {
            var vm = new ItemAddEditListViewModel
            {
                CompanyList = _repository.BrowseAllCompanies().Select(x => new SelectListItem { Text = x.CompanyName, Value = x.ID.ToString() }),
                VendorList = _repository.BrowseAccount().Where(x => x.AccountType.Equals("Supplier") && x.RoleID.Equals(GetCurrentUserRole())).Select(x => new SelectListItem { Text = x.FullName, Value = x.ID.ToString() }),
                UnitTypeList = _repository.UnitTypes.Select(x => new SelectListItem { Text = x.UnitType1, Value = x.UnitTypeID.ToString() })
            };

            if (Id.HasValue)
            {
                var obj = _repository.Items.FirstOrDefault(x => x.ItemID == Id);
                vm.CompanyId = obj.CompanyId;
                vm.VendorId = obj.VendorId;
                vm.Description = obj.Description;
                vm.ItemID = obj.ItemID;
                vm.ProductImage = obj.Description;
                vm.ProductName = obj.ProductName;
                vm.UnitTypeID = obj.UnitTypeID;
            }

            return View(vm);
        }

        public JsonResult SaveAddEdit(ItemAddEditListViewModel model)
        {
            var objVen = _repository.Accounts.Where(x => x.ID == model.VendorId).FirstOrDefault();
            if (model.ItemID > 0)
            {
                var obj = _repository.Items.FirstOrDefault(x => x.ItemID == model.ItemID);
                if (obj != null)
                {
                    obj.UnitTypeID = model.UnitTypeID;
                    obj.CompanyId = objVen.CompanyId;
                    obj.VendorId = model.VendorId;
                    obj.Description = model.Description;
                    obj.ProductImage = "";
                    obj.ProductName = model.ProductName;
                    obj.IsDelete = obj.IsDelete;
                    obj.ModifiedDate = DateTime.Now;
                    obj.ModifyBy = User.Identity.Name;
                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    _repository.SaveChanges();
                }
            }
            else
            {
                var obj = new Item();
                if (obj != null)
                {
                    obj.UnitTypeID = model.UnitTypeID;
                    obj.CompanyId = objVen.CompanyId;
                    obj.VendorId = model.VendorId;
                    obj.Description = model.Description;
                    obj.ProductImage = "";
                    obj.ProductName = model.ProductName;
                    obj.IsDelete = false;
                    obj.CreatedBy = User.Identity.Name;
                    obj.CreatedDate = DateTime.Now;
                    obj.RoleID = GetCurrentUserRole();
                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Added;
                    _repository.SaveChanges();
                }
            }

            return Json(new { detail = "Data has been saved successfully!", Result = "OK" });
        }

        public ActionResult ItemDelete(int Id)
        {
            var obj = _repository.Items.Where(x => x.ItemID == Id).FirstOrDefault();
            if (obj != null)
            {
                obj.IsDelete = true;
                _repository.SaveChanges();
            }
            return Redirect(GetWebsiteUrl() + "/Items/ItemList");
        }



        public ActionResult SubItemList(int? Id)
        {
            var vm = new SubItemAddEditListViewModel();

            if (Id.HasValue)
            {
                vm.SubItemList = _repository.BrowseAllSubItems().Where(x => x.ItemID == Id && x.RoleID.Equals(GetCurrentUserRole())).ToList();
            }
            else
            {
                vm.SubItemList = _repository.BrowseAllSubItems().Where(x => x.RoleID.Equals(GetCurrentUserRole())).ToList();
            }

            return View(vm);
        }

        public ActionResult SubItemAdd()
        {
            string UserRole = GetCurrentUserRole();
            var vm = new SubItemAddEditListViewModel();
            vm.ItemsList = _repository.Items.Where(x => x.IsDelete.Value == false && x.IsScrap != true && x.RoleID.Equals(UserRole)).Select(x => new SelectListItem { Text = x.ProductName, Value = x.ItemID.ToString() });

            vm.SubItemCode = "";
            vm.SubItemName = "";
            vm.SalePrice = 0;
            vm.PurchasePrice = 0;
            vm.SalesTax = 0;
            vm.UserRole = UserRole;
            vm.RPCharges = 1;
            vm.OpeningInventoryId = 0;
            vm.OpeningInventoryQty = 0;
            vm.PicesPerCorton = 0;

            return View(vm);
        }

        public ActionResult SubItemEdit(int? Id)
        {
            string UserRole = GetCurrentUserRole();
            var vm = new SubItemAddEditListViewModel();

            vm.ItemsList = _repository.Items.Where(x => x.IsDelete.Value == false && x.IsScrap != true && x.RoleID.Equals(UserRole)).Select(x => new SelectListItem { Text = x.ProductName, Value = x.ItemID.ToString() });

            if (Id.HasValue)
            {
                var obj = _repository.SubItems.FirstOrDefault(o => o.SubItemID == Id);
                if (obj != null)
                {
                    vm.SubItemCode = obj.SubItemCode;
                    vm.SubItemName = obj.SubItemName;
                    vm.SalePrice = obj.SalePrice;
                    vm.PurchasePrice = obj.PurchasePrice;
                    vm.SalesTax = obj.SalesTax;
                    vm.UserRole = UserRole;
                    vm.SubItemID = obj.SubItemID;
                    vm.ItemID = obj.ItemID;
                    vm.RPCharges = obj.RPCharges;
                    vm.OpeningInventoryId = obj.OpeningInventoryId;
                    vm.OpeningInventoryQty = obj.OpeningInventoryQty;
                    vm.PicesPerCorton = obj.PicesPerCorton;
                }
            }


            return View(vm);
        }

        public JsonResult SaveSubItem(SubItemAddEditListViewModel vm)
        {
            try
            {
                foreach (var item in vm.AddSubItemList)
                {
                    var objItem = _repository.Items.FirstOrDefault(x => x.ItemID == item.ItemID);

                    var obj = new SubItem();
                    obj.CompanyId = objItem.CompanyId;
                    obj.ItemID = item.ItemID;
                    obj.SubItemCode = item.SubItemCode;
                    obj.SubItemName = item.SubItemName;
                    obj.PurchasePrice = item.PurchasePrice;
                    obj.SalePrice = item.SalePrice;
                    obj.SalesTax = item.SalesTax;
                    obj.RPCharges = item.RPCharges;
                    obj.OpeningInventoryQty = item.OpeningInventoryQty;
                    obj.PicesPerCorton = item.PicesPerCorton;
                    obj.CreatedDate = DateTime.Now;
                    obj.CreatedBy = User.Identity.Name;
                    obj.IsDeleted = false;
                    obj.RoleID = GetCurrentUserRole();
                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Added;
                    _repository.SaveChanges();
                }

                string message = "Product has been saved!";
                return Json(new { status = "Success", message = "Error", detail = message, Result = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult EditSubItem(SubItemAddEditListViewModel vm)
        {
            try
            {
                var obj = _repository.SubItems.FirstOrDefault(o => o.SubItemID == vm.SubItemID);
                if (obj != null)
                {
                    var objItem = _repository.Items.FirstOrDefault(x => x.ItemID == vm.ItemID);

                    obj.CompanyId = objItem.CompanyId;
                    obj.ItemID = vm.ItemID;
                    obj.SubItemCode = vm.SubItemCode;
                    obj.SubItemName = vm.SubItemName;
                    obj.PurchasePrice = vm.PurchasePrice;
                    obj.SalePrice = vm.SalePrice;
                    obj.SalesTax = vm.SalesTax;
                    obj.RPCharges = vm.RPCharges;
                    obj.OpeningInventoryId = vm.OpeningInventoryId;
                    obj.OpeningInventoryQty = vm.OpeningInventoryQty;
                    obj.PicesPerCorton = vm.PicesPerCorton;
                    obj.ModifiedDate = DateTime.Now;
                    obj.ModifiedBy = User.Identity.Name;
                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    _repository.SaveChanges();

                    string message = "Item has been saved!";
                    return Json(new { status = "Success", message = "Error", detail = message, Result = "OK" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string message = "Record not found!";
                    return Json(new { status = "Success", message = "Error", detail = message, Result = "OK" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SubItemDelete(int Id)
        {
            var obj = _repository.SubItems.Where(x => x.SubItemID == Id).FirstOrDefault();
            if (obj != null)
            {
                obj.IsDeleted = true;
                _repository.SaveChanges();
            }
            return Redirect(GetWebsiteUrl() + "/Items/SubItemList");
        }

    }
}