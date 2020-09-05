using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class OrderListViewModel:_BaseViewModel
    {
        public User User { get; set; }
        public List<Order> Orders { get; set; }
    }
}