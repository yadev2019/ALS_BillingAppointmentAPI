using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALS_BillingAppointmentAPI.Controllers
{
    public class BillingAppointmentController : Controller
    {
        // GET: BillingAppointmentController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BillingAppointmentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BillingAppointmentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BillingAppointmentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BillingAppointmentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BillingAppointmentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BillingAppointmentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BillingAppointmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
