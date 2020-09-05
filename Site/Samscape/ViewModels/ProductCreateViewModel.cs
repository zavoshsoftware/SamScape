using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace ViewModels
{
    public class ProductColorViewModel
    {
        [Display(Name = "ColorId", ResourceType = typeof(Resources.Models.Product))]
        public Guid ColorId { get; set; }
        
        [Display(Name = "موجودی")]
        public int Stock { get; set; }



        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
    }
 
}