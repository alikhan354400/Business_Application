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
    public class CityAreaController : BaseController
    {
        public ActionResult CityList()
        {
            string UserRole = GetCurrentUserRole();
            var vm = new CityViewDataModel
            {
                CityList = _repository.Cities.ToList(),
            };
            return View(vm);
        }

        public ActionResult CityAddEdit(int? Id)
        {
            var vm = new CityViewDataModel();
            if (Id.HasValue)
            {
                var obj = _repository.Cities.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    vm.Name = obj.Name;
                    vm.Id = obj.Id;
                }
            }
            return View(vm);
        }

        public JsonResult SaveCityAddEdit(CityViewDataModel model)
        {
            if (model.Id > 0)
            {
                var obj = _repository.Cities.FirstOrDefault(x => x.Id == model.Id);
                if (obj != null)
                {
                    obj.Name = model.Name;
                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    _repository.SaveChanges();
                }
            }
            else
            {
                var obj = new   City();
                if (obj != null)
                {
                    obj.Name = model.Name;
                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Added;
                    _repository.SaveChanges();
                }
            }

            return Json(new { detail = "Data has been saved successfully!", Result = "OK" });
        }

        public ActionResult CityDelete(int Id)
        {
            var obj = _repository.Cities.Where(x => x.Id == Id).FirstOrDefault();
            if (obj != null)
            {
                _repository.Cities.Remove(obj);
                _repository.SaveChanges();
            }
            return Redirect(GetWebsiteUrl() + "/CityArea/CityList");
        }



        public ActionResult AreaList()
        {
            string UserRole = GetCurrentUserRole();
            var vm = new CityAreaAddEditViewModel
            {
                CityAreaList = _repository.BrowseAllCityArea().ToList(),
            };
            return View(vm);
        }

        public ActionResult AreaAddEdit(int? Id)
        {
            var vm = new CityAreaAddEditViewModel
            {
                CityList = _repository.Cities.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };

            if (Id.HasValue)
            {
                var obj = _repository.CityAreas.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    vm.Name = obj.Name;
                    vm.ID = obj.Id;
                    vm.CityID = obj.CityId;
                }
            }
            return View(vm);
        }

        public JsonResult SaveAddEdit(CityAreaAddEditViewModel model)
        {
            if (model.ID > 0)
            {
                var obj = _repository.CityAreas.FirstOrDefault(x => x.Id == model.ID);
                if (obj != null)
                {
                    obj.CityId = model.CityID;
                    obj.Name = model.Name;
                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    _repository.SaveChanges();
                }
            }
            else
            {
                var obj = new  CityArea();
                if (obj != null)
                {
                    obj.CityId = model.CityID;
                    obj.Name = model.Name;
                    _repository.Entry(obj).State = System.Data.Entity.EntityState.Added;
                    _repository.SaveChanges();
                }
            }

            return Json(new { detail = "Data has been saved successfully!", Result = "OK" });
        }

        public ActionResult AreaDelete(int Id)
        {
            var obj = _repository.CityAreas.Where(x => x.Id == Id).FirstOrDefault();
            if (obj != null)
            {
                _repository.CityAreas.Remove(obj);
                _repository.SaveChanges();
            }
            return Redirect(GetWebsiteUrl() + "/CityArea/AreaList");
        }

    }

}