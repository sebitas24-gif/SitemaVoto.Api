using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotoModelos;
using Voto.ApiConsumer;

namespace VotoMVC.Controllers
{
    public class VotantesController : Controller
    {
        // GET: VotantesController
        public VotantesController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.Votante>.UrlBase = "http://10.241.253.223:5208/api/Votantes/";
        }
        public ActionResult Index()
        {
            var apiResult = Voto.ApiConsumer.Crud<Votante>.ReadAll();
            var modelo = apiResult?.Data ?? new List<Votante>();
            return View(modelo);
        }

        // GET: VotantesController/Details/5
        public ActionResult Details(int id)
        {
            var result = Crud<Votante>.GetById(id);
            if(result== null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // GET: VotantesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VotantesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Votante data)
        {
            try
            {
                var result = Crud <Votante>.Create(data);

                // Si la API devuelve el objeto creado, 'result.Data' no será nulo
                if (result != null && result.Data != null)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "La API no pudo guardar los datos.");
                return View(data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(data);
            }
        }

        // GET: VotantesController/Edit/5
        public ActionResult Edit(int id)
        {
            var result = Crud<Votante>.GetById(id);
            if (result == null || result.Data == null)
            {
                return NotFound();
            }
            return View(result.Data);
        }

        // POST: VotantesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Votante data)
        {
            try
            {
                var result = Crud<Votante>.Update(id, data);

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

        // GET: VotantesController/Delete/5
        public ActionResult Delete(int id)
        {
            var result = Crud<Votante>.GetById(id);

            // 2. Si no hay datos (API caída o ID inexistente), regresamos al Index
            if (result == null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // 3. Enviamos SOLO la especie a la vista
            return View(result.Data);
        }

        // POST: VotantesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Votante data)
        {
            try
            {
                var result = Crud<Votante>.Delete(id);

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
