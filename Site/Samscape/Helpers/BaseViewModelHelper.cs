using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Models;
using ViewModels;

//using ViewModels;

namespace Helpers
{
    public class BaseViewModelHelper
    {
        private DatabaseContext db = new DatabaseContext();

        public List<MenuProductGroups> GetMenuProductGroup()
        {
            List<MenuProductGroups> productGroups = new List<MenuProductGroups>();

            List<ProductGroup> parentProductGroups = db.ProductGroups.Where(c => c.ParentId == null)
                .OrderBy(current => current.Order).ToList();

            foreach (ProductGroup parentProductGroup in parentProductGroups)
            {
                productGroups.Add(new MenuProductGroups()
                {
                    ProductGroup = parentProductGroup,

                    ChildProductGroups = db.ProductGroups.Where(c => c.ParentId == parentProductGroup.Id)
                        .OrderBy(current => current.Order).ToList()
                });
            }

            return productGroups;
        }
        public List<MenuBlogs> GetMenuBlogs()
        {
            List<MenuBlogs> MenuBlogs = new List<ViewModels.MenuBlogs>();
            List<BlogGroup> blogGroups = db.BlogGroups.Where(b => b.IsActive && !b.IsDeleted).OrderBy(b => b.CreationDate).Take(2).ToList();
            foreach (BlogGroup group in blogGroups)
            {
                MenuBlogs.Add(new ViewModels.MenuBlogs()
                {
                    BlogGroup = group,
                    Blogs = db.Blogs.Where(b => b.IsActive && !b.IsDeleted && b.BlogGroupId == group.Id).OrderBy(b => b.CreationDate).Take(4).ToList()
                });
            }
               
            return MenuBlogs;
        }
        public FooterInfo GetFooterInfo()
        {
            FooterInfo footerInfo = new FooterInfo();
            footerInfo.FooterAbout = db.Texts
                .Where(current => current.TextType.UrlParam == "footerabout")
                .FirstOrDefault();
            footerInfo.ContactAddress = db.Texts
                .Where(current => current.UrlParam == "address").FirstOrDefault();
            footerInfo.ContactEmail = db.Texts
                .Where(current => current.UrlParam == "email").FirstOrDefault();
            footerInfo.ContactPhone = db.Texts
                .Where(current => current.UrlParam == "phone").FirstOrDefault();
            footerInfo.FooterBlogs = db.Blogs.Include(current=>current.BlogGroup)
                .Where(current => current.IsActive && !current.IsDeleted).OrderBy(current => current.CreationDate).Take(5).ToList();


            return footerInfo;
        }

       

    //public List<TextItem> GetHeaderTextItems()
        //{
        //    Guid headeTextTypeId=new Guid("7146dfb3-9877-4e78-9de6-101356a098ef");
        //    return db.TextItems.Where(current =>current.TextItemTypeId== headeTextTypeId&&  current.IsDeleted == false && current.IsActive).ToList();
        //}
        //public List<TextItem> GetFooterTextItems()
        //{
        //    Guid headeTextTypeId=new Guid("412f6c3e-0059-4b3f-8faf-324ce8b73138");
        //    return db.TextItems.Where(current =>current.TextItemTypeId== headeTextTypeId&&  current.IsDeleted == false && current.IsActive).ToList();
        //}

        //public Guid? GetTextTypeId(string typeTitle)
        //{
        //    if(typeTitle=="homeabout")
        //        return new Guid("2843c44d-720e-4436-81ef-a2cf9aa820d7");

        //    if(typeTitle=="homeinstagram")
        //        return new Guid("9ba46e93-8c0b-4290-a489-d863f16885f7");

        //    if(typeTitle=="homenumbers")
        //        return new Guid("0738b716-eb3a-4d19-9015-622b121ee0d5");

        //    if(typeTitle=="suggesthome")
        //        return new Guid("25608a88-42ef-4253-a72a-4e7dc9915ed9");

        //    if(typeTitle=="sidebarImage")
        //        return new Guid("4615f80b-1c8a-498e-954e-5286391e13b6");

        //    if(typeTitle== "aboutPageId")
        //        return new Guid("78206e80-9c24-4bd9-9ee7-0e2ea44db8a9");


        //    if(typeTitle== "whyus")
        //        return new Guid("8e27a70b-0d4a-43b5-b467-7c8fe26c3d1c");


        //    if(typeTitle== "contactpageId")
        //        return new Guid("c856de2e-0070-4733-980a-df99e7b7cc21");


        //    return null;
        //}


    }
}