using AuthSystem;
using Microsoft.AspNetCore.Identity;
using ServiceCatalogBGC_V2.Areas.Identity.Data; // ApplicationUser_
namespace AuthSystem;

public static class IdentitySeed
{
    public static async Task RunAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser_>>();

        // สร้างบทบาทถ้ายังไม่มี
        foreach (var r in new[] { AppRoles.Admin, AppRoles.User })
            if (!await roleMgr.RoleExistsAsync(r))
                await roleMgr.CreateAsync(new IdentityRole(r));

        // แอดมินเริ่มต้น (เปลี่ยนตามต้องการ)
        var adminEmail = "admin@bgc.local";
        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser_ { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userMgr.CreateAsync(admin, "a123456"); // เปลี่ยนรหัสจริง
        }
        if (!await userMgr.IsInRoleAsync(admin, AppRoles.Admin))
            await userMgr.AddToRoleAsync(admin, AppRoles.Admin);
    }
}
