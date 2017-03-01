using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModernTricks.Models;

namespace ModernTricks.Controllers
{
    public class SearchController : Controller
    {
        private MainDBEntities db = new MainDBEntities();

        // GET: Search
        public ActionResult Index(string q)
        {
            List<Products> list=new List<Products>();

            list.AddRange(db.Product_Tags.Where(t=>t.TagTitle==q).Select(t=>t.Products).ToList());
            list.AddRange(db.Products.Where(p => 
            p.Title.Contains(q)||
            p.ShortDescription.Contains(q)||
            p.Text.Contains(q)).ToList());
            return View(list.Distinct());
            //db.Products.SqlQuery()
        }
    }
}