using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


public class RBACAttribute : AuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        //Create permission string based on the requested controller name and action name in the format 'controllername-action'
        string requiredPermission = String.Format("{0}-{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);

        //Create an instance of our custom user authorization object passing requesting user's 'Windows Username' into constructor
        RBACUser requestingUser = new RBACUser(filterContext.RequestContext.HttpContext.User.Identity.Name);
        //Check if the requesting user has the permission to run the controller's action
        if (!requestingUser.HasPermission(requiredPermission) & !requestingUser.IsSysAdmin)
        {
            //User doesn't have the required permission and is not a SysAdmin, return our custom “401 Unauthorized” access error
            //Since we are setting filterContext.Result to contain an ActionResult page, the controller's action will not be run.
            //The custom “401 Unauthorized” access error will be returned to the browser in response to the initial request.
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // Add this (code 401 does not work)
                filterContext.HttpContext.Response.StatusCode = 401;
                // <-- in here
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        msg = "دوست گرامی، لطفا وارد سایت شوید"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "Unauthorised" },
                    { "returnUrl",filterContext.HttpContext.Request.Url }});
            }
        }
        //If the user has the permission to run the controller's action, then filterContext.Result will be uninitialized and
        //executing the controller's action is dependant on whether filterContext.Result is uninitialized.
    }
}
