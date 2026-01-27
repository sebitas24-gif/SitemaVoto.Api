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
        public async Task<IActionResult> Padron()
        {
            try
            {
                var data = await _api.GetPadronAsync();
                ViewBag.Debug = $"Count={data.Count}";
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.Debug = ex.Message;
                return View(new List<PadronItemDto>());
            }
        }


        [HttpGet]
        public IActionResult Exportar() => View();

        // Botón descargar (si la API no da PDF, entregamos uno dummy para que funcione)
        [HttpPost]
        public async Task<IActionResult> DescargarPdf(string provincia, CancellationToken ct)
        {
            var prov = string.IsNullOrWhiteSpace(provincia) ? "Nacional" : provincia.Trim();

            // 1) Solo permitir exportar si el proceso ya terminó (CERRADO)
            var proc = await _api.GetProcesoActivoAsync(ct);
            var estado = proc?.data?.estado ?? 0;

            if (estado != (int)EstadoProceso.Cerrado)
            {
                TempData["Msg"] = "⚠️ No se puede exportar todavía. El proceso debe estar CERRADO.";
                return RedirectToAction("Exportar");
            }

            // 2) Traer resultados reales
            string estadoProcesoTexto = "CERRADO";
            List<(string nombre, long votos)> filas = new();

            if (prov.Equals("Nacional", StringComparison.OrdinalIgnoreCase))
            {
                var r = await _api.GetResultadosNacionalAsync(ct);
                estadoProcesoTexto = r?.estadoProceso ?? "CERRADO";

                foreach (var it in (r?.porCandidato ?? new List<ResultadosNacionalResponse.PorCandidato>()))
                    filas.Add((it.nombre ?? "", it.votos));
            }
            else
            {
                var rProv = await _api.GetResultadosPorProvinciaAsync(prov, ct)
                            ?? new List<ResultadosNacionalResponse.PorCandidato>();

                foreach (var it in rProv)
                    filas.Add((it.nombre ?? "", it.votos));
            }

            // 3) Generar PDF simple (pero real) en el MVC (sin depender de API/pdf)
            var bytes = PdfResultadosSimple(prov, estadoProcesoTexto, filas);

            var fileName = $"Resultados_{prov}.pdf";
            return File(bytes, "application/pdf", fileName);
        }
        private static byte[] PdfResultadosSimple(string provincia, string estadoProceso, List<(string nombre, long votos)> filas)
        {
            static string Esc(string s) => (s ?? "").Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");

            var fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            // ---- Contenido del PDF (texto)
            var lines = new List<string>
    {
        $"Resultados - {provincia}",
        $"Estado: {estadoProceso}",
        $"Generado: {fecha}",
        "----------------------------------------"
    };

            if (filas.Count == 0)
                lines.Add("Sin datos de resultados.");
            else
            {
                foreach (var (nombre, votos) in filas.OrderByDescending(x => x.votos))
                    lines.Add($"{nombre}  -  {votos:N0} votos");
            }

            var sb = new StringBuilder();
            sb.AppendLine("BT");
            sb.AppendLine("/F1 14 Tf");
            sb.AppendLine("72 760 Td");
            sb.AppendLine($"({Esc(lines[0])}) Tj");

            sb.AppendLine("/F1 11 Tf");
            for (int i = 1; i < lines.Count; i++)
            {
                sb.AppendLine("0 -16 Td");
                sb.AppendLine($"({Esc(lines[i])}) Tj");
            }
            sb.AppendLine("ET");

            var contentBytes = Encoding.ASCII.GetBytes(sb.ToString());

            // ---- Objetos PDF
            var objs = new List<byte[]>();

            byte[] Obj(string s) => Encoding.ASCII.GetBytes(s);

            objs.Add(Obj("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n"));
            objs.Add(Obj("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n"));
            objs.Add(Obj("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>\nendobj\n"));

            // Obj 4 (stream)
            var obj4Header = Obj($"4 0 obj\n<< /Length {contentBytes.Length} >>\nstream\n");
            var obj4Footer = Obj("\nendstream\nendobj\n");

            // Obj 5 (font)
            objs.Add(Obj("5 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n"));

            // ---- Armar PDF con xref real
            using var ms = new MemoryStream();
            ms.Write(Obj("%PDF-1.4\n"));

            var offsets = new List<int> { 0 }; // xref necesita obj 0
            int pos;

            // 1..3
            for (int i = 0; i < 3; i++)
            {
                pos = (int)ms.Position;
                offsets.Add(pos);
                ms.Write(objs[i]);
            }

            // 4
            pos = (int)ms.Position;
            offsets.Add(pos);
            ms.Write(obj4Header);
            ms.Write(contentBytes);
            ms.Write(obj4Footer);

            // 5
            pos = (int)ms.Position;
            offsets.Add(pos);
            ms.Write(objs[3]); // ojo: objs[3] es el obj 5 (porque metimos 1..3 y luego 5)

            // xref
            var xrefPos = (int)ms.Position;
            ms.Write(Obj($"xref\n0 {offsets.Count}\n"));
            ms.Write(Obj("0000000000 65535 f \n"));

            for (int i = 1; i < offsets.Count; i++)
                ms.Write(Obj($"{offsets[i]:D10} 00000 n \n"));

            ms.Write(Obj($"trailer\n<< /Size {offsets.Count} /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF"));

            return ms.ToArray();
        }
        [HttpPost]
        public async Task<IActionResult> GenerarCodigosPad(CancellationToken ct)
        {
            var ok = await _api.GenerarCodigosPadDemoAsync(ct);
            TempData["Msg"] = ok ? "✅ Códigos PAD generados (demo)." : "❌ No se pudo generar códigos.";
            return RedirectToAction("Padron");
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

        [HttpGet]
        public async Task<IActionResult> Candidatos(CancellationToken ct)
        {
            // Necesitamos el proceso activo para saber ProcesoElectoralId
            var proc = await _api.GetProcesoActivoAsync(ct);

            var procesoId = proc?.data?.id ?? 0;

            var lista = await _api.GetCandidatosAsync(ct) ?? new List<CandidatoDto>();

            var vm = new CandidatosVm
            {
                ProcesoElectoralId = procesoId,
                Lista = lista,
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Candidatos(CandidatosVm vm, CancellationToken ct)
        {
            if (vm.Nuevo == null)
            {
                TempData["Err"] = "Formulario vacío.";
                return RedirectToAction(nameof(Candidatos));
            }

            if (vm.Nuevo.ProcesoElectoralId <= 0)
            {
                TempData["Err"] = "No hay proceso activo. Crea/activa un proceso primero.";
                return RedirectToAction(nameof(Candidatos));
            }

            if (string.IsNullOrWhiteSpace(vm.Nuevo.NombreCompleto) || string.IsNullOrWhiteSpace(vm.Nuevo.Partido))
            {
                TempData["Err"] = "NombreCompleto y Partido son obligatorios.";
                return RedirectToAction(nameof(Candidatos));
            }

            try
            {
                await _api.CrearCandidatoAsync(vm.Nuevo, ct);

                TempData["Ok"] = "Candidato creado correctamente.";
                return RedirectToAction(nameof(Candidatos));
            }
            catch (Exception ex)
            {
                TempData["Err"] = ex.Message;
                return RedirectToAction(nameof(Candidatos));
            }
        }


        public class CandidatosVm
        {
            public int ProcesoElectoralId { get; set; }
            public List<CandidatoDto> Lista { get; set; } = new();
            public CandidatoCrearApiRequest Nuevo { get; set; } = new();
        }
    }
}
