using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopPhone.Models;
using PagedList;
namespace ShopPhone.Controllers
{
    public class SanphamController : Controller
    {
        // GET: Sanpham
        ShopPhoneDbContext db = new ShopPhoneDbContext();
        public ActionResult index(int? page)
        {
            var list = db.Products.Where(m => m.status == 1).OrderBy(m=>m.ID);
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            ViewBag.page = page;
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult _productHome(int id)
        {
            var list = db.Products.Where(m => m.status == 1).
                Where(m => m.catid == id || m.Submenu == id).OrderBy(m => m.ID).OrderBy(m => m.ID).Take(8);
            return View("_productHome", list);
        }
        public ActionResult _productNew()
        {
            var list = db.Products.Where(m => m.status == 1).OrderByDescending(m => m.ID).Take(8);
            return View("_productHome", list);
        }
        public ActionResult _productBestSeller()
        {
            var list = db.Products.Where(m => m.status == 1).OrderByDescending(m => m.sold).Take(8);
            return View("_productHome", list);
        }
        public ActionResult _productsale()
        {
            ViewBag.title = "Sản phẩm Khuyến mãi";
            var list = db.Products.Where(m => m.status == 1).
                Where(m => m.pricesale > 0).OrderBy(m => m.ID).OrderBy(m => m.ID).Take(8);
            return View("_ProductSale", list);
        }
        public ActionResult category(String slug)
        {
            var catid = db.Categorys.Where(m =>m.slug == slug).First();
            return View("category", catid);
        }

        public ActionResult detail( String slug)
        {
            var list = db.Products.Where(m => m.status == 1 && m.slug == slug).First();
            return View(list);
        }
        public ActionResult cungloai(int catid)
        {
            var list = db.Products.Where(m => m.catid == catid && m.status == 1);
            return View("_cungloai_detail", list.ToList().Take(6));
        }
        public ActionResult subcategory(int catid,string slug, int? page)
        {
            var list = db.Products.Where(m => m.catid == catid || m.Submenu==catid && m.status == 1).OrderBy(m=>m.ID);
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            ViewBag.page = page;
            ViewBag.slug = slug;
            return View("~/Views/Sanpham/_Subcategory.cshtml", list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult SearchProduct(string keyw, int? page) {
            @ViewBag.keyw = keyw;
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            var list = db.Products.Where(m => m.status == 1 && m.name.Contains(keyw)).OrderBy(m => m.ID);
            return View("~/Views/Sanpham/_SearchProduct.cshtml", list.ToPagedList(pageNumber, pageSize));
        }

    }
}