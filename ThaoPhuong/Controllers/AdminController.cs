using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThaoPhuong.Models;
using ThaoPhuong.Utils;

namespace ThaoPhuong.Controllers
{
    [FilterUrl]
    public class AdminController : Controller
    {
        THAOPHUONGEntities db = new THAOPHUONGEntities();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.data = SconfigController.LayDuLieuThongBaoKhachHang(db);
            return View();
        }
    }
}