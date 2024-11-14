using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopPhone.Models;
using PagedList;
namespace ShopPhone.Controllers
{
    public class BaivietController : Controller

    {
        // GET: Post
        ShopPhoneDbContext db = new ShopPhoneDbContext();
        [ValidateInput(false)]
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            ViewBag.page = page;
            var list = db.posts.Where(m => m.status == 1).OrderByDescending(m => m.ID).OrderBy(m=>m.ID);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [ValidateInput(false)]
        public ActionResult topic_category( String slug)
        {
            var catid = db.topics.Where(m => m.status == 1 && m.slug == slug).First();
            return View("_post_category",catid);
        }
        public ActionResult _post_off_category(int catid)
        {
            var list = db.posts.Where(m => m.status == 1 && m.topid == catid).ToList();
            return View("_post_off_category", list);
        }
        [ValidateInput(false)]
        public ActionResult topiccategory()
        {
            var list = db.topics.Where(m => m.status == 1).OrderBy(m => m.orders).ToList();
            return View("_topic_post",list);
        }

    }
}