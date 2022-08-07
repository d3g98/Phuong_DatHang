using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThaoPhuong.Models;

namespace ThaoPhuong.Controllers
{
    public class SconfigController : Controller
    {
        THAOPHUONGEntities db = new THAOPHUONGEntities();

        // GET: Sconfig
        [HttpGet]
        public ActionResult ThongBaoKhachHang()
        {
            ViewBag.data = LayDuLieuThongBaoKhachHang(db);
            return View();
        }

        public static string LayDuLieuThongBaoKhachHang(THAOPHUONGEntities db)
        {
            string data = "";
            List<SCONFIG> lstSconfig = db.SCONFIGs.ToList();
            if (lstSconfig.Count != 0)
            {
                data = lstSconfig[0].THONGBAOKHACHHANG;
            }
            data = HttpUtility.HtmlDecode(data);
            return data;
        }

        [HttpPost]
        public ActionResult ThongBaoKhachhang(string data)
        {
            if (ModelState.IsValid)
            {
                List<SCONFIG> lstSconfig = db.SCONFIGs.ToList();
                SCONFIG sconfig = null;
                bool newRecord = lstSconfig.Count == 0;
                if (newRecord)
                {
                    sconfig = new SCONFIG();
                    sconfig.ID = Guid.NewGuid().ToString();
                }
                else
                {
                    sconfig = lstSconfig[0];
                }
                sconfig.THONGBAOKHACHHANG = data;
                if (newRecord)
                {
                    db.SCONFIGs.Add(sconfig);
                }
                else
                {
                    db.Entry(sconfig);
                }
                db.SaveChanges();
            }
            ViewBag.data = data;
            return View();
        }
    }
}