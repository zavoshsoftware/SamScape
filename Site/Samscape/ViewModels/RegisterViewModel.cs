using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class RegisterViewModel:_BaseViewModel
    {
        [Display(Name="نام و نام خانوادگی*")]
        [Required(ErrorMessage = "مقدار {0} را وارد نمایید")]
        public string FullName { get; set; }
        [Display(Name="شماره موبایل*")]
        [Required(ErrorMessage = "مقدار {0} را وارد نمایید")]
        public string CellNumber { get; set; }
        [Display(Name= "کلمه عبور*")]
        [Required(ErrorMessage = "مقدار {0} را وارد نمایید")]
        public string Password { get; set; }
        [Display(Name= "تکرار کلمه عبور*")]
        [Required(ErrorMessage = "مقدار {0} را وارد نمایید")]
        public string RepeatPassword { get; set; }
    
    }
}