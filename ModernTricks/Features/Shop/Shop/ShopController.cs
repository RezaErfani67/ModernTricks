using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModernTricks.Common;
using ModernTricks.Models;
using ModernTricks.Features.Shop.Shop;

namespace ModernTricks.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ShopController : Controller
    {
        private MainDBEntities db = new MainDBEntities();


        public ActionResult Index()
        {
            return View();
        }
        // GET: Shop
        public ActionResult AddTooCart(Guid  id)
        {
            List<ShopCartItem> _list=new List<ShopCartItem>();

            if (Session["ShopCart"] != null)
            {
                _list = Session["ShopCart"] as List<ShopCartItem>;
            }

            var product = db.Products.Find(id);
            if (_list.Any(p => p.ProductID == id))
            {
                int index = _list.FindIndex(p => p.ProductID == id);
                if (WareHouseChecker.Count(id) > _list[index].Count)
                {
                    _list[index].Count += 1;
                }
            }
            else
            {
                _list.Add(new ShopCartItem()
                {
                    Count = 1,Title = product.Title,ProductID = id
                });
            }

            Session["ShopCart"] = _list;
            return PartialView("ShowListShopCart", _list);
        }

#region متد نمایش لیست کالاها در سبد خرید
        public ActionResult ShowListShopCart()
        {
            List<ShopCartItem> _list=new List<ShopCartItem>();

            if (Session["ShopCart"] != null)
            {
                _list = Session["ShopCart"] as List<ShopCartItem>;
            }

            return PartialView(_list);
        }
#endregion

        public ActionResult CommandShopCart(Guid id, int type)
        {
            List<ShopCartItem> _list=new List<ShopCartItem>();

            if (Session["ShopCart"] != null)
            {
                _list = Session["ShopCart"] as List<ShopCartItem>;
                int index = _list.FindIndex(p => p.ProductID == id);
                switch (type)
                {
                    case 1:
                    {
                       
                        if (WareHouseChecker.Count(id) > _list[index].Count)
                        {
                            _list[index].Count += 1;
                        }

                        break;
                    }
                    case 2:
                    {
                       

                        _list[index].Count -= 1;
                        if (_list[index].Count == 0)
                        {
                            _list.RemoveAt(index);
                        }
                        break;
                    }
                    case 3:
                    {
                        _list.RemoveAt(index);
                        break;
                    }
                }

            }


            Session["ShopCart"] = _list;
            return PartialView("ShowListShopCart", _list);
        }


        public ActionResult Order()
        {
            return View();
        }

        public ActionResult ShowOrder()
        {
            List<ShowOrderItemViewModel> _list=new List<ShowOrderItemViewModel>();

            if (Session["ShopCart"] != null)
            {
                List<ShopCartItem> _sessionlist = Session["ShopCart"] as List<ShopCartItem>;

                foreach (var item in _sessionlist)
                {
                    int Price = db.Products.Find(item.ProductID).Price;
                    ShowOrderItemViewModel order = new ShowOrderItemViewModel()
                    {
                        Count = item.Count,
                        Price = Price,
                        ProductID = item.ProductID,
                        Title = item.Title,
                        Sum = item.Count * Price
                    };

                    _list.Add(order);

                }
            }

            return PartialView(_list);
        }


        public ActionResult ChangeCount(Guid id,int count)
        {
             List<ShowOrderItemViewModel> _list=new List<ShowOrderItemViewModel>();

            if (Session["ShopCart"] != null)
            {
                List<ShopCartItem> _sessionlist = Session["ShopCart"] as List<ShopCartItem>;

                _sessionlist[_sessionlist.FindIndex(p => p.ProductID == id)].Count = count;
                Session["ShopCart"] = _sessionlist;
                foreach (var item in _sessionlist)
                {
                    int Price = db.Products.Find(item.ProductID).Price;
                    ShowOrderItemViewModel order = new ShowOrderItemViewModel()
                    {
                        Count = item.Count,
                        Price = Price,
                        ProductID = item.ProductID,
                        Title = item.Title,
                        Sum = item.Count * Price
                    };

                    _list.Add(order);

                }
            }
            return PartialView("ShowOrder", _list);

        }

        public ActionResult DeleteItemOrder(Guid id)
        {
            List<ShowOrderItemViewModel> _list = new List<ShowOrderItemViewModel>();

            if (Session["ShopCart"] != null)
            {
                List<ShopCartItem> _sessionlist = Session["ShopCart"] as List<ShopCartItem>;

               _sessionlist.RemoveAt(_sessionlist.FindIndex(p=>p.ProductID==id));

                Session["ShopCart"] = _sessionlist;

                foreach (var item in _sessionlist)
                {
                    int Price = db.Products.Find(item.ProductID).Price;
                    ShowOrderItemViewModel order = new ShowOrderItemViewModel()
                    {
                        Count = item.Count,
                        Price = Price,
                        ProductID = item.ProductID,
                        Title = item.Title,
                        Sum = item.Count * Price
                    };

                    _list.Add(order);

                }
            }
            return PartialView("ShowOrder", _list);

        }

        [Authorize]
        public ActionResult Save()
        {
            Guid UserID = db.USERS.First(u => u.User_Id == new Guid(User.Identity.Name)).User_Id;

            Orders orders=new Orders()
            {
                UserID = UserID,
                IsFinaly = false,
                OrderDate = DateTime.Now,
                
            };
            db.Orders.Add(orders);
            if (Session["ShopCart"] != null)
            {
                List<ShopCartItem> _sessionlist = Session["ShopCart"] as List<ShopCartItem>;
                foreach (var item in _sessionlist)
                {
                    db.OrderDetails.Add(new OrderDetails()
                    {
                        OrderID = orders.OrderID,
                        ProductID = item.ProductID,
                        ProductCount = item.Count,
                        ProductPrice = db.Products.Find(item.ProductID).Price
                    });

                }
            }

            db.SaveChanges();

            VM_Shop_RequestPayment RQP = new VM_Shop_RequestPayment();

            RQP.amount = 1000;
            RQP.description = "درگاه پرداخت آنلاین";
            RQP.email = "erfani.mr@outlook.com";
            RQP.MerchantCode = "";
            RQP.mobile = "09153580399";
            RQP.verifyUrl = "localhost:52717/Shop/VerifyPayment";
            


            return RedirectToAction("RequestPayment",RQP);

        }


        #region پرداخت زرین پال
        //[HttpGet]
        //public ActionResult RequestPayment()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult RequestPayment(VM_Shop_RequestPayment RQP)
        //{

        //    System.Net.ServicePointManager.Expect100Continue = false;
        //    Zarinpal.PaymentGatewayImplementationServicePortTypeClient zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();
        //    string Authority;

        //    int Status = zp.PaymentRequest(RQP.MerchantCode, RQP.amount, RQP.description, RQP.email, RQP.mobile, RQP.verifyUrl, out Authority);

        //    if (Status == 100)
        //    {
        //        Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority);
        //    }
        //    else
        //    {
        //        Response.Write("error: " + Status);
        //    }
        //    return View();
        //}


        //[HttpGet]
        //public ActionResult VerifyPayment()
        //{
        //    if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
        //    {
        //        if (Request.QueryString["Status"].ToString().Equals("OK"))
        //        {
        //            int Amount = 100;
        //            long RefID;
        //            System.Net.ServicePointManager.Expect100Continue = false;
        //            Zarinpal.PaymentGatewayImplementationServicePortTypeClient zp = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();

        //            int Status = zp.PaymentVerification("YOUR-ZARINPAL-MERCHANT-CODE", Request.QueryString["Authority"].ToString(), Amount, out RefID);

        //            if (Status == 100)
        //            {
        //                ViewBag.state = "success";
        //                ViewBag.result = "شماره پیگیری: " + RefID;
        //                return View();   
                        
        //             }
        //            else
        //            {
                        
        //                ViewBag.state = "Error";
        //                ViewBag.result = Status;
        //                return View();
                        
        //            }

        //        }
        //        else
        //        {
                  
        //            ViewBag.state = "Error";
        //            ViewBag.result =" Authority: " + Request.QueryString["Authority"].ToString() + " Status: " + Request.QueryString["Status"].ToString();
        //        }
        //    }
        //    else
        //    {
                
        //        ViewBag.state = "error";
        //        ViewBag.result = "داده ها معتبر نیستند.";

        //    }
        //    return View();

        //}
       

        #endregion
    }
}