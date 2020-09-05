using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class ProductListViewModel:_BaseViewModel
    {
        public ProductGroup MainProductGroup { get; set; }
        public List<Product> Products { get; set; }
        public ProductGroup ProductGroup { get; set; }
      
    }

    
}