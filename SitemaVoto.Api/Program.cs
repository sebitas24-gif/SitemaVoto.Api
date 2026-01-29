using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SitemaVoto.Api.Services.Notificaciones;
using SitemaVoto.Api.Services.Email;
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


            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            builder.Services.AddSwaggerGen(c =>
            {
                // ✅ Evita nombres con "+" de clases anidadas
                c.CustomSchemaIds(t => t.FullName!.Replace("+", "."));
            });


            builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
            builder.Services.Configure<OtpOptions>(builder.Configuration.GetSection("Otp"));

            // =========================
            // INFRA (Email/SMS/Notifs)
            // =========================
            builder.Services.AddTransient<IEmailSenderApp, SmtpEmailSender>();
            builder.Services.AddSingleton<ISmsSenderApp, NullSmsSender>();

          

            // =========================
            // BUSINESS SERVICES
            // =========================
            builder.Services.AddScoped<IProcesoService, ProcesoService>();
            builder.Services.AddScoped<IPadronService, PadronService>();
            builder.Services.AddScoped<IVotacionService, VotacionService>();
            builder.Services.AddScoped<VotacionService>();
            builder.Services.AddScoped<IResultadosService, ResultadosService>();

            // OTP service (si lo usas como helper)
            builder.Services.AddSingleton<OtpService>();

           

            var app = builder.Build();

            // =========================
            // MIDDLEWARE ORDER
            // =========================
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            // (Opcional) si publicas detrás de proxy y necesitas:
            // app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SitemaVoto.Api v1");
                c.RoutePrefix = "swagger";
            });

            // Si usas Auth:
            // app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
