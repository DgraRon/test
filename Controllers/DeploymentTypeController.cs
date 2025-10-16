using AuthSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceCatalogBGC_V2.data;
using ServiceCatalogBGC_V2.Models;
using System;

[Authorize(Policy = AppPolicies.CatalogMeta)]
public class DeploymentTypeController : Controller
{
    private readonly ApplicationDbContext _context;
    public DeploymentTypeController(ApplicationDbContext context) => _context = context;

    public IActionResult Index() =>
        View(_context.DeploymentTypes.Where(x => !x.Deleted).ToList());

    // GET: /ServiceType/Create?returnUrl=/Catalog/Create
    public IActionResult Create(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(DeploymentType model, string? returnUrl)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        model.CreatedBy = User.Identity?.Name ?? "system";
        model.CreatedDate = DateTime.Now;
        model.ModifiedBy = model.CreatedBy;
        model.ModifiedDate = model.CreatedDate;

        _context.DeploymentTypes.Add(model);
        _context.SaveChanges();

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    // GET
    public IActionResult Edit(int id, string? returnUrl)
    {
        var item = _context.DeploymentTypes.Find(id);
        if (item == null) return NotFound();
        ViewBag.ReturnUrl = returnUrl;
        return View(item);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, DeploymentType model, string? returnUrl)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var original = _context.DeploymentTypes.Find(id);
        if (original == null) return NotFound();

        original.Name = model.Name;
        original.Description = model.Description;
        original.ModifiedBy = User.Identity?.Name ?? "system";
        original.ModifiedDate = DateTime.Now;

        _context.Update(original);
        _context.SaveChanges();

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var item = _context.DeploymentTypes.Find(id);
        if (item != null)
        {
            item.Deleted = true;
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /DeploymentType/CreateQuick
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateQuick([FromForm] string name)
    {
        name = (name ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "กรุณากรอกชื่อ Deployment Type" });

        var exists = _context.DeploymentTypes
            .FirstOrDefault(x => !x.Deleted && x.Name.ToLower() == name.ToLower());

        if (exists != null)
            return Ok(new { id = exists.Id, name = exists.Name, duplicate = true });

        var item = new DeploymentType
        {
            Name = name,
            CreatedBy = User.Identity?.Name ?? "system",
            CreatedDate = DateTime.Now,
            ModifiedBy = User.Identity?.Name ?? "system",
            ModifiedDate = DateTime.Now,
            Deleted = false
        };

        _context.DeploymentTypes.Add(item);
        _context.SaveChanges();

        return Ok(new { id = item.Id, name = item.Name });
    }

}
