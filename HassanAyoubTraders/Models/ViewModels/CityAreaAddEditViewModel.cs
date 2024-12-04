using HassanAyoubTraders.Models.EF;
using System.Collections.Generic;
using System.Web.Mvc;


namespace HassanAyoubTraders.Models.ViewModels
{
    public class CityAreaAddEditViewModel
    {
        public int ID { get; set; }
        public int? CityID { get; set; }
        public string Name{ get; set; }

        public IList<BrowseAllCityArea_Result> CityAreaList { get; set; }
        public IEnumerable<SelectListItem> CityList { get; set; }

        public CityAreaAddEditViewModel()
        {
            CityAreaList = new List<BrowseAllCityArea_Result>();
        }
    }
}