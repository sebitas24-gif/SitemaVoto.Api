using Microsoft.AspNetCore.Mvc;
using System.Text;
using VotacionMVC.Service;
using System.Text;
using VotacionMVC.Models.DTOs;

namespace VotacionMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _api;
        public AdminController(ApiService api) => _api = api;

        [HttpGet]
        public IActionResult Procesos() => View(); // vista simple (form)

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



        [HttpPost]
        public async Task<IActionResult> Procesos(ProcesoCrearRequest model, CancellationToken ct)
        {
            var resp = await _api.CrearProcesoAsync(model, ct);

            if (resp == null)
            {
                ViewBag.Msg = "No se pudo crear el proceso. " + (_api.LastError ?? "");
                return View(model);
            }

            if (resp.ok == false)
            {
                ViewBag.Msg = "No se pudo crear el proceso: " + (resp.error ?? _api.LastError ?? "Error desconocido");
                return View(model);
            }

            TempData["Msg"] = "✅ Proceso creado correctamente.";
            return RedirectToAction("Procesos");
        }


    }
}
