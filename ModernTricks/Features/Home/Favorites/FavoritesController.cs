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
using ModernTricks.Features.Home;
using ModernTricks.Models;

namespace ModernTricks.Controllers
{
    public class FavoritesController : Controller
    {

        private MainDBEntities database = new MainDBEntities();

        //-------------
        #region Fav

        [RBAC]
        public ActionResult MyFav()
        {

            ////روش اول
            //var query1 = (from n in database.News
            //              join f in database.Favorates
            //              on
            //              n.ID equals f.ListItemID
            //              where (f.Username == HttpContext.User.Identity.Name && f.ListTitle == "News")
            //              //  select new  { Title = n.Title, ShortDescription = n.ShortDescription, Image = n.Image }).ToList();
            //              select n);

            //   var query2 = (from v in database.Videos
            //             join f in database.Favorates
            //             on
            //             v.ID equals f.ListItemID
            //             where (f.Username == HttpContext.User.Identity.Name && f.ListTitle == "Videos")
            //             select v);


            //return View(new FavViewModel { News = query1,Videos = query2});


            //روش دوم
            return View(database.Favorites.Where(i=>i.Username==HttpContext.User.Identity.Name).ToList());

        }


        [RBAC]
        public ActionResult AddToMyFav(int? newsID, int? videosID)
        {
            
            if (newsID != null)
            {
                if (!database.Favorites.Any(i => i.NewsID == newsID && i.Username == HttpContext.User.Identity.Name))
                {
                    database.Favorites.Add(new Favorites { NewsID = newsID, Username = HttpContext.User.Identity.Name });
                    database.SaveChanges();
                    return Json(new { msg = "ثبت خبر با موفقیت انجام شد" });
                }
                else
                {
                    return Json(new { msg = "این خبر قبلا در لیست علاقمندی ها ثبت شده است" });
                }
            }
            if (videosID != null)
            {
                if (!database.Favorites.Any(i => i.VideosID== videosID && i.Username == HttpContext.User.Identity.Name))
                {
                    database.Favorites.Add(new Favorites { VideosID = videosID, Username = HttpContext.User.Identity.Name });
                    database.SaveChanges();
                    return Json(new { msg = "ثبت ویدیو با موفقیت انجام شد" });
                }
                else
                {
                    return Json(new { msg = "این ویدیو قبلا در لیست علاقمندی ها ثبت شده است" });
                }
            }
            return Json(new { msg = "...." });

        }
        #endregion


    }
}