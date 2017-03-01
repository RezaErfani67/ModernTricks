using Excel;
using ModernTricks.Features.Automation;
using ModernTricks.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ModernTricks.Features.BaniChav
{
    public class BaniChavController : Controller
    {
        MainDBEntities database = new MainDBEntities();
        // GET: BaniChav

        public ActionResult UploadExcel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadExcel(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    reader.IsFirstRowAsColumnNames = true;

                    DataSet result = reader.AsDataSet();
                    reader.Close();






                    var list = result.Tables[0].AsEnumerable()
                        .Select(q => new
                        {
                            SuperVisorOfSupplyCode = q.Field<string>("کد سرپرست توزیع"),
                            SuperVisorOfSaleCode = q.Field<string>("کد سرپرست فروش"),
                            VisitorCode = q.Field<string>("کد ویزیتور"),
                            DelivererCode = q.Field<string>("کد تحویل دهنده"),
                            CustomerCode = q.Field<string>("کد مشتری"),
                            CustomerName = q.Field<string>("نام مشتری"),
                            TeamOfSale = q.Field<string>("تیم فروش"),
                            Price = q.Field<string>("مبلغ فاکتور"),
                            BranchCode = q.Field<string>("کد شعبه"),
                            CreatedDate = q.Field<DateTime>("تاریخ"),

                        });



                    var listUsers = database.USERS;
                    var list2 = from p in list
                                select new BaniChav_Total
                                {
                                    ID = Guid.NewGuid(),
                                    SuperVisorOfSupplyCode = p.SuperVisorOfSupplyCode,
                                    SuperVisorOfSaleCode = p.SuperVisorOfSaleCode,
                                    VisitorCode = p.VisitorCode,
                                    Price = Convert.ToInt32(p.Price),
                                    CreatedDate = p.CreatedDate,
                                    BranchCode = p.BranchCode,
                                    CustomerCode = p.CustomerCode,
                                    CustomerName = p.CustomerName,
                                    TeamOfSale = p.TeamOfSale,
                                    DelivererID = (from u in listUsers where (u.Code == p.DelivererCode) select u.User_Id).FirstOrDefault(),
                                    SuperVisorOfSupplyID = (from u in listUsers where (u.Code == p.SuperVisorOfSupplyCode) select u.User_Id).FirstOrDefault(),
                                    SuperVisorOfSaleID = (from u in listUsers where (u.Code == p.SuperVisorOfSaleCode) select u.User_Id).FirstOrDefault(),
                                    VisitorID = (from u in listUsers where (u.Code == p.VisitorCode) select u.User_Id).FirstOrDefault(),
                                    DelivererCode = p.DelivererCode,

                                };

                    database.BaniChav_Total.AddRange(list2.ToList());
                    database.SaveChanges();

                    ViewBag.data = list2;
                    return View(result.Tables[0]);
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
        }
        public ActionResult Select_ReturenedProduct_Async(Guid genericId)
        {

            var description =
                database.BaniChav_ReturnedProducts
                .Where(q => q.genericId == genericId)
                .Select(q => q.Description);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(description),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,

            };
        }
        public ActionResult UpdateDescription_ReturenedProduct_Async(Guid genericId, string Description)
        {
            var name = database.USERS.Find(new Guid(HttpContext.User.Identity.Name)).LFName;
            var item = database.BaniChav_ReturnedProducts.FirstOrDefault(q => q.genericId == genericId);
            item.Description = item.Description + "<br/>" + name + Description;
            database.Entry(item).State = EntityState.Modified;
            database.SaveChanges();

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject("ثبت با موفقیت انجام شد"),
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,

            };
        }
        [HttpGet]
        public ActionResult Create_ReturenedProduct()
        {
            //ViewBag.Totals = database.BaniChav_Total.Where(q =>
            //q.DelivererID.ToString() == HttpContext.User.Identity.Name &&
            //q.CreatedDate == DateTime.Today &&
            //!q.BaniChav_ReturnedProducts.Any())
            //.Select(q => new
            //{
            //    ID = q.ID,
            //    BranchCode = q.BranchCode,
            //    CreatedDate = q.CreatedDate,
            //    CustomerCode = q.CustomerCode,
            //    CustomerName = q.CustomerName,

            //    TeamOfSale = q.TeamOfSale,
            //    Price = q.Price
            //}.ToExpando());
            var Totals = database.BaniChav_Total.ToList()
         .Select(q => new
         {
             ID = q.ID,
             BranchCode = q.BranchCode,
             CreatedDate = q.CreatedDate,
             CustomerCode = q.CustomerCode,
             CustomerName = q.CustomerName,
             TeamOfSale = q.TeamOfSale,
             Price = q.Price
         }.ToExpando());

            ViewBag.Totals = Totals;

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create_ReturenedProduct(string totalItem, string description)
        {
            if (ModelState.IsValid)
            {
                //ایجاد آیتم در جنریک
                var GenericItem = database.Automation_Generic.Add(
                    new Automation_Generic
                    {
                        ID = Guid.NewGuid(),
                        Title = "BaniChav_ReturnedProducts"
                    });

                //ایجاد آیتم در ریترند پروداکت
                var ReturnedItem = database.BaniChav_ReturnedProducts.Add(
                    new BaniChav_ReturnedProducts
                    {
                        ID = Guid.NewGuid(),
                        genericId = GenericItem.ID,
                        Title = "",
                        Description = description,
                        BaniChav_TotalID = new Guid(totalItem)
                    });
                //ایجاد آیتم در تسک لیست
                var taskId = Guid.NewGuid();
                List<Automation_Tasks> taskList = new List<Automation_Tasks>();
                Automation_Tasks task1 = new Automation_Tasks
                {
                    ID = taskId,
                    Title = "01 - رویه کالای مرجوعی",
                    Filter1 = "مرجوعی",
                    Filter2 = "مرجوعی",
                    Filter3 = "مرجوعی",
                    genericId = GenericItem.ID,
                    IsActive = true,
                    IsDone = false,
                    Responsible = new Guid("8a27ed23-32d8-4139-b959-6441c49adaad"),
                    TaskOrder = 1,
                    ApproveHtml = "<button class=btnApprove id='btnApprove_" + taskId + "'><span class='glyphicon glyphicon-ok' area-hidden=true></span></button>",
                    RejectHtml = "<button  class=btnReject id='btnReject_" + taskId + "'><span class='glyphicon glyphicon-remove' area-hidden=true></span></button>",
                    ApproveScript =
                    "$(document).on('click','#btnApprove_" + taskId + "',function(e){" +
                    "e.preventDefault();" +
                    "var txtDescription=$(this).closest('.panel').find('.txtDescription').val();" +
                    "if(txtDescription != '')" +
                    "{" +
                    "ASQ()" +
                    ".all" +
                    "(" + // start all
                    "DoneTask('" + taskId + "')," +
                    "ActiveTask('" + GenericItem.ID + "',2)" +
                    ")" + // end all
                    //".then" +
                    //"(" +
                    //"function(done)" +
                    //"{" +
                    //"$.ajax({url:'/BaniChav/Select_ReturenedProduct_Async',data:{genericId:'" + GenericItem.ID + "'}})" +
                    //".success(function(result){" +
                    //"done(result);" +
                    //"})" +//end success ajax
                    //"}"+//end function(done)
                    //")" +//end then
                     ".then" +
                    "(" +
                    "function(done)" +
                    "{" +
                    "$.ajax({url:'/BaniChav/UpdateDescription_ReturenedProduct_Async',data:{genericId:'" + GenericItem.ID + "',Description:txtDescription}})" +
                    ".success(function(result){" +
                    "done(result);" +
                    "})" +//end success
                    "}" +//end function(done)
                    ")" +//end then
                     ".then" +
                    "(" +
                    "function(done)" +
                    "{" +
                    "Notify('success','ثبت با موفقیت انجام شد',2000,2);" +
                    "HidePanel('" + taskId + "')" +
                    "}" +//end function(done)
                    ")" +//end then
                    "}" +//end If
                    "else" +
                    "{" +
                    "Notify('error','لطفا توضیحات را وارد نمایید',2000,2);" +
                    "}" +
                    "});"//end click
                    ,
                    Html =
                    "<table class=table>" +
                    "<tr style=background-color:#D9EDF7;color:#3A87AD;>" +
                    "<th>توضیحات</th>" +
                    "</tr>" +
                    "<tr>" +
                    "<td><input type=text class='txtDescription' /></td>" +
                    "</tr>" +
                    "<table>",
                    RejectScript =
                    "$(document).on('click', '#btnReject_" + taskId + "', function(e){" +
                    "e.preventDefault();" +
                    "if(confirm('مطمئن هستید برای لغو رویه'))" +
                    "{" +
                    "RejectTask('" + taskId + "');" +
                    "Notify('error','رویه با موفقیت لغو گردید',2000,2);" +
                    "HidePanel('" + taskId + "');" +
                    "}" +
                    "});"//end  click
                };
                taskList.Add(task1);




                database.Automation_Tasks.AddRange(taskList);
                await database.SaveChangesAsync();

                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(""),
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8,

                };
            }
            else
            {
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(false),
                    ContentType = "application/json",
                    ContentEncoding = Encoding.UTF8
                };

            }

        }
    }
}