using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;

namespace Arako.Controllers
{
    [Authorize(Roles = "administrator")]
    public class ShopConfigurationsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: ShopConfigurations
        public ActionResult Index()
        {
            return View(db.ShopConfigurations.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Name,Value,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] ShopConfiguration shopConfiguration)
        {
            if (ModelState.IsValid)
            {
				shopConfiguration.IsDeleted=false;
				shopConfiguration.CreationDate= DateTime.Now; 
				shopConfiguration.LastModifiedDate= DateTime.Now; 
                shopConfiguration.Id = Guid.NewGuid();
                db.ShopConfigurations.Add(shopConfiguration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shopConfiguration);
        }

        // GET: ShopConfigurations/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopConfiguration shopConfiguration = db.ShopConfigurations.Find(id);
            if (shopConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(shopConfiguration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Name,Value,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] ShopConfiguration shopConfiguration)
        {
            if (ModelState.IsValid)
            {
                shopConfiguration.LastModifiedDate= DateTime.Now;
				shopConfiguration.IsDeleted=false;
                db.Entry(shopConfiguration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shopConfiguration);
        }

        // GET: ShopConfigurations/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopConfiguration shopConfiguration = db.ShopConfigurations.Find(id);
            if (shopConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(shopConfiguration);
        }

        // POST: ShopConfigurations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ShopConfiguration shopConfiguration = db.ShopConfigurations.Find(id);
			shopConfiguration.IsDeleted=true;
			shopConfiguration.DeletionDate=DateTime.Now;
 
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
    }
}
