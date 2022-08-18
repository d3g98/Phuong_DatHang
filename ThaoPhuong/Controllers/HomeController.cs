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
    public class HomeController : Controller
    {
        THAOPHUONGEntities db = new THAOPHUONGEntities();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.data = SconfigController.LayDuLieuThongBaoKhachHang(db);
            return View();
        }
        public ActionResult ChiTietCongNo(string id)
        {
            string query = @"SELECT A.*, FORMAT(A.TIMECREATED, 'dd/MM/yyyy') AS NGAY FROM
            (
	            SELECT TIMECREATED, NAME, (CASE WHEN COALESCE(LOAI, 0) = 0 THEN N'Đơn gom' ELSE N'Đơn nhặt' END) AS DIENGIAI, NOTE,
	            TONGCONG, CAST(0 AS DECIMAL(18,2)) AS TIENTHANHTOAN, CAST(0 AS DECIMAL(18,2)) AS LUYKE
	            FROM TDONHANG WHERE DKHACHHANGID = @DKHACHHANGID AND EXISTS (SELECT * FROM TTHANHTOANCHITIET WHERE TDONHANGID = TDONHANG.ID)
	            UNION ALL
	            SELECT TIMECREATED, NAME, N'Thanh toán', NOTE, 0, TIENTHANHTOAN, 0 FROM TTHANHTOAN WHERE DKHACHHANGID = @DKHACHHANGID
            )
            A
            ORDER BY A.TIMECREATED ASC";

            ViewBag.layout = Contants.LAYOUT_HOME;
            DKHACHHANG currentUser = Session[Contants.USER_SESSION_NAME] as DKHACHHANG;

            DKHACHHANG khRow = db.DKHACHHANGs.Find(id);

            ViewBag.NAME = khRow.NAME;
            ViewBag.isAdmin = false;
            if (SessionUtils.IsAdmin(Session))
            {
                ViewBag.isAdmin = true;
                ViewBag.layout = Contants.LAYOUT_ADMIN;
            }
            else
            {
                if (id != currentUser.ID)
                {
                    DataTable temp = new DataTable();
                    return View(temp);
                }
            }
            SqlParameter p = new SqlParameter("@DKHACHHANGID", SqlDbType.VarChar);
            p.Value = id;
            DataTable dt = DbUtils.GetTable(db, query, p);
            decimal luyKe = 0;
            foreach (DataRow row in dt.Rows)
            {
                luyKe += (decimal)row["TONGCONG"] - (decimal)row["TIENTHANHTOAN"];
                row["LUYKE"] = luyKe;
            }

            ViewBag.congNo = DbUtils.NumberToText(luyKe);

            return View(dt);
        }
    }
}