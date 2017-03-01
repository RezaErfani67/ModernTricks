using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using ModernTricks.Models;
using System.Web.Security;
using System.IO;
using InsertShowImage;
using System.Threading.Tasks;
using System.Net;
using System.Data.Entity;
using System.Web.UI;

using Newtonsoft.Json;
using System.Text;
using ModernTricks.Models;

namespace ModernTricks.Controllers
{
    public class NewsController : Controller
    {

        private MainDBEntities database = new MainDBEntities();
     
        #region News
        public IList<News> GetLatestNews(int pageNumber, int recordsPerPage = 3)
        {
            MainDBEntities database = new MainDBEntities();
            var skipRecords = pageNumber * recordsPerPage;
            return database.News
                        .OrderByDescending(x => x.ID)
                        .Skip(skipRecords)
                        .Take(recordsPerPage)
                        .ToList();
        }
        public ActionResult NewsIndex()
        {

            return View();
        }
        public ActionResult _NewsIndex(int Page= 1)
        {

            var list = database.News.OrderByDescending(q => q.ID).Skip((Page - 1) * 10).Take(10);
            TempData["pageIndex"] = Page;
            TempData["totalItemCount"] = database.News.Count();
            return PartialView("_NewsIndex",list);
        }

        [HttpGet]
        public ActionResult Post(int? id)
        {
            if (id == null)
                return Redirect("/");

            //todo: show the content here
            return Content("Post " + id.Value);
        }
        public ActionResult _LastNews()
        {

            return PartialView(database.News.Take(3).OrderByDescending(a => a.ID).ToList());
        }

        [Route("DetailNews/{title}")]
        public ActionResult DetailsNews(int? id, string title)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = database.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            ViewData["newsID"] = id;
            return View(news);
        }
        [RBAC]
        public ActionResult CreateNews()
        {
            return View();
        }

        [HttpPost]
        //[ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [RBAC]
        public ActionResult CreateNews([Bind(Include = "ID,Title,ShortDescription,Text")] News news, HttpPostedFileBase image)

        {
            if (ModelState.IsValid)
            {
                string imagename = "no-photo.jpg";

                if (image != null)
                {
                    imagename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    image.SaveAs(Server.MapPath("/Content/News/Images/" + imagename));
                    image.InputStream.ResizeImage(350,200,Path.Combine(Server.MapPath("/Content/News/Images"), imagename),Utilty.ImageComperssion.Normal);
                }


                database.News.Add(new News { Image = imagename, CreateDate = DateTime.Now, Title = news.Title, See = 0, ShortDescription = news.ShortDescription, Text = news.Text });
                database.SaveChanges();
                return Redirect("/News/NewsIndex");
            }

            return View(news);
        }


        [RBAC]
        public ActionResult EditNews(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News EditNews = database.News.Find(id);
            if (EditNews == null)
            {
                return HttpNotFound();
            }
            return View(EditNews);
        }


        [RBAC]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNews(News news, HttpPostedFileBase NewImage)
        {
            if (ModelState.IsValid)
            {

                if (NewImage != null)//اگر عکس انتخاب شده بود
                {
                    if (news.Image != "no-photo.jpg")
                    {
                        //System.IO.File.SetAttributes(Server.MapPath("~/Content/News/Images/" + news.Image), FileAttributes.Normal);
                        System.IO.File.Delete(Server.MapPath("~/Content/News/Images/" + news.Image));
                    }

                    news.Image = Guid.NewGuid().ToString() + Path.GetExtension(NewImage.FileName);
                    NewImage.InputStream.ResizeImage(800, 448, Path.Combine(Server.MapPath("/Content/News/Images"), news.Image));

                }

                database.Entry(news).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("newsIndex","News");
            }
            return View(news);
        }


        public string DeleteNews(int? id, int? page)
        {
            News news = database.News.Find(id);


            //delete favorites
            var favList = database.Favorites.Where(q => q.NewsID == news.ID).ToList();
            database.Favorites.RemoveRange(favList);


            //delete comments
            var comments = database.News_Comments.Where(q => q.NewsID == news.ID).ToList();
            database.News_Comments.RemoveRange(comments);



            if (news.Image != "no-photo.jpg")
            {
                System.IO.File.Delete(Server.MapPath("/Content/News/Images/" + news.Image));
            }
            database.News.Remove(news);
            database.SaveChanges();
            return "salam";
        }

        [RBAC]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedNews(int id)
        {

            News news = database.News.Find(id);

            //delete favorites
            var favList = database.Favorites.Where(q => q.News == news);
            database.Favorites.RemoveRange(favList);


            if (news.Image != "no-photo.jpg")
            {
                System.IO.File.Delete(Server.MapPath("/Content/News/Images/" + news.Image));
            }
            database.News.Remove(news);
            database.SaveChanges();
            return RedirectToAction("NewsIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
        //-------------
        #region NewsComments
            
            [HttpPost]
        public ActionResult AddComments(News_Comments comment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    database.News_Comments.Add(new News_Comments
                    {
                        NewsID = comment.NewsID,
                        ParentID = comment.ParentID,
                        Name = comment.Name,
                        Text = comment.Text,
                        Email = comment.Email,
                        IsApprove = false,
                        Date =DateTime.Now,
                       
                    });
                    database.SaveChanges();
                    return Json(new { state = "success", msg = "نظر شما پس از تایید، نمایش داده می شود." });
                }
                else
                {
                    var errorList=from x in ModelState.Keys
                    where ModelState[x].Errors.Count > 0
                    select new
                    {
                        key = x,
                        errors = ModelState[x].Errors.
                                                      Select(y => y.ErrorMessage).
                                                      ToArray()
                    };
                    return Json(new {state="warning", msg = errorList.Select(q=>q.errors).ToArray()[0]});
                }
            }
            catch(Exception e)
            {
                return Json(new { state = "error", msg = e.Message });
            }
        }
        public ActionResult _NewsComments(int newsID)
        {
            var list = database.News_Comments.Where(q=>q.NewsID==newsID && q.IsApprove==true).ToList();
            return PartialView("_NewsComments",list);
        }

        public ActionResult Page_UnApproveComments()
        {
            return View();
        }
        public ActionResult Data_UnApproveComments()
        {
            var list = from n in database.News
                       join c in database.News_Comments.Where(q => q.IsApprove == false)
                       on n.ID equals c.NewsID
                       select new { newsID = n.ID, newsTitle = n.Title, commentID = c.ID, commentName = c.Name, commentEmail = c.Email, commentText = c.Text };

            return new ContentResult { Content = JsonConvert.SerializeObject(list), ContentType = "application/json", ContentEncoding = Encoding.UTF8 };
        }

        [RBAC]
        public JsonResult ApproveComments(int id)
        {

            var comment = database.News_Comments.Find(id);
            comment.IsApprove = true;
            database.Entry(comment).State = EntityState.Modified;
            database.SaveChanges();
            return Json(new {state="success",msg="نظر مورد تایید قرار گرفت" },JsonRequestBehavior.AllowGet);
        }
        [RBAC]
        public JsonResult RejectComments(int id)
        {

            var comment = database.News_Comments.Find(id);
            database.News_Comments.Remove(comment);
            
            database.SaveChanges();
            return Json(new { state = "success", msg = "با موفقیت حذف گردید." }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
