using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VotoMVC_Login.Areas.Identity.Data;

namespace VotoMVC_Login.Data;

public class VotoMVC_LoginContext : IdentityDbContext<VotoMVC_LoginUser>
{
    public VotoMVC_LoginContext(DbContextOptions<VotoMVC_LoginContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
