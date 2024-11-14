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
    [CustomAuthorizeAttribute(RoleID = "ADMIN")]
    public class OrdersController : BaseController
    {
        private ShopPhoneDbContext db = new ShopPhoneDbContext();

        // GET: Admin/Orders
        //danh sách đơn hàng
        public ActionResult Index()
        {
            var list = db.Orders.Where(m => m.status != 0).ToList();
            return View(list);
        }
        // thông tin chi tiết
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.customer = db.Orders.Where(m => m.ID == id).First();
            var lisst = db.Orderdetails.Where(m => m.orderid == id).ToList();
            return View("Orderdetail", lisst);
        }
        //status chỉnh sửa đơn hàng
        public ActionResult Status(int id)
        {
            Morder morder = db.Orders.Find(id);
            morder.status = (morder.status == 1) ? 2 : 1;
            morder.updated_at = DateTime.Now;
            morder.updated_by = int.Parse(Session["Admin_id"].ToString());
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        //trash trả danh sách đơn hàng bị xoá
        public ActionResult trash()
        {
            var list = db.Orders.Where(m => m.status == 0).ToList();
            return View("Trash", list);
        }
        // xoá đơn hàng , cập nhật lưu thay đổi
        public ActionResult Deltrash(int id)
        {
            Morder morder = db.Orders.Find(id);
            morder.status = 0;
            morder.updated_at = DateTime.Now;
            morder.updated_by = int.Parse(Session["Admin_id"].ToString());
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }
        // khôi phục đơn hàng đã xoá
        public ActionResult Retrash(int id)
        {
            Morder morder = db.Orders.Find(id);
            morder.status = 2;
            morder.updated_at = DateTime.Now;
            morder.updated_by = int.Parse(Session["Admin_id"].ToString());
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Khôi phục thành công", "success");
            return RedirectToAction("trash");
        }
        // xoá nhưng vẫn lưu lại để khôi phục
        public ActionResult deleteTrash(int id)
        {
            Morder morder = db.Orders.Find(id);
            db.Orders.Remove(morder);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 Đơn hàng", "success");
            return RedirectToAction("trash");
        }
        // huỷ đơn hàng
        public ActionResult Dahuy(int id)
        {
            Morder morder = db.Orders.Find(id);
            morder.status = 5;
            morder.updated_at = DateTime.Now;
            morder.updated_by = int.Parse(Session["Admin_id"].ToString());
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("index");
        }
        // đang giao hàng
        public ActionResult DangGiao(int id)
        {
            Morder morder = db.Orders.Find(id);
            morder.status = 3;
            morder.updated_at = DateTime.Now;
            morder.updated_by = int.Parse(Session["Admin_id"].ToString());
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("index");
        }
        // thay đổi trạng thái thành đã giao
        public ActionResult Dagiao(int id)
        {
            Morder morder = db.Orders.Find(id);
            morder.status = 4;
            morder.updated_at = DateTime.Now;
            morder.updated_by = int.Parse(Session["Admin_id"].ToString());
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("index");
        }
    }
}
