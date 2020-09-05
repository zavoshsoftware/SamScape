using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Models
{
   public class DatabaseContext:DbContext
    {
        static DatabaseContext()
        {
           System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.Configuration>());
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Models.ProductGroup> ProductGroups { get; set; }
        public DbSet<Models.Product> Products { get; set; }

        public System.Data.Entity.DbSet<Models.ProductImage> ProductImages { get; set; }

        public DbSet<BlogGroup> BlogGroups { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<TextType> TextTypes { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Slider> Sliders { get; set; }

        public DbSet<City> Cities { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<ZarinpallAuthority> ZarinpallAuthorities { get; set; }
        public DbSet<ShopConfiguration> ShopConfigurations { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }











    }
}
