using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voto.ApiConsumer;
using VotoModelos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VotoMVC.Controllers
{
    public class OpcionElectoralesController : Controller
    {
        public OpcionElectoralesController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.OpcionElectoral>.UrlBase = "http://10.241.253.223:8080/api/OpcionElectorales";
        }
        // GET: OpcionElectoralesController
        public ActionResult Index()
        {
            var apiResult = Voto.ApiConsumer.Crud<OpcionElectoral>.ReadAll();
            var modelo = apiResult?.Data ?? new List<OpcionElectoral>();
            return View(modelo);
        }

        // GET: OpcionElectoralesController/Details/5
        public ActionResult Details(int id)
        {
            var result = Crud<Votante>.GetById(id);
            if (result?.Data == null)
                return RedirectToAction(nameof(Index));

            return View(result.Data);
        }

        // GET: OpcionElectoralesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OpcionElectoralesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OpcionElectoral data)
        {
            try
            {
                var result = Crud<OpcionElectoral>.Create(data);

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

        // GET: OpcionElectoralesController/Edit/5
        public ActionResult Edit(int id)
        {
            var result = Crud<OpcionElectoral>.GetById(id);

            if (result == null || result.Data == null)
            {
                return NotFound();
            }

            return View(result.Data);
        }

        // POST: OpcionElectoralesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, OpcionElectoral data)
        {
            try
            {
                var result = Crud<OpcionElectoral>.Update(id, data);

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

        // GET: OpcionElectoralesController/Delete/5
        public ActionResult Delete(int id)
        {

            var result = Crud<OpcionElectoral>.GetById(id);

            // 2. Si no hay datos (API caída o ID inexistente), regresamos al Index
            if (result == null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // 3. Enviamos SOLO la especie a la vista
            return View(result.Data);
        }

        // POST: OpcionElectoralesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, OpcionElectoral data)
        {
            try
            {
                var result = Crud<OpcionElectoral>.Delete(id);

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
