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
    public class ProductGroup:BaseEntity
    {
        public ProductGroup()
        {
            Products=new List<Product>();
            ProductGroups = new List<ProductGroup>();
        }
        [Display(Name = "Order", ResourceType = typeof(Resources.Models.ProductGroup))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Order { get; set; }

        [Display(Name = "Title", ResourceType = typeof(Resources.Models.ProductGroup))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "UrlParam", ResourceType = typeof(Resources.Models.ProductGroup))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string UrlParam { get; set; }

        [Display(Name = "PageTitle", ResourceType = typeof(Resources.Models.ProductGroup))]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string PageTitle { get; set; }

        [Display(Name = "PageDescription", ResourceType = typeof(Resources.Models.ProductGroup))]
        [StringLength(1000, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        [DataType(DataType.MultilineText)]
        public string PageDescription { get; set; }

        [Display(Name = "ImageUrl", ResourceType = typeof(Resources.Models.ProductGroup))]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string ImageUrl { get; set; }

        [Display(Name = "Summery", ResourceType = typeof(Resources.Models.ProductGroup))]
        [DataType(DataType.MultilineText)]
        public string Summery { get; set; }

        [Display(Name = "Body", ResourceType = typeof(Resources.Models.ProductGroup))]
        [UIHint("RichText")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        public string Body { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<ProductGroup> ProductGroups { get; set; }

        public virtual ProductGroup Parent { get; set; }
        public Guid? ParentId { get; set; }


        internal class configuration : EntityTypeConfiguration<ProductGroup>
        {
            public configuration()
            {
                HasRequired(p => p.Parent).WithMany(t => t.ProductGroups).HasForeignKey(p => p.ParentId);
            }
        }

    }
}