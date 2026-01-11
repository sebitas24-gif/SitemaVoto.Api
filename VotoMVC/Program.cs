using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using VotoMVC.Services;
namespace VotoMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<VotoMVCContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("VotoMVCContext") ?? throw new InvalidOperationException("Connection string 'VotoMVCContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpClient();

            builder.Services.AddHttpClient<VotoMVC.Services.AdminApiService>();
            builder.Services.AddScoped<VotoMVC.Services.OpcionApiService>();
            builder.Services.AddScoped<VotoMVC.Services.ProcesoApiService>();
            builder.Services.AddScoped<VotoMVC.Services.AuthApiService>();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddScoped<VotoMVC.Services.AuthApiService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                  name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
