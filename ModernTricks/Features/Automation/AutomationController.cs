using ModernTricks.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ModernTricks.Features.Automation
{
    public class AutomationController : Controller
    {
        public MainDBEntities database = new MainDBEntities();
        // GET: Automation

        public ActionResult MyTask()
        {

            if (HttpContext.User.Identity.Name.ToString() != "")
            {
                ViewBag.CurrentUserID = HttpContext.User.Identity.Name.ToString();
                ViewBag.CurrentUserLFName = database.USERS.Find(new Guid(HttpContext.User.Identity.Name)).LFName;
            }

            return View();
        }


        public string DetectDetailView(Guid genericId, string parentTableName)
        {

            string htm = "";
            if (parentTableName == "BaniChav_ReturnedProducts")
            {
                var item = database.BaniChav_ReturnedProducts.FirstOrDefault(q => q.genericId == genericId);

                htm += "<div class='panel panel-info'>";
                htm += "<div class=panel-heading>مرجوعی بانی چاو</div>";
                htm += "<div class=panel-body>";
                htm += "<table class='table table-bordered'>";
                htm += "<tr><th>عنوان</th></tr>";
                htm += "<tr>";
                htm += "<td>  " + item.Title + "  </td>";
                htm += "</tr>";
                htm += "</table>";
                htm += "</div>";
                htm += "</div>";

                return htm;
            }
            return "";

        }
        public ActionResult TraceTask(string genericId)
        {
            Guid genericId_Dycrypt = new Guid(AESEncrytDecry.DecryptStringAES(genericId));
            string parentTableName = database.Automation_Generic.FirstOrDefault(q => q.ID == genericId_Dycrypt).Title;
            string parentTableHtml = DetectDetailView(genericId_Dycrypt, parentTableName);

           
            ViewData["parentTableHtml"] = parentTableHtml;

            return View();
        }
        public ActionResult TraceTask_Async(string genericId)
        {
            Guid genericId_Dycrypt = new Guid(AESEncrytDecry.DecryptStringAES(genericId));

            //اسم جدولی که باید در هدر صفحه نمایش داده شود(جدول پرنت)
            //رو داریم در میارم
            //و بعد میدیمش به  تابع دیتکت ویو
            //و اون تابع اچ تی ام ال رو میسازه و تحویل ما میده
            
            var list =
                from p in database.Automation_Tasks.Where(q => q.genericId == genericId_Dycrypt)
                join u in database.USERS on p.Responsible equals u.User_Id
                select new {p.Title, p.IsActive, p.IsDone, p.Description, u.LFName,u.Username};
            
                

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(list),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8
            };
        }
        public ActionResult FillTask_Async()
        {
            var list = database.Automation_Tasks.Where(q =>
            q.Responsible == new Guid(HttpContext.User.Identity.Name) &&
            q.IsActive == true &&
            q.IsDone == false).Select(q => new VM_Task
            {
                ID= q.ID,
                Title= q.Title,
                Html= q.Html,
                Script= q.Script,
                ApproveScript= q.ApproveScript,
                ApproveHtml= q.ApproveHtml,
                RejectHtml= q.RejectHtml,
                RejectScript= q.RejectScript,
                Description= q.Description,
                Filter1=q.Filter1,
                Filter2=q.Filter2,
                Filter3=q.Filter3,
                genericId=q.genericId,
                GenericTitle=q.Automation_Generic.Title,
                BaniChav_ReturnedProducts_Title = q.Automation_Generic.BaniChav_ReturnedProducts.FirstOrDefault().Title,
               
            });
         
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(list,
                       Formatting.None,
                       new JsonSerializerSettings
                       {
                           //مهم
                           //از خطای سیرکولار رفرنسینگ جلوگیری میکند
                           //چون کوئری ما در عمقی بیشتر میره جلو و جداولی که فارین کی دارند به این جدول، رو در میاره
                           //بنابراین باید این خط اضافه بشه تا وقتی جی سان،پارس میشه،عمق های داخلی رو هم شامل بشه
                           //نه اینکه فقط یک سطح بره داخل کوئری
                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                       }),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,
            };
        }
     
        public ActionResult FillPeople_Async()
        {
            var list = database.USERS.Select(q => new { q.User_Id, q.LFName }).ToList();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(list),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8
            };

        }
        public ActionResult FillMessage_Async()
        {
            var list = database.Automation_Messages.Select(q => new
            { 
                q.ReadBy,
                q.Description,
                q.UnreadBy,
                q.Title,
                q.ForwardLink,
             }).ToList();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(list),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8
            };

        }
        public ActionResult CreateMessage([Bind(Exclude = "ID")]Automation_Messages item)
        {
            if (ModelState.IsValid)
            {
                var name = database.USERS.Find(new Guid(HttpContext.User.Identity.Name)).LFName;
                database.Automation_Messages.Add(new Automation_Messages
                {

                    ID = Guid.NewGuid(),
                    genericId = item.genericId,
                    Title = item.Title,
                    Description=name +": " + item.Description,
                    ForwardLink = item.ForwardLink,
                    ReadBy = item.ReadBy,
                    UnreadBy = item.UnreadBy,
                });
                database.SaveChanges();
                return new ContentResult
                {
                  Content = JsonConvert.SerializeObject(new { State = "success" },
                  Formatting.None,
                  new JsonSerializerSettings
                  {
                      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                  }),
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8,
                };
            }
            else
            {
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(new { State = "error" },
                      Formatting.None,
                      new JsonSerializerSettings
                      {
                          ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                      }),
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8,
                };
            }
        }
        public async Task<ActionResult> CreateTask(Automation_Tasks task)
        {
           
            var newItem=database.Automation_Tasks.Add(task);
           await database.SaveChangesAsync();

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(newItem),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8
            };
        }
        public async Task<ActionResult> UpdateTask(Automation_Tasks task)
        {

            var newItem = database.Automation_Tasks.Find(task.ID);
            database.Entry(task).State = EntityState.Modified;
            await database.SaveChangesAsync();

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(newItem),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8
            };
        }
        public async Task<ActionResult> ActiveTask(Guid genericId,int taskOrder)
        {
            var nextItem = database.Automation_Tasks.FirstOrDefault(q => q.genericId == genericId && q.TaskOrder == taskOrder);
            if (nextItem != null)
            {
               nextItem.IsActive = true;
                nextItem.IsDone= false;
                database.Entry(nextItem).State = EntityState.Modified;
                await database.SaveChangesAsync();
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(nextItem),
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8
                };

            }
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(false),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8
            };


        }
        public async Task<ActionResult> RejectTask(Guid taskId,string message)
        {
            var name = database.USERS.Find(new Guid(HttpContext.User.Identity.Name)).LFName;
            var item = database.Automation_Tasks.Find(taskId);
            item.IsActive = false;
            item.IsDone = false;
            item.Description = item.Description + "<br/>"+ name + message;
            //حذف تسک ها
            var list=database.Automation_Tasks.Where(q=>q.TaskOrder > item.TaskOrder);
            database.Automation_Tasks.RemoveRange(list);

            database.Entry(item).State = EntityState.Modified;
            await database.SaveChangesAsync();

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(item),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8
            };
        }
        public async Task<ActionResult> DoneTask(Guid taskId)
        {

            var item = database.Automation_Tasks.Find(taskId);
            item.IsDone= true;
            database.Entry(item).State = EntityState.Modified;
            await database.SaveChangesAsync();

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(item),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8
            };
        }

        public async Task<ActionResult> ChangeResponsible(Guid genericId, int taskOrder,Guid userId)
        {
            var nextItem = database.Automation_Tasks.FirstOrDefault(q => q.genericId == genericId && q.TaskOrder == taskOrder);
            if (nextItem != null)
            {
                nextItem.Responsible = userId;
              
                database.Entry(nextItem).State = EntityState.Modified;
                await database.SaveChangesAsync();
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(nextItem),
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8
                };

            }
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(false),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8
            };


        }
    }
}