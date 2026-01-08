using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voto.ApiConsumer;
using VotoModelos;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var result = Crud<Candidato>.GetById(id);

            // Verificamos que no sea nulo antes de ir a la vista
            if (result == null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
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
            var result = Crud<Candidato>.GetById(id);

            if (result == null || result.Data == null)
            {
                return NotFound();
            }

            return View(result.Data);
        }

        // POST: CandidatosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Candidato data)
        {
            try
            {
                var result = Crud<Candidato>.Update(id, data);

                if (result != null && result.Data != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(data);
            }
            catch
            {
                return View(data);
            }
        }

        // GET: CandidatosController/Delete/5
        public ActionResult Delete(int id)
        {
            var result = Crud<Candidato>.GetById(id);

            // 2. Si no hay datos (API caída o ID inexistente), regresamos al Index
            if (result == null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // 3. Enviamos SOLO la candidato a la vista
            return View(result.Data);
        }

        // POST: CandidatosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Candidato data)
        {
            try
            {
                var result = Crud<Candidato>.Delete(id);

                if (result != null && result.Data == true) // Si Data es true, se eliminó bien
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(data);
            }
            catch
            {
                return View(data);
            }
        }
    }
}
