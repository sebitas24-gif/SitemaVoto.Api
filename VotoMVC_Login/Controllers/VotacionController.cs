using Microsoft.AspNetCore.Mvc;
using VotoMVC_Login.Service;
using VotoMVC_Login.Services;
using VotoMVC_Login.Models.ViewModels;
using VotoMVC_Login.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using Net.Codecrete.QrCodeGenerator;

namespace VotoMVC_Login.Controllers
{

    public class VotacionController : Controller
    {
        private readonly ApiService _api;

        private const string VOTO_PROCESO = "VOTO_PROCESO";
        private const string VOTO_CANDIDATO = "VOTO_CANDIDATO";

        public VotacionController(ApiService api) => _api = api;

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string cedula, string codigoPad, CancellationToken ct)
        {
            cedula = (cedula ?? "").Trim();
            codigoPad = (codigoPad ?? "").Trim();

            if (cedula.Length != 10 || string.IsNullOrWhiteSpace(codigoPad))
            {
                ViewBag.Error = "Cédula (10 dígitos) y código PAD son obligatorios.";
                return View();
            }

            // 1) Validar que exista proceso ACTIVO antes de validar PAD
            var proc = await _api.GetProcesoActivoAsync(ct);

            if (proc == null || !proc.ok || proc.data == null)
            {
                // limpiar sesión por seguridad
                HttpContext.Session.Clear();

                ViewBag.Error = "No hay un proceso electoral ACTIVO. No se puede votar.";
                return View(); // o RedirectToAction(nameof(Index))
            }
            var procesoId = proc.data.id;


            // 2) Validar PAD (aquí tu API debe devolver si ya está usado)
            var r = await _api.ValidarPadConGetAsync(cedula, codigoPad, ct);
            if (!r.Ok)
            {
                ViewBag.Error = r.Error ?? "No se pudo validar el PAD.";
                return View();
            }

            // ✅ Si tu API devuelve algo tipo: r.Data.usado / r.Data.puedeVotar
            // adapta el nombre. Ejemplo recomendado:
            if (r.Data != null && r.Data.usado == true)
            {
                ViewBag.Error = "Este código PAD ya fue utilizado. No se puede volver a votar.";
                return View();
            }

            // Guardar sesión
            HttpContext.Session.SetString(SessionKeys.Cedula, cedula);
            HttpContext.Session.SetString(SessionKeys.CodigoUnico, (r.Data?.codigoPad ?? "").Trim());

            // Guardar el proceso (para que no te cambien en medio)
            HttpContext.Session.SetInt32(VOTO_PROCESO, procesoId);

            return RedirectToAction(nameof(Papeleta));
        }

