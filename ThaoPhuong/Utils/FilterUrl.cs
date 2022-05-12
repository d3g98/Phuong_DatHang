using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using ThaoPhuong.Models;

namespace ThaoPhuong.Utils
{
    public class FilterUrl : ActionFilterAttribute, IAuthenticationFilter
    {
        THAOPHUONGEntities db = new THAOPHUONGEntities();
        string[] adminUrls = new string[] { "Admin", "DQUAY", "DKHACHHANG", "DTRANGTHAI" };
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            string controller = filterContext.RouteData.Values["Controller"].ToString();
            string action = filterContext.RouteData.Values["Action"].ToString();
            if (!(controller == "Profile" && (action == "Login" || action == "Register")))
            {
                DKHACHHANG user = filterContext.HttpContext.Session[Contants.USER_SESSION_NAME] as DKHACHHANG;
                if (user == null)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }
                else
                {
                    user = db.DKHACHHANGs.Where(x => x.ID == user.ID).FirstOrDefault();
                    if (user == null) filterContext.Result = new HttpUnauthorizedResult();
                    else
                    {
                        if (user.ISACTIVE == 0)
                        {
                            filterContext.Result = new HttpUnauthorizedResult();
                            return;
                        }
                        //truy cập đường dẫn admin thì phải là admin
                        if (adminUrls.Contains(controller) && user.ISADMIN == 0)
                        {
                            filterContext.Result = new HttpUnauthorizedResult();
                            return;
                        }
                    }
                }
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            string controller = filterContext.RouteData.Values["Controller"].ToString();
            string action = filterContext.RouteData.Values["Action"].ToString();

            if (controller == "Admin")
            {
                controller = "Home";
                action = "Index";
            }
            else
            {
                controller = "Profile";
                action = "Login";
            }

            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                //Redirecting the user to the Login View of Account Controller
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary
                   {
                        { "controller", "Profile" },
                        { "action", "Login" }
                   });
                //If you want to redirect to some error view, use below code
                //filterContext.Result = new ViewResult()
                //{
                //    ViewName = "Login"
                //};
            }
        }
    }
}