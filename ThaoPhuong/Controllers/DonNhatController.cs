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
    public class DonNhatController : Controller
    {
        DbEntities db = new DbEntities();
        // GET: DonNhat
        public ActionResult Index(string fDateStr, string tDateStr, string DKHACHHANGID, string DQUAYID, string TRANGTHAI, string sortName, string sortDirection)
        {
            //giao diện
            ViewBag.layout = Contants.LAYOUT_HOME;
            ViewBag.khachHangs = db.DKHACHHANGs.Where(x => x.ISADMIN != 30 && x.ISACTIVE > 0).ToList();
            ViewBag.quays = db.DQUAYs.ToList();
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
            //Lọc ngày, lọc theo trạng thái, lọc khách hàng
            int intTt = (TRANGTHAI == "" || TRANGTHAI == null) ? -1 : Convert.ToInt32(TRANGTHAI);
            DateTime toDay = DateTime.Now.Date;
            DateTime fromDate = (fDateStr != null && fDateStr.Length > 0) ? DateTime.ParseExact(fDateStr, "MM/dd/yyyy", null) : toDay.AddMonths(-1);
            DateTime toDate = (tDateStr != null && tDateStr.Length > 0) ? DateTime.ParseExact(tDateStr, "MM/dd/yyyy", null) : toDay;
            ViewBag.fDateStr = fromDate.ToString("MM/dd/yyyy");
            ViewBag.tDateStr = toDate.ToString("MM/dd/yyyy");
            ViewBag.DKHACHHANGID = DKHACHHANGID;
            ViewBag.DQUAYID = DQUAYID;
            ViewBag.TRANGTHAI = intTt;
            //Sắp xếp theo ngày, vị trí - Tăng giảm
            ViewBag.sortName = sortName ?? "vitri";
            ViewBag.sortDirection = sortDirection ?? "tang";
            List<TDONHANG> lst = new List<TDONHANG>();
            IQueryable<TDONHANG> iQueryable = db.TDONHANGs.Where(x => x.LOAI == 1);
            iQueryable = iQueryable.Where(x => DbFunctions.TruncateTime(x.TIMECREATED ?? toDay).Value >= fromDate && DbFunctions.TruncateTime(x.TIMECREATED ?? toDay).Value <= toDate);
            iQueryable = iQueryable.Where(x => x.DKHACHHANGID == DKHACHHANGID || DKHACHHANGID == null || DKHACHHANGID.Length == 0);
            iQueryable = iQueryable.Where(x => x.DQUAYID == DQUAYID || DQUAYID == null || DQUAYID.Length == 0);
            if (intTt != -1)
            {
                iQueryable = iQueryable.Where(x => (x.TRANGTHAI ?? (int)TrangThaiDon.ChoXuLy) == intTt);
            }
            IOrderedQueryable<TDONHANG> iOrderedQueryable;
            if (ViewBag.sortDirection == "tang")
            {
                if (ViewBag.sortName == "ngay") iOrderedQueryable = iQueryable.OrderBy(x => x.TIMECREATED);
                else iOrderedQueryable = iQueryable.OrderBy(x => x.DQUAY.POSITION);
            }
            else
            {
                if (ViewBag.sortName == "ngay") iOrderedQueryable = iQueryable.OrderByDescending(x => x.TIMECREATED);
                else iOrderedQueryable = iQueryable.OrderByDescending(x => x.DQUAY.POSITION);
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
            ViewBag.quays = db.DQUAYs.OrderBy(x => x.POSITION).ToList();
            ViewBag.isAdmin = isAdmin;

            TDONHANG dhRow = null;
            if (id != null && id.Length > 0)
            {
                dhRow = db.TDONHANGs.Where(x => x.ID == id).FirstOrDefault();
                if (dhRow == null) return View("~/Views/Shared/Error.cshtml");

                ViewBag.imgs = DonGomController.GetDicAnhs(db, dhRow);
            }
            else dhRow = new TDONHANG();
            if (dhRow.NAME == null || dhRow.NAME.Length == 0) dhRow.NAME = "Tự động";

            return View(dhRow);
        }

        [HttpPost]
        public ActionResult AddOrUpdate(TDONHANG item)
        {
            bool isNewItem = false;
            TDONHANG dhRow = new TDONHANG();
            try
            {
                ViewBag.quays = db.DQUAYs.OrderBy(x => x.POSITION).ToList();
                ViewBag.imgs = DonGomController.GetDicAnhs(db, dhRow);
                if (ModelState.IsValid)
                {
                    //kiểm tra xem up ảnh hay chưa
                    List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
                    files.AddRange(Request.Files.GetMultiple("images[]"));
                    string[] preloaded = Request.Params.GetValues("preloaded[]");
                    bool hasImg = false;
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file.ContentLength > 0) hasImg = true;
                    }
                    if (!hasImg) hasImg = preloaded != null && preloaded.Length > 0;

                    if (item.ID == null)
                    {
                        isNewItem = true;
                        dhRow = new TDONHANG();
                        //thêm mới
                        dhRow.LOAI = 1;
                        dhRow.NAME = DbUtils.GenCode("DN", "TDONHANG", "NAME");
                        dhRow.TIMECREATED = DateTime.Now;
                        dhRow.DKHACHHANGID = (Session[Contants.USER_SESSION_NAME] as DKHACHHANG).ID;
                        dhRow.TRANGTHAI = (int)TrangThaiDon.ChoXuLy;
                    }
                    else
                    {
                        dhRow = db.TDONHANGs.Where(x => x.ID == item.ID).FirstOrDefault();
                        dhRow.TIMEUPDATED = DateTime.Now;
                        dhRow.TRANGTHAI = item.TRANGTHAI;
                    }
                    dhRow.TENSP = item.TENSP;
                    dhRow.TIENDUKIEN = item.TIENDUKIEN;
                    dhRow.TIENDANHAT = item.TIENDANHAT;
                    //tính công nhặt
                    decimal congNhat = 0;
                    foreach (TDONHANGCHITIET chRow in item.TDONHANGCHITIETs)
                    {
                        congNhat += (decimal)(chRow.SOLUONGDANHAT ?? 0) * 10000;
                    }
                    dhRow.TIENCONG = congNhat;
                    dhRow.TONGCONG = (dhRow.TIENDANHAT == null ? 0 : dhRow.TIENDANHAT) + dhRow.TIENCONG;
                    dhRow.DQUAYID = item.DQUAYID;
                    dhRow.NOTE = item.NOTE;

                    if (!hasImg || item.TDONHANGCHITIETs == null || item.TDONHANGCHITIETs.Count == 0)
                    {
                        bool isAdmin = SessionUtils.IsAdmin(Session);
                        ViewBag.layout = Contants.LAYOUT_HOME;
                        if (isAdmin)
                        {
                            ViewBag.layout = Contants.LAYOUT_ADMIN;
                        }
                        ViewBag.quays = db.DQUAYs.OrderBy(x => x.POSITION).ToList();
                        ViewBag.isAdmin = isAdmin;
                        if (!hasImg) ViewBag.error = "Bạn chưa chọn hình ảnh nào!";
                        else ViewBag.error = "Bạn chưa thêm mặt hàng nhặt nào!";
                        dhRow.TDONHANGCHITIETs = item.TDONHANGCHITIETs;
                        return View(dhRow);
                    }

                    if (isNewItem)
                    {
                        dhRow.ID = Guid.NewGuid().ToString();
                        db.TDONHANGs.Add(dhRow);
                    }
                    else
                    {
                        db.Entry(dhRow);
                    }
                    db.SaveChanges();
                    List<TDONHANGCHITIET> lstTemp = db.TDONHANGCHITIETs.Where(x => x.TDONHANGID == dhRow.ID).ToList();
                    foreach (TDONHANGCHITIET itChiTiet in lstTemp)
                    {
                        db.TDONHANGCHITIETs.Remove(itChiTiet);
                    }
                    db.SaveChanges();
                    foreach (TDONHANGCHITIET itChiTiet in item.TDONHANGCHITIETs)
                    {
                        if (itChiTiet.ID == null || itChiTiet.ID.Length == 0) itChiTiet.ID = Guid.NewGuid().ToString();
                        itChiTiet.TDONHANGID = dhRow.ID;
                        db.TDONHANGCHITIETs.Add(itChiTiet);
                    }
                    db.SaveChanges();

                    //upload anh
                    DonGomController.uploadAnhMatHang(db, Server, preloaded, files, dhRow);

                    ViewBag.imgs = DonGomController.GetDicAnhs(db, dhRow);
                    ViewBag.success = "Tạo đơn nhặt thành công";
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                if (isNewItem) dhRow.NAME = "Tự động";
            }
            //return View(dhRow);
            return RedirectToAction("Index", "DonNhat");
        }
    }
}