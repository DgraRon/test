using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceCatalogBGC_V2.Areas.Identity.Data;

namespace AuthSystem;

public class Data_AuthDbContext : IdentityDbContext<ApplicationUser_>
{
    public Data_AuthDbContext(DbContextOptions<Data_AuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        // ซ่อน User ที่ถูกปิดใช้งานจากทุก query ปกติ
        builder.Entity<ApplicationUser_>()
               .HasQueryFilter(u => !u.IsDisabled);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
