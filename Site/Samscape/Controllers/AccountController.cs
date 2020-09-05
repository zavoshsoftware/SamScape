using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Arako.Helpers;
using Helpers;
using ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SmsIrRestful;

namespace Arako.Controllers
{
    public class AccountController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        BaseViewModelHelper baseViewModel = new BaseViewModelHelper();

        [Route("login")]
        public ActionResult Login(string ReturnUrl = "")
        {
            ViewBag.Message = "";
            ViewBag.ReturnUrl = ReturnUrl;

            LoginViewModel login = new LoginViewModel()
            {
              
            };

            return View(login);
        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                User oUser = db.Users.Include(u => u.Role)
                    .FirstOrDefault(a =>
                    a.CellNum == model.Username
                    && a.Password == model.Password
                    && a.IsActive
                    && a.IsDeleted == false);

                if (oUser != null)
                {
                    var ident = new ClaimsIdentity(
                      new[] { 
              // adding following 2 claim just for supporting default antiforgery provider
              new Claim(ClaimTypes.NameIdentifier, oUser.CellNum),
              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

              new Claim(ClaimTypes.Name,oUser.Id.ToString()),

              // optionally you could add roles if any
               new Claim(ClaimTypes.Role, oUser.Role.Name),
               new Claim(ClaimTypes.Surname, oUser.FullName),

                      },
                      DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(
                       new AuthenticationProperties
                       {
                           IsPersistent = true,
                           ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(600),

                       },
                       ident);
                    return RedirectToLocal(returnUrl, oUser.Role.Name); // auth succeed 
                }
                else
                {
                    // invalid username or password
                    TempData["WrongPass"] = "شماره موبایل و یا کلمه عبور وارد شده صحیح نمی باشد.";
                }
            }
            // If we got this far, something failed, redisplay form
            LoginViewModel login = new LoginViewModel()
            {
              

                Username = model.Username,
                Password = model.Password
            };

            return View(login);
        }




        public User CreateUserObject(string fullName, string cellNumber, Guid roleId)
        {
            User user = new User();

            user.FullName = fullName;
            user.CellNum = cellNumber;
            user.RoleId = roleId;
            user.IsActive = false;
            user.CreationDate = DateTime.Now;
            user.IsDeleted = false;
            user.Code = ReturnCode();

            return user;
        }

        public int ReturnCode()
        {
            User user = db.Users.OrderByDescending(current => current.Code).FirstOrDefault();
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
            String r = generator.Next(0, 10000).ToString("D5");
            return Convert.ToInt32(r);
        }

        private ActionResult RedirectToLocal(string returnUrl, string roleName)
        {

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                if (roleName == "Administrator")
                    return RedirectToAction("index", "Products");

                return RedirectToAction("Index", "home");
            }
        }
        public ActionResult LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut();
            }
            return Redirect("/");
        }


        //[Route("profile")]
        //[Authorize]
        //public ActionResult Profile()
        //{
        //    ProfileViewModel profile = new ProfileViewModel();
        //    profile.Brands = baseViewModel.GetMenu();
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
        //        string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

        //        Guid userId = new Guid(id);

        //        User user = db.Users.Find(userId);

        //        if (user != null)
        //        {
        //            profile.User = user;
        //            profile.Orders = db.Orders.Where(current => current.UserId == userId)
        //                .OrderByDescending(c => c.CreationDate).ToList();
        //        }
        //    }
        //    return View(profile);
        //}


        [AllowAnonymous]
        public ActionResult SendOtp(string cellNumber)
        {
            try
            {
                cellNumber = cellNumber.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");
                bool isValidMobile = Regex.IsMatch(cellNumber, @"(^(09|9)[0-9][0-9]\d{7}$)|(^(09|9)[3][12456]\d{7}$)", RegexOptions.IgnoreCase);

                if (isValidMobile)
                {
                    User user = db.Users.FirstOrDefault(current => current.CellNum == cellNumber);

                    if (user != null)
                    {
                        if (user.Role.Name == "customer")
                            SendSms.SendOtp(cellNumber, user.Password);

                        return Json("true", JsonRequestBehavior.AllowGet);
                    }


                    return Json("invalidUser", JsonRequestBehavior.AllowGet);

                }
                return Json("invalidCellNumber", JsonRequestBehavior.AllowGet);

                //else
                //{
                //    Guid userId = CreateUser(fullName, cellNumber, email, employeeType);
                //    int codeInt = CreateActivationCode(userId);
                //    code = codeInt.ToString();
                //}


                //UnitOfWork.Save();

            }

            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult CompleteRegister(string cellNumber, string fullName)
        {
            try
            {
                cellNumber = cellNumber.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");

                User user = db.Users.FirstOrDefault(current => current.CellNum == cellNumber);

                int code = 0;

                if (user == null)
                {
                    user = CreateUser(fullName, cellNumber);

                    SendSms.SendOtp(user.CellNum, user.Password);

                    return Json("true", JsonRequestBehavior.AllowGet);
                }
                return Json("false", JsonRequestBehavior.AllowGet);
            }

            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult CheckOtp(string cellNumber, string activationCode)
        {
            try
            {
                cellNumber = cellNumber.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");
                activationCode = activationCode.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9");

                User user = db.Users.FirstOrDefault(current => current.CellNum == cellNumber);

                if (user != null)
                {
                    if (user.Password == activationCode)
                    {
                        user.IsActive = true;
                        user.LastModifiedDate = DateTime.Now;
                        db.SaveChanges();

                        LoginWithOtp(user);
                        if(user.Role.Name=="customer")
                            return Json("true", JsonRequestBehavior.AllowGet);

                        else if (user.Role.Name == "administrator")
                            return Json("true-admin", JsonRequestBehavior.AllowGet);

                        else if (user.Role.Name == "partner")
                            return Json("true-partner", JsonRequestBehavior.AllowGet);

                    }
                    return Json("invalid", JsonRequestBehavior.AllowGet);
                }
                return Json("invalid", JsonRequestBehavior.AllowGet);
            }

            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public User CreateUser(string fullName, string cellNumber)
        {
            Guid roleId = new Guid("7FBF2F0B-9DF5-4C37-A004-9C98173A26DD");

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

        public void LoginWithOtp(User oUser)
        {
            var ident = new ClaimsIdentity(
                new[] { 
                    // adding following 2 claim just for supporting default antiforgery provider
                    new Claim(ClaimTypes.NameIdentifier, oUser.CellNum),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                    new Claim(ClaimTypes.Name,oUser.Id.ToString()),

                    // optionally you could add roles if any
                    new Claim(ClaimTypes.Role, oUser.Role.Name),
                    new Claim(ClaimTypes.Surname, oUser.FullName),

                },
                DefaultAuthenticationTypes.ApplicationCookie);

            HttpContext.GetOwinContext().Authentication.SignIn(
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(600),

                },
                ident);

        }




    }
}