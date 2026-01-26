using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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
            // Add services to the container.
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();

            // Servicios negocio
            builder.Services.AddScoped<IProcesoService, ProcesoService>();
            builder.Services.AddScoped<IPadronService, PadronService>();
            builder.Services.AddScoped<IVotacionService, VotacionService>();
            builder.Services.AddScoped<IResultadosService, ResultadosService>();
            builder.Services.AddScoped<IOtpService, OtpService>();



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers().AddNewtonsoftJson(
                options =>
                options.SerializerSettings.ReferenceLoopHandling
                = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // ✅ evita choque de nombres de DTOs (CandidatoCreateDto repetido, etc.)
                c.CustomSchemaIds(t => t.FullName);
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            app.UseCors("AllowAll");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // Usa la ruta absoluta para que ZeroTier no se confunda
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SitemaVoto.Api v1");
                c.RoutePrefix = "swagger";
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            
            
          
            //app.UseHttpsRedirection();


            app.UseAuthorization();


            app.MapControllers();
      

            app.Run();
        }
    }
}
