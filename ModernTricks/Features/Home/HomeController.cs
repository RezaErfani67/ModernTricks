using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using ModernTricks.Models;

namespace ModernTricks.Controllers
{
    public class HomeController : Controller
    {

        private MainDBEntities database = new MainDBEntities();
        #region test

        public ActionResult Index()
        {
            return View(database.Gallery.Take(10).OrderByDescending(x => x.ID).ToList());
            // FormsAuthentication.SetAuthCookie("erfani", true);

        }
        public ActionResult KhademIndex()
        {
            return View(database.Gallery.Take(10).
                Union(database.Gallery.OrderByDescending(x => x.ID).Take(12)).ToList());
        }
        public ActionResult test()
        {
            return View();
        }
        [RBAC]
        public ActionResult About()
        {
            if (this.HasRole("Administrator"))
            {
                //Perform additional tasks and/or extract additional data from 
                //database into view model/viewbag due to administrative privileges...                
            }

            return View(database.REPORTS.First());
        }

        public ActionResult Contact(bool SuccessSend = false)
        {
            ViewBag.ISOK = SuccessSend;
            return View();
        }
        public ActionResult SendEmailForContact(string name, string email, string subject, string body)
        {
            SendMail.SendEmailGmail.Send("rezaerfani67@gmail.com", subject + " از طرف: " + name, body + "</br>" + "این نامه از طرف" + email + "ارسال شده است.");


            return RedirectToAction("Contact", new { SuccessSend = true });
        }

        #endregion

        #region Gallery
        public ActionResult Gallery()
        {
            return View(database.Gallery.Take(50).ToList());
        }
        [RBAC]
        public ActionResult CreateGallery()
        {
            return View();
        }
        [RBAC]
        public FileContentResult Download()
        {
            var userId = HttpContext.User.Identity.Name;
            //if (id == null)
            //{
            //    throw new HttpException(404, "Not found");
            //}
            //var fzip = db.ApprovedToDownloads.SingleOrDefault(a => a.GUID == id && a.UserId == userId && a.FileLive && a.ExpireDate <= DateTime.Today);

            //string fid = id.Value.ToString();
            //string fldr = fzip.Asset.ItemName;
            //string fnom = fzip.Asset.FileName;
            //string fext = fzip.Asset.FileExtension;
            // Read bytes from disk
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Content/Gallery/images/large/2.jpg"));

            return File(fileBytes, "application/jpg", "123");

        }

        //  What are my best options for imple
        [HttpPost]
        [RBAC]
        public ActionResult CreateGallery(IEnumerable<HttpPostedFileBase> files, string Alt, bool? IsUsedInSlider)
        {
            if(IsUsedInSlider == null)
            {
                IsUsedInSlider = false;
            }
            foreach (var file in files)
            {
                //-------------------------------------------Save File To database
                //byte[] fileData = new byte[file.InputStream.Length];
                //file.InputStream.Read(fileData, 0, Convert.ToInt32(file.InputStream.Length));

                //    var entity = new Gallery
                //    {
                //        Name = Path.GetFileName(file.FileName),
                //     //   Alt = Path.GetExtension(file.FileName),
                //        FileData = fileData,
                //    };
                //    database.Gallery.Add(entity);
                //    database.SaveChanges();

                //-----------------------------------------------

                if (file != null && file.ContentLength > 0)
                {
                    var FileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(Server.MapPath("~/Content/Gallery/images/large"), FileName));

                    file.InputStream.ResizeImage(200, 150, Path.Combine(Server.MapPath("~/Content/Gallery/images/thumbs"), FileName));
                    database.Gallery.Add(new Gallery { Name = FileName, Alt = Alt,IsUsedInSlider=IsUsedInSlider });
                    database.SaveChanges();
                    //new ImageResizer().Resize(Server.MapPath("/Images/ProductImages/Image/" + products.ProductID + "_" + Image.FileName),
                    //    Server.MapPath("/Images/ProductImages/thumbnails/" + products.ProductID + "_" + Image.FileName));

                }
            }
            return View();
        }
        #endregion

        #region Video
        [HttpGet]
        public ActionResult PageManageVideoAndPicture()
        {
            return View();
        }
        [RBAC]
      
        public ActionResult CreateVideo(Videos video)
        {

            database.Videos.Add(new Videos { Name = video.Name, Title = video.Title, IsTeaching = false });
            database.SaveChanges();
            return View("CreateGallery");
        }

        [HttpGet]
        [RBAC]
        public ActionResult DataShowVideos()
        {
            var list = database.Videos.Where(q => q.IsTeaching == false).Select(q => new {ID=q.ID,Title= q.Title,Name= q.Name }).ToList();
            return new ContentResult { Content = JsonConvert.SerializeObject(list), ContentType = "application/json", ContentEncoding = Encoding.UTF8 };
        }
        public ActionResult DataShowPicture()
        {
            var list = database.Gallery.Select(q => new { ID = q.ID, Name = q.Name }).ToList();
            return new ContentResult { Content = JsonConvert.SerializeObject(list), ContentType = "application/json", ContentEncoding = Encoding.UTF8 };
        }

