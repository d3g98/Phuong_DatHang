using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThaoPhuong.Models;
using ThaoPhuong.Utils;

namespace ThaoPhuong.Controllers
{
    [FilterUrl]
    public class ThanhToanController : Controller
    {
        THAOPHUONGEntities db = new THAOPHUONGEntities();
        // GET: ThanhToan
        public ActionResult Index(string fDateStr, string tDateStr, string DKHACHHANGID, string DTRANGTHAIID, string sortName, string sortDirection)
        {
            //giao diện
            ViewBag.layout = Contants.LAYOUT_HOME;
            ViewBag.khachHangs = db.DKHACHHANGs.Where(x => x.ISADMIN != 30 && x.ISACTIVE > 0).ToList();
            DKHACHHANG khRow = Session[Contants.USER_SESSION_NAME] as DKHACHHANG;
            ViewBag.isAdmin = false;
            if (SessionUtils.IsAdmin(Session))
            {
                ViewBag.isAdmin = true;
                ViewBag.layout = Contants.LAYOUT_ADMIN;
            }
            else
            {
                DKHACHHANGID = khRow.ID;
            }
            //Lọc ngày, lọc khách hàng
            DateTime toDay = DateTime.Now.Date;
            DateTime fromDate = (fDateStr != null && fDateStr.Length > 0) ? DateTime.ParseExact(fDateStr, "MM/dd/yyyy", null) : toDay.AddMonths(-1);
            DateTime toDate = (tDateStr != null && tDateStr.Length > 0) ? DateTime.ParseExact(tDateStr, "MM/dd/yyyy", null) : toDay;
            ViewBag.fDateStr = fromDate.ToString("MM/dd/yyyy");
            ViewBag.tDateStr = toDate.ToString("MM/dd/yyyy");
            ViewBag.DKHACHHANGID = DKHACHHANGID;
            //Sắp xếp theo ngày, vị trí - Tăng giảm
            ViewBag.sortDirection = sortDirection ?? "tang";
            List<TTHANHTOAN> lst = new List<TTHANHTOAN>();
            IQueryable<TTHANHTOAN> iQueryable = db.TTHANHTOANs.Where(x => DbFunctions.TruncateTime(x.TIMECREATED ?? toDay).Value >= fromDate && DbFunctions.TruncateTime(x.TIMECREATED ?? toDay).Value <= toDate);
            iQueryable = iQueryable.Where(x => x.DKHACHHANGID == DKHACHHANGID || DKHACHHANGID == null || DKHACHHANGID.Length == 0);
            IOrderedQueryable<TTHANHTOAN> iOrderedQueryable;
            if (ViewBag.sortDirection == "tang")
            {
                iOrderedQueryable = iQueryable.OrderBy(x => x.TIMECREATED);
            }
            else
            {
                iOrderedQueryable = iQueryable.OrderByDescending(x => x.TIMECREATED);
            }
            lst = iOrderedQueryable.ToList();
            return View(lst);
        }

        [HttpGet]
        public ActionResult AddOrUpdate(string id)
        {
            bool isAdmin = SessionUtils.IsAdmin(Session);
            ViewBag.layout = Contants.LAYOUT_HOME;
            if (isAdmin)
            {
                ViewBag.layout = Contants.LAYOUT_ADMIN;
            }
            ViewBag.isAdmin = isAdmin;
            ViewBag.khachHangs = db.DKHACHHANGs.Where(x => x.ISADMIN != 30 && x.ISACTIVE > 0).ToList();
            TTHANHTOAN ttRow = new TTHANHTOAN();
            if (id != null)
            {
                ttRow = db.TTHANHTOANs.Where(x => x.ID == id).FirstOrDefault();
                if (ttRow == null) return View("~/Views/Shared/Error.cshtml");
            }
            else
            {
                ttRow.NAME = "Tự động";
            }
            return View(ttRow);
        }

