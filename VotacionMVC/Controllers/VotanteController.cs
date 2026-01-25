using Microsoft.AspNetCore.Mvc;
using VotacionMVC.Service;
using VotacionMVC.Models.DTOs;

namespace VotacionMVC.Controllers
{
    public class VotanteController : Controller
    {
        private readonly ApiService _api;

        public VotanteController(ApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Papeleta(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");

            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Votante", "Acceso");

            // Endpoint candidatos (ajusta si el tuyo es diferente)
            // ejemplo: "api/Candidatos"
            var candidatos = await _api.GetCandidatosAsync() ?? new List<CandidatoDto>();
            return View(candidatos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Emitir(int candidatoId, CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");

            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Votante", "Acceso");

            if (candidatoId <= 0)
            {
                TempData["Msg"] = "Seleccione un candidato (o voto en blanco).";
                return RedirectToAction("Papeleta");
            }

            var req = new VotacionEmitirRequest
            {
                cedula = cedula!,
                codigoPad = codigoPad!,
                candidatoId = candidatoId
            };

            // Endpoint: api/Votacion/emitir
            var resp = await _api.PostAsync<VotacionEmitirRequest, VotacionEmitirResponse>("api/Votacion/emitir", req, ct);

            if (resp == null || resp.ok == false)
            {
                TempData["Msg"] = resp?.error ?? "No se pudo emitir el voto.";
                return RedirectToAction("Papeleta");
            }

            HttpContext.Session.SetString("comprobante", resp.comprobante ?? "");

            // Si tu API ya envía correo, aquí solo mostramos el mensaje.
            // Si no lo envía todavía, podemos implementarlo luego con otro endpoint.
            TempData["CorreoMsg"] = "Comprobante enviado por correo (simulado o por API).";

            return RedirectToAction("Comprobante");
        }

        [HttpGet]
        public IActionResult Comprobante()
        {
            var cedula = HttpContext.Session.GetString("cedula");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction("Votante", "Acceso");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Acceso");
        }
    }
    }

