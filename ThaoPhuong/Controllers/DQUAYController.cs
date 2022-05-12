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
    public class DQUAYController : Controller
    {
        private THAOPHUONGEntities db = new THAOPHUONGEntities();

        // GET: DQUAY
        public ActionResult Index()
        {
            return View(db.DQUAYs.OrderByDescending(x=>x.POSITION).ToList());
        }

        // GET: DQUAY/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DQUAY/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,POSITION")] DQUAY dQUAY)
        {
            if (ModelState.IsValid)
            {
                dQUAY.ID = Guid.NewGuid().ToString();
                db.DQUAYs.Add(dQUAY);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dQUAY);
        }

        // GET: DQUAY/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DQUAY dQUAY = db.DQUAYs.Find(id);
            if (dQUAY == null)
            {
                return HttpNotFound();
            }
            return View(dQUAY);
        }

        // POST: DQUAY/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,POSITION")] DQUAY dQUAY)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dQUAY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dQUAY);
        }

        // GET: DQUAY/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DQUAY dQUAY = db.DQUAYs.Find(id);
            if (dQUAY == null)
            {
                return HttpNotFound();
            }
            return View(dQUAY);
        }

        // POST: DQUAY/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DQUAY dQUAY = db.DQUAYs.Find(id);
            db.DQUAYs.Remove(dQUAY);
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
