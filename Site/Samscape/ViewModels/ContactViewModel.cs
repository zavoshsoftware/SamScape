using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class ContactViewModel:_BaseViewModel
    {
        public List<Text> Texts { get; set; }
    }
}