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
    public class booksController : Controller
    {
        private ql_sachEntities db = new ql_sachEntities();

        //GET: Administrator/books
        public ActionResult Index(string searchString)
        {
            var books = db.books.Include(b => b.supplier);
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.book_name.Contains(searchString));
            }
            return View(books.ToList());
        }


        //[HttpPost]
        //public string Index(FormCollection fc, string searchString)
        //{
        //    return "<h3> From [HttpPost]Index: " + searchString + "</h3>";
        //}

        // GET: Administrator/books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Administrator/books/Create
        public ActionResult Create()
        {
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name");
            return View();
        }

        // POST: Administrator/books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "book_id,book_name,fk_supplier,book_stock,book_seling_price,book_purchase_price")] book book)
        {
            if (ModelState.IsValid)
            {
                db.books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", book.fk_supplier);
            return View(book);
        }

        // GET: Administrator/books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", book.fk_supplier);
            return View(book);
        }

        // POST: Administrator/books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "book_id,book_name,fk_supplier,book_stock,book_seling_price,book_purchase_price")] book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fk_supplier = new SelectList(db.suppliers, "supplier_id", "supplier_name", book.fk_supplier);
            return View(book);
        }

        // GET: Administrator/books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Administrator/books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            book book = db.books.Find(id);
            db.books.Remove(book);
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
