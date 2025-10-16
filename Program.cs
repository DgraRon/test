using AuthSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using ServiceCatalogBGC_V2.Areas.Identity.Data;
using ServiceCatalogBGC_V2.data;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DbContexts
builder.Services.AddDbContext<Data_AuthDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add Identity and Roles
builder.Services
    .AddDefaultIdentity<ApplicationUser_>(opt =>
    {
        opt.SignIn.RequireConfirmedAccount = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<Data_AuthDbContext>();

// 3. Add Authorization Policies
// This is the most critical part. We define all policies here.
builder.Services.AddAuthorization(o =>
{
    // Policies for the Catalog system, as required by CatalogController
    o.AddPolicy(AppPolicies.CatalogRead, p => p.RequireRole(AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.User));
    o.AddPolicy(AppPolicies.CatalogWrite, p => p.RequireRole(AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.User));
    o.AddPolicy(AppPolicies.CatalogMeta, p => p.RequireRole(AppRoles.SuperAdmin, AppRoles.Admin));

    // General Admin policies for menus and other areas
    o.AddPolicy("SuperAdminOnly", p => p.RequireRole(AppRoles.SuperAdmin));
    o.AddPolicy("AdminOnly", p => p.RequireRole(AppRoles.SuperAdmin, AppRoles.Admin));
});

// 4. Add MVC Controllers, Global Authorization, and Runtime Compilation in ONE step
builder.Services.AddControllersWithViews(o =>
{
    // Apply a global authorization filter, requiring all users to be logged in by default
    o.Filters.Add(new AuthorizeFilter(
        new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()
    ));
}).AddRazorRuntimeCompilation();

// 5. Add Razor Pages for Identity UI
builder.Services.AddRazorPages(o =>
{
    o.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
    o.Conventions.AuthorizeAreaPage("Identity", "/Account/Register", "AdminOnly"); // Only Admins can register new users
});

// 6. Configure Cookie settings for login/logout redirects
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Identity/Account/Login";
    o.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddHttpContextAccessor();

// --- Build the App ---
var app = builder.Build();

// --- Configure the HTTP request pipeline (Middleware) ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // This is crucial for CSS, JS, and images to work.
app.UseRouting();

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalog}/{action=Index}/{id?}");

app.MapRazorPages();

// --- Seed database with initial roles and SuperAdmin user ---
using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser_>>();

    // Create roles if they don't exist
    foreach (var role in new[] { AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.User })
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));

    // Create the initial SuperAdmin user if they don't exist
    var adminEmail = builder.Configuration["Seed:Admin:Email"] ?? "admin@bgc.local";
    var adminPass = builder.Configuration["Seed:Admin:Password"] ?? "a123456";

    var admin = await userMgr.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new ApplicationUser_
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator"
        };

        var result = await userMgr.CreateAsync(admin, adminPass);
        if (!result.Succeeded)
            throw new Exception("Seed admin failed: " +
                string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));

        await userMgr.AddToRolesAsync(admin, new[] { AppRoles.SuperAdmin, AppRoles.Admin });
    }
    else
    {
        // Ensure the existing admin has the correct roles
        await userMgr.AddToRolesAsync(admin, new[] { AppRoles.SuperAdmin, AppRoles.Admin });
    }
}

app.Run();

