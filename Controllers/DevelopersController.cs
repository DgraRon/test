using AuthSystem; // ถ้าต้องการผูก Policy ให้ปุ่ม Add ใช้ได้เฉพาะกลุ่มเดียวกับ Category
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceCatalogBGC_V2.data;
using ServiceCatalogBGC_V2.Models;

namespace ServiceCatalogBGC_V2.Controllers
{
    public class DevelopersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DevelopersController(ApplicationDbContext db) => _db = db;

        // GET: /Developers
        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Developers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(d =>
                    d.Email.Contains(q) ||
                    (d.Name != null && d.Name.Contains(q)));
            }

            var items = await query
                .OrderByDescending(d => d.IsActive)
                .ThenBy(d => d.Email)
                .ToListAsync();

            ViewBag.Search = q;
            return View(items);
        }

        // GET: /Developers/Create
        public IActionResult Create() => View(new Developer { IsActive = true });

        // POST: /Developers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Developer input)
        {
            Normalize(input);

            if (await _db.Developers.AnyAsync(d => d.Email == input.Email))
            {
                ModelState.AddModelError(nameof(Developer.Email), "อีเมลนี้ถูกใช้แล้ว");
            }

            if (!ModelState.IsValid) return View(input);

            try
            {
                _db.Developers.Add(input);
                await _db.SaveChangesAsync();
                TempData["ok"] = "เพิ่ม Developer สำเร็จ";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // เผื่อกรณี race กับ unique index
                ModelState.AddModelError(nameof(Developer.Email), "อีเมลนี้ถูกใช้แล้ว");
                return View(input);
            }
        }

        // GET: /Developers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Developers.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Developers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Developer input)
        {
            if (id != input.Id) return BadRequest();

            Normalize(input);

            if (await _db.Developers.AnyAsync(d => d.Email == input.Email && d.Id != id))
            {
                ModelState.AddModelError(nameof(Developer.Email), "อีเมลนี้ถูกใช้แล้ว");
            }

            if (!ModelState.IsValid) return View(input);

            var existing = await _db.Developers.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Email = input.Email;
            existing.Name = input.Name;
            existing.IsActive = input.IsActive;

            try
            {
                await _db.SaveChangesAsync();
                TempData["ok"] = "บันทึกการแก้ไขสำเร็จ";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "บันทึกไม่สำเร็จ กรุณาลองใหม่");
                return View(input);
            }
        }

        // POST: /Developers/CreateQuick
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuick([FromForm] string email, [FromForm] string? name)
        {
            // trim + lower ให้สอดคล้องกับ Normalize()
            email = (email ?? string.Empty).Trim().ToLowerInvariant();
            name = string.IsNullOrWhiteSpace(name) ? null : name!.Trim();

            // validate เบื้องต้น
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "กรุณากรอกอีเมล" });

            // (ออปชัน) เช็คฟอร์แมตอีเมลแบบหยาบ ๆ พอประมาณ
            try
            {
                _ = new System.Net.Mail.MailAddress(email);
            }
            catch
            {
                return BadRequest(new { message = "รูปแบบอีเมลไม่ถูกต้อง" });
            }

            // ถ้ามีอยู่แล้ว -> ส่งกลับ id เดิม (เหมือนแนวทาง Category.CreateQuick ที่ส่ง duplicate)
            var exists = await _db.Developers
                .FirstOrDefaultAsync(d => d.Email == email);

            if (exists != null)
            {
                return Ok(new
                {
                    id = exists.Id,
                    email = exists.Email,
                    name = exists.Name,
                    duplicate = true
                });
            }

            // ไม่มี -> เพิ่มใหม่ (กำหนด IsActive = true เหมือนหน้าสร้างปกติ)
            var item = new Developer
            {
                Email = email,
                Name = name,
                IsActive = true
            };

            _db.Developers.Add(item);
            await _db.SaveChangesAsync();

            // ส่งกลับสำหรับเติมเข้า Select/Select2 ได้ทันที
            return Ok(new
            {
                id = item.Id,
                email = item.Email,
                name = item.Name
            });
        }


        // POST: /Developers/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var dev = await _db.Developers.FindAsync(id);
            if (dev == null) return NotFound();

            dev.IsActive = !dev.IsActive;
            await _db.SaveChangesAsync();
            TempData["ok"] = dev.IsActive ? "เปิดใช้งานแล้ว" : "ปิดการใช้งานแล้ว";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Developers/Delete/5
        // ลบจริงเมื่อไม่ถูกใช้งาน;
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var dev = await _db.Developers.FindAsync(id);
            if (dev == null) return NotFound();

            var inUse = await _db.CatalogAssignments.AnyAsync(a => a.DeveloperId == id);
            if (inUse)
            {
                TempData["err"] = "ไม่สามารถลบได้เนื่องจากถูกใช้งานอยู่ในบาง Service — กรุณาปิดการใช้งานแทน";
                return RedirectToAction(nameof(Index));
            }

            _db.Developers.Remove(dev);
            await _db.SaveChangesAsync();
            TempData["ok"] = "ลบ Developer สำเร็จ";
            return RedirectToAction(nameof(Index));
        }




        // --- helpers ---
        private static void Normalize(Developer d)
        {
            d.Email = (d.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(d.Name))
                d.Name = d.Name.Trim();
        }
    }
}
