using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Shad_BookingApplication.Models;

namespace Shad_BookingApplication.Controllers
{
    public class AspNetCompanyCustomersController : Controller
    {
        private BookingModelEntities db = new BookingModelEntities();

        // GET: AspNetCompanyCustomers
        public ActionResult Index()
        {
            var aspNetCompanyCustomers = db.AspNetCompanyCustomers.Include(a => a.AspNetCompanyNotifination).Include(a => a.AspNetCustomerDetail);
            return View(aspNetCompanyCustomers.ToList());
        }

        // GET: AspNetCompanyCustomers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetCompanyCustomer aspNetCompanyCustomer = db.AspNetCompanyCustomers.Find(id);
            if (aspNetCompanyCustomer == null)
            {
                return HttpNotFound();
            }
            return View(aspNetCompanyCustomer);
        }

        // GET: AspNetCompanyCustomers/Create
        public ActionResult Create()
        {
            ViewBag.CompanyNotificationId = new SelectList(db.AspNetCompanyNotifinations, "Id", "EmailBeforeArrive");
            ViewBag.DetailId = new SelectList(db.AspNetCustomerDetails, "Id", "BussinessName");
            return View();
        }

        // POST: AspNetCompanyCustomers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DetailId,Adress,City,PostCode,TimeZone,Gender,BateofBirth,RefferedBy,Contradiction,CompanyNotificationId")] AspNetCompanyCustomer aspNetCompanyCustomer)
        {
            if (ModelState.IsValid)
            {
                db.AspNetCompanyCustomers.Add(aspNetCompanyCustomer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyNotificationId = new SelectList(db.AspNetCompanyNotifinations, "Id", "EmailBeforeArrive", aspNetCompanyCustomer.CompanyNotificationId);
            ViewBag.DetailId = new SelectList(db.AspNetCustomerDetails, "Id", "BussinessName", aspNetCompanyCustomer.DetailId);
            return View(aspNetCompanyCustomer);
        }

        // GET: AspNetCompanyCustomers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetCompanyCustomer aspNetCompanyCustomer = db.AspNetCompanyCustomers.Find(id);
            if (aspNetCompanyCustomer == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyNotificationId = new SelectList(db.AspNetCompanyNotifinations, "Id", "EmailBeforeArrive", aspNetCompanyCustomer.CompanyNotificationId);
            ViewBag.DetailId = new SelectList(db.AspNetCustomerDetails, "Id", "BussinessName", aspNetCompanyCustomer.DetailId);
            return View(aspNetCompanyCustomer);
        }

        // POST: AspNetCompanyCustomers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DetailId,Adress,City,PostCode,TimeZone,Gender,BateofBirth,RefferedBy,Contradiction,CompanyNotificationId")] AspNetCompanyCustomer aspNetCompanyCustomer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetCompanyCustomer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyNotificationId = new SelectList(db.AspNetCompanyNotifinations, "Id", "EmailBeforeArrive", aspNetCompanyCustomer.CompanyNotificationId);
            ViewBag.DetailId = new SelectList(db.AspNetCustomerDetails, "Id", "BussinessName", aspNetCompanyCustomer.DetailId);
            return View(aspNetCompanyCustomer);
        }

        // GET: AspNetCompanyCustomers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetCompanyCustomer aspNetCompanyCustomer = db.AspNetCompanyCustomers.Find(id);
            if (aspNetCompanyCustomer == null)
            {
                return HttpNotFound();
            }
            return View(aspNetCompanyCustomer);
        }

        // POST: AspNetCompanyCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetCompanyCustomer aspNetCompanyCustomer = db.AspNetCompanyCustomers.Find(id);
            db.AspNetCompanyCustomers.Remove(aspNetCompanyCustomer);
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
