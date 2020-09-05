using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Arako.Helpers;
using Eshop.Helpers;
using Helpers;
using Models;
using Models;
using SmsIrRestful;
using ViewModels;

namespace Arako.Controllers
{
    public class BasketController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        BaseViewModelHelper _baseViewModel = new BaseViewModelHelper();
        ZarinPalHelper zp = new ZarinPalHelper();

        [Route("cart")]
        [HttpPost]
        public ActionResult AddToCart(string code, string qty)
        {
            SetCookie(code, qty);
            return Json("true", JsonRequestBehavior.AllowGet);
        }


        [Route("Basket")]
        public ActionResult Basket(string qty, string code)
        {
            CartViewModel cart = new CartViewModel();

           
            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

            cart.Products = productInCarts;

            decimal subTotal = GetSubtotal(productInCarts);

            decimal shiping = GetShippmentAmount(subTotal);
           
            cart.SubTotal = subTotal.ToString("n0") + " تومان";

            cart.ShipmentAmount = shiping.ToString("n0") + " تومان";

            decimal discountAmount = GetDiscount();

            cart.DiscountAmount = discountAmount.ToString("n0") + " تومان";

            cart.Total = (subTotal - discountAmount + shiping).ToString("n0");

            cart.Provinces = db.Provinces.OrderBy(c => c.Title).ToList();

            return View(cart);
        }

        [Route("Basket/remove/{code}")]
        public ActionResult RemoveFromBasket(string code)
        {
            string[] coockieItems = GetCookie();


            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                string[] coockieItem = coockieItems[i].Split('^');

                if (coockieItem[0] == code)
                {
                    string removeArray = coockieItem[0] + "^" + coockieItem[1];
                    coockieItems = coockieItems.Where(current => current != removeArray).ToArray();
                    break;
                }
            }

