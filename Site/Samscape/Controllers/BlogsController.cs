using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Models;
using ViewModels;
using Helpers;
using System.Text.RegularExpressions;

namespace Samscape.Controllers
{
    public class BlogsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();


        public ActionResult Index()
        {
            var blogs = db.Blogs.Include(b => b.BlogGroup).Where(b => b.IsDeleted == false).OrderByDescending(b => b.CreationDate);
            return View(blogs.ToList());
        }


        public ActionResult Create()
        {
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/Blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);

                    fileupload.SaveAs(physicalFilename);

                    blog.ImageUrl = newFilenameUrl;
                }
                #endregion
                blog.IsDeleted = false;
                blog.CreationDate = DateTime.Now;
                blog.LastModifiedDate = DateTime.Now;
                blog.Id = Guid.NewGuid();
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Blog blog, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/Blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);

                    fileupload.SaveAs(physicalFilename);

                    blog.ImageUrl = newFilenameUrl;
                }
                #endregion
                blog.LastModifiedDate = DateTime.Now;
                blog.IsDeleted = false;
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Blog blog = db.Blogs.Find(id);
            blog.IsDeleted = true;
            blog.DeletionDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

      
        readonly BaseViewModelHelper _helper = new BaseViewModelHelper();

        [AllowAnonymous]
        [Route("blog/{blogGroupUrlParam}")]
        public ActionResult List(string blogGroupUrlParam)
        {
            BlogGroup blogGroup = db.BlogGroups.Where(current => current.UrlParam == blogGroupUrlParam && current.IsActive == true && current.IsDeleted == false).FirstOrDefault();
            if (blogGroup == null)
            {
                return HttpNotFound();
            }
            BlogGroupViewModel blogGroupViewModel = new BlogGroupViewModel()
            {
                BlogGroup = blogGroup,
                Blogs = db.Blogs.Where(current => current.BlogGroupId == blogGroup.Id && current.IsActive == true && current.IsDeleted == false).ToList()
            };
            return View(blogGroupViewModel);
        }

        [AllowAnonymous]
        [Route("blog")]
        public ActionResult PureList()
        {
            BlogGroupViewModel blogGroupViewModel = new BlogGroupViewModel()
            {
               
                Blogs = db.Blogs.Where(current=>current.IsActive == true && current.IsDeleted == false).ToList()
            };
            return View(blogGroupViewModel);
        }

        [AllowAnonymous]
        [Route("blog/{groupUrlParam}/{blogUrlParam}")]
        public ActionResult Details(string groupUrlParam, string blogUrlParam)
        {
            BlogGroup blogGroup = db.BlogGroups.Where(current => current.UrlParam == groupUrlParam && current.IsDeleted==false && current.IsActive==true).FirstOrDefault();
            if (blogGroup == null)
            {
                return HttpNotFound();
            }
            Blog blog = db.Blogs.Where(current => current.UrlParam == blogUrlParam && current.IsActive == true && current.IsDeleted == false).FirstOrDefault();
            if (blog == null)
            {
                return HttpNotFound();
            }
            BlogDetailViewModel blogDetailViewModel = new BlogDetailViewModel()
            {
                
                BlogGroup = blogGroup,
                Blog = blog,
                Categories = db.BlogGroups.Where(current => current.Id != blogGroup.Id && current.IsActive == true && current.IsDeleted == false).ToList(),
                RecentBlogs = db.Blogs.Where(current => current.Id != blog.Id && current.IsActive == true && current.IsDeleted == false).OrderByDescending(current => current.CreationDate).Take(5).ToList(),
                RelatedBlogs = db.Blogs.Where(current => current.BlogGroupId == blogGroup.Id && current.Id != blog.Id && current.IsActive == true && current.IsDeleted == false).Take(2).ToList(),
                BlogComments = db.BlogComments.Where(current => current.BlogId == blog.Id && current.IsActive==true && current.IsDeleted==false).ToList()
            };
            return View(blogDetailViewModel);
        }
        [AllowAnonymous]
        public ActionResult InsertComment(string email, string name, string message, string blogId)
        {
            try
            {
                bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                if (!isEmail)
                    return Json("invalidEmail", JsonRequestBehavior.AllowGet);

                BlogComment blogComment = new BlogComment()
                {
                    Email = email,
                    Name = name,
                    Message = message,
                    BlogId = new Guid(blogId),
                    CreationDate = DateTime.Now,
                    IsDeleted = false,
                };
                db.BlogComments.Add(blogComment);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
