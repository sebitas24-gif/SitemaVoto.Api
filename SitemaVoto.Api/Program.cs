using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SitemaVoto.Api.Services.Email;
using SitemaVoto.Api.Services.Notificaciones;
using SitemaVoto.Api.Services.Otp;
using SitemaVoto.Api.Services.Padron;
using SitemaVoto.Api.Services.Procesos;
using SitemaVoto.Api.Services.Resultados;
using SitemaVoto.Api.Services.Votacion;

using VotoModelos;
namespace SitemaVoto.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddDbContext<SitemaVotoApiContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("SitemaVotoApiContext")
                ?? throw new InvalidOperationException("Connection string 'SitemaVotoApiContext' not found.")));

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            builder.Services.AddMemoryCache();

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // ✅ Swagger (evita choque de DTOs + soporta cancelToken en swagger)
            builder.Services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(t => t.FullName);
            });

            // ✅ Options (leer appsettings.json)
            builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
            builder.Services.Configure<TwilioOptions>(builder.Configuration.GetSection("Twilio"));
            builder.Services.Configure<OtpOptions>(builder.Configuration.GetSection("Otp"));

            // ✅ Envío REAL: Email y SMS
            builder.Services.AddTransient<IEmailSenderApp, SmtpEmailSender>();
            builder.Services.AddTransient<ISmsSenderApp, TwilioSmsSender>();

            // ✅ Notificadores que usa el OtpController
            builder.Services.AddScoped<EmailNotificador>();
            builder.Services.AddScoped<SmsNotificador>();

            // ✅ Servicios negocio
            builder.Services.AddScoped<IProcesoService, ProcesoService>();
            builder.Services.AddScoped<IPadronService, PadronService>();
            builder.Services.AddScoped<IVotacionService, VotacionService>();
            builder.Services.AddScoped<IResultadosService, ResultadosService>();

            // ✅ OTP (memoria)
            builder.Services.AddSingleton<OtpService>();

            var app = builder.Build();

            app.UseCors("AllowAll");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SitemaVoto.Api v1");
                c.RoutePrefix = "swagger";
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            // (si no usas auth todavía, no pasa nada)
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
