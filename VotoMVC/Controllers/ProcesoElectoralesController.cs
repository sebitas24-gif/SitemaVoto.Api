using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voto.ApiConsumer;
using VotoModelos;

namespace VotoMVC.Controllers
{
    public class ProcesoElectoralesController : Controller
    {
        public ProcesoElectoralesController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.ProcesoElectoral>.UrlBase = "http://10.241.253.223:8080/api/ProcesoElectorales";
        }
        // GET: ProcesoElectoralesController
        public ActionResult Index()
        {
            var apiResult = Voto.ApiConsumer.Crud<ProcesoElectoral>.ReadAll();
            var modelo = apiResult?.Data ?? new List<ProcesoElectoral>();
            return View(modelo);
        }

        // GET: ProcesoElectoralesController/Details/5
        public ActionResult Details(int id)
        {
            var result = Crud<ProcesoElectoral>.GetById(id);
            if (result?.Data == null)
                return RedirectToAction(nameof(Index));

            return View(result.Data);
        }

        // GET: ProcesoElectoralesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProcesoElectoralesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProcesoElectoral data)
        {
            // Filtros de validación
            if (string.IsNullOrWhiteSpace(data.Nombre) || data.Nombre.Length < 5)
            {
                ModelState.AddModelError("Nombre", "El nombre debe ser descriptivo (ej. Elecciones Presidenciales 2026).");
                return View(data);
            }

            if (data.FechaFin <= data.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La elección no puede terminar antes de empezar.");
                return View(data);
            }

            // Ajuste de Fechas para Swagger/PostgreSQL (UTC)
            data.FechaInicio = new DateTime(data.FechaInicio.Year, data.FechaInicio.Month, data.FechaInicio.Day,
                                           data.FechaInicio.Hour, data.FechaInicio.Minute, 0, DateTimeKind.Utc);

            data.FechaFin = new DateTime(data.FechaFin.Year, data.FechaFin.Month, data.FechaFin.Day,
                                         data.FechaFin.Hour, data.FechaFin.Minute, 0, DateTimeKind.Utc);

            data.Id = 0;
         

            
            var result = Crud<ProcesoElectoral>.CreateProceso(data);

            if (result != null && result.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", result?.Message ?? "Error al conectar con la API.");
            return View(data);
        }

        // GET: ProcesoElectoralesController/Edit/5
        public ActionResult Edit(int id)
        {

            var result = Crud<ProcesoElectoral>.GetById(id);

            if (result == null || result.Data == null)
            {
                return NotFound();
            }

            return View(result.Data);
        }

        // POST: ProcesoElectoralesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ProcesoElectoral data)
        {
            if (id != data.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Forzamos UTC y segundos a 0 para mantener la paz con la API
                data.FechaInicio = new DateTime(data.FechaInicio.Year, data.FechaInicio.Month, data.FechaInicio.Day,
                                               data.FechaInicio.Hour, data.FechaInicio.Minute, 0, DateTimeKind.Utc);

                data.FechaFin = new DateTime(data.FechaFin.Year, data.FechaFin.Month, data.FechaFin.Day,
                                             data.FechaFin.Hour, data.FechaFin.Minute, 0, DateTimeKind.Utc);

             
                var result = Crud<ProcesoElectoral>.UpdateProceso(id, data);

                if (result != null && result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result?.Message ?? "Error al actualizar en la API.");
            }
            return View(data);
        }

        // GET: ProcesoElectoralesController/Delete/5
        public ActionResult Delete(int id)
        {
            var result = Crud<ProcesoElectoral>.GetById(id);
            if (result?.Data == null)
                return RedirectToAction(nameof(Index));

            return View(result.Data);
        }

        // POST: ProcesoElectoralesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ProcesoElectoral data)
        {
            try
            {
                var result = Crud<ProcesoElectoral>.Delete(id);

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
