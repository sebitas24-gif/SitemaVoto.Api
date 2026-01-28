using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VotoMVC_Login.Data;

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
            options.SignIn.RequireConfirmedAccount = false; // simple
        })
        .AddRoles<IdentityRole>() // IMPORTANTÍSIMO para Roles
        .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages(); // Necesario para que existan las páginas de Identity (aunque no las uses)

        // --------------------
        // 3) App
        // --------------------
        var app = builder.Build();

        // --------------------
        // 4) Middleware pipeline
        // --------------------
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication(); // IMPORTANTÍSIMO
        app.UseAuthorization();  // IMPORTANTÍSIMO

        // --------------------
        // 5) Crear Roles automáticamente (para cumplir requisito)
        // --------------------
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "JefeJunta", "Votante", "Users" };

            foreach (var r in roles)
            {
                var exists = roleManager.RoleExistsAsync(r).GetAwaiter().GetResult();
                if (!exists)
                {
                    roleManager.CreateAsync(new IdentityRole(r)).GetAwaiter().GetResult();
                }
            }
        }


        // --------------------
        // 6) Rutas
        //    ✅ Mandar por defecto al login personalizado por cédula + código (PAD/OTP)
        // --------------------
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Acceso}/{action=Index}/{id?}");

        // Necesario para Identity (aunque no uses /Identity/Account/Login)
        app.MapRazorPages();

        app.Run();
    }
}
