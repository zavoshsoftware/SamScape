using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Slider:BaseEntity
    {
        [Display(Name="اولویت")]
        public int Order { get; set; }

        [Display(Name="تصویر")]
        public string ImageUrl { get; set; }

        [Display(Name="عنوان")]
        public string Title { get; set; }
        [Display(Name = "عنوان دوم")]
        public string Title2 { get; set; }

        [Display(Name="عنوان لینک")]
        public string LinkTitle { get; set; }

        [Display(Name="صفحه فرود لینک")]
        public string LandingPage { get; set; }
    }
}