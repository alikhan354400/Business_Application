using HassanAyoubTraders.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HassanAyoubTraders.Models.ViewModels
{
    public class CityViewDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<City> CityList { get; set; }

        public CityViewDataModel()
        {
            CityList = new List<City>();
        }
    }
}