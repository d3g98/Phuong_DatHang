using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThaoPhuong.Models;
using ThaoPhuong.Utils;

namespace ThaoPhuong.Controllers
{
    [FilterUrl]
    public class DKHACHHANGController : Controller
    {
        private DbEntities db = new DbEntities();
        // GET: DKHACHHANG
        public ActionResult Index()
        {
            return View(db.DKHACHHANGs.Where(x=>x.ISADMIN != 30).ToList());
        }

        public ActionResult KichHoat(string id)
        {
            string error = "";
            try
            {
                DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.ID == id).FirstOrDefault();
                if (khRow == null)
                {
                    return Content("Khách hàng không tồn tại trong hệ thống!");
                }
                khRow.ISACTIVE = khRow.ISACTIVE == 0 ? 30 : 0;
                db.Entry(khRow);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return Content(error);
        }

        public ActionResult ResetPass(string id)
        {
            string error = "";
            try
            {
                DKHACHHANG khRow = db.DKHACHHANGs.Where(x => x.ID == id).FirstOrDefault();
                if (khRow == null)
                {
                    return Content("Khách hàng không tồn tại trong hệ thống!");
                }
                khRow.PASSWORD = DbUtils.EncrytPass("1");
                db.Entry(khRow);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return Content(error);
        }

        // GET: DKHACHHANG/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DKHACHHANG/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,USERNAME,PASSWORD,DIENTHOAI,DIACHI,ISADMIN,ISACTIVE")] DKHACHHANG dKHACHHANG)
        {
            if (ModelState.IsValid)
            {
                db.DKHACHHANGs.Add(dKHACHHANG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dKHACHHANG);
        }

        // GET: DKHACHHANG/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DKHACHHANG dKHACHHANG = db.DKHACHHANGs.Find(id);
            if (dKHACHHANG == null)
            {
                return HttpNotFound();
            }
            return View(dKHACHHANG);
        }

        // POST: DKHACHHANG/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,USERNAME,PASSWORD,DIENTHOAI,DIACHI,ISADMIN,ISACTIVE")] DKHACHHANG dKHACHHANG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dKHACHHANG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dKHACHHANG);
        }

        // GET: DKHACHHANG/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DKHACHHANG dKHACHHANG = db.DKHACHHANGs.Find(id);
            if (dKHACHHANG == null)
            {
                return HttpNotFound();
            }
            return View(dKHACHHANG);
        }

        // POST: DKHACHHANG/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DKHACHHANG dKHACHHANG = db.DKHACHHANGs.Find(id);
            db.DKHACHHANGs.Remove(dKHACHHANG);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
