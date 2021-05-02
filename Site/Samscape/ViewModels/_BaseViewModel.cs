 

using System.Collections.Generic;
using Models;
using Telerik.Windows.Documents.Fixed.Model.Editing.Lists;
using Helpers;

namespace ViewModels
{
    public class _BaseViewModel
    {
        readonly BaseViewModelHelper _baseViewModelHelper = new BaseViewModelHelper();
        public List<Product> MenuProducts
        {
            get { return _baseViewModelHelper.GetMenuProductGroup(); }
        }
        public FooterInfo FooterInfo
        {
            get{ return _baseViewModelHelper.GetFooterInfo(); }
        }
        public List<MenuBlogs> MenuBlogs
        {
            get { return _baseViewModelHelper.GetMenuBlogs(); }
        }
    }

    //public class MenuProductGroups
    //{
    //    public ProductGroup ProductGroup { get; set; }
    //    public List<ProductGroup> ChildProductGroups { get; set; }
    //}
    public class MenuBlogs
    {
        public BlogGroup BlogGroup { get; set; }
        public List<Blog> Blogs { get; set; }
    }

    public class FooterInfo
    {
        public Text FooterAbout { get; set; }
        public Text ContactAddress { get; set; }
        public Text ContactEmail { get; set; }
        public Text ContactPhone { get; set; }
        public List<Blog> FooterBlogs { get; set; }

    }
}