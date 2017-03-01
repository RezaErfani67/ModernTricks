using Excel;
using ModernTricks.Common;
using SendMail;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ModernTricks.Controllers
{
    public class SendEmailController : Controller
    {
        // GET: SendEmail
        public ActionResult ActiveUser()
        {
            return PartialView();
        }

        public ActionResult SendPass()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult SendGroupMail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendGroupMail(HttpPostedFileBase upload)
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
                    var list = result.Tables[0].AsEnumerable();
                    reader.Close();
                    foreach(var item in list)
                    {
                        string Body = this.RenderPartialToString("BodyGroupMail");

                        SendEmailGmail.Send(item.Field<string>("Title"), "بازیابی کلمه عبور", Body);
                        System.Threading.Thread.Sleep(4000);


                    }
                    return View(result.Tables[0]);
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
        }
    }
}