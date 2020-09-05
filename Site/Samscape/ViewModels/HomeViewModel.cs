using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class HomeViewModel:_BaseViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Product> HomeProducts { get; set; }
        public List<ProductGroup> HomeProductGroups { get; set; }

        public List<Blog> HomeBlogs { get; set; }
        public Text MiddleBanner { get; set; }
        public List<Text> SubActivities { get; set; }

    }
}