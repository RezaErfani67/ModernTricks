
using ModernTricks.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ModernTricks.Controllers
{
    [RBAC]
    public class ReportsController : Controller
    {
        private MainDBEntities database = new MainDBEntities();

        // GET: Reports
        public ActionResult Index()
        {
            //Called from flow diagram process 1    
            return View(this.GetReports());
        }

        public ActionResult Preview(int report_id)
        {
            //If the Web.config contains the 'ReportViewerUrl' key, then append to the beginning of the URL stored in the database.
            //This assumes that the database only contains the report name section of the URL and that the Report Viewer name/database 
            //instance is stored in the Web.config hence only needs to be maintained in a single place and the database URL's are fixed
            //across ALL enviornments (otherwise different environments point to different Reporting servers and each environment
            //requires the REPORTS table to be specific to that environment as the URL's would need to point to that Reporting Server)...

            ViewBag.ReportUrl = ConfigurationManager.AppSettings.Get("ReportViewerUrl");

            //REPORT _report = this.GetReports().Where(p => p.Report_Id == report_id).FirstOrDefault();
            //if (_report.PARAMETERS.Count == 0)
            //  return ExecuteReportviaSP(_report, "");

            return View(this.GetReports().Where(p => p.Report_Id == report_id).FirstOrDefault());
        }

        //private ActionResult ExecuteReportviaSP(REPORTS _report, string rawParams, string _defaultReportTemplate = "DefaultResultsTemplate")
        //{
        //    List<dynamic> _list = new List<dynamic>();
        //    string _reportName = _report.Template;
        //    try
        //    {
        //        _list = CommonSql.ExecuteStoredProcedure(_report, rawParams, this);
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Error", "Unauthorised", new RouteValueDictionary(new { _errorMsg = ex.Message }));
        //    }

        //    string _targetFile = string.Format("{0}/{1}.cshtml", Server.MapPath("~/Views/Reports"), _reportName);
        //    if (!System.IO.File.Exists(_targetFile))
        //    {
        //        _reportName = _defaultReportTemplate;
        //    }
        //    return View(_reportName, _list);
        //}

        [HttpGet]
        //public ActionResult Execute(int report_id, string rawParams)
        //{
        //    //Called from flow diagram process 3

        //    //Generates flow diagram process 4
        //    ViewBag.ReportName = this.GetReports().Where(p => p.Report_Id == report_id).FirstOrDefault().ReportName;
        //    return ExecuteReportviaSP(this.GetReports().Where(p => p.Report_Id == report_id).FirstOrDefault(), rawParams);
        //}

        [HttpPost]
        public ActionResult ExportData(FormCollection form)
        {
            //Called from flow diagram process 5    
            List<dynamic> _list = TempData["ModelData"] as List<dynamic>;
            try
            {
                int _recordsExported = DynamicDataExport2CSV.Export(_list);
                return RedirectToAction("Error", "Unauthorised", new RouteValueDictionary(new { _errorMsg = string.Format("Records Exported: {0}", _recordsExported) }));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Unauthorised", new RouteValueDictionary(new { _errorMsg = ex.Message }));
            }
        }
    }
}