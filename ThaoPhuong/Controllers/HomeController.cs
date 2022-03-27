using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThaoPhuong.Utils;

namespace ThaoPhuong.Controllers
{
    public class HomeController : Controller
    {
        [FilterUrl]
        public ActionResult Index()
        {
            return View();
        }
    }
}