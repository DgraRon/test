//// Controllers/UsersAdminController.cs
//using AuthSystem; // AppRoles
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using ServiceCatalogBGC_V2.Areas.Identity.Data; // ApplicationUser_
//using System.Linq;

//[Authorize(Policy = "AdminOnly")]
//public class UsersAdminController : Controller
//{
//    private readonly UserManager<ApplicationUser_> _userMgr;
//    private readonly RoleManager<IdentityRole> _roleMgr;

//    public UsersAdminController(UserManager<ApplicationUser_> userMgr, RoleManager<IdentityRole> roleMgr)
//    {
//        _userMgr = userMgr;
//        _roleMgr = roleMgr;
//    }

//    public async Task<IActionResult> Index()
//    {
//        var users = _userMgr.Users.ToList();
//        var list = new List<UserListItemVM>();
//        foreach (var u in users)
//        {
//            var roles = await _userMgr.GetRolesAsync(u);
//            list.Add(new UserListItemVM { Id = u.Id, Email = u.Email ?? "", Roles = roles });
//        }
//        return View(list);
//    }

//    public IActionResult Create() => View(new UserCreateVM());

//    [HttpPost, ValidateAntiForgeryToken]
//    public async Task<IActionResult> Create(UserCreateVM vm)
//    {
//        if (!ModelState.IsValid) return View(vm);

//        // สร้างผู้ใช้
//        var user = new ApplicationUser_ { UserName = vm.Email, Email = vm.Email, EmailConfirmed = true };
//        var result = await _userMgr.CreateAsync(user, vm.Password);
//        if (!result.Succeeded)
//        {
//            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
//            return View(vm);
//        }

//        // ให้บทบาท
//        var role = vm.IsAdmin ? AppRoles.Admin : AppRoles.User;
//        if (!await _roleMgr.RoleExistsAsync(role))
//            await _roleMgr.CreateAsync(new IdentityRole(role));
//        await _userMgr.AddToRoleAsync(user, role);

//        TempData["ok"] = "User created.";
//        return RedirectToAction(nameof(Index));
//    }

//    [HttpPost, ValidateAntiForgeryToken]
//    public async Task<IActionResult> MakeAdmin(string id)
//    {
//        var user = await _userMgr.FindByIdAsync(id);
//        if (user == null) return NotFound();

//        await _userMgr.RemoveFromRolesAsync(user, new[] { AppRoles.User, AppRoles.Admin });
//        await _userMgr.AddToRoleAsync(user, AppRoles.Admin);
//        return RedirectToAction(nameof(Index));
//    }

//    [HttpPost, ValidateAntiForgeryToken]
//    public async Task<IActionResult> MakeUser(string id)
//    {
//        var user = await _userMgr.FindByIdAsync(id);
//        if (user == null) return NotFound();

//        await _userMgr.RemoveFromRolesAsync(user, new[] { AppRoles.User, AppRoles.Admin });
//        await _userMgr.AddToRoleAsync(user, AppRoles.User);
//        return RedirectToAction(nameof(Index));
//    }

//    [HttpPost, ValidateAntiForgeryToken]
//    public async Task<IActionResult> Delete(string id)
//    {
//        var me = await _userMgr.GetUserAsync(User);
//        if (me != null && me.Id == id)
//        {
//            TempData["err"] = "You cannot delete yourself.";
//            return RedirectToAction(nameof(Index));
//        }

//        var user = await _userMgr.FindByIdAsync(id);
//        if (user == null) return NotFound();

//        await _userMgr.DeleteAsync(user);
//        return RedirectToAction(nameof(Index));
//    }
//}
