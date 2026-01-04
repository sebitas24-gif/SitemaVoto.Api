using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotoModelos;
using Voto.ApiConsumer;

namespace VotoMVC.Controllers
{
    public class VotosController : Controller
    {
        public VotosController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.Voto>.UrlBase = "http://10.241.253.223:5208/api/Votos";
        }
        // GET: VotosController1
        public ActionResult Index()
        {
            var apiResult = Crud<VotoModelos.Voto>.ReadAll();

            // Si apiResult es nulo o Data es nulo, mandamos lista vacía
            var modelo = apiResult?.Data ?? new List<VotoModelos.Voto>();

            return View(modelo);
        }

        // GET: VotosController1/Details/5
        public ActionResult Details(int id)
        {
            var result = Crud<VotoModelos.Voto>.GetById(id);

            // Verificamos que no sea nulo antes de ir a la vista
            if (result == null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // GET: VotosController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VotosController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VotoModelos.Voto data)
        {
            try
            {
                var result = Crud<VotoModelos.Voto>.Create(data);

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

        // GET: VotosController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VotosController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VotoModelos.Voto data)
        {
            try
            {
                var result = Crud<VotoModelos.Voto>.Update(id, data);

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

        // GET: VotosController1/Delete/5
        public ActionResult Delete(int id)
        {
            var result = Crud<VotoModelos.Voto>.GetById(id);

            // 2. Si no hay datos (API caída o ID inexistente), regresamos al Index
            if (result == null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // 3. Enviamos SOLO la especie a la vista
            return View(result.Data);
        }

        // POST: VotosController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VotoModelos.Voto data)
        {

            try
            {
                var result = Crud<VotoModelos.Voto>.Delete(id);

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
