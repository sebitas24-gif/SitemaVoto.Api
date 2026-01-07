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
            Voto.ApiConsumer.Crud<VotoModelos.Voto>.UrlBase = "http://10.241.253.223:8080/api/Votoes";
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
        public ActionResult Create(VotoModelos.Voto data, int idVotante)
        {
            var votanteResult = Crud<VotoModelos.Votante>.GetById(idVotante);

            if (votanteResult?.Data == null)
            {
                ModelState.AddModelError("", "El votante no existe.");
                return View(data);
            }

           

            // 3️⃣ Guardar voto
            var votoResult = Crud<VotoModelos.Voto>.Create(data);

            if (votoResult == null || !string.IsNullOrEmpty(votoResult.Message))
            {
                ModelState.AddModelError("", votoResult?.Message ?? "Error al registrar el voto.");
                return View(data);
            }

            // 4️⃣ Marcar votante como que ya votó
          
            Crud<VotoModelos.Votante>.Update(votanteResult.Data.Id, votanteResult.Data);

            // 5️⃣ Fin
            return RedirectToAction("Confirmacion");
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