            string cookievalue = null;
            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                cookievalue = cookievalue + coockieItems[i] + "/";
            }

            HttpContext.Response.Cookies.Set(new HttpCookie("basket")
            {
                Name = "basket",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });

            return RedirectToAction("Basket");
        }
        [AllowAnonymous]
        [Route("Basket/update/{code}/{qty}")]
        public ActionResult UpdateBasket(string code , string qty)
        {
            string[] coockieItems = GetCookie();


            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                string[] coockieItem = coockieItems[i].Split('^');

                if (coockieItem[0] == code)
                {
                    coockieItem[1] = qty;
                    break;
                }
            }
            string cookievalue = null;
            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                cookievalue = cookievalue + coockieItems[i] + "/";
            }

            HttpContext.Response.Cookies.Set(new HttpCookie("basket")
            {
                Name = "basket",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });

            return RedirectToAction("Basket");
        }
        [AllowAnonymous]
        public ActionResult DiscountRequestPost(string coupon)
        {
            DiscountCode discount = db.DiscountCodes.FirstOrDefault(current => current.Code == coupon);

            string result = CheckCouponValidation(discount);

            if (result != "true")
                return Json(result, JsonRequestBehavior.AllowGet);

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();
            decimal subTotal = GetSubtotal(productInCarts);

            decimal total = subTotal;

            DiscountHelper helper = new DiscountHelper();

            decimal discountAmount = helper.CalculateDiscountAmount(discount, total);

            SetDiscountCookie(discountAmount.ToString(), coupon);

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public decimal GetSubtotal(List<ProductInCart> orderDetails)
        {
            decimal subTotal = 0;

            foreach (ProductInCart orderDetail in orderDetails)
            {
                decimal amount = orderDetail.Product.Amount;

                if (orderDetail.Product.IsInPromotion)
                    amount = orderDetail.Product.DiscountAmount.Value;

                

                subTotal = subTotal + (amount * orderDetail.Quantity);
            }

            return subTotal;
        }
        public List<ProductInCart> GetProductInBasketByCoockie()
        {
            List<ProductInCart> productInCarts = new List<ProductInCart>();

            string[] basketItems = GetCookie();

            if (basketItems != null)
            {
                for (int i = 0; i < basketItems.Length - 1; i++)
                {
                    string[] productItem = basketItems[i].Split('^');

                    int productCode = Convert.ToInt32(productItem[0]);

                    Product product =
                        db.Products.FirstOrDefault(current =>
                            current.IsDeleted == false && current.Code == productCode);

                    productInCarts.Add(new ProductInCart()
                    {
                        Product = product,
                        Quantity = Convert.ToInt32(productItem[1]),
                    });
                }
            }

            return productInCarts;
        }
        public void SetCookie(string code, string quantity)
        {
            string cookievalue = null;

            if (Request.Cookies["basket"] != null)
            {
                bool changeCurrentItem = false;

                cookievalue = Request.Cookies["basket"].Value;

                string[] coockieItems = cookievalue.Split('/');

                for (int i = 0; i < coockieItems.Length - 1; i++)
                {
                    string[] coockieItem = coockieItems[i].Split('^');

                    if (coockieItem[0] == code)
                    {
                        coockieItem[1] = (Convert.ToInt32(coockieItem[1]) + 1).ToString();
                        changeCurrentItem = true;
                        coockieItems[i] = coockieItem[0] + "^" + coockieItem[1];
                        break;
                    }
                }

                if (changeCurrentItem)
                {
                    cookievalue = null;
                    for (int i = 0; i < coockieItems.Length - 1; i++)
                    {
                        cookievalue = cookievalue + coockieItems[i] + "/";
                    }

                }
                else
                    cookievalue = cookievalue + code + "^" + quantity + "/";

            }
            else
                cookievalue = code + "^" + quantity + "/";

            HttpContext.Response.Cookies.Set(new HttpCookie("basket")
            {
                Name = "basket",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
        }

        public string[] GetCookie()
        {
            if (Request.Cookies["basket"] != null)
            {
                string cookievalue = Request.Cookies["basket"].Value;

                string[] basketItems = cookievalue.Split('/');

                return basketItems;
            }

            return null;
        }

        [AllowAnonymous]
        public string CheckCouponValidation(DiscountCode discount)
        {
            if (discount == null)
                return "Invald";

            if (!discount.IsMultiUsing)
            {
                if (db.Orders.Any(current => current.DiscountCodeId == discount.Id))
                    return "Used";
            }

            if (discount.ExpireDate < DateTime.Today)
                return "Expired";

            return "true";
        }


        public void SetDiscountCookie(string discountAmount, string discountCode)
        {
            HttpContext.Response.Cookies.Set(new HttpCookie("discount")
            {
                Name = "discount",
                Value = discountAmount + "/" + discountCode,
                Expires = DateTime.Now.AddDays(1)
            });
        }
        public decimal GetDiscount()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('/');
                    return Convert.ToDecimal(basketItems[0]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }

        public decimal GetShippmentAmount(decimal subTotal)
        {
            decimal shiping = 0;
            if (subTotal < ShopConfig.GetFreeShipingLimit())
                shiping = ShopConfig.GetShipingAmount();


            return shiping;
        }



        public ActionResult FillCities(string id)
        {
            Guid provinceId = new Guid(id);
            //   ViewBag.cityId = ReturnCities(provinceId);
            var cities = db.Cities.Where(c => c.ProvinceId == provinceId).OrderBy(current => current.Title).ToList();
            List<CityItemViewModel> cityItems = new List<CityItemViewModel>();
            foreach (Models.City city in cities)
            {
                cityItems.Add(new CityItemViewModel()
                {
                    Text = city.Title,
                    Value = city.Id.ToString()
                });
            }
            return Json(cityItems, JsonRequestBehavior.AllowGet);
        }



        [AllowAnonymous]
        public ActionResult CheckUser(string cellNumber, string fullname)
        {
            try
            {
                cellNumber = cellNumber.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");

                bool isValidMobile = Regex.IsMatch(cellNumber, @"(^(09|9)[0-9][0-9]\d{7}$)|(^(09|9)[3][12456]\d{7}$)", RegexOptions.IgnoreCase);

                if (!isValidMobile)
                    return Json("invalidMobile", JsonRequestBehavior.AllowGet);
                 
                User user = db.Users.FirstOrDefault(current =>
                    current.CellNum == cellNumber && current.IsDeleted == false);

                string code;

                if (user != null)
                {
                    code = user.Password;
                }

                else
                {
                   User oUser = CreateUser(fullname, cellNumber);
                  
                    code = oUser.Password;
                }


                db.SaveChanges();


                SendSms.SendOtp(cellNumber, code);

                return Json("true", JsonRequestBehavior.AllowGet);

            }

            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }


   


        public User CreateUser(string fullName, string cellNumber)
        {
            Guid roleId = new Guid("7fbf2f0b-9df5-4c37-a004-9c98173a26dd");
 

            User user = new User()
            {
                CellNum = cellNumber,
                FullName = fullName,
                RoleId = roleId,
                CreationDate = DateTime.Now,
                IsDeleted = false,
                Code = ReturnCode(),
                IsActive = false,
                Id = Guid.NewGuid(),
                Password = RandomCode().ToString()
        };
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }

        public int ReturnCode()
        {
     
            User user = db.Users.Where(current => current.IsDeleted==false).OrderByDescending(current => current.Code).FirstOrDefault();
            if (user != null)
            {
                return user.Code + 1;
            }
            else
            {
                return 300001;
            }
        }

  
      

        private Random random = new Random();
        public int RandomCode()
        {
            Random generator = new Random();
            String r = generator.Next(0, 100000).ToString("D5");
            return Convert.ToInt32(r);
        }






        [AllowAnonymous]
        public ActionResult Finalize(string cellnumber, string postal, string address, string city, string fullname, string activationCode)
        {
            try
            {
                cellnumber = cellnumber.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3")
                    .Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8")
                    .Replace("۹", "9");

                activationCode = activationCode.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3")
                    .Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8")
                    .Replace("۹", "9");

                User user = db.Users.FirstOrDefault(current => current.CellNum == cellnumber);

                if (user != null)
                {
                    if (user.Password == activationCode)
                    {
                        user.IsActive = true;
                        user.LastModifiedDate = DateTime.Now;
                        db.SaveChanges();
                        
                        List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                        Order order = ConvertCoockieToOrder(productInCarts);

                        if (order != null)
                        {
                            order.UserId = user.Id;
                            order.DeliverFullName = fullname;
                            order.DeliverCellNumber = cellnumber;
                            order.Address = address;
                            order.PostalCode = postal;
                            order.CityId = new Guid(city);



                            OrderStatus orderStatus = db.OrderStatuses.FirstOrDefault(current => current.Code == 2);
                            if (orderStatus != null)
                                order.OrderStatusId = orderStatus.Id;

                            db.SaveChanges();
                            RemoveCookie();

                            string res = "";

                            if (order.TotalAmount == 0)
                                res = "freecallback?orderid=" + order.Id;

                            else
                                res = zp.ZarinPalRedirect(order, order.TotalAmount);

                            return Json(res, JsonRequestBehavior.AllowGet);
                        }
                        return Json("false", JsonRequestBehavior.AllowGet);


                    }
                    return Json("invalid", JsonRequestBehavior.AllowGet);
                }

                return Json("false", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }


        public void RemoveCookie()
        {
            if (Request.Cookies["basket"] != null)
            {
                Response.Cookies["basket"].Expires = DateTime.Now.AddDays(-1);
            }
        }
        public decimal GetTotalAmount(decimal? subtotal, decimal? discount, decimal? shippment)
        {
            decimal discountAmount = 0;
            if (discount != null)
                discountAmount = (decimal)discount;

            decimal shipmentAmount = 0;
            if (shippment != null)
                shipmentAmount = (decimal)shippment;

            if (subtotal == null)
                subtotal = 0;

            return (decimal)subtotal - discountAmount + shipmentAmount;
        }

        public Order ConvertCoockieToOrder(List<ProductInCart> products)
            {
                try
                {
                    CodeCreator codeCreator = new CodeCreator();

                    Order order = new Order();

                    order.Id = Guid.NewGuid();
                    order.IsActive = true;
                    order.IsDeleted = false;
                    order.IsPaid = false;
                    order.CreationDate = DateTime.Now;
                    order.LastModifiedDate = DateTime.Now;
                    order.Code = codeCreator.ReturnOrderCode();
                    order.OrderStatusId = db.OrderStatuses.FirstOrDefault(current => current.Code == 1).Id;

                    decimal subTotal = GetSubtotal(products);
                    order.SubTotal = subTotal;


                    order.DiscountAmount = GetDiscount();
                    order.DiscountCodeId = GetDiscountId();
                    order.ShippingAmount = GetShippmentAmount(subTotal);

                    order.TotalAmount = Convert.ToDecimal(order.SubTotal + order.ShippingAmount - order.DiscountAmount);


                    db.Orders.Add(order);
                
                    
                    foreach (ProductInCart product in products)
                    {
                        decimal amount = product.Product.Amount;

                       

                         if (product.Product.IsInPromotion)
                            amount = product.Product.DiscountAmount.Value;

                        OrderDetail orderDetail = new OrderDetail()
                        {
                            ProductId = product.Product.Id,
                            Quantity = product.Quantity,
                            Amount = amount * product.Quantity,
                            IsDeleted = false,
                            IsActive = true,
                            CreationDate = DateTime.Now,
                            OrderId = order.Id,
                            Price = amount
                        };

                        db.OrderDetails.Add(orderDetail);
                    }

                    return order;
                }
                catch (Exception e)
                {
                    return null;
                }
            }


            public Guid? GetDiscountId()
            {
                if (Request.Cookies["discount"] != null)
                {
                    try
                    {
                        string cookievalue = Request.Cookies["discount"].Value;

                        string[] basketItems = cookievalue.Split('/');

                        DiscountCode discountCode =
                            db.DiscountCodes.FirstOrDefault(current => current.Code == basketItems[1]);

                        return discountCode?.Id;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                return null;
            }




        [Route("callback")]
        public ActionResult CallBack(string authority, string status)
        {
            String Status = status;
            CallBackViewModel callBack = new CallBackViewModel()
            {
               
        };

            if (Status != "OK")
            {
                callBack.IsSuccess = false;
            }

            else
            {
                try
                {
                    string merchantId = db.ShopConfigurations.FirstOrDefault(c => c.Name == "zarinpal").Value;

                    var zarinpal = ZarinPal.ZarinPal.Get();
                    zarinpal.EnableSandboxMode();
                    String Authority = authority;
                    long Amount = GetAmountByAuthority(Authority);

                    var verificationRequest = new ZarinPal.PaymentVerification(merchantId, Amount, Authority);
                    var verificationResponse = zarinpal.InvokePaymentVerification(verificationRequest);
                    if (verificationResponse.Status == 100)
                    {
                        Order order = GetOrderByAuthority(authority);
                        if (order != null)
                        {
                            order.IsPaid = true;
                            order.PaymentDate = DateTime.Now;
                            order.RefId = verificationResponse.RefID;
                            order.LastModifiedDate=DateTime.Now;

                            db.SaveChanges();

                            callBack.IsSuccess = true;
                            callBack.OrderCode = order.Code.ToString();
                            callBack.RefrenceId = verificationResponse.RefID;
                            callBack.TotalAmount = order.TotalAmount.ToString("N0") + " تومان";
                            // UpdateUserPoint(order);
                          
                        }
                        else
                        {
                            callBack.IsSuccess = false;
                            callBack.RefrenceId = "سفارش پیدا نشد";
                        }
                    }
                    else
                    {
                        callBack.IsSuccess = false;
                        callBack.RefrenceId = verificationResponse.Status.ToString();
                    }
                }
                catch (Exception e)
                {
                    callBack.IsSuccess = false;
                    callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
                }
            }
            return View(callBack);

        }

        [Route("PaymentafterShippment")]
        public ActionResult CallBackFree(string code)
        {
            CallBackViewModel callBack = new CallBackViewModel();
            


            try
            {
                callBack.IsSuccess = true;
                callBack.OrderCode = code;
            }
            catch (Exception e)
            {
                callBack.IsSuccess = false;
                callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
            }

            return View(callBack);

        }

        public void ChangeStockAfterPayment(Guid orderId)
        {
            List<OrderDetail> orderDetails = db.OrderDetails.Where(current =>
                current.OrderId == orderId && current.IsDeleted == false && current.IsActive == true).ToList();

            foreach (OrderDetail orderDetail in orderDetails)
            {
                Product product = db.Products.FirstOrDefault(current => current.Id == orderDetail.ProductId);

                if (product != null)
                {
                    product.Stock = product.Stock
                                    - orderDetail.Quantity;
                }
            }
        }


        public long GetAmountByAuthority(string authority)
        {
            ZarinpallAuthority zarinpallAuthority =
                db.ZarinpallAuthorities.FirstOrDefault(c => c.Authority == authority);
              

            if (zarinpallAuthority != null)
                return Convert.ToInt64(zarinpallAuthority.Amount);

            return 0;
        }


        public Order GetOrderByAuthority(string authority)
        {
            ZarinpallAuthority zarinpallAuthority =
                db.ZarinpallAuthorities.FirstOrDefault(c => c.Authority == authority);

            if (zarinpallAuthority != null)
                return zarinpallAuthority.Order;

            else
                return null;
        }
    }
}