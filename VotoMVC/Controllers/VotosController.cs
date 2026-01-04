using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotoModelos;
using Voto.ApiConsumer;
using BCrypt.Net;

namespace VotoMVC.Controllers
{
    public class VotosController : Controller
    {
        public VotosController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.Voto>.UrlBase = "http://10.241.253.223:5208/api/Votoes";
        }
        // GET: VotosController1
        public ActionResult Index()
        {
            var apiResult = Voto.ApiConsumer.Crud<VotoModelos.Voto>.ReadAll();
            var modelo = apiResult?.Data ?? new List<VotoModelos.Voto>();
            return View(modelo);
        }

        // GET: VotosController1/Details/5
        public ActionResult Details(int id)
        {

            // Verificamos que no sea nulo antes de ir a la vista
            var result = Crud<VotoModelos.Voto>.GetById(id);
            if (result?.Data == null)
                return RedirectToAction(nameof(Index));

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
            // CAMBIO 1: Limpiamos el ID para que la DB asigne el nuevo
            data.Id = 0;

            // CAMBIO 2: Generamos la fecha exacta para Swagger (Formato ISO automático)
            data.FechaHora = DateTime.Now;

            // CAMBIO 3: Encriptamos el voto antes de enviarlo
            data.VotoEncriptado = BCrypt.Net.BCrypt.HashPassword(data.OpcionElectoralId.ToString());

            try
            {
                // Usamos la ruta completa para evitar errores de ambigüedad
                var result = Voto.ApiConsumer.Crud<VotoModelos.Voto>.Create(data);

                // MODIFICACIÓN AQUÍ: Si el resultado no es nulo y la API no dio error de conexión
                if (result != null && (result.Data != null || result.Message == null))
                {
                    return RedirectToAction(nameof(Index));
                }

                // Si realmente hubo un error en la API, lo mostramos
                ModelState.AddModelError("", "Respuesta API: " + (result?.Message ?? "Error desconocido"));
                return View(data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar: " + ex.Message);
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
        // GET: Votos/Edit/5
        public ActionResult Edit(int id)
        {
            // Buscamos el voto actual en la API
            var result = Voto.ApiConsumer.Crud<VotoModelos.Voto>.GetById(id);

            if (result == null || result.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }
        // POST: Votos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VotoModelos.Voto data)
        {
            try
            {
                // 1. Mantenemos el ID que viene de la URL
                data.Id = id;

                // 2. IMPORTANTE: Si el usuario cambió el candidato, 
                // debemos re-generar el hash de seguridad
                data.VotoEncriptado = BCrypt.Net.BCrypt.HashPassword(data.OpcionElectoralId.ToString());

                // 3. La fecha normalmente no se edita, pero la API la requiere.
                // Si en tu vista la quitaste, asegúrate de que no vaya nula:
                if (data.FechaHora == DateTime.MinValue)
                {
                    data.FechaHora = DateTime.Now;
                }

                // 4. Enviamos la actualización a la API
                var result = Voto.ApiConsumer.Crud<VotoModelos.Voto>.Update(id, data);

                if (result != null && (result.Data != null || result.Message == null))
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "La API no permitió editar: " + result?.Message);
                return View(data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al editar: " + ex.Message);
                return View(data);
            }
        }

        // POST: VotosController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VotoModelos.Voto data)
        {

            try
            {
                // Ruta completa para evitar errores
                var result = Voto.ApiConsumer.Crud<VotoModelos.Voto>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
