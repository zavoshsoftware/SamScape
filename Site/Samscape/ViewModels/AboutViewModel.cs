using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class AboutViewModel:_BaseViewModel
    {
       public Text About { get; set; }
        public List<Text> WhySampet { get; set; }
    }
}