using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public ActionResult CongNoKhachHang()
        {
            string query = @"SELECT * FROM
            (
	            SELECT ID, NAME, DIENTHOAI, DIACHI, 
	            (
		            SELECT SUM(COALESCE(TONGCONG, 0) - COALESCE(TIENTHANHTOAN, 0)) FROM TTHANHTOAN WHERE DKHACHHANGID = DKHACHHANG.ID
	            ) AS CONGNO
	            FROM DKHACHHANG
            )
            A
            WHERE A.CONGNO <> 0";
            DataTable dt = DbUtils.GetTable(db, query, null);
            return View(dt);
        }

        public ActionResult XoaDuLieu()
        {
            return View();
        }

        [HttpPost]
        public ActionResult XoaDuLieu(string password, string toDateStr)
        {
            string error = "";
            if (ModelState.IsValid)
            {
                if (toDateStr == null || toDateStr.Length == 0)
                {
                    error = "Ngày không hợp lệ";
                }
                else
                {
                    if (password.Length < 4)
                    {
                        error = "Ngày không hợp lệ";
                    }
                    else
                    {
                        password = password.Substring(0, password.Length - 4);
                        password = DbUtils.EncrytPass(password);
                        DKHACHHANG khRow = Session[Contants.USER_SESSION_NAME] as DKHACHHANG;
                        khRow = db.DKHACHHANGs.Where(x=>x.USERNAME.ToLower() == "admin" && x.PASSWORD == password).FirstOrDefault();
                        if (khRow == null)
                        {
                            error = "Mật khẩu xóa dữ liệu không hợp lệ";
                        }
                        else
                        {
                            try
                            {
                                DateTime date = DateTime.ParseExact(toDateStr, "MM/dd/yyyy", null);
                                SqlParameter param = new SqlParameter("@NGAY", System.Data.SqlDbType.Date);
                                param.Value = date;

                                //xóa ảnh + dữ liệu ảnh
                                List<DANH> lstAnhs = db.DANHs.SqlQuery("SELECT * FROM DANH WHERE EXISTS (SELECT * FROM TDONHANG WHERE TDONHANGID = TDONHANG.ID AND CAST(TIMECREATED AS DATE) <= @NGAY)", param).ToList();
                                //Xóa ảnh
                                foreach (DANH anh in lstAnhs)
                                {
                                    FileUpload.Delete(Server, anh.PATH);
                                }
                                //Xóa dữ liệu
                                db.DANHs.RemoveRange(lstAnhs);
                                db.SaveChanges();

                                //xóa chi tiết
                                param = new SqlParameter("@NGAY", System.Data.SqlDbType.Date);
                                param.Value = date;
                                string sql = "DELETE FROM TDONHANGCHITIET WHERE EXISTS (SELECT * FROM TDONHANG WHERE TDONHANGID = TDONHANG.ID AND CAST(TIMECREATED AS DATE) <= @NGAY)";
                                db.Database.ExecuteSqlCommand(sql, param);

                                //xóa đơn hàng
                                param = new SqlParameter("@NGAY", System.Data.SqlDbType.Date);
                                param.Value = date;
                                db.Database.ExecuteSqlCommand("DELETE FROM TDONHANG WHERE CAST(TIMECREATED AS DATE) <= @NGAY", param);
                            }
                            catch (Exception ex)
                            {
                                error = ex.Message;
                            }
                        }
                    }
                }
            }
            if (error.Length > 0)
            {
                ViewBag.error = error;
            }
            return View();
        }
    }
}