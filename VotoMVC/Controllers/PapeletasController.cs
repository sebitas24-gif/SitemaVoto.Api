using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VotoMVC.Controllers
{
    public class PapeletasController : Controller
    {
        // GET: PapeletasController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PapeletasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PapeletasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PapeletasController/Create
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

        // GET: PapeletasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PapeletasController/Edit/5
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

        // GET: PapeletasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PapeletasController/Delete/5
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