        [HttpPost]
        public ActionResult AddOrUpdate(TTHANHTOAN temp)
        {
            bool isAdmin = SessionUtils.IsAdmin(Session);
            ViewBag.layout = Contants.LAYOUT_HOME;
            if (isAdmin)
            {
                ViewBag.layout = Contants.LAYOUT_ADMIN;
            }
            ViewBag.isAdmin = isAdmin;
            ViewBag.khachHangs = db.DKHACHHANGs.Where(x => x.ISADMIN != 30 && x.ISACTIVE > 0).ToList();
            if (temp.TTHANHTOANCHITIETs.Count == 0)
            {
                ViewBag.error = "Chưa chọn hóa đơn nào để thanh toán";
                return View(temp);
            }

            TTHANHTOAN ttRow;
            bool isNewItem = false;
            if (temp.ID == null)
            {
                isNewItem = true;
                ttRow = new TTHANHTOAN();
                //thêm mới
                ttRow.ID = Guid.NewGuid().ToString();
                ttRow.NAME = DbUtils.GenCode("TT", "TTHANHTOAN", "NAME");
                ttRow.TIMECREATED = DateTime.Now;
                ttRow.DKHACHHANGID = temp.DKHACHHANGID;
            }
            else
            {
                ttRow = db.TTHANHTOANs.Where(x => x.ID == temp.ID).FirstOrDefault();
                ttRow.TIMEUPDATED = DateTime.Now;
            }
            ttRow.KETTHUC = (temp.KETTHUC != null && temp.KETTHUC > 0) ? 30 : 0;
            ttRow.TIENHANG = temp.TIENHANG;
            ttRow.PHUPHI = temp.PHUPHI;
            ttRow.TONGCONG = temp.TONGCONG;
            ttRow.TIENTHANHTOAN = temp.TIENTHANHTOAN;
            ttRow.NOTE = temp.NOTE;
            if (isNewItem) db.TTHANHTOANs.Add(ttRow);
            else
            {
                db.Entry(ttRow);
            }
            db.SaveChanges();
            db.TTHANHTOANCHITIETs.RemoveRange(db.TTHANHTOANCHITIETs.Where(x => x.TTHANHTOANID == ttRow.ID).ToList());
            foreach (TTHANHTOANCHITIET ctRow in temp.TTHANHTOANCHITIETs)
            {
                ctRow.ID = Guid.NewGuid().ToString();
                ctRow.TTHANHTOANID = ttRow.ID;
                ctRow.NOTE = ctRow.NOTE;
                db.TTHANHTOANCHITIETs.Add(ctRow);
            }
            db.SaveChanges();
            //Cập nhật lại đơn hàng
            foreach (TTHANHTOANCHITIET ctRow in temp.TTHANHTOANCHITIETs)
            {
                TDONHANG dhRow = db.TDONHANGs.Where(x => x.ID == ctRow.TDONHANGID).FirstOrDefault();
                if (dhRow != null && ttRow.KETTHUC > 0)
                {
                    dhRow.DTRANGTHAIID = "7";
                    db.Entry(dhRow);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index", "ThanhToan");
        }

        public ActionResult SearchHoaDonThanhToan(string id)
        {
            string huyStr = "4";
            string hoanThanhStr = "3";
            string daThanhToan = "7";
            //List<TDONHANG> lst = db.TDONHANGs.Where(x => x.DKHACHHANGID == id
            //&& (x.DTRANGTHAIID != hoanThanhStr && x.DTRANGTHAIID != huyStr && x.DTRANGTHAIID != daThanhToan)
            //&& x.TTHANHTOANCHITIETs.Count == 0)
            //    .ToList();
            List<TDONHANG> lst = db.TDONHANGs.Where(x => x.DKHACHHANGID == id
             && x.DTRANGTHAIID == hoanThanhStr
             && x.TTHANHTOANCHITIETs.Count == 0)
                 .ToList();
            return PartialView(lst);
        }

        public ActionResult Delete(string id)
        {
            //Xóa thanh toán chi tiết
            TTHANHTOAN thanhToanRow = db.TTHANHTOANs.Where(x => x.ID == id).FirstOrDefault();
            if (thanhToanRow == null) return View("~/Views/Shared/Error.cshtml");
            db.TTHANHTOANCHITIETs.RemoveRange(thanhToanRow.TTHANHTOANCHITIETs);
            db.SaveChanges();
            db.TTHANHTOANs.Remove(thanhToanRow);
            db.SaveChanges();
            return RedirectToAction("Index", "ThanhToan");
        }
    }
}