using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models
{
    public class ProductImage : BaseEntity
    {
        [Display(Name="تصویر")]
        public string ImageUrl { get; set; }
        [Display(Name= "تصویر Thumbnail")]
        public string ThumbImageUrl { get; set; }
        [Display(Name="متن جایگزین")]
        public string Alt { get; set; }

        [Display(Name="محصول")]
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        internal class configuration : EntityTypeConfiguration<ProductImage>
        {
            public configuration()
            {
                HasRequired(p => p.Product).WithMany(t => t.ProductImages).HasForeignKey(p => p.ProductId);
            }
        }

    }
}