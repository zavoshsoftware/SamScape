using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using ViewModels;
namespace Arako.Controllers
{
    public class OrdersController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        private BaseViewModelHelper baseViewModel = new BaseViewModelHelper();

        [Authorize(Roles = "customer,partner")]
        public ActionResult List()
        {
            User user = GetOnlineUser();

            OrderListViewModel orders = new OrderListViewModel();

           
            orders.User = user;
            orders.Orders = db.Orders.Where(c => c.UserId == user.Id).OrderByDescending(c => c.CreationDate).ToList();

            return View(orders);
        }
        [Authorize(Roles = "customer,partner")]
        public ActionResult Details(int id)
        {
            Order order = db.Orders.Where(c => c.Code == id).FirstOrDefault();

            OrderDetailViewModel orderDetail = new OrderDetailViewModel();

           
            orderDetail.Order = order;
            orderDetail.OrderDetails =db.OrderDetails.Where(c => c.OrderId == order.Id).ToList();
            

            return View(orderDetail);
        }
        public User GetOnlineUser()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            string name = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
            Guid userId = new Guid(name);


            return db.Users.Find(userId);
        }
    }
}