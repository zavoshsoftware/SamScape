using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class Text:BaseEntity
    {
        [Display(Name = "عنوان")]
        public string Title { get; set; }
        [Display(Name = "پارامتر URL")]
        public string UrlParam { get; set; }
        [Display(Name = "خلاصه")]
        public string Summery { get; set; }
        [Display(Name = "تصویر")]
        public string ImageUrl { get; set; }
        [Display(Name = "توضیحات")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }
        [Display(Name = "امتیاز")]
        public decimal? AverageRate { get; set; }

        public Guid TextTypeId { get; set; }
        public TextType TextType { get; set; }

        internal class configuration : EntityTypeConfiguration<Text>
        {
            public configuration()
            {
                HasRequired(p => p.TextType).WithMany(j => j.Texts).HasForeignKey(p => p.TextTypeId);
            }
        }
    }
}