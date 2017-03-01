using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModernTricks.Models;

namespace ModernTricks.Features.Forum
{
    public class ForumPostsController : Controller
    {
        private MainDBEntities db = new MainDBEntities();

        // GET: ForumPosts
        public ActionResult Index()
        {
            var forumPosts = db.ForumPosts.Include(f => f.ForumGroups);
            return View(forumPosts.ToList());
        }

        // GET: ForumPosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumPosts forumPosts = db.ForumPosts.Find(id);
            if (forumPosts == null)
            {
                return HttpNotFound();
            }
            return View(forumPosts);
        }

        // GET: ForumPosts/Create
        public ActionResult Create()
        {
            ViewBag.ForumGroupID = new SelectList(db.ForumGroups, "ID", "Title");
            return View();
        }

        // POST: ForumPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Text,ForumGroupID,CreatedBy,CreatedDate")] ForumPosts forumPosts)
        {
            if (ModelState.IsValid)
            {
                db.ForumPosts.Add(forumPosts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ForumGroupID = new SelectList(db.ForumGroups, "ID", "Title", forumPosts.ForumGroupID);
            return View(forumPosts);
        }

        // GET: ForumPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumPosts forumPosts = db.ForumPosts.Find(id);
            if (forumPosts == null)
            {
                return HttpNotFound();
            }
            ViewBag.ForumGroupID = new SelectList(db.ForumGroups, "ID", "Title", forumPosts.ForumGroupID);
            return View(forumPosts);
        }

        // POST: ForumPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Text,ForumGroupID,CreatedBy,CreatedDate")] ForumPosts forumPosts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forumPosts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ForumGroupID = new SelectList(db.ForumGroups, "ID", "Title", forumPosts.ForumGroupID);
            return View(forumPosts);
        }

        // GET: ForumPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumPosts forumPosts = db.ForumPosts.Find(id);
            if (forumPosts == null)
            {
                return HttpNotFound();
            }
            return View(forumPosts);
        }

        // POST: ForumPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ForumPosts forumPosts = db.ForumPosts.Find(id);
            db.ForumPosts.Remove(forumPosts);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
