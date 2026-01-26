using Microsoft.AspNetCore.Mvc;
using System.Text;
using VotacionMVC.Service;
using System.Text;
using VotacionMVC.Models.DTOs;
using VotoModelos.Enums;

namespace VotacionMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _api;
        public AdminController(ApiService api) => _api = api;

       

        [HttpGet]
        public async Task<IActionResult> Candidatos(CancellationToken ct)
        {
            var lista = await _api.GetCandidatosAsync(ct) ;
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Padron(CancellationToken ct)
        {
            var lista = await _api.GetPadronAsync(ct);
            return View(lista);
        }

        [HttpGet]
        public IActionResult Exportar() => View();

        // Botón descargar (si la API no da PDF, entregamos uno dummy para que funcione)
        [HttpPost]
        public async Task<IActionResult> DescargarPdf(string provincia, CancellationToken ct)
        {
            var prov = string.IsNullOrWhiteSpace(provincia) ? "Nacional" : provincia;

            var bytes = await _api.TryDownloadResultadosPdfAsync(prov, ct)
                        ?? PdfDummy(prov); // ✅ aquí se llama al método privado del controller

            var fileName = $"Resultados_{prov}.pdf";
            return File(bytes, "application/pdf", fileName);
        }

        // Botón enviar (simulado)
        [HttpPost]
        public IActionResult EnviarPdf(string provincia)
        {
            TempData["Msg"] = $"PDF enviado (simulado) para: {(provincia ?? "Nacional")}.";
            return RedirectToAction("Exportar");
        }
        private static byte[] PdfDummy(string provincia)
        {
            var contenido = $@"%PDF-1.4
1 0 obj
<< /Type /Catalog /Pages 2 0 R >>
endobj
2 0 obj
<< /Type /Pages /Kids [3 0 R] /Count 1 >>
endobj
3 0 obj
<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R >>
endobj
4 0 obj
<< /Length 120 >>
stream
BT
/F1 18 Tf
72 720 Td
(Resultados - {provincia}) Tj
ET
endstream
endobj
xref
0 5
0000000000 65535 f
0000000010 00000 n
0000000060 00000 n
0000000117 00000 n
0000000204 00000 n
trailer
<< /Root 1 0 R /Size 5 >>
startxref
320
%%EOF";
            return Encoding.ASCII.GetBytes(contenido);
        }



        [HttpGet]
        public IActionResult Procesos()
        {
            return View(new ProcesoCrearRequest
            {
                Tipo = TipoEleccion.Plancha,
                Estado = EstadoProceso.Configuracion,
                InicioLocal = DateTime.Now,
                FinLocal = DateTime.Now.AddDays(1)
            });
        }

        // POST: /Admin/Procesos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Procesos(ProcesoCrearRequest model, CancellationToken ct)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                TempData["Error"] = "El nombre del proceso es obligatorio.";
                return View(model);
            }

            if (model.FinLocal <= model.InicioLocal)
            {
                TempData["Error"] = "La fecha fin debe ser mayor que la fecha inicio.";
                return View(model);
            }

            // Por seguridad: asegura estado por defecto
            if (model.Estado == 0)
                model.Estado = EstadoProceso.Configuracion;

            // Llamada API
            var resp = await _api.CrearProcesoAsync(model, ct);

            // Si falló (resp null o error HTTP)
            if (resp == null)
            {
                TempData["Error"] = _api.LastError ?? "No se pudo crear el proceso.";
                ViewBag.JsonEnviado = _api.LastJsonSent;  // (opcional debug)
                return View(model);
            }

            // ✅ Ajusta estas 3 líneas si tu response tiene otros nombres:
            // Por ejemplo: resp.Ok, resp.Error, resp.Data
            var okProp = resp.GetType().GetProperty("ok") ?? resp.GetType().GetProperty("Ok");
            var errProp = resp.GetType().GetProperty("error") ?? resp.GetType().GetProperty("Error");
            var dataProp = resp.GetType().GetProperty("data") ?? resp.GetType().GetProperty("Data");

            bool ok = okProp != null && (bool)(okProp.GetValue(resp) ?? false);
            string? err = errProp?.GetValue(resp)?.ToString();
            var data = dataProp?.GetValue(resp);

            if (!ok)
            {
                TempData["Error"] = err ?? _api.LastError ?? "No se pudo crear el proceso.";
                ViewBag.JsonEnviado = _api.LastJsonSent; // (opcional debug)
                return View(model);
            }

            TempData["Ok"] = $"✅ Proceso creado. ID: {data}";
            return RedirectToAction(nameof(Procesos));
        }


    }
}
