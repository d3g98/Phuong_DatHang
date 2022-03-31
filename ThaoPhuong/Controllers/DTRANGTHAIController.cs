using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThaoPhuong.Models;

namespace ThaoPhuong.Controllers
{
    public class DTRANGTHAIController : Controller
    {
        private DbEntities db = new DbEntities();

        // GET: DTRANGTHAI
        public ActionResult Index()
        {
            return View(db.DTRANGTHAIs.OrderBy(x=>x.ID).ToList());
        }

        // GET: DTRANGTHAI/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,BASIC")] DTRANGTHAI dTRANGTHAI)
        {
            if (ModelState.IsValid)
            {
                dTRANGTHAI.BASIC = 0;
                dTRANGTHAI.ID = Guid.NewGuid().ToString();
                db.DTRANGTHAIs.Add(dTRANGTHAI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dTRANGTHAI);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DTRANGTHAI dTRANGTHAI = db.DTRANGTHAIs.Find(id);
            if (dTRANGTHAI == null)
            {
                return HttpNotFound();
            }
            return View(dTRANGTHAI);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,BASIC")] DTRANGTHAI dTRANGTHAI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dTRANGTHAI).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dTRANGTHAI);
        }

        // GET: DTRANGTHAI/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DTRANGTHAI dTRANGTHAI = db.DTRANGTHAIs.Find(id);
            if (dTRANGTHAI == null)
            {
                return HttpNotFound();
            }
            if (dTRANGTHAI.BASIC > 0)
            {
                ViewBag.error = "Trạng thái mặc định không thể xóa";
            }
            return View(dTRANGTHAI);
        }

        // POST: DTRANGTHAI/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DTRANGTHAI dTRANGTHAI = db.DTRANGTHAIs.Find(id);
            db.DTRANGTHAIs.Remove(dTRANGTHAI);
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
