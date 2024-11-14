using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopPhone.Common;
using ShopPhone.Models;

namespace ShopPhone.Areas.Admin.Controllers
{

    public class CategoryController : BaseController
    {
        private ShopPhoneDbContext db = new ShopPhoneDbContext();

        // GET: Admin/Category
        [CustomAuthorizeAttribute(RoleID = "SALESMAN")] // truy cập
        public ActionResult Index()
        {
         
            ViewBag.listCate = db.Categorys.Where(m => m.status != 0).ToList();
            var list = db.Categorys.Where(m => m.status > 0).ToList();
            return View(list);
        }
        [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            
            ViewBag.listCate = db.Categorys.Where(m => m.status !=0 ).ToList();
            return View();
        }
        [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Mcategory mcategory)
        {
          
            if (ModelState.IsValid)
            {
                //category
                string slug = Mystring.ToSlug(mcategory.name.ToString());
                if (db.Categorys.Where(m=>m.slug == slug).Count()>0) {
                    Message.set_flash("Loại sản phẩm đã tồn tại trong bảng Category", "danger");
                    return View(mcategory);
                }
                //topic
              
                if (db.topics.Where(m => m.slug == slug).Count() > 0)
                {
                    Message.set_flash("Loại sản phẩm đã tồn tại trong bảng Topic", "danger");
                    return View(mcategory);
                }
                if (db.Products.Where(m => m.slug == slug).Count() > 0)
                {
                    Message.set_flash("Loại sản phẩm đã tồn tại trong bảng Product", "danger");
                    return View(mcategory);
                }
                mcategory.slug = slug;
                mcategory.created_at = DateTime.Now;
                mcategory.updated_at = DateTime.Now;
                mcategory.created_by = int.Parse(Session["Admin_id"].ToString());
                mcategory.updated_by = int.Parse(Session["Admin_id"].ToString());
                db.Categorys.Add(mcategory);
                db.SaveChanges();
                link tt_link1 = new link();
                tt_link1.slug = slug;
                tt_link1.tableId = 2;
                tt_link1.type = "category";
                tt_link1.parentId = mcategory.ID;
                db.Link.Add(tt_link1);
               
                db.SaveChanges();
                Message.set_flash("Thêm  thành công", "success");
                return RedirectToAction("index");
            }
            Message.set_flash("Thêm  Thất Bại", "danger");
            ViewBag.listCate = db.Categorys.Where(m => m.status != 0).ToList();// truyền vào
            return View(mcategory);
        }
        [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.listCate = db.Categorys.Where(m => m.status != 0).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mcategory mcategory = db.Categorys.Find(id);
            if (mcategory == null)
            {
                return HttpNotFound();
            }
            return View(mcategory);
        }

        [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Mcategory mcategory)
        {
            if (ModelState.IsValid)
            {
                string slug = Mystring.ToSlug(mcategory.name.ToString());
                mcategory.slug = slug;
                mcategory.updated_at = DateTime.Now;
                mcategory.updated_by = int.Parse(Session["Admin_id"].ToString());
                db.Entry(mcategory).State = EntityState.Modified;
                link tt_link = db.Link.Where(m => m.slug == mcategory.slug).FirstOrDefault();
                if (tt_link == null)
                {
                    link tt_link1 = new link();
                    tt_link1.slug = slug;
                    tt_link1.tableId = 2;
                    tt_link1.type = "category";
                    tt_link1.parentId = mcategory.ID;
                    db.Link.Add(tt_link1);
                }
                else
                {
                    var thisLink = db.Link.Where(m => m.tableId == 2 && m.parentId == mcategory.ID).First();
                    link tt_link3 = db.Link.Find(thisLink.ID);
                    tt_link3.slug = slug;
                    tt_link3.tableId = 2;
                    tt_link3.type = "category";
                    tt_link3.parentId = mcategory.ID;
                    db.Entry(tt_link3).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Message.set_flash("Sửa thất bại", "success");
            return View(mcategory);
        }

        [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
        //status
        public ActionResult Status(int id)
        {
            Mcategory mcategory = db.Categorys.Find(id);
            mcategory.status = (mcategory.status == 1) ? 2 : 1;
            mcategory.updated_at = DateTime.Now;
            mcategory.updated_by = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mcategory).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
        //trash
        public ActionResult trash()
        {
            var list = db.Categorys.Where(m => m.status == 0).ToList();
            return View("Trash", list);                 // danh mục
        }
        public ActionResult Deltrash(int id)
        {
            Mcategory mcategory = db.Categorys.Find(id);
            mcategory.status =  0;
            mcategory.updated_at = DateTime.Now;
            mcategory.updated_by = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mcategory).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");           // hiển thị danh mục
        }
       
        public ActionResult Retrash(int id)
        {
            Mcategory mcategory = db.Categorys.Find(id);
            mcategory.status = 2;
            mcategory.updated_at = DateTime.Now;
            mcategory.updated_by = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mcategory).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("khôi phục thành công", "success");
            return RedirectToAction("trash");
        }
        public ActionResult deleteTrash(int id)
        {
            Mcategory mcategory = db.Categorys.Find(id);
            db.Categorys.Remove(mcategory);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 sản phẩm", "success");
            return RedirectToAction("trash");
        }

    }
}
