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

        var cs = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("No hay DefaultConnection.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(cs));

        builder.Services
            .AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        var apiBase = builder.Configuration["Api:BaseUrl"] ?? "https://sitemavoto-api.onrender.com/";
        if (string.IsNullOrWhiteSpace(apiBase))
            throw new InvalidOperationException("Falta configuración Api:BaseUrl en appsettings.json");

        builder.Services.AddHttpClient("Api", c =>
        {
            c.BaseAddress = new Uri(apiBase);
            c.Timeout = TimeSpan.FromSeconds(120);
        });

        builder.Services.AddScoped<ApiService>();

        builder.Services.ConfigureApplicationCookie(opt =>
        {
            opt.LoginPath = "/Acceso/Index";
            opt.AccessDeniedPath = "/Acceso/Index";
            opt.ExpireTimeSpan = TimeSpan.FromHours(2);
            opt.SlidingExpiration = true;
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
            app.UseHttpsRedirection(); // ✅ solo producción
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "JefeJunta", "Votante", "Users" };

            foreach (var r in roles)
            {
                var exists = roleManager.RoleExistsAsync(r).GetAwaiter().GetResult();
                if (!exists)
                    roleManager.CreateAsync(new IdentityRole(r)).GetAwaiter().GetResult();
            }
        }

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Acceso}/{action=Index}/{id?}");

        app.MapRazorPages();
        app.Run();
    }
}
