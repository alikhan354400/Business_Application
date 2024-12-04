using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class AddEditCompanyViewModel
    {
        public int? ID { get; set; }
        public string CompanyName { get; set; }
        public string OwnerName { get; set; }
        public string Address1 { get; set; }
        public string ContactNo1 { get; set; }
        public string ContactNo2 { get; set; }
        public string Email { get; set; }
        public string RegistrationNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string STRNNumber { get; set; }
        public string RoleID { get; set; }
        public string ImagePath { get; set; }
        public HttpPostedFileBase LogoImage { get; set; }

        public AddEditCompanyViewModel()
        {
        }
    }
}