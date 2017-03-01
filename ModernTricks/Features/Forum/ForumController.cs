
using ModernTricks.Models;
using MvcAjaxPager;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ModernTricks.Features.Forum
{
    public class ForumController : Controller
    {
        private MainDBEntities database = new MainDBEntities();
        // GET: Forum

        #region Group
        public ActionResult Index()
        {
            var list = from g in database.ForumGroups
                       join
                            u in database.USERS.Select(q => new { q.User_Id, q.FLName })
                       on
                       g.AdminUserId equals u.User_Id

                       select new VM_Forum_Index { groupID = g.ID, groupTitle = g.Title, groupManager = u.FLName ,groupParentID=g.ParentID};

            return View(list);
        }

        [HttpGet]
        [RBAC]
        public ActionResult CreateGroup()
        {
            ViewBag.AdminUserId = new SelectList(database.USERS, "FullName", "UserName","انتخاب نمایید");
            ViewBag.ParentID = new SelectList(database.ForumGroups, "ID", "Title","انتخاب نمایید");
            return View();
        }
        [HttpPost]
        [RBAC]
        public async Task<ActionResult> CreateGroup(ForumGroups group)
        {


            database.ForumGroups.Add(new ForumGroups {
                AdminUserId=group.AdminUserId,
                ID=Guid.NewGuid(),
                Title = group.Title,
                ParentID=group.ParentID
            });
            await database.SaveChangesAsync();
          
            return RedirectToAction("Index");
        }
        [HttpGet]
        [RBAC]
        public ActionResult EditGroup(Guid id)
        {
            var group = database.ForumGroups.Find(id);
            ViewBag.ParentID = new SelectList(database.ForumGroups, "ID", "Title", group.ParentID);
            ViewBag.AdminUserId = new SelectList(database.USERS, "User_Id", "UserName", group.AdminUserId);
            return View(group);
        }
        [HttpPost]
        [RBAC]
        public ActionResult EditGroup(ForumGroups group)
        {

            database.Entry(group).State = EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region Post
        public ActionResult AllPost(Guid groupID, int page = 1)
        {
            //var list = database.ForumPosts.Where(q => q.ForumGroupID == groupId).OrderByDescending(q => q.ID).Skip((page - 1) * 2).Take(2);
            var list = (from p in database.ForumPosts.Select(q => new { q.ID, q.Title, q.ForumGroupID, q.CreatedBy, q.CreatedDate }).Where(q => q.ForumGroupID == groupID)
                        join u in database.USERS.Select(q => new { q.User_Id, q.Firstname, q.Lastname, q.Pic,q.FLName })
                        on p.CreatedBy equals u.User_Id
                        select new VM_Forum_AllPost { postID = p.ID, Title = p.Title, CreatedBy_FLName = u.FLName, CreatedDate = p.CreatedDate, Pic = u.Pic }).OrderByDescending(q => q.CreatedDate);
            const int itemsPerPage = 20;
            
            var items = list.ToPagedList(page, itemsPerPage);

            var Admin = database.ForumGroups.Find(groupID).AdminUserId;

            ViewBag.Admin = Admin;

            if (!Request.IsAjaxRequest())
            {
                ViewBag.groupID = groupID;
                return View(items);
            }

            return PartialView("_AllPost", items);
        }


        public ActionResult ShowPost(Guid id)
        {
            var post = database.ForumPosts.Find(id);
            ViewBag.CreatedBy_FLName = database.USERS.Select(q => new { q.User_Id, q.Firstname, q.Lastname , FLName=q.Firstname + " "+ q.Lastname}).First(q => q.User_Id == post.CreatedBy).FLName;
            return View(post);
        }



        [HttpGet]
        [RBAC]//Standard User
        public ActionResult CreatePost(Guid groupID)
        {
            ViewBag.ForumGroupID = new SelectList(database.ForumGroups, "ID", "Title", groupID);
            return View();
        }
        [HttpPost]
        [RBAC]//Standard User
        public ActionResult CreatePost(ForumPosts post)
        {
            database.ForumPosts.Add(new ForumPosts
            {
                ID = Guid.NewGuid(),
                Title = post.Title,
                CreatedDate = DateTime.Now,
                Text = post.Text,
                CreatedBy =new Guid(HttpContext.User.Identity.Name),
                ForumGroupID = post.ForumGroupID
            });
            database.SaveChanges();
            return RedirectToAction("AllPost", new { groupID = post.ForumGroupID });
        }
        [HttpGet]
        [RBAC]//Standard User
        public ActionResult EditPost(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForumPosts item = database.ForumPosts.Find(id);

            //اگر شخص مورد نظر، پست را ایجاد کرده بود، یا ادمین کل بود
            if (item.CreatedBy.ToString() == HttpContext.User.Identity.Name || ControllerContext.Controller.IsSysAdmin())
            {
                ViewBag.ForumGroupID = new SelectList(database.ForumGroups, "ID", "Title", item.ForumGroupID);
                return View(item);
            }
            else
            {
                return RedirectToAction("Unauthorised");
            }
        }
        [HttpPost]
        [RBAC]//Standard User
        public ActionResult EditPost([Bind(Exclude = "ForumGroupID,ID")]ForumPosts post, string id, string ForumGroupID)
        {
            post.ID = new Guid(id.ToString().Decrypt());
            post.ForumGroupID = new Guid(ForumGroupID.Decrypt());
            post.CreatedBy =new Guid(HttpContext.User.Identity.Name);
            post.CreatedDate = DateTime.Now;
            database.Entry(post).State = EntityState.Modified;
          
            if (ModelState.IsValid)
            {
                database.SaveChanges();

            }
            return RedirectToAction("AllPost", new { groupID = post.ForumGroupID });

        }

        public ActionResult DeletePost(Guid id)
        {
            var list=database.ForumComments.Where(q => q.ForumPostID == id);
            database.ForumComments.RemoveRange(list);
            

            ForumPosts post = database.ForumPosts.Find(id);
            database.ForumPosts.Remove(post);
            database.SaveChanges();
            return Json(new { state = "success", msg = "با موفقیت انجام شد." }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Comment
        public ActionResult ShowComment(Guid postID)
        {
            //جوین لیست کامنت ها با لیست کاربران، برای بدست آوردن نام ونام خانوادگی
            var list1 = (from p in database.ForumComments.Where(q => q.ForumPostID == postID)
                         join u in database.USERS.Select(q => new { q.User_Id, q.Firstname, q.Lastname, q.Pic })
                         on p.CreatedBy equals u.User_Id
                         select new VM_Forum_ShowComments
                         {
                             ID = p.ID,
                             ForumGroupID = p.ForumGroupID,
                             ForumPostID = p.ForumPostID,
                             Username = u.Firstname + " " + u.Lastname,
                             CreatedDate = p.CreatedDate,
                             ParentID = p.ParentID,
                             Text = p.Text,
                         }).OrderByDescending(q => q.CreatedDate);

            ViewBag.postID = postID;

            //جوین لیست گروه ها و پست ها برای بدست آوردن ادمین گروه
            //ادمین گروه باید بتواند، کامنت را حذف کند
            var group = (from g in database.ForumGroups
                         join p in database.ForumPosts.Where(q => q.ID == postID) on
                         g.ID equals p.ForumGroupID
                         select new { admin = g.AdminUserId, groupID = g.ID }).First();

            ViewBag.groupID = group.groupID;
            ViewBag.Admin = group.admin;

            return PartialView(list1);
        }

        [RBAC]//Standard User
        public ActionResult AddComment(ForumComments comment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    database.ForumComments.Add(new ForumComments
                    {
                        ID = Guid.NewGuid(),
                        ForumGroupID = comment.ForumGroupID,
                        ForumPostID = comment.ForumPostID,
                        ParentID = comment.ParentID,
                        Text = comment.Text,
                        CreatedDate = DateTime.Now,
                        CreatedBy =new Guid(HttpContext.User.Identity.Name)

                    });
                    database.SaveChanges();
                    return Json(new { state = "success", msg = "پاسخ شما درج گردید" });
                }
                else
                {
                    var errorList = from x in ModelState.Keys
                                    where ModelState[x].Errors.Count > 0
                                    select new
                                    {
                                        key = x,
                                        errors = ModelState[x].Errors.
                                                                      Select(y => y.ErrorMessage).
                                                                      ToArray()
                                    };
                    return Json(new { state = "warning", msg = errorList.Select(q => q.errors).ToArray()[0] });
                }
            }
            catch (Exception e)
            {
                return Json(new { state = "error", msg = e.Message });
            }
        }
        public ActionResult DeleteComment(Guid? id)
        {

            ForumComments item = database.ForumComments.Find(id);

            //ادمین گروه باید مشخص شود

            var Admin_ID = (from g in database.ForumGroups
                            join p in database.ForumPosts.Where(q => q.ID == item.ForumPostID) on
                            g.ID equals p.ForumGroupID
                            select new { admin = g.AdminUserId }).First().admin;
            if (Admin_ID.ToString() == HttpContext.User.Identity.Name || ControllerContext.Controller.IsSysAdmin())
            {
                database.ForumComments.Remove(item);
                database.SaveChanges();
                return Json(new { state = "success", msg = "" });
            }
            else
            {
                return Json(new { state = "error", msg = "دسترسی لازم برای این ار را ندارید" });
            }
        }
        #endregion
    }
}