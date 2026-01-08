using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voto.ApiConsumer;
using VotoModelos;

namespace VotoMVC.Controllers
{
    public class CandidatosController : Controller
    {
        public CandidatosController()
        {
            // IMPORTANTE: Si la API en el otro PC usa un puerto específico
            Voto.ApiConsumer.Crud<VotoModelos.Candidato>.UrlBase = "http://10.241.253.223:8085/api/Candidatos";
        }
        // GET: CandidatosController
        public ActionResult Index()
        {
            var apiResult = Crud<Candidato>.ReadAll();

            // Si apiResult es nulo o Data es nulo, mandamos lista vacía
            var modelo = apiResult?.Data ?? new List<Candidato>();

            return View(modelo);
        }

        // GET: CandidatosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CandidatosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CandidatosController/Create
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

        // GET: CandidatosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CandidatosController/Edit/5
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

        // GET: CandidatosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CandidatosController/Delete/5
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
