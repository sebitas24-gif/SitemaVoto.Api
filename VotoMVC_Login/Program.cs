using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VotoMVC_Login.Data;
using VotoMVC_Login.Service;

namespace VotoMVC_Login;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // =======================
        // DB Postgres (Identity)
        // =======================
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // =======================
        // Identity + Roles
        // =======================
        builder.Services
                .AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        // =======================
        // HttpClient hacia tu API
        // =======================
        builder.Services.AddHttpClient("Api", c =>
        {
            c.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!);
            c.Timeout = TimeSpan.FromSeconds(180); // ✅ 3 minutos para Render
        });

        // =======================
        // Session (para guardar cedula mientras OTP)
        // =======================
        builder.Services.AddSession(o =>
        {
            o.IdleTimeout = TimeSpan.FromMinutes(20);
            o.Cookie.HttpOnly = true;
            o.Cookie.IsEssential = true;
        });

        builder.Services.AddScoped<ApiService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();           // ✅ CRÍTICO
        app.UseAuthentication();    // ✅
        app.UseAuthorization();     // ✅

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        app.Run();
    }
}
