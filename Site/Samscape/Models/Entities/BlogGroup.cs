using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class BlogGroup : BaseEntity
    {
        [Display(Name="گروه مطالب وبلاگ")]
        public string Title { get; set; }
        [Display(Name="خلاصه")]
        public string Summery { get; set; }
        [Display(Name="تصویر")]
        public string ImageUrl { get; set; }
        [Display(Name="پارامتر url")]
        public string UrlParam { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }

    }
}