        [HttpGet]
        public async Task<IActionResult> Papeleta(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessionKeys.Cedula);
            var pad = HttpContext.Session.GetString(SessionKeys.CodigoUnico);

            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(pad))
                return RedirectToAction(nameof(Index));

            // 1) Verificar proceso activo
            var proc = await _api.GetProcesoActivoAsync(ct);

            if (proc == null || !proc.ok || proc.data == null)
            {
                // limpiar sesión por seguridad
                HttpContext.Session.Clear();

                ViewBag.Error = "No hay un proceso electoral ACTIVO. No se puede votar.";
                return View(); // o RedirectToAction(nameof(Index))
            }
            var procesoId = proc.data.id;

            // 2) Revalidar que PAD no esté usado (por si intentan reingresar)
            var padCheck = await _api.ValidarPadConGetAsync(cedula!, pad!, ct);
            if (!padCheck.Ok)
            {
                TempData["Error"] = padCheck.Error ?? "No se pudo validar el PAD.";
                return RedirectToAction(nameof(Index));
            }
            if (padCheck.Data != null && padCheck.Data.usado == true)
            {
                TempData["Error"] = "Este código PAD ya fue utilizado. No se puede volver a votar.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new VotacionPapeletaVm
            {
                Cedula = cedula!,
                CodigoPad = pad!,
                Proceso = proc,
                ProcesoId = procesoId,
                ProcesoNombre = proc?.data?.nombre ?? "Proceso Activo",
                Tipo = "Plancha (Binomio)",
                Normas = "Seleccione 1 opción y confirme."
            };

            var lista = await _api.GetCandidatosAsync(ct) ?? new List<ApiService.CandidatoDto>();
            vm.Candidatos = lista.Where(x => x.activo).ToList();

            if (vm.Candidatos.Count == 0)
                vm.Error = "No hay candidatos activos.";

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmar(int procesoId, int candidatoId)
        {
            HttpContext.Session.SetInt32(VOTO_PROCESO, procesoId);
            HttpContext.Session.SetInt32(VOTO_CANDIDATO, candidatoId);
            return RedirectToAction(nameof(Confirmar));
        }

        [HttpGet]
        public async Task<IActionResult> Confirmar(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessionKeys.Cedula) ?? "";
            var pad = HttpContext.Session.GetString(SessionKeys.CodigoUnico) ?? "";
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(pad))
                return RedirectToAction(nameof(Index));

            // Verificar proceso activo
            var proc = await _api.GetProcesoActivoAsync(ct);

            if (proc == null || !proc.ok || proc.data == null)
            {
                TempData["Error"] = "No hay proceso ACTIVO. No se puede votar.";
                return RedirectToAction(nameof(Index));
            }

            var procesoActivoId = proc.data.id;

            var procesoId = HttpContext.Session.GetInt32(VOTO_PROCESO) ?? 0;
            var candidatoId = HttpContext.Session.GetInt32(VOTO_CANDIDATO) ?? 0;

            // Si cambió el proceso mientras estaba en pantalla, lo boto
            if (procesoId != procesoActivoId)
            {
                TempData["Error"] = "El proceso activo cambió. Vuelve a ingresar.";
                return RedirectToAction(nameof(Index));
            }

            // Revalidar PAD
            var padCheck = await _api.ValidarPadConGetAsync(cedula, pad, ct);
            if (!padCheck.Ok || (padCheck.Data != null && padCheck.Data.usado == true))
            {
                TempData["Error"] = "El código PAD ya fue utilizado o no es válido.";
                return RedirectToAction(nameof(Index));
            }

            var lista = await _api.GetCandidatosAsync(ct) ?? new List<ApiService.CandidatoDto>();

            var vm = new VotacionPapeletaVm
            {
                Cedula = cedula,
                CodigoPad = pad,
                Proceso = proc,
                ProcesoId = procesoId,
                ProcesoNombre = proc?.data?.nombre ?? "Proceso Activo",
                Tipo = "Plancha (Binomio)",
                Normas = "Seleccione 1 opción y confirme.",
                Candidatos = lista.Where(x => x.activo).ToList(),
                CandidatoId = candidatoId
            };

            if (procesoId <= 0) vm.Error = "Proceso inválido. Vuelve a la papeleta.";
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmitirVoto(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessionKeys.Cedula);
            var pad = HttpContext.Session.GetString(SessionKeys.CodigoUnico);

            if (string.IsNullOrWhiteSpace(cedula)) return RedirectToAction(nameof(Index));

            // USAMOS EL MISMO MÉTODO QUE EL JEFE DE JUNTA
            // Esto asegura que traiga la mesa correctamente
            var data = await _api.GetPadronPorCedulaAsync(cedula, ct);

            if (data == null)
            {
                TempData["Error"] = "No se encontraron sus datos en el padrón electoral.";
                return RedirectToAction(nameof(Index));
            }

            // GUARDAR DATOS (Usando exactamente los campos que usa el Jefe)
            TempData["Nombre"] = $"{data.nombres} {data.apellidos}";
            TempData["Cedula"] = data.cedula;
            TempData["Provincia"] = data.provincia;
            TempData["Canton"] = data.canton;

            // Aquí está la clave: data.mesa es lo que el Jefe de Junta ve en su panel
            TempData["Mesa"] = !string.IsNullOrWhiteSpace(data.mesa) ? data.mesa : "MESA-02";

            TempData["Email"] = data.correo;

            // ... lógica de envío de voto (EmitirVotoAsync) ...
            var candidatoId = HttpContext.Session.GetInt32(VOTO_CANDIDATO);
            var dto = new ApiService.EmitirVotoDto { Cedula = cedula, CodigoPad = pad, CandidatoId = candidatoId == 0 ? null : candidatoId };
            var resp = await _api.EmitirVotoAsync(dto, ct);

            if (resp != null && resp.Ok)
            {
                TempData["Comprobante"] = resp.Comprobante;
                TempData.Keep(); // Mantenemos todo para Exito y PDF
                return RedirectToAction(nameof(Exito));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Exito()
        {
            // Usamos Keep para asegurar que el PDF todavía pueda leer estos datos después de ver la pantalla
            TempData.Keep();

            // Verificación de seguridad: si no hay datos, volver al inicio
            if (TempData.Peek("Cedula") == null) return RedirectToAction("Index");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DescargarComprobantePdf([FromServices] EmailService emailService)
        {
            TempData.Keep();
            QuestPDF.Settings.License = LicenseType.Community;

            var email = TempData.Peek("Email") as string ?? "";
            var nombre = TempData.Peek("Nombre") as string ?? "N/A";
            var cedula = TempData.Peek("Cedula") as string ?? "N/A";
            var provincia = TempData.Peek("Provincia") as string ?? "N/A";
            var canton = TempData.Peek("Canton") as string ?? "N/A";
            var mesa = TempData.Peek("Mesa") as string ?? "MESA-00";
            var comp = TempData.Peek("Comprobante") as string ?? "N/A";

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Helvetica"));

                    page.Content().Column(col =>
                    {
                        col.Spacing(20);

                        col.Item().AlignCenter().Text("REPÚBLICA DEL ECUADOR").FontSize(10);
                        col.Item().AlignCenter().Text("CONSEJO NACIONAL ELECTORAL").Bold().FontSize(14);
                        col.Item().PaddingTop(10).AlignCenter().Text("CERTIFICADO DE SUFRAGIO")
                            .FontSize(24).Bold().FontColor(Colors.Blue.Medium);

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().Padding(10).Background(Colors.Grey.Lighten5).Column(c =>
                        {
                            c.Spacing(8);
                            c.Item().Text(t => { t.Span("Nombre: ").Bold(); t.Span(nombre); });
                            c.Item().Text(t => { t.Span("Cédula: ").Bold(); t.Span(cedula); });
                            c.Item().Text(t => { t.Span("Provincia: ").Bold(); t.Span(provincia); });
                            c.Item().Text(t => { t.Span("Cantón: ").Bold(); t.Span(canton); });
                            c.Item().Text(t => { t.Span("Mesa Electoral: ").Bold(); t.Span(mesa).Bold().FontColor(Colors.Blue.Darken2); });
                        });

                        col.Item().AlignRight().Text(t => {
                            t.Span("CÓDIGO DE VERIFICACIÓN: ").FontSize(9);
                            t.Span(comp).Bold().FontSize(12);
                        });

                        col.Item().PaddingTop(50).AlignCenter().Text("Este documento es una representación digital del sufragio emitido.")
                            .Italic().FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            var pdfBytes = pdf.GeneratePdf();
            var fileName = $"Certificado_Voto_{cedula}.pdf";

            // ✅ Enviar el mismo PDF por correo (si hay email)
            if (!string.IsNullOrWhiteSpace(email))
            {
                await emailService.EnviarPdfAdjuntoAsync(
                    paraEmail: email,
                    asunto: "Comprobante de votación (PDF)",
                    texto: $"Hola {nombre},\nAdjunto está tu comprobante.\nCódigo: {comp}\n\nVotoEcua (Demo)",
                    pdfBytes: pdfBytes,
                    fileName: fileName
                );

                TempData["PapeletaEnviada"] = true; // si quieres mostrarlo en la vista
            }
            else
            {
                TempData["PapeletaEnviada"] = false;
            }

            // ✅ Igual te lo devuelve para descargar
            return File(pdfBytes, "application/pdf", fileName);
        }


        [HttpPost]
        public async Task<IActionResult> ProcesarBusqueda(string cedula, CancellationToken ct)
        {
            var resultado = await _api.GetCiudadanoAsync(cedula, ct);

            if (resultado.Ok && resultado.Data != null)
            {
                var d = resultado.Data;

                // Guardamos en TempData usando llaves consistentes
                TempData["Nombre"] = $"{d.nombres} {d.apellidos}";
                TempData["Cedula"] = d.cedula;
                TempData["Provincia"] = d.provincia;
                TempData["Canton"] = d.canton;

                // IMPORTANTE: Aquí capturamos la mesa del DTO
                TempData["Mesa"] = !string.IsNullOrWhiteSpace(d.mesa) ? d.mesa : "No asignada";

                TempData["Comprobante"] = d.codigoPad ?? "CONF-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                // Mantenemos los datos para la descarga del PDF
                TempData.Keep();

                return View("Exito"); // O la vista que corresponda
            }

            return RedirectToAction("Index");
        }
    }
}
