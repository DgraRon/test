using AuthSystem; // สำหรับ AppRoles
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceCatalogBGC_V2.Areas.Identity.Data;
using ServiceCatalogBGC_V2.ViewModels.AdminUsers;
using ServiceCatalogBGC_V2.data; // DbContext หลัก (มีตาราง Developers)

namespace ServiceCatalogBGC_V2.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser_> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;
        private readonly string _superAdminEmail;
        private readonly ApplicationDbContext _db; // ใช้อ่าน Developer Emails

        public AdminUsersController(
            UserManager<ApplicationUser_> userMgr,
            RoleManager<IdentityRole> roleMgr,
            IConfiguration config,
            ApplicationDbContext db)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
            _superAdminEmail = (config["Seed:Admin:Email"] ?? "admin@bgc.local").Trim();
            _db = db;
        }

        /* ---------------- Helpers ---------------- */
        private async Task<bool> IsSuperAdminAsync(ApplicationUser_ user)
            => await _userMgr.IsInRoleAsync(user, AppRoles.SuperAdmin);

        private string? CurrentUserId => _userMgr.GetUserId(User);

        private static readonly string[] KnownRoles =
        {
            AppRoles.SuperAdmin, AppRoles.Admin, AppRoles.User
        };

        private async Task FillRoleOptionsAsync(CreateUserVM vm)
        {
            bool iAmSuper = User.IsInRole(AppRoles.SuperAdmin);

            List<string> roles;
            try
            {
                roles = await _roleMgr.Roles
                    .AsNoTracking()
                    .Select(r => r.Name!)
                    .ToListAsync();

                if (roles.Count == 0) roles = KnownRoles.ToList();
            }
            catch
            {
                roles = KnownRoles.ToList();
            }

            var allowed = iAmSuper
                ? roles
                : roles.Where(r => r.Equals(AppRoles.User, StringComparison.OrdinalIgnoreCase));

            vm.RoleOptions = allowed
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .Select(x => new SelectListItem { Value = x, Text = x })
                .ToList();
        }

        /* ---------------- Index ---------------- */
        public async Task<IActionResult> Index(bool includeDisabled = false)
        {
            IQueryable<ApplicationUser_> q = _userMgr.Users;
            if (includeDisabled) q = q.IgnoreQueryFilters();

            var list = await q
                .AsNoTracking()
                .Select(u => new UserRowVM
                {
                    Id = u.Id,
                    Email = u.Email!,
                    IsDisabled = u.IsDisabled,
                    IsSuperAdmin = u.Email != null &&
                                   u.Email.Trim().ToLower() == _superAdminEmail.ToLower()
                })
                .ToListAsync();

            foreach (var row in list)
            {
                var user = await _userMgr.Users.IgnoreQueryFilters()
                    .FirstAsync(x => x.Id == row.Id);

                row.Roles = string.Join(", ", await _userMgr.GetRolesAsync(user));
            }

            ViewBag.IncludeDisabled = includeDisabled;
            return View(list);
        }

        /* ---------------- Create (GET) ---------------- */
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateUserVM();
            await FillRoleOptionsAsync(vm);

            ViewBag.DeveloperEmails = await _db.Developers
                .AsNoTracking()
                .Select(d => d.Email)
                .Where(e => e != null && e != "")
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();

            return View(vm);
        }

        /* ---------------- Create (POST) ---------------- */
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM vm)
        {
            await FillRoleOptionsAsync(vm);

            if (!ModelState.IsValid)
            {
                ViewBag.DeveloperEmails = await _db.Developers
                    .AsNoTracking()
                    .Select(d => d.Email)
                    .Where(e => e != null && e != "")
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync();

                return View(vm);
            }

            if (await _userMgr.FindByEmailAsync(vm.Email) is not null)
            {
                ModelState.AddModelError(nameof(vm.Email), "This email is already in use.");
                ViewBag.DeveloperEmails = await _db.Developers
                    .AsNoTracking()
                    .Select(d => d.Email)
                    .Where(e => e != null && e != "")
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync();

                return View(vm);
            }

            var allowedRoles = vm.RoleOptions.Select(x => x.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!allowedRoles.Contains(vm.Role))
            {
                ModelState.AddModelError(nameof(vm.Role), "You are not allowed to assign this role.");
                ViewBag.DeveloperEmails = await _db.Developers
                    .AsNoTracking()
                    .Select(d => d.Email)
                    .Where(e => e != null && e != "")
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync();

                return View(vm);
            }

            var user = new ApplicationUser_
            {
                UserName = vm.Email,
                Email = vm.Email,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                EmailConfirmed = true,
                LockoutEnabled = true
            };

            var result = await _userMgr.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                ViewBag.DeveloperEmails = await _db.Developers
                    .AsNoTracking()
                    .Select(d => d.Email)
                    .Where(e => e != null && e != "")
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync();

                return View(vm);
            }

            await _userMgr.AddToRoleAsync(user, vm.Role);

            TempData["Ok"] = "User created.";
            return RedirectToAction(nameof(Index));
        }

        // ====== เพิ่ม DTO รับค่าจากฟอร์ม ======
        public class SetPasswordDirectVM
        {
            public string Id { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        // ====== เพิ่ม Action สำหรับ SuperAdmin เท่านั้น ======
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdminOnly")]
        public async Task<IActionResult> SetPasswordDirect(SetPasswordDirectVM vm)
        {
            if (vm is null || string.IsNullOrWhiteSpace(vm.Id))
            {
                TempData["err"] = "Invalid request.";
                return RedirectToAction(nameof(Index));
            }
            if (vm.NewPassword != vm.ConfirmPassword)
            {
                TempData["err"] = "รหัสยืนยันไม่ตรงกัน";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userMgr.Users.IgnoreQueryFilters()
                             .FirstOrDefaultAsync(u => u.Id == vm.Id);
            if (user == null)
            {
                TempData["err"] = "ไม่พบผู้ใช้";
                return RedirectToAction(nameof(Index));
            }

            // กันเผลอตั้งรหัสให้ SuperAdmin seed หรือให้ตัวเอง
            if (user.Email?.Trim().ToLower() == _superAdminEmail.ToLower())
            {
                TempData["err"] = "ห้ามเปลี่ยนรหัสของ SuperAdmin seed.";
                return RedirectToAction(nameof(Index));
            }

            // ใช้เส้นทาง Reset ด้วย token (ไม่ต้องทราบรหัสเดิม และเคารพ PasswordOptions)
            var token = await _userMgr.GeneratePasswordResetTokenAsync(user);
            var reset = await _userMgr.ResetPasswordAsync(user, token, vm.NewPassword);
            if (!reset.Succeeded)
            {
                TempData["err"] = string.Join("; ", reset.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }

            // ตัดทุก session เดิม
            await _userMgr.UpdateSecurityStampAsync(user);

            // TODO: บันทึก Audit log ที่นี่ (ผู้กระทำ, เป้าหมาย, เวลา, IP)

            TempData["Ok"] = $"ตั้งรหัสใหม่ให้ {user.Email} เรียบร้อย";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { TempData["err"] = "Invalid user id."; return RedirectToAction(nameof(Index)); }

            var user = await _userMgr.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) { TempData["err"] = "User not found."; return RedirectToAction(nameof(Index)); }

            // กันกรณีพิเศษ (ปรับตามกติกาคุณ)
            // if (/*ห้ามปรับสิทธิ์ตัวเองหรือ superadmin seed */) { ... }

            // เอา role User/Admin ตามต้องการ
            await _userMgr.RemoveFromRoleAsync(user, "User");
            var res = await _userMgr.AddToRoleAsync(user, "Admin");
            if (!res.Succeeded) { TempData["err"] = string.Join("; ", res.Errors.Select(e => e.Description)); return RedirectToAction(nameof(Index)); }

            TempData["ok"] = $"เลื่อนสิทธิ์ {user.Email} เป็น Admin แล้ว";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { TempData["err"] = "Invalid user id."; return RedirectToAction(nameof(Index)); }

            var user = await _userMgr.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) { TempData["err"] = "User not found."; return RedirectToAction(nameof(Index)); }

            // กันกรณีพิเศษ (เช่น ห้ามลดสิทธิ์ SuperAdmin คนสุดท้าย)
            // if (/*rule*/){ ... }

            await _userMgr.RemoveFromRoleAsync(user, "Admin");
            var res = await _userMgr.AddToRoleAsync(user, "User");
            if (!res.Succeeded) { TempData["err"] = string.Join("; ", res.Errors.Select(e => e.Description)); return RedirectToAction(nameof(Index)); }

            TempData["ok"] = $"ลดสิทธิ์ {user.Email} เป็น User แล้ว";
            return RedirectToAction(nameof(Index));
        }



        /* ---------------- (Optional) AJAX for Select2 ---------------- */
        [HttpGet]
        public async Task<IActionResult> DeveloperEmails(string? q)
        {
            var query = _db.Developers.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(d => d.Email.Contains(q));

            var emails = await query
                .Select(d => d.Email)
                .Where(e => e != null && e != "")
                .Distinct()
                .OrderBy(e => e)
                .Take(50)
                .ToListAsync();

            return Json(emails);
        }

        // ปิดการใช้งานผู้ใช้
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(string id, bool includeDisabled = false)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            // ต้อง IgnoreQueryFilters เพื่อให้หาเจอทุกสถานะ
            var user = await _userMgr.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user is null) return NotFound();

            // กันปิด SuperAdmin seed และกันปิดตัวเอง
            if (user.Email?.Trim().ToLower() == _superAdminEmail.ToLower()) return Forbid();
            if (user.Id == CurrentUserId) return Forbid();

            if (!user.IsDisabled)
            {
                user.IsDisabled = true;
                await _userMgr.UpdateAsync(user);
                TempData["Ok"] = $"Disabled {user.Email}.";
            }

            // หลังปิดแล้ว รายการปกติจะไม่แสดงผู้ใช้ที่ถูกปิด ให้กลับไปหน้าเดิมตาม includeDisabled ที่ส่งมา
            return RedirectToAction(nameof(Index), new { includeDisabled });
        }

        // เปิดใช้งานกลับ
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(string id, bool includeDisabled = true)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var user = await _userMgr.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user is null) return NotFound();

            if (user.IsDisabled)
            {
                user.IsDisabled = false;
                await _userMgr.UpdateAsync(user);
                TempData["Ok"] = $"Restored {user.Email}.";
            }

            // ค่าเริ่มต้นให้โชว์ disabled ไว้ก่อน (true) เพื่อให้เห็นผลลัพธ์เพิ่งกด
            return RedirectToAction(nameof(Index), new { includeDisabled });
        }


    }
}
