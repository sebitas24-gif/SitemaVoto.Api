using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotoModelos;

namespace VotoMVC.Controllers
{
    public class AuditoriasController : Controller
    {
        public AuditoriasController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.Auditoria>.UrlBase = "http://10.241.253.223:5208/api/Auditorias";
        }
        // GET: AuditoriasController
        public ActionResult Index()
        {
            var apiResult = Voto.ApiConsumer.Crud<Auditoria>.ReadAll();
            var modelo = apiResult?.Data ?? new List<Auditoria>();
            return View(modelo);
        }

        // GET: AuditoriasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AuditoriasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuditoriasController/Create
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

        // GET: AuditoriasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AuditoriasController/Edit/5
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

        // GET: AuditoriasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AuditoriasController/Delete/5
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
