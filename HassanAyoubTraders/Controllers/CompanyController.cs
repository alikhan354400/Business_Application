using HassanAyoubTraders.Controllers.BaseControllers;
using HassanAyoubTraders.Custom;
using HassanAyoubTraders.Models.EF;
using HassanAyoubTraders.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Controllers
{
    [SiteAuthAttribute(Roles = "Filer,Non-Filer")]
    public class CompanyController : BaseController
    {
        // GET: Company

        public ActionResult CompanyList()
        {
            var vm = new CompanyViewModel();
            vm.CompanyList = _repository.BrowseAllCompanies().ToList();
            return View(vm);
        }

        public ActionResult CompanyAddEdit(int? id)
        {
            var vm = new AddEditCompanyViewModel();

            if (id.HasValue) // edit
            {
                var obj = _repository.CompanyInformations.Where(x => x.ID == id).FirstOrDefault();
                if (obj != null)
                {
                    vm.ID = id;
                    vm.CompanyName = obj.CompanyName;
                    vm.OwnerName = obj.OwnerName;
                    vm.Address1 = obj.Address1;
                    vm.ContactNo1 = obj.ContactNo1;
                    vm.ContactNo2 = obj.ContactNo2;
                    vm.Email = obj.Email;
                    vm.RegistrationNumber = obj.RegistrationNumber;
                    vm.ReferenceNumber = obj.ReferenceNumber;
                    vm.STRNNumber = obj.STRNNumber;
                    if (System.IO.File.Exists(Server.MapPath(obj.ImagePath)))
                        vm.ImagePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["WebsiteUrl"], obj.ImagePath.Replace("~", ""));
                    else
                        vm.ImagePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["WebsiteUrl"], "/Content/images/no_image.png");
                }
            }
            else // get
            {
                vm.ID = 0;
                vm.CompanyName = "";
                vm.OwnerName = "";
                vm.Address1 = "";
                vm.ContactNo1 = "";
                vm.ContactNo2 = "";
                vm.Email = "";
                vm.RegistrationNumber = "";
                vm.ReferenceNumber = "";
                vm.STRNNumber = "";
                vm.ImagePath = "~/Content/images/no_image.png";
                vm.ImagePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["WebsiteUrl"], "/Content/images/no_image.png");
            }

            return View(vm);
        }

        public JsonResult SaveCompany([Bind(Include = "ID, OwnerName, CompanyName, Address1, ContactNo1, ContactNo2, Email, RegistrationNumber, ReferenceNumber, STRNNumber")] AddEditCompanyViewModel model)
        {
            var status = string.Empty;

            using (var context = new HassanAyoubDBEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var obj = (model.ID == null || model.ID == 0) ? new CompanyInformation() : context.CompanyInformations.FirstOrDefault(x => x.ID == model.ID);

                        obj.CompanyName = model.CompanyName;
                        obj.OwnerName = model.OwnerName;
                        obj.Address1 = model.Address1;
                        obj.ContactNo1 = model.ContactNo1;
                        obj.ContactNo2 = model.ContactNo2;
                        obj.Email = model.Email;
                        obj.RegistrationNumber = model.RegistrationNumber;
                        obj.ReferenceNumber = model.ReferenceNumber;
                        obj.STRNNumber = model.STRNNumber;
                        //obj.ImagePath = "~/Content/images/no_image.png";

                        HttpPostedFileBase file = Request.Files.Count == 0 ? null : Request.Files[0]; // your file
                        //var file = model.LogoImage;
                        if (file != null)
                        {
                            var imgPath = "~/Documents/Logo/UploadCompanyLog/";
                            if (!Directory.Exists(Server.MapPath(imgPath)))
                                Directory.CreateDirectory(Server.MapPath(imgPath));

                            imgPath = string.Format("{0}{1}", imgPath, file.FileName);
                            file.SaveAs(Server.MapPath(imgPath));
                            obj.ImagePath = imgPath;

                            //BinaryReader reader = new BinaryReader(file.InputStream);
                            //imagebyte = reader.ReadBytes(file.ContentLength);
                            //StoreImage img = new StoreImage();
                            //img.ImageTitle = file.FileName;
                            //img.ImageByte = imagebyte;
                            //img.ImagePath = "/UploadImage/" + file.FileName;
                            //db.StoreImages.Add(img);
                            //db.SaveChanges();
                            //imgId = img.ImageId;
                        }

                        if (model.ID == null || model.ID == 0)
                        {
                            obj.RoleId = "Non-Filer";
                            context.Entry(obj).State = System.Data.Entity.EntityState.Added;
                        }
                        else
                        {
                            obj.ID = model.ID.Value;
                            context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                        }
                        context.SaveChanges();
                        dbContextTransaction.Commit();

                        return Json(new { status = "Success", message = "Error", detail = "Company has been saved!", Result = "OK" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(new { status = "Failed", message = "Error", detail = ex.Message, Result = "FAIL" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        public ActionResult CompanyDelete(int Id)
        {
            var obj = _repository.CompanyInformations.Where(x => x.ID == Id).FirstOrDefault();
            if (obj != null)
            {
                _repository.CompanyInformations.Remove(obj);
                _repository.SaveChanges();
            }
            return Redirect(GetWebsiteUrl() + "/Company/CompanyList");
        }
    }
}