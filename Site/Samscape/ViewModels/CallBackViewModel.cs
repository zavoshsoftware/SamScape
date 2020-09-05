using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class CallBackViewModel:_BaseViewModel
    {
        public bool IsSuccess { get; set; }
        public string RefrenceId { get; set; }
        public string OrderCode { get; set; }
        public string TotalAmount { get; set; }
    }
}