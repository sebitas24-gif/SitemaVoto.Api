using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VotoMVC.Controllers
{
    public class ResultadoOpcionesController : Controller
    {
        // GET: ResultadoOpcionesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ResultadoOpcionesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ResultadoOpcionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ResultadoOpcionesController/Create
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

        // GET: ResultadoOpcionesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ResultadoOpcionesController/Edit/5
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

        // GET: ResultadoOpcionesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ResultadoOpcionesController/Delete/5
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
