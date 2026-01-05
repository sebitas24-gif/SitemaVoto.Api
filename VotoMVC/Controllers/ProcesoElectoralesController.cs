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
            // FILTRO 1: Validar que el nombre no sea nulo o muy corto
            if (string.IsNullOrWhiteSpace(data.Nombre) || data.Nombre.Length < 5)
            {
                ModelState.AddModelError("Nombre", "El nombre del proceso debe tener al menos 5 caracteres.");
                return View(data);
            }

            // FILTRO 2: Validar coherencia de fechas (Lógica de tiempo)
            // No puedes terminar antes de empezar
            if (data.FechaFin <= data.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La fecha de finalización debe ser posterior a la de inicio.");
                return View(data);
            }

            // FILTRO 3: Validar que el proceso no empiece en el pasado
            // Le damos un margen de 10 minutos por desfases de reloj entre tu PC y Render
            if (data.FechaInicio < DateTime.Now.AddMinutes(-10))
            {
                ModelState.AddModelError("FechaInicio", "La fecha de inicio no puede ser una fecha pasada.");
                return View(data);
            }

            // FILTRO 4: Validar que no exista otro proceso con el mismo nombre activo
            var listado = Crud<ProcesoElectoral>.ReadAll();
            if (listado != null && listado.Data != null)
            {
                var duplicado = listado.Data.FirstOrDefault(p =>
                    p.Nombre.Trim().ToLower() == data.Nombre.Trim().ToLower() && p.Activo);

                if (duplicado != null)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un proceso electoral activo con este nombre.");
                    return View(data);
                }
            }

            // AJUSTE DE FECHAS PARA LA API (Evitar el error de formato)
            // Forzamos a que sean tratadas como UTC para que PostgreSQL en Render no de error
            data.FechaInicio = DateTime.SpecifyKind(data.FechaInicio, DateTimeKind.Utc);
            data.FechaFin = DateTime.SpecifyKind(data.FechaFin, DateTimeKind.Utc);

            // PREPARACIÓN FINAL
            data.Id = 0;
            data.Activo = true;

            // INTENTO DE GUARDADO
            var result = Crud<ProcesoElectoral>.Create(data);

            if (result != null && result.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", result?.Message ?? "Error de comunicación con la API en Render.");
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
            try
            {
                var result = Crud<ProcesoElectoral>.Update(id, data);
                if (result != null && result.Success) return RedirectToAction(nameof(Index));
                return View(data);
            }
            catch { return View(data); }
        }

        // GET: ProcesoElectoralesController/Delete/5
        public ActionResult Delete(int id)
        {
            var result = Crud<ProcesoElectoral>.GetById(id);

            // 2. Si no hay datos (API caída o ID inexistente), regresamos al Index
            if (result == null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // 3. Enviamos SOLO la especie a la vista
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
