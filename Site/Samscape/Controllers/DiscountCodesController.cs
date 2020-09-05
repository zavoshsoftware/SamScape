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
    public class DiscountCodesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: DiscountCodes
        public ActionResult Index()
        {
            return View(db.DiscountCodes.Where(a => a.IsDeleted == false).OrderByDescending(a => a.CreationDate).ToList());
        }
        
        public ActionResult Create()
        {
            return View();
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,ExpireDate,IsPercent,Amount,IsMultiUsing,IsActive,CreationDate,CreateUserId,LastModifiedDate,IsDeleted,DeletionDate,DeleteUserId,Description")] DiscountCode discountCode)
        {
            if (ModelState.IsValid)
            {
                discountCode.IsDeleted = false;
                discountCode.CreationDate = DateTime.Now;
                discountCode.Id = Guid.NewGuid();
                db.DiscountCodes.Add(discountCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(discountCode);
        }

        // GET: DiscountCodes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscountCode discountCode = db.DiscountCodes.Find(id);
            if (discountCode == null)
            {
                return HttpNotFound();
            }
            return View(discountCode);
        }

        // POST: DiscountCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,ExpireDate,IsPercent,Amount,IsMultiUsing,IsActive,CreationDate,CreateUserId,LastModifiedDate,IsDeleted,DeletionDate,DeleteUserId,Description")] DiscountCode discountCode)
        {
            if (ModelState.IsValid)
            {
                discountCode.IsDeleted = false;
                db.Entry(discountCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(discountCode);
        }

        // GET: DiscountCodes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscountCode discountCode = db.DiscountCodes.Find(id);
            if (discountCode == null)
            {
                return HttpNotFound();
            }
            return View(discountCode);
        }

        // POST: DiscountCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DiscountCode discountCode = db.DiscountCodes.Find(id);
            discountCode.IsDeleted = true;
            discountCode.DeletionDate = DateTime.Now;

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

       

     

        //[AllowAnonymous]
        //public void ReverceOldDiscountCode(DiscountCode discount, Order currentOrder)
        //{
        //    decimal oldDiscountAmoun;
        //    if (currentOrder.DiscountAmount == null)
        //        oldDiscountAmoun = 0;
        //    else
        //        oldDiscountAmoun = currentOrder.DiscountAmount.Value;

        //    currentOrder.TotalAmount = currentOrder.TotalAmount + oldDiscountAmoun;
        //}
        //[AllowAnonymous]
        //public string SubmitDiscount(DiscountCode discount, Order currentOrder)
        //{
        //    DiscountHelper helper = new DiscountHelper();

        //    decimal discountAmount = helper.CalculateDiscountAmount(discount, currentOrder.TotalAmount);

        //    currentOrder.DiscountCodeId = discount.Id;
        //    currentOrder.DiscountAmount = discountAmount;
        //    currentOrder.TotalAmount = currentOrder.TotalAmount - discountAmount;

        //    db.SaveChanges();
        //    return currentOrder.TotalAmount.ToString("n0") + " تومان / " + discountAmount.ToString("n0") + " تومان";

        //}


        //[AllowAnonymous]
        //public string CheckCouponValidation(DiscountCode discount)
        //{
        //    if (discount == null)
        //        return "Invald";

        //    if (!discount.IsMultiUsing)
        //    {
        //        if (db.Orders.Any(current => current.DiscountCodeId == discount.Id))
        //            return "Used";
        //    }

        //    if (discount.ExpireDate < DateTime.Today)
        //        return "Expired";

        //    return "true";
        //}
        //[AllowAnonymous]
        //public Order GetUserOpenOrder(Guid userId)
        //{
        //    Order order = db.Orders.FirstOrDefault(current => current.IsDeleted == false && current.IsPaid == false);

        //    return order;
        //}
    }
}
