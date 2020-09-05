using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class TextType:BaseEntity
    {
        public TextType()
        {
            Texts =new List<Text>();
        }
        [Display(Name = "عنوان")]
        public string Title { get; set; }
        [Display(Name = "پارامتر URL")]
        public string UrlParam { get; set; }
        [Display(Name = "تصویر")]
        public string ImageUrl { get; set; }

        public virtual ICollection<Text> Texts { get; set; }
    }
}