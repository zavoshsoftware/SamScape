using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class ShopConfiguration:BaseEntity
    {
        [Display(Name="عنوان")]
        public string Title { get; set; }
        public string Name { get; set; }
        [Display(Name="مقدار")]
        public string Value { get; set; }
    }
}