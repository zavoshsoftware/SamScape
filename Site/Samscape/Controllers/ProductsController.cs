using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using ViewModels;
using System.Text.RegularExpressions;

namespace Samscape.Controllers
{
    public class ProductsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            var products = db.Products.Where(p => p.IsDeleted == false).OrderByDescending(p => p.CreationDate).Include(p => p.ProductGroup);
            return View(products.ToList());
        }



        public ActionResult Create()
        {

            ViewBag.ProductGroupId = new SelectList(db.ProductGroups.Where(p => p.IsActive && !p.IsDeleted && p.ParentId != null).ToList(), "Id", "Title");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);

                    fileupload.SaveAs(physicalFilename);

                    product.ImageUrl = newFilenameUrl;
                }

                #endregion

                product.Code = ReturnCode();
                product.IsDeleted = false;
                product.CreationDate = DateTime.Now;
                product.Id = Guid.NewGuid();
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title", product.ProductGroupId);
            return View(product);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProductGroupId = new SelectList(db.ProductGroups.Where(p => p.IsActive && !p.IsDeleted && p.ParentId != null).ToList(), "Id", "Title", product.ProductGroupId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    string newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);

                    fileupload.SaveAs(physicalFilename);

                    product.ImageUrl = newFilenameUrl;
                }
                #endregion
                product.LastModifiedDate = DateTime.Now;
                product.IsDeleted = false;

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductGroupId = new SelectList(db.ProductGroups, "Id", "Title", product.ProductGroupId);
            return View(product);
        }


       

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Product product = db.Products.Find(id);
            product.IsDeleted = true;
            product.DeletionDate = DateTime.Now;

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

        public int ReturnCode()
        {
            Product product = db.Products.Where(current => current.IsDeleted == false).OrderByDescending(current => current.Code).FirstOrDefault();

            if (product != null)
            {
                return product.Code + 1;
            }
            else
            {
                return 1000;
            }
        }

        readonly BaseViewModelHelper _helper = new BaseViewModelHelper();
        [AllowAnonymous]
        [Route("product/list/{urlParam}")]
        public ActionResult List(string urlParam)
        {
            ProductGroup productGroup = db.ProductGroups.Where(p => p.IsActive && !p.IsDeleted && p.UrlParam == urlParam).FirstOrDefault();
            ProductListViewModel productListView = new ProductListViewModel()
            {
                MainProductGroup = db.ProductGroups.Where(p => p.IsActive && !p.IsDeleted && p.Id == productGroup.ParentId).FirstOrDefault(),
                ProductGroup = productGroup,
                Products = db.Products.Where(p => p.IsActive && !p.IsDeleted && p.ProductGroupId == productGroup.Id).ToList()
            };
            return View(productListView);
        }

        [AllowAnonymous]
        [Route("product/{code}")]
        public ActionResult Details(int code)
        {

            Product product = db.Products.FirstOrDefault(current => current.Code == code);
            ProductGroup productGroup = db.ProductGroups.Where(p => p.Id == product.ProductGroupId && p.IsActive && !p.IsDeleted).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }

            ProductDetailViewModel productDetail = new ProductDetailViewModel()
            {
                Product = product,
                //ProductGroup = productGroup,
              //  MainProductGroup = db.ProductGroups.Where(p => p.Id == productGroup.ParentId && p.IsActive && !p.IsDeleted).FirstOrDefault(),
                RelatedProducts = db.Products.Where(c => c.ProductGroupId == product.ProductGroupId && c.IsDeleted == false && c.IsActive && c.Id != product.Id)
                    .OrderBy(c => c.Order).Take(4).ToList(),
                ProductImages = db.ProductImages.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).ToList(),
                ProductComments = db.ProductComments.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).ToList()
            };

            ViewBag.Title = product.PageTitle;

            return View(productDetail);
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostSubmitComment(string name, string email, string body, string code)
        {
            try
            {
                bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                if (!isEmail)
                    return Json("InvalidEmail", JsonRequestBehavior.AllowGet);

                int productCode = Convert.ToInt32(code);

                Product product = db.Products.FirstOrDefault(c => c.Code == productCode && c.IsDeleted == false);

                if (product == null)
                    return Json("false", JsonRequestBehavior.AllowGet);

                ProductComment comment = new ProductComment();

                comment.Name = name;
                comment.Email = email;
                comment.Message = body;
                comment.CreationDate = DateTime.Now;
                comment.IsDeleted = false;
                comment.Id = Guid.NewGuid();
                comment.ProductId = product.Id;
                comment.IsActive = false;
                comment.CreationDate = DateTime.Now;


                db.ProductComments.Add(comment);
                db.SaveChanges();
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        //[AllowAnonymous]
        //[Route("category/{parentUrlParam}")]
        //public ActionResult ListByParentCategory(string parentUrlParam, string brandparam, string ageRangeparam)
        //{

        //    ProductGroup parentProductGroup = db.ProductGroups.FirstOrDefault(current => current.UrlParam == parentUrlParam);

        //    if (parentProductGroup == null)
        //        return RedirectPermanent("/category");

        //    if (parentProductGroup.ParentId != null)
        //        return RedirectPermanent("/category/" + parentProductGroup.Parent.UrlParam + "/" +
        //                                 parentProductGroup.UrlParam);

        //    string url = "category/" + parentUrlParam;

        //    ProductListViewModel productList = new ProductListViewModel()
        //    {
        //        MenuProductGroups = _helper.GetMenuProductGroup(),
        //        FooterInfo = _helper.GetFooterInfo(),

        //        ProductGroup = parentProductGroup,

        //        Products = GetParentProducts(parentProductGroup.Id),

        //        //ProductGroups =
        //        //    db.ProductGroups
        //        //        .Where(current => current.ParentId == null && current.IsDeleted == false)
        //        //        .OrderBy(current => current.Order).ToList(),


        //    };

        //    productList.Products = FilterProducts(productList.Products, brandparam, ageRangeparam, url);

        //    if (productList.Products == null)
        //        return RedirectPermanent(url);

        //    return View(productList);
        //}

        //public List<Product> FilterProducts(List<Product> products, string brandparam, string ageRangeparam, string url)
        //{
        //    if (brandparam != null)
        //    {
        //        Brand brand = db.Brands.FirstOrDefault(c => c.UrlParam == brandparam && c.IsDeleted == false && c.IsActive);

        //        if (brand == null)
        //            return null;

        //        return products.Where(current => current.BrandId == brand.Id).ToList();
        //    }

        //    if (ageRangeparam != null)
        //    {
        //        AgeRange ageRange = db.AgeRanges.FirstOrDefault(c => c.UrlParam == ageRangeparam && c.IsDeleted == false && c.IsActive);

        //        if (ageRange == null)
        //            return null;

        //        return products.Where(current => current.AgeRangeId == ageRange.Id).ToList();
        //    }

        //    return products;
        //}
        //public List<Product> GetParentProducts(Guid parentProductGroupId)
        //{
        //    List<Product> products = new List<Product>();

        //    List<ProductGroup> productGroups = db.ProductGroups
        //        .Where(c => c.ParentId == parentProductGroupId && c.IsActive && !c.IsDeleted).ToList();

        //    foreach (ProductGroup productGroup in productGroups)
        //    {
        //        List<Product> childProducts = db.Products
        //            .Where(c => c.ProductGroupId == productGroup.Id  && c.IsDeleted == false && c.IsActive).ToList();

        //        foreach (Product childProduct in childProducts)
        //        {
        //            products.Add(childProduct);
        //        }
        //    }

        //    List<Product> parentProducts = db.Products
        //        .Where(c => c.ProductGroupId == parentProductGroupId && c.IsDeleted == false && c.IsActive).ToList();

        //    foreach (Product parentProduct in parentProducts)
        //    {
        //        products.Add(parentProduct);
        //    }

        //    return products.OrderBy(current => current.Order).ToList();
        //}

        //[AllowAnonymous]
        //[Route("category/{parentUrlParam}/{urlParam}")]
        //public ActionResult ListByChildCategory(string parentUrlParam, string urlParam, string brandparam, string ageRangeparam)
        //{
        //    ProductGroup productGroup = db.ProductGroups.FirstOrDefault(current => current.UrlParam == urlParam);

        //    ProductGroup parentProductGroup = db.ProductGroups.FirstOrDefault(current => current.UrlParam == parentUrlParam);

        //    if (productGroup == null)
        //        return RedirectPermanent("/category");

        //    if (parentProductGroup == null)
        //    {
        //        if (productGroup.ParentId != null)
        //            return RedirectPermanent("/product/" + productGroup.Parent.UrlParam + "/" + urlParam);

        //        return RedirectPermanent("/category");
        //    }

        //    string url = "category/" + parentUrlParam + "/" + urlParam;
        //    ViewBag.Canonical = url;


        //    ProductListViewModel productList = new ProductListViewModel()
        //    {
        //        MenuProductGroups = _helper.GetMenuProductGroup(),
        //        FooterInfo = _helper.GetFooterInfo(),

        //        ProductGroup = productGroup,

        //        Products = db.Products
        //             .Where(current =>
        //                 current.ProductGroupId == productGroup.Id  && current.IsDeleted == false && current.IsActive)
        //             .OrderBy(current => current.Order).ToList(),

        //        //ProductGroups =
        //        //     db.ProductGroups
        //        //         .Where(current => current.ParentId == productGroup.ParentId && current.IsDeleted == false)
        //        //         .OrderBy(current => current.Order).ToList(),

        //        //Brands = db.Brands.Where(current => current.IsDeleted == false).OrderBy(current => current.Order)
        //        //     .ToList(),

        //        //AgeRanges = db.AgeRanges.Where(current => current.IsDeleted == false).OrderBy(current => current.Order)
        //        //     .ToList(),

        //        //Url = url,

        //        //IsPartner = _helper.IsPartner()
        //    };

        //productList.Products = FilterProducts(productList.Products, brandparam, ageRangeparam, url);

        //    if (productList.Products == null)
        //        return RedirectPermanent(url);

        //    return View(productList);
        //}


    }
}
