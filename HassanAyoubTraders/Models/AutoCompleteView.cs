using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HassanAyoubTraders.Models
{
    public class AutoCompleteView
    {
        public List<ItemView> Items { set; get; }//iphone 4, iphone 5, samsung galaxy s8...
    }

    public class ItemView
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public string Icon { set; get; }
    }
}