using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShopPhone.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        //thừa kế lớp Controller
        // GET: Admin/Base
        // kiểm tra đăng nhập
        // khởi tạo đăng nhập khi ng dùng chưa đăng nhập
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Session["Admin_id"].Equals(""))
            {
                RouteValueDictionary route = new RouteValueDictionary(new { Controller = "Auth", Action = "Login" });
                filterContext.Result = new RedirectToRouteResult(route);
                return;
            }
        }
    }
}