        [RBAC]
        public ActionResult DeleteVideo(int id)
        {
            Videos video = database.Videos.Find(id);


            //delete favorites
            var favList = database.Favorites.Where(q => q.VideosID == video.ID);
            database.Favorites.RemoveRange(favList);


            database.Videos.Remove(video);
            database.SaveChanges();
            return Json(new {state="success",msg="ویدیو با موفقیت حذف شد" },JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeletePicture(int id)
        {
            Gallery pic = database.Gallery.Find(id);
            System.IO.File.Delete(Server.MapPath("~/Content/Gallery/images/thumbs/" + pic.Name));
            System.IO.File.Delete(Server.MapPath("~/Content/Gallery/images/large/" + pic.Name));
            database.Gallery.Remove(pic);
            database.SaveChanges();
            return Json(new { state = "success", msg = "عکس با موفقیت حذف شد" }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult _LastVideos()
        {
            var list = database.Videos.Where(q => q.IsTeaching == false).OrderByDescending(a => a.ID).Take(9).ToList();
            return PartialView("_LastVideos", list);
        }

        #endregion

        #region TeachingVideo
        public ActionResult UploadVideoForTeach(HttpPostedFileBase file, string title)
        {
            if (file != null && file.ContentLength > 0)
            {
                var FileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                file.SaveAs(Path.Combine(Server.MapPath("~/Content/Video/Teaching"), FileName));
                database.Videos.Add(new Videos { Name = FileName, Title = title, IsTeaching = true });
                database.SaveChanges();
            }
            return View("CreateGallery");
        }
        //----------------------------------------------------------------------------
        [RBAC]
        //Just For SysAdmin
        public ActionResult ShowUsers(string email)
        {
            if (email!="")
            {
                var list = database.USERS.Where(q => q.EMail.Contains(email)).Select(x => new { Username = x.Username, Email = x.EMail }).ToList();
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(list, new JsonSerializerSettings
                    {

                    }),
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8
                };
            }
            else { return Json(false,JsonRequestBehavior.AllowGet); }
        }
        [RBAC]
        //Just For SysAdmin
        public ActionResult ShowTeachingVideos(string title)
        {
            if (title!= "")
            {
                var list = database.Videos.Where(q => q.Title.Contains(title) && q.IsTeaching == true).Select(x => new { Title = x.Title, VideoID = x.ID ,VideoName=x.Name }).ToList();
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(list, new JsonSerializerSettings
                    {

                    }),
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8
                };
            }
            else { return Json(false, JsonRequestBehavior.AllowGet); }
        }

        [RBAC]
        public ActionResult ShowApproveVideoPage()
        {

            return View();
        }

        [RBAC]
        public ActionResult ApproveVideoForUser(int video,string user)
        {
            if (!database.LNK_User_Video.Any(p => p.VideoID == video && p.Username == user))
            {
                database.LNK_User_Video.Add(new LNK_User_Video { VideoID = video, Username = user });
                database.SaveChanges();
                return Json(new { msg = "ثبت با موفقیت انجام شد.", state = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { msg = "این ویدیو قبلا برای این کاربر باز شده است.", state = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowTeachingVideoForUser()
        {
            var list = from p in database.LNK_User_Video.Where(q => q.Username == HttpContext.User.Identity.Name)
                       join q in database.Videos
                       on p.VideoID equals q.ID
                       select new { VideoID=q.ID,VideoTitle=q.Title  };
            return new ContentResult {Content= JsonConvert.SerializeObject(list) , ContentType ="application/json" , ContentEncoding=Encoding.UTF8 };
        }
        public ActionResult DownloadTeachingVideoPage()
        {
            return View();
        }

        [RBAC]
        public FileContentResult TeachVideoDownload(int videoID)
        {
            var userId = HttpContext.User.Identity.Name;
            var fileName = database.Videos.Where(q => q.ID == videoID).Select(q=>q.Name).FirstOrDefault();
           
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Content/Video/Teaching/"+fileName));

            return File(fileBytes, "application/mp4", "دانلود ویدیو");

        }

        #endregion

        #region CkEditor
        [HttpPost]
        [RBAC]
        public ActionResult UploadImage(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor,
           string langCode)
        {
            string vImagePath = String.Empty;
            string vMessage = String.Empty;
            string vFilePath = String.Empty;
            string vOutput = String.Empty;
            try
            {
                string ex = Path.GetExtension(upload.FileName).ToLower();
                if (ex == ".jpg" || ex == ".png")
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        var vFileName = DateTime.Now.ToString("yyyyMMdd-HHMMssff") +
                                        Path.GetExtension(upload.FileName).ToLower();
                        var vFolderPath = Server.MapPath("~/Content/News/Images/");
                        if (!Directory.Exists(vFolderPath))
                        {
                            Directory.CreateDirectory(vFolderPath);
                        }
                        vFilePath = Path.Combine(vFolderPath, vFileName);
                        upload.SaveAs(vFilePath);
                        vImagePath = Url.Content("~/Content/News/Images/" + vFileName);
                        vMessage = "Image was saved correctly";
                    }
                }
            }
            catch
            {
                vMessage = "There was an issue uploading";
            }
            vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vImagePath + "\", \"" + vMessage + "\");</script></body></html>";
            return Content(vOutput);
        }
        #endregion

        
    }
}