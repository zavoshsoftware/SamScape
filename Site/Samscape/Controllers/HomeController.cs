using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace Samscape.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Home
        public ActionResult Index()
        {
            HomeViewModel homeView = new HomeViewModel()
            {
                Sliders = db.Sliders.Where(a => a.IsActive && !a.IsDeleted).OrderBy(a=>a.Order).ToList(),
                HomeProducts = db.Products.Where(a => a.IsActive && !a.IsDeleted).OrderBy(a => a.Order).Take(8).ToList(),
                HomeBlogs = db.Blogs.Where(a => a.IsActive && !a.IsDeleted).OrderBy(a => a.CreationDate).Take(3).ToList(),
                HomeProductGroups=db.ProductGroups.Where(a => a.IsActive && !a.IsDeleted && a.ParentId!=null).OrderBy(a => a.Order).Take(8).ToList(),
                MiddleBanner = db.Texts.Where(a=>a.TextType.UrlParam== "otheractivity" && a.IsActive && !a.IsDeleted).FirstOrDefault(),
                SubActivities = db.Texts.Where(a => a.TextType.UrlParam == "subactivity" && a.IsActive && !a.IsDeleted).ToList()
            };
            return View(homeView);
        }
        [Route("about")]
        public ActionResult About()
        {
            AboutViewModel aboutView = new AboutViewModel()
            {
                About = db.Texts.Where(a => a.IsActive && !a.IsDeleted && a.TextType.UrlParam == "about").FirstOrDefault(),
                WhySampet = db.Texts.Where(a => a.IsActive && !a.IsDeleted && a.TextType.UrlParam == "whysampet").ToList()
            };
            return View(aboutView);
        }

        [Route("contact")]
        public ActionResult Contact()
        {
            ContactViewModel contactView = new ContactViewModel()
            {
                Texts = db.Texts.Where(a => a.IsActive && !a.IsDeleted && a.TextType.UrlParam == "contact").ToList()
            };
            return View(contactView);
        }
        [Route("GlobalSell")]
        public ActionResult GlobalSell()
        {
            ContactViewModel contactView = new ContactViewModel()
            {
                Texts = db.Texts.Where(a => a.IsActive && !a.IsDeleted && a.TextType.UrlParam == "contact").ToList()
            };
            return View(contactView);
        }
    }
}