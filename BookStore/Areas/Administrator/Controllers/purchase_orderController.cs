using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookStore.Models;

namespace BookStore.Areas.Administrator.Controllers
{
    public class purchase_orderController : Controller
    {
        private ql_sachEntities db = new ql_sachEntities();

        // GET: Administrator/purchase_order
        public ActionResult Index()
        {
            var purchase_order = db.purchase_order.Include(p => p.supplier);
            return View(purchase_order.ToList());
        }

        // GET: Administrator/purchase_order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase_order purchase_order = db.purchase_order.Find(id);
            if (purchase_order == null)
            {
                return HttpNotFound();
            }
            return View(purchase_order);
        }

        // GET: Administrator/purchase_order/Create
        public ActionResult Create()
        {
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name");
            return View();
        }

        // POST: Administrator/purchase_order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "purchase_order_id,fk_supplier,purchase_order_created_at,purchase_order_total_money,purchase_order_status")] purchase_order purchase_order)
        {
            if (ModelState.IsValid)
            {
                db.purchase_order.Add(purchase_order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", purchase_order.fk_supplier);
            return View(purchase_order);
        }

        // GET: Administrator/purchase_order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase_order purchase_order = db.purchase_order.Find(id);
            if (purchase_order == null)
            {
                return HttpNotFound();
            }
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", purchase_order.fk_supplier);
            return View(purchase_order);
        }

        // POST: Administrator/purchase_order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "purchase_order_id,fk_supplier,purchase_order_created_at,purchase_order_total_money,purchase_order_status")] purchase_order purchase_order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchase_order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", purchase_order.fk_supplier);
            return View(purchase_order);
        }

        // GET: Administrator/purchase_order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase_order purchase_order = db.purchase_order.Find(id);
            if (purchase_order == null)
            {
                return HttpNotFound();
            }
            return View(purchase_order);
        }

        // POST: Administrator/purchase_order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            purchase_order purchase_order = db.purchase_order.Find(id);
            db.purchase_order.Remove(purchase_order);
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
