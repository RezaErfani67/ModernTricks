using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModernTricks.Models;

namespace ModernTricks.Controllers
{
    public class ProductController : Controller
    {
        private MainDBEntities db = new MainDBEntities();
        // GET: Product

        [OutputCache(Duration = 60)]
        public ActionResult ShowGroups()
        {
            return PartialView(db.Product_Groups.Where(g=>g.Products.Any()));
        }

        [OutputCache(Duration = 60)]
        public ActionResult MenuHeader()
        {
            return PartialView(db.Product_Groups);
        }


        public ActionResult ShowTopProduct()
        {
            return PartialView(db.Products.OrderBy(p => p.ProductID).Take(3));
        }


        public ActionResult ShowProduct(Guid id)
        {
            return View(db.Products.Find(id));
        }


        public ActionResult Comment(Guid? ParentID, Guid id)
        {
            return PartialView(new Product_Comments()
            {
                ParentID = ParentID,
                ProductID = id
            });

        }

        [HttpPost]
         public ActionResult Comment(Product_Comments comment)
        {
            if (ModelState.IsValid)
            {
                comment.CommentID = Guid.NewGuid();
                comment.Date = DateTime.Now;
                db.Product_Comments.Add(comment);
                db.SaveChanges();
            }
            return PartialView("ListComment", db.Product_Comments.Where(p => p.ProductID == comment.ProductID && p.ParentID == null));
        }


        public ActionResult ListComment(Guid id)
        {
            return PartialView(db.Product_Comments.Where(p => p.ProductID == id&&p.ParentID==null));
        }
    }
}