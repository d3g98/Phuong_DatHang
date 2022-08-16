using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThaoPhuong.Models;
using ThaoPhuong.Utils;

namespace ThaoPhuong.Controllers
{
    [FilterUrl]
    public class DonGomController : Controller
    {
        THAOPHUONGEntities db = new THAOPHUONGEntities();
        // GET: DonGom
        public ActionResult Index()
        {
            string fDateStr = null, tDateStr = null, fUpdateStr = null, tUpdateStr = null, DKHACHHANGID = null, DQUAYID = null, DTRANGTHAIID = null, sortName = null, sortDirection = null, giaoDich = null, loadIndex = null;
            GetParamFromUrl(Request.Params, ref fDateStr, ref tDateStr, ref fUpdateStr, ref tUpdateStr, ref DKHACHHANGID, ref DQUAYID, ref DTRANGTHAIID, ref sortName, ref sortDirection, ref giaoDich, ref loadIndex, false);

            giaoDich = giaoDich ?? "-1";
            //giao diện
            ViewBag.layout = Contants.LAYOUT_HOME;
            ViewBag.khachHangs = db.DKHACHHANGs.Where(x => x.ISADMIN != 30 && x.ISACTIVE > 0).ToList();
            ViewBag.quays = db.DQUAYs.ToList();
            ViewBag.trangthais = db.DTRANGTHAIs.OrderBy(x => x.ID).ToList();
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
            if (DTRANGTHAIID == null || DTRANGTHAIID.Length == 0) DTRANGTHAIID = "1";
            DateTime toDay = DateTime.Now.Date;
            DateTime fromDate = (fDateStr != null && fDateStr.Length > 0) ? DateTime.ParseExact(fDateStr, "MM/dd/yyyy", null) : toDay.AddMonths(-1);
            DateTime toDate = (tDateStr != null && tDateStr.Length > 0) ? DateTime.ParseExact(tDateStr, "MM/dd/yyyy", null) : toDay;
            DateTime fromUpdate = (fUpdateStr != null && fUpdateStr.Length > 0) ? DateTime.ParseExact(fUpdateStr, "MM/dd/yyyy", null) : toDay.AddMonths(-1);
            DateTime toUpdate = (tUpdateStr != null && tUpdateStr.Length > 0) ? DateTime.ParseExact(tUpdateStr, "MM/dd/yyyy", null) : toDay;
            ViewBag.fDateStr = fromDate.ToString("MM/dd/yyyy");
            ViewBag.tDateStr = toDate.ToString("MM/dd/yyyy");

            bool locNgayUpdate = (fUpdateStr != null && fUpdateStr.Length > 0) && (tUpdateStr != null && tUpdateStr.Length > 0);
            if (locNgayUpdate)
            {
                ViewBag.fUpdateStr = fromUpdate.ToString("MM/dd/yyyy");
                ViewBag.tUpdateStr = toUpdate.ToString("MM/dd/yyyy");
            }

            ViewBag.DKHACHHANGID = DKHACHHANGID;
            ViewBag.DQUAYID = DQUAYID;
            ViewBag.DTRANGTHAIID = DTRANGTHAIID;
            ViewBag.giaoDich = giaoDich;
            //Sắp xếp theo ngày, vị trí - Tăng giảm
            //Khách hàng thì mặc định ngày giảm
            ViewBag.sortName = sortName ?? "vitri";
            ViewBag.sortDirection = sortDirection ?? "tang";
            if (!ViewBag.isAdmin)
            {
                ViewBag.sortName = sortName ?? "ngay";
                ViewBag.sortDirection = sortDirection ?? "giam";
            }

            List<TDONHANG> lst = new List<TDONHANG>();
            IQueryable<TDONHANG> iQueryable = db.TDONHANGs.Where(x => x.LOAI == 0);
            iQueryable = iQueryable.Where(x => DbFunctions.TruncateTime(x.TIMECREATED ?? toDay).Value >= fromDate && DbFunctions.TruncateTime(x.TIMECREATED ?? toDay).Value <= toDate);
            iQueryable = iQueryable.Where(x => x.DKHACHHANGID == DKHACHHANGID || DKHACHHANGID == null || DKHACHHANGID.Length == 0);
            iQueryable = iQueryable.Where(x => x.DTRANGTHAIID == DTRANGTHAIID || DTRANGTHAIID == "TatCa");
            iQueryable = iQueryable.Where(x => x.DQUAYID == DQUAYID || DQUAYID == null || DQUAYID.Length == 0);

            if (locNgayUpdate)
            {
                DateTime tmp = toUpdate.AddMonths(1);
                iQueryable = iQueryable.Where(x => DbFunctions.TruncateTime(x.TIMEUPDATED ?? tmp).Value >= fromUpdate && DbFunctions.TruncateTime(x.TIMEUPDATED ?? tmp).Value <= toUpdate);
            }

            if (giaoDich != "-1")
            {
                iQueryable = iQueryable.Where(x => ((x.DQUAYID != null || x.DQUAYID.Length > 0) && x.DQUAY.GIAODICH.ToString() == giaoDich)
                || (giaoDich == "2" && x.DQUAY == null));
            }
            IOrderedQueryable<TDONHANG> iOrderedQueryable;
            if (ViewBag.sortDirection == "tang")
            {
                if (ViewBag.sortName == "ngay") iOrderedQueryable = iQueryable.OrderBy(x => x.TIMECREATED);
                else iOrderedQueryable = iQueryable.OrderBy(x => x.DQUAY.POSITION ?? 999999999);
            }
            else
            {
                if (ViewBag.sortName == "ngay") iOrderedQueryable = iQueryable.OrderByDescending(x => x.TIMECREATED);
                else iOrderedQueryable = iQueryable.OrderByDescending(x => x.DQUAY.POSITION ?? 999999999);
            }
            lst = iOrderedQueryable.ToList();

            return View(lst);
        }

        [HttpGet]
        public ActionResult AddOrUpdate(string id, int? loadIndex)
        {
            bool isAdmin = SessionUtils.IsAdmin(Session);
            ViewBag.layout = Contants.LAYOUT_HOME;
            if (isAdmin)
            {
                ViewBag.layout = Contants.LAYOUT_ADMIN;
            }
            ViewBag.quays = db.DQUAYs.OrderBy(x => x.POSITION).ToList();
            ViewBag.trangthais = db.DTRANGTHAIs.OrderBy(x => x.ID).ToList();
            ViewBag.isAdmin = isAdmin;

            TDONHANG dhRow = null;
            if (id != null && id.Length > 0)
            {
                dhRow = db.TDONHANGs.Where(x => x.ID == id).FirstOrDefault();
                if (dhRow == null) return View("~/Views/Shared/Error.cshtml");

                ViewBag.imgs = GetDicAnhs(db, dhRow);
            }
            else dhRow = new TDONHANG();
            if (dhRow.NAME == null || dhRow.NAME.Length == 0)
            {
                dhRow.NAME = "Tự động";
                dhRow.TONGCONG = 10000;
            }

            ViewBag.loadIndex = loadIndex;
            return View(dhRow);
        }

        public static void GetParamFromUrl(NameValueCollection @params, ref string fDateStr, ref string tDateStr, ref string fUpdateStr, ref string tUpdateStr,
            ref string DKHACHHANGID, ref string DQUAYID, ref string DTRANGTHAIID, ref string sortName, ref string sortDirection, ref string giaoDich,
            ref string loadIndex, bool endp)
        {
            List<string> lstKeys = new List<string>(@params.AllKeys);
            if (lstKeys.Contains((endp ? "p" : "") + "fDateStr")) fDateStr = @params[(endp ? "p" : "") + "fDateStr"];
            if (lstKeys.Contains((endp ? "p" : "") + "tDateStr")) tDateStr = @params[(endp ? "p" : "") + "tDateStr"];
            if (lstKeys.Contains((endp ? "p" : "") + "fUpdateStr")) fUpdateStr = @params[(endp ? "p" : "") + "fUpdateStr"];
            if (lstKeys.Contains((endp ? "p" : "") + "tUpdateStr")) tUpdateStr = @params[(endp ? "p" : "") + "tUpdateStr"];
            if (lstKeys.Contains((endp ? "p" : "") + "DKHACHHANGID")) DKHACHHANGID = @params[(endp ? "p" : "") + "DKHACHHANGID"];
            if (lstKeys.Contains((endp ? "p" : "") + "DQUAYID")) DQUAYID = @params[(endp ? "p" : "") + "DQUAYID"];
            if (lstKeys.Contains((endp ? "p" : "") + "DTRANGTHAIID")) DTRANGTHAIID = @params[(endp ? "p" : "") + "DTRANGTHAIID"];
            if (lstKeys.Contains((endp ? "p" : "") + "sortName")) sortName = @params[(endp ? "p" : "") + "sortName"];
            if (lstKeys.Contains((endp ? "p" : "") + "sortDirection")) sortDirection = @params[(endp ? "p" : "") + "sortDirection"];
            if (lstKeys.Contains((endp ? "p" : "") + "giaoDich")) giaoDich = @params[(endp ? "p" : "") + "giaoDich"];
            if (lstKeys.Contains("loadIndex")) loadIndex = @params["loadIndex"];
        }

        public static Dictionary<string, string> GetDicAnhs(THAOPHUONGEntities db, TDONHANG dhRow)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            List<DANH> lstAnhs = db.DANHs.Where(x => x.TDONHANGID == dhRow.ID).ToList();
            foreach (DANH anh in lstAnhs)
            {
                dic.Add(anh.ID, "/Images/Upload/" + anh.PATH);
            }
            return dic;
        }

        [HttpPost]
        public ActionResult AddOrUpdate(TDONHANG item)
        {
            string fDateStr = null, tDateStr = null, fUpdateStr = null, tUpdateStr = null, DKHACHHANGID = null, DQUAYID = null, DTRANGTHAIID = null, sortName = null, sortDirection = null, giaoDich = null, loadIndex = null;
            GetParamFromUrl(Request.Params, ref fDateStr, ref tDateStr, ref fUpdateStr, ref tUpdateStr, ref DKHACHHANGID, ref DQUAYID, ref DTRANGTHAIID, ref sortName, ref sortDirection, ref giaoDich, ref loadIndex, true);

            bool isNewItem = false;
            TDONHANG dhRow = new TDONHANG();
            try
            {
                ViewBag.quays = db.DQUAYs.OrderBy(x => x.POSITION).ToList();
                ViewBag.trangthais = db.DTRANGTHAIs.OrderBy(x => x.ID).ToList();
                ViewBag.imgs = GetDicAnhs(db, dhRow);
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
                        dhRow.LOAI = 0;
                        dhRow.NAME = DbUtils.GenCode("DG", "TDONHANG", "NAME");
                        dhRow.TIMECREATED = DateTime.Now;
                        dhRow.DKHACHHANGID = (Session[Contants.USER_SESSION_NAME] as DKHACHHANG).ID;
                    }
                    else
                    {
                        dhRow = db.TDONHANGs.Where(x => x.ID == item.ID).FirstOrDefault();
                        dhRow.TIMEUPDATED = DateTime.Now;
                    }
                    dhRow.DTRANGTHAIID = item.DTRANGTHAIID ?? "1";
                    dhRow.TONGCONG = item.TONGCONG;
                    dhRow.DQUAYID = item.DQUAYID;
                    dhRow.NOTE = item.NOTE;

                    //if (!hasImg || dhRow.DQUAYID == null || dhRow.DQUAYID.Length == 0)
                    if (dhRow.DQUAYID == null || dhRow.DQUAYID.Length == 0)
                    {
                        bool isAdmin = SessionUtils.IsAdmin(Session);
                        ViewBag.layout = Contants.LAYOUT_HOME;
                        if (isAdmin)
                        {
                            ViewBag.layout = Contants.LAYOUT_ADMIN;
                        }
                        ViewBag.quays = db.DQUAYs.OrderBy(x => x.POSITION).ToList();
                        ViewBag.isAdmin = isAdmin;

                        //if (!hasImg)
                        //{
                        //    ViewBag.error = "Bạn chưa chọn hình ảnh nào!";
                        //}
                        //else
                        //{
                        ViewBag.error = "Bạn chưa chọn quầy giao dịch";
                        //}
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

                    //upload anh
                    uploadAnhMatHang(db, Server, preloaded, files, dhRow);

                    if (!hasImg)
                    {
                        DANH anh = new DANH();
                        anh.TDONHANGID = dhRow.ID;
                        anh.PATH = "noimage.jpg";
                        anh.ID = Guid.NewGuid().ToString();
                        db.DANHs.Add(anh);
                        db.SaveChanges();
                    }

                    ViewBag.imgs = GetDicAnhs(db, dhRow);
                    ViewBag.success = "Tạo đơn gom thành công";
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                if (isNewItem) dhRow.NAME = "Tự động";
            }
            return RedirectToAction("Index", "DonGom", new
            {
                fDateStr = fDateStr,
                tDateStr = tDateStr,
                fUpdateStr = fUpdateStr,
                tUpdateStr = tUpdateStr,
                DKHACHHANGID = DKHACHHANGID,
                DQUAYID = DQUAYID,
                DTRANGTHAIID = DTRANGTHAIID,
                sortName = sortName,
                sortDirection = sortDirection,
                giaoDich = giaoDich,
                loadIndex = loadIndex
            });
        }

        public static void uploadAnhMatHang(THAOPHUONGEntities db, HttpServerUtilityBase httpServer, string[] olds, List<HttpPostedFileBase> files, TDONHANG item)
        {
            //Lấy danh sách ảnh trong database cái nào không còn trong olds thì xóa.
            List<DANH> lstAnhs = db.DANHs.Where(x => x.TDONHANGID == item.ID).ToList();
            if (lstAnhs != null && lstAnhs.Count > 0)
            {
                foreach (DANH itemAnh in lstAnhs)
                {
                    if (olds == null || !olds.Contains(itemAnh.ID))
                    {
                        FileUpload.Delete(httpServer, itemAnh.PATH);
                        db.DANHs.Remove(itemAnh);
                    }
                }
            }
            //Thêm ảnh từ file
            if (files != null && files.Count > 0)
            {
                foreach (HttpPostedFileBase fileItem in files)
                {
                    if (fileItem.ContentLength == 0) continue;
                    string file = FileUpload.Upload(httpServer, fileItem);
                    DANH itemAnh = new DANH();
                    itemAnh.ID = Guid.NewGuid().ToString();
                    itemAnh.TDONHANGID = item.ID;
                    itemAnh.PATH = file;
                    db.DANHs.Add(itemAnh);
                }
            }
            db.SaveChanges();
        }
    }
}