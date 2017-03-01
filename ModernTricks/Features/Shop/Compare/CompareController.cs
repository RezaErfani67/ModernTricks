using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ModernTricks.Controllers
{
    public class CompareController : Controller
    {
        // GET: Compare
        public ActionResult AddTooComparisonList(Guid id)
        {
            List<Guid> list = new List<Guid>();
            if (Session["ComparisonList"] != null)
            {
                list = Session["ComparisonList"] as List<Guid>;
            }
            if (!list.Any(l => l == id))
            {
                list.Add(id);
            }

            Session["ComparisonList"] = list;
            return PartialView("ComparisonList", list);
        }

        public ActionResult DeleteComparisonList(Guid id)
        {
            List<Guid> list = new List<Guid>();
            if (Session["ComparisonList"] != null)
            {
                list = Session["ComparisonList"] as List<Guid>;
                int Index = list.FindIndex(p => p == id);
                list.RemoveAt(Index);
            }
            Session["ComparisonList"] = list;
            return PartialView("ComparisonList", list);
        }

        public ActionResult ComparisonList()
        {
            List<Guid> list = new List<Guid>();
            if (Session["ComparisonList"] != null)
            {
                list = Session["ComparisonList"] as List<Guid>;
            }
            return PartialView(list);
        }

        public ActionResult ShowComparisonList()
        {
            List<Guid> list = new List<Guid>();
            if (Session["ComparisonList"] != null)
            {
                list = Session["ComparisonList"] as List<Guid>;
            }
            return View(list);
        }
    }
}