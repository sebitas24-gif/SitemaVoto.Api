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
            Voto.ApiConsumer.Crud<VotoModelos.Votante>.UrlBase = "http://10.241.253.223:5208/api/Votantes";
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
            if (result?.Data == null)
                return RedirectToAction(nameof(Index));

            return View(result.Data);
        }

        // GET: VotantesController/Create
        public ActionResult Create()
        {
            return View();
        }
        private bool ValidarCedulaEcuatoriana(string cedula)
        {
            if (string.IsNullOrEmpty(cedula) || cedula.Length != 10 || !long.TryParse(cedula, out _))
                return false;

            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24) return false;

            int tercerDigito = int.Parse(cedula.Substring(2, 1));
            if (tercerDigito > 6) return false;

            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;

            for (int i = 0; i < coeficientes.Length; i++)
            {
                int valor = int.Parse(cedula[i].ToString()) * coeficientes[i];
                suma += valor >= 10 ? valor - 9 : valor;
            }

            int digitoVerificadorRecibido = int.Parse(cedula[9].ToString());
            int residuo = suma % 10;
            int digitoVerificadorCalculado = residuo == 0 ? 0 : 10 - residuo;

            return digitoVerificadorCalculado == digitoVerificadorRecibido;
        }

        // POST: VotantesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Votante data)
        {
            // FILTRO 1: Validar que la cédula sea real 
            if (!ValidarCedulaEcuatoriana(data.Cedula))
            {
                ModelState.AddModelError("Cedula", "La cédula ingresada es falsa o incorrecta.");
                return View(data);
            }

            // FILTRO 2: Validar que no exista ya en la base de datos
            // Obtenemos todos los registros para buscar el duplicado
            var listado = Crud<Votante>.ReadAll();
            if (listado != null && listado.Data != null)
            {
                // Buscamos si algún votante ya tiene esa cédula
                var duplicado = listado.Data.FirstOrDefault(v => v.Cedula == data.Cedula);
                if (duplicado != null)
                {
                    ModelState.AddModelError("Cedula", "Esta cédula ya está registrada en el sistema.");
                    return View(data);
                }
            }

            // VALIDACIÓN DE EDAD (La que ya tienes y funciona)
            if (data.FechaNacimiento == null)
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha es obligatoria.");
                return View(data);
            }

            var hoy = DateTime.Today;
            var edad = hoy.Year - data.FechaNacimiento.Value.Year;
            if (data.FechaNacimiento.Value > hoy.AddYears(-edad)) edad--;

            if (edad < 18)
            {
                ModelState.AddModelError("FechaNacimiento", "Debe ser mayor de edad.");
                return View(data);
            }

            // Si pasó todos los filtros, preparamos y guardamos
            data.Id = 0;
            data.YaVoto = false;
            data.EstaHabilitado = true;
            data.ImagenVerificacion ??= "N/A";

            var result = Crud<Votante>.Create(data);

            if (result != null && string.IsNullOrEmpty(result.Message))
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", result?.Message ?? "Error al conectar con la API.");
            return View(data);
        }

        // GET: VotantesController/Edit/5
        public ActionResult Edit(int id)
        {
            var result = Crud<Votante>.GetById(id);
            if (result?.Data == null)
                return RedirectToAction(nameof(Index));

            return View(result.Data);
        }

        // POST: VotantesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Votante data)
        {
            if (!data.FechaNacimiento.HasValue)
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha es obligatoria.");
                return View(data);
            }

            // 2️⃣ Validación de edad (FORMA CORRECTA)
            var hoy = DateTime.Today;
            var fecha = data.FechaNacimiento.Value.Date;

            var edad = hoy.Year - fecha.Year;
            if (fecha > hoy.AddYears(-edad)) edad--;

            if (edad < 18)
            {
                ModelState.AddModelError("FechaNacimiento", "Debe ser mayor de edad.");
                return View(data);
            }

            // 3️⃣ Update
            var result = Crud<Votante>.Update(id, data);

            if (string.IsNullOrEmpty(result?.Message))
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", result?.Message ?? "Error al actualizar.");
            return View(data);
        }

        // GET: VotantesController/Delete/5
        public ActionResult Delete(int id)
        {
            var result = Crud<Votante>.GetById(id);
            if (result?.Data == null)
                return RedirectToAction(nameof(Index));

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
