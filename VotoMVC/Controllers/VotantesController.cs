using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voto.ApiConsumer;
using VotoModelos;

namespace VotoMVC.Controllers
{
    public class VotantesController : Controller
    {
        public VotantesController()
        {
            // IMPORTANTE: Si la API en el otro PC usa un puerto específico
            Voto.ApiConsumer.Crud<VotoModelos.Votante>.UrlBase = "http://10.241.253.223:8085/api/Votantes";
        }
        // GET: VotantesController
        public ActionResult Index()
        {
            var apiResult = Crud<Votante>.ReadAll();

            // Si apiResult es nulo o Data es nulo, mandamos lista vacía
            var modelo = apiResult?.Data ?? new List<Votante>();

            return View(modelo);
        }

        // GET: VotantesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VotantesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VotantesController/Create
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

        // GET: VotantesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VotantesController/Edit/5
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

        // GET: VotantesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VotantesController/Delete/5
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
