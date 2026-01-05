using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voto.ApiConsumer;
using VotoModelos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VotoMVC.Controllers
{
    public class PapeletasController : Controller
    {
        public PapeletasController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.Papeleta>.UrlBase = "http://10.241.253.223:8080/api/Papeletas";
        }
        // GET: PapeletasController
        public ActionResult Index()
        {
            var apiResult = Voto.ApiConsumer.Crud<Papeleta>.ReadAll();
            var modelo = apiResult?.Data ?? new List<Papeleta>();
            return View(modelo);
        }

        // GET: PapeletasController/Details/5
        public ActionResult Details(int id)
        {
            var result = Crud<Papeleta>.GetById(id);
            if (result?.Data == null)
                return RedirectToAction(nameof(Index));

            return View(result.Data);
        }

        // GET: PapeletasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PapeletasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Papeleta data)
        {
            try
            {
                var result = Crud<Papeleta>.Create(data);

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

        // GET: PapeletasController/Edit/5
        public ActionResult Edit(int id)
        {
            var result = Crud<Papeleta>.GetById(id);

            if (result == null || result.Data == null)
            {
                return NotFound();
            }

            return View(result.Data);
        }

        // POST: PapeletasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Papeleta data)
        {
            try
            {
                var result = Crud<Papeleta>.Update(id, data);

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

        // GET: PapeletasController/Delete/5
        public ActionResult Delete(int id)
        {
            var result = Crud<Papeleta>.GetById(id);

            // 2. Si no hay datos (API caída o ID inexistente), regresamos al Index
            if (result == null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // 3. Enviamos SOLO la especie a la vista
            return View(result.Data);
        }

        // POST: PapeletasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Papeleta data)
        {
            try
            {
                var result = Crud<Papeleta>.Delete(id);

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
