using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookStore.Models;
using BookStore.Areas.Administrator.ViewsModels;

namespace BookStore.Areas.Administrator.Controllers
{
    public class sale_orderController : Controller
    {
        private ql_sachEntities db = new ql_sachEntities();

        // GET: Administrator/sale_order
        public ActionResult Index()
        {
            var sale_order = db.sale_order.Include(s => s.agency);
            return View(sale_order.ToList());
        }

        // GET: Administrator/sale_order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_order sale_order = db.sale_order.Find(id);
            if (sale_order == null)
            {
                return HttpNotFound();
            }
            return View(sale_order);
        }

        // GET: Administrator/sale_order/Create
        public ActionResult Create()
        {
            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name");
            ViewBag.fk_book = new SelectList(db.books, "book_id", "book_name");
            return View();
        }

        // POST: Administrator/sale_order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Create([Bind(Prefix = "sale_order")] sale_order sale_order,
            [Bind(Prefix = "item")] sale_order_item[] sale_order_item)
        {
            big_sale_order big_sale_order = new big_sale_order();
            big_sale_order.sale_order = sale_order;
            if (ModelState.IsValid)
            {
                int sale_order_id = 1;
                if (db.sale_order.Any())
                    sale_order_id = db.sale_order.Max(n => n.sale_order_id) + 1;
                sale_order_item[0] = null;
                sale_order.sale_order_total_money = 0;

                

                //Thiết lập bảng nợ
                agency_debt agency_debt_A = db.agency_debt.OrderByDescending(n => n.agency_debt_time).FirstOrDefault(o => o.fk_agency == sale_order.fk_agency);
                if(agency_debt_A==null)
                {

                    agency_debt agency_debtt = new agency_debt();
                        agency_debtt.agency_debt_total_money = 0;
                        agency_debtt.agency_debt_time = DateTime.Now;
                        agency_debtt.fk_agency = sale_order.fk_agency;
                    int agency_debt_id = 1;
                    if (db.agency_debt.Any())
                        agency_debt_id = db.agency_debt.Max(p => p.agency_debt_id) + 1;
                    List<agency_debt_item> agency_debt_item=new List<agency_debt_item>();
                    for (int i=1;i<sale_order_item.Count();i++)
                    {
                        decimal? price = db.books.Find(sale_order_item[i].fk_book).book_seling_price;
                        decimal? purchase_price = db.books.Find(sale_order_item[i].fk_book).book_purchase_price;
                        //Kiểm tra số lượng
                        store store = new store();
                        store.store_time = DateTime.Now;
                        store.store_seling_price = price;
                        store.store_purchase_price = purchase_price;
                        store.fk_book = sale_order_item[i].fk_book;
                        var check_store = db.stores.OrderByDescending(a => a.store_time).FirstOrDefault(a => a.fk_book == store.fk_book);
                        if((check_store!=null) && (check_store.store_quantity>=sale_order_item[i].sale_order_item_quantity))
                        {
                            store.store_quantity = check_store.store_quantity - sale_order_item[i].sale_order_item_quantity;
                            db.stores.Add(store);
                        }else
                        {
                            ModelState.AddModelError("", "Không đủ số lượng");
                            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name");
                            ViewBag.fk_book = new SelectList(db.books, "book_id", "book_name");
                            return View(big_sale_order);
                        }

                                sale_order_item[i].fk_sale_order = sale_order_id;
                        sale_order_item[i].sale_order_item_price = price;
                        sale_order_item[i].sale_order_money = sale_order_item[i].sale_order_item_price * sale_order_item[i].sale_order_item_quantity;
                        sale_order.sale_order_total_money += sale_order_item[i].sale_order_money;
                            //Thêm nợ cho đại lý
                        agency_debt_item itemm = new agency_debt_item();
                        itemm.fk_book = sale_order_item[i].fk_book;
                        itemm.fk_agency_debt = agency_debt_id;
                        itemm.agency_debt_item_quantity = sale_order_item[i].sale_order_item_quantity;
                        itemm.agency_debt_item_money = sale_order_item[i].sale_order_money;
                        agency_debtt.agency_debt_total_money += itemm.agency_debt_item_money;
                        agency_debt_item.Add(itemm);


                    }
                    agency_debtt.agency_debt_item = agency_debt_item;
                    db.agency_debt.Add(agency_debtt);
                }
                else
                {
                    agency_debt agency_debt = new agency_debt();
                    agency_debt.agency_debt_time = DateTime.Now;
                    agency_debt.fk_agency = sale_order.fk_agency;
                    agency_debt.agency_debt_total_money = 0;
                    int agency_debt_id = 1;
                    if (db.agency_debt.Any())
                        agency_debt_id = db.agency_debt.Max(p => p.agency_debt_id) + 1;
                    List<agency_debt_item> agency_debt_item = new List<agency_debt_item>();
                    for (int i = 1; i < sale_order_item.Count(); i++)
                    {
                        int fk_book = sale_order_item[i].fk_book;
                        decimal? price = db.books.Find(fk_book).book_seling_price;
                        decimal? purchase_price = db.books.Find(fk_book).book_purchase_price;
                        //Kiểm tra số lượng
                        store store = new store();
                        store.store_time = DateTime.Now;
                        store.store_seling_price = price;
                        store.store_purchase_price = purchase_price;
                        store.fk_book = fk_book;
                        var check_store = db.stores.OrderByDescending(a => a.store_time).FirstOrDefault(a => a.fk_book == store.fk_book);
                        if ((check_store != null) && (check_store.store_quantity >= sale_order_item[i].sale_order_item_quantity))
                        {
                            store.store_quantity = check_store.store_quantity - sale_order_item[i].sale_order_item_quantity;
                            db.stores.Add(store);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Không đủ số lượng");
                            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name");
                            ViewBag.fk_book = new SelectList(db.books, "book_id", "book_name");
                            return View(big_sale_order);
                        }

                        sale_order_item[i].fk_sale_order = sale_order_id;
                        sale_order_item[i].sale_order_item_price = price;
                        sale_order_item[i].sale_order_money = sale_order_item[i].sale_order_item_price * sale_order_item[i].sale_order_item_quantity;
                        sale_order.sale_order_total_money += sale_order_item[i].sale_order_money;
                        //Thêm nợ cho đại lý
                        agency_debt_item itemm = db.agency_debt_item.FirstOrDefault(a => (a.fk_book == fk_book)
                                            && (a.fk_agency_debt == agency_debt_A.agency_debt_id));
                        agency_debt_item itemmm = new agency_debt_item();
                        itemmm.fk_agency_debt = agency_debt_id;
                        itemmm.fk_book = fk_book;
                        itemmm.agency_debt_item_quantity = sale_order_item[i].sale_order_item_quantity + itemm.agency_debt_item_quantity;
                        itemmm.agency_debt_item_money = sale_order_item[i].sale_order_money + itemm.agency_debt_item_money;
                        agency_debt.agency_debt_total_money += itemmm.agency_debt_item_money;
                        agency_debt_item.Add(itemmm);


                    }
                    agency_debt.agency_debt_item = agency_debt_item;
                    db.agency_debt.Add(agency_debt);
                }
                sale_order.sale_order_item = sale_order_item;
                sale_order.sale_order_created_at = DateTime.Now;
                db.sale_order.Add(sale_order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name", sale_order.fk_agency);
            return View(big_sale_order);
        }

        // GET: Administrator/sale_order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sale_order sale_order = db.sale_order.Find(id);
            big_sale_order big = new big_sale_order();
            big.sale_order = sale_order;
            if (sale_order == null)
            {
                return HttpNotFound();
            }
            ViewBag.fk_book = new SelectList(db.books, "book_id", "book_name");
            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name", sale_order.fk_agency);
            return View(big);
        }

        // POST: Administrator/sale_order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Prefix = "sale_order")] sale_order sale_order,
            [Bind(Prefix = "item")] sale_order_item[] sale_order_item)
        {
            if (ModelState.IsValid)
            {
                int sale_order_id = sale_order.sale_order_id;
                int fk_agency = sale_order.fk_agency;
                //sale_order_item[0] = null;
                sale_order.sale_order_total_money = 0;

                var sale_order_item_old = db.sale_order_item.Where(a => a.fk_sale_order == sale_order_id).ToList();
                //cap nhat kho
                foreach(var item in sale_order_item_old)
                {
                    int fk_book = item.fk_book;
                    store store = db.stores.OrderByDescending(s => s.store_time).FirstOrDefault(s => s.fk_book == fk_book);
                    store.store_quantity += item.sale_order_item_quantity;
                }

                foreach(var item in sale_order_item)
                {
                    int fk_book = item.fk_book;
                    store store= db.stores.OrderByDescending(s => s.store_time).FirstOrDefault(s => s.fk_book == fk_book);
                    store.store_quantity -= item.sale_order_item_quantity;
                }

                


                db.Entry(sale_order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            big_sale_order big = new big_sale_order();
            big.sale_order = sale_order;
            ViewBag.fk_book = new SelectList(db.books, "book_id", "book_name");
            ViewBag.fk_agency = new SelectList(db.agencies, "agency_id", "agency_name", sale_order.fk_agency);
            return View(big);
        }

        // GET: Administrator/sale_order/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(saveChangesError.GetValueOrDefault())
            {
                ViewBag.MessageError = "Không thể xóa phiếu xuất, vui lòng liên hệ chủ tịch nước để đàm phán.";
            }
            sale_order sale_order = db.sale_order.Find(id);
            if (sale_order == null)
            {
                return HttpNotFound();
            }
            return View(sale_order);
        }

        // POST: Administrator/sale_order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var sale_order_item = (from p in db.sale_order_item where p.fk_sale_order == id select p).ToList();
                foreach(var item in sale_order_item)
                {
                    db.sale_order_item.Remove(item);
                }
                db.SaveChanges();
                sale_order sale_order = db.sale_order.Find(id);
                db.sale_order.Remove(sale_order);
                db.SaveChanges();
            } catch(DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
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
