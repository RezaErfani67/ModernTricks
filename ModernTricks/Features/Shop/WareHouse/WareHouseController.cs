using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModernTricks.Models;
using ModernTricks.Common;
using ModernTricks.Models.ViewModel;

namespace ModernTricks.Controllers
{
    public class WareHouseController : Controller
    {
        private MainDBEntities db = new MainDBEntities();

        // GET: Admin/WareHouse
        public ActionResult Index()
        {

            List<WareHouseItem> list=new List<WareHouseItem>();

            foreach (var productse in db.Products)
            {
                list.Add(new WareHouseItem()
                {
                    ProductID = productse.ProductID,
                    ProductTitle = productse.Title,
                    Count = WareHouseChecker.Count(productse.ProductID)
                });
            }
            return View(list);
        }

        public ActionResult Create(Guid id)
        {
            ViewBag.TypeID = new SelectList(db.WareHouse_Types, "TypeID", "TypeTitle");
            return View(new WareHouse()
            {
                ProductID = id
            });
        }


        [HttpPost]
        public ActionResult Create(WareHouse ware)
        {
            ware.Date = DateTime.Now;
            db.WareHouse.Add(ware);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Report(Guid id)
        {
            ViewBag.ProductID = id;
            return View(db.WareHouse.Where(p => p.ProductID == id).OrderByDescending(p => p.Date).ToList());
        }
    }
}