
using MoMo;
using Newtonsoft.Json.Linq;
using ShopPhone.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ShopPhone.Controllers
{
    public class CheckoutController : BaseController
    {
        private const string SessionCart = "SessionCart";
        ShopPhoneDbContext db = new ShopPhoneDbContext();
        public ActionResult Index()
        {
            var cart = Session[SessionCart];
            var list = new List<Cart_item>();
            if (cart != null)
            {
                list = (List<Cart_item>)cart;
            }
            return View(list);

        }
        [HttpPost]
        public ActionResult Index(Morder order)
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            int numIterations = 0;
            numIterations = rand.Next(1, 100000);
            DateTime time = DateTime.Now;

            string orderCode = numIterations + "" + time;
            double sumOrder = Convert.ToDouble(Request["sumOrder"]);
            string payment_method = Request["option_payment"];
            // Neu Ship COde
            if (payment_method.Equals("COD"))
            {
                // cap nhat thong tin sau khi dat hang thanh cong

                saveOrder(order, "COD", 2, orderCode);
                var cart = Session[SessionCart];
                var list = new List<Cart_item>();
                ViewBag.cart = (List<Cart_item>)cart;
                Session["SessionCart"] = null;
                var listProductOrder = db.Orderdetails.Where(m => m.orderid == order.ID);
                return View("payment");
            }
            if (payment_method.Equals("MoMo"))
            {
                // cap nhat thong tin sau khi dat hang thanh cong

                saveOrder(order, "MoMo", 2, orderCode);
                var cart = Session[SessionCart];
                var list = new List<Cart_item>();
                ViewBag.cart = (List<Cart_item>)cart;
                Session["SessionCart"] = null;
                var listProductOrder = db.Orderdetails.Where(m => m.orderid == order.ID);
                string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
                string partnerCode = "MOMOOJOI20210710";
                string accessKey = "iPXneGmrJH0G8FOP";
                string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
                string orderInfo = "Thanh toán hóa đơn";
                string returnUrl = "https://localhost:44394/Home/ConfirmPaymentClient";
                string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
                var tien = sumOrder;
                string amount = tien.ToString();
                string orderid = orderCode; //mã đơn hàng
                string requestId = DateTime.Now.Ticks.ToString();
                string extraData = "";

                //Before sign HMAC SHA256 signature
                string rawHash = "partnerCode=" +
                    partnerCode + "&accessKey=" +
                    accessKey + "&requestId=" +
                    requestId + "&amount=" +
                    amount + "&orderId=" +
                    orderid + "&orderInfo=" +
                    orderInfo + "&returnUrl=" +
                    returnUrl + "&notifyUrl=" +
                    notifyurl + "&extraData=" +
                    extraData;

                MoMoSecurity crypto = new MoMoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);

                //build body json request
                JObject message = new JObject
                {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

                string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

                JObject jmessage = JObject.Parse(responseFromMomo);
                return Redirect(jmessage.GetValue("payUrl").ToString());
            }
            //Neu Thanh toan Ngan Luong
            return View("payment");
        }
        //Khi huy thanh toán Ngan Luong
        public ActionResult cancel_order()
        {

            return View("cancel_order");
        }
        //Khi thanh toán Ngan Luong XOng
        public ActionResult confirm_orderPaymentOnline()
        {

            //String Token = Request["token"];
            //RequestCheckOrder info = new RequestCheckOrder();
            //info.Merchant_id = nganluongInfo.Merchant_id;
            //info.Merchant_password = nganluongInfo.Merchant_password;
            //info.Token = Token;
            //APICheckoutV3 objNLChecout = new APICheckoutV3();
            //ResponseCheckOrder result = objNLChecout.GetTransactionDetail(info);
            //if (result.errorCode=="00")
            //{
            //    var cart = Session[SessionCart];
            //    var list = new List<Cart_item>();
            //    ViewBag.cart = (List<Cart_item>)cart;
            //    Session["SessionCart"] = null;
            //    var OrderInfo = db.Orders.OrderByDescending(m=>m.ID).FirstOrDefault();
            //    ViewBag.name = OrderInfo.deliveryname;
            //    ViewBag.email = OrderInfo.deliveryemail;
            //    ViewBag.address = OrderInfo.deliveryaddress;
            //    ViewBag.code = OrderInfo.code;
            //    ViewBag.phone = OrderInfo.deliveryphone;
            //    OrderInfo.StatusPayment = 1;
            //    db.Entry(OrderInfo).State = EntityState.Modified;
            //    db.SaveChanges();
            //    ViewBag.paymentStatus = OrderInfo.StatusPayment;
            //    ViewBag.Methodpayment = OrderInfo.deliveryPaymentMethod;
            //    return View("payment");
            //}
            //else
            //{
            //     ViewBag.status = false;
            //}

            return View("confirm_orderPaymentOnline");
        }

        //function ssave order when order success!
        public void saveOrder(Morder order, string paymentMethod, int StatusPayment, string ordercode)
        {
            var cart = Session[SessionCart];
            var list = new List<Cart_item>();
            if (cart != null)
            {
                list = (List<Cart_item>)cart;
            }

            if (ModelState.IsValid)
            {

                order.code = ordercode;
                order.userid = Convert.ToInt32(Session["id"]);
                order.deliveryPaymentMethod = paymentMethod;
                order.StatusPayment = StatusPayment;
                order.created_ate = DateTime.Now;
                order.updated_by = 1;
                order.updated_at = DateTime.Now;
                order.updated_by = 1;
                order.status = 2;
                order.exportdate = DateTime.Now;
                db.Orders.Add(order);
                db.SaveChanges();
                ViewBag.name = order.deliveryname;
                ViewBag.email = order.deliveryemail;
                ViewBag.address = order.deliveryaddress;
                ViewBag.code = order.code;
                ViewBag.phone = order.deliveryphone;
                Mordersdetail orderdetail = new Mordersdetail();

                foreach (var item in list)
                {
                    float price = 0;
                    int sale = (int)item.product.pricesale;
                    if (sale > 0)
                    {
                        price = (float)item.product.price - (int)item.product.price / 100 * (int)sale * item.quantity;
                    }
                    else
                    {
                        price = (float)item.product.price * (int)item.quantity;
                    }
                    orderdetail.orderid = order.ID;
                    orderdetail.productid = item.product.ID;
                    orderdetail.priceSale = (int)item.product.pricesale;
                    orderdetail.price = item.product.price;
                    orderdetail.quantity = item.quantity;
                    orderdetail.amount = price;

                    db.Orderdetails.Add(orderdetail);
                    db.SaveChanges();
                    var updatedProduct = db.Products.Find(item.product.ID);
                    updatedProduct.number = (int)updatedProduct.number - (int)item.quantity;
                    db.Products.Attach(updatedProduct);
                    db.Entry(updatedProduct).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
        }
        //
    }
}