using AuthSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceCatalogBGC_V2.data;
using ServiceCatalogBGC_V2.Models;
using ServiceCatalogBGC_V2.ViewModels.Catalogs;
using System.Linq;

[Authorize]
public class CatalogController : Controller
{
    private readonly ApplicationDbContext _context;
    public CatalogController(ApplicationDbContext context) => _context = context;

    // =========================
    // Index
    // =========================
    [Authorize(Policy = AppPolicies.CatalogRead)]
    public async Task<IActionResult> Index(string? searchString)
    {
        var q = _context.Catalogs
            .AsNoTracking()
            .Include(c => c.Type)
            .Include(c => c.Category)
            .Include(c => c.Status)
            .Include(c => c.Priority)
            .Include(c => c.DeploymentType)
            .Include(c => c.Assignments!)
                .ThenInclude(a => a.Developer)
            .Where(c => !c.Deleted);

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            q = q.Where(c =>
                (c.Name != null && EF.Functions.Like(c.Name, $"%{searchString}%")) ||
                (c.Description != null && EF.Functions.Like(c.Description, $"%{searchString}%")) ||
                (c.Type != null && c.Type.Name != null && EF.Functions.Like(c.Type.Name, $"%{searchString}%")) ||
                (c.Category != null && c.Category.Name != null && EF.Functions.Like(c.Category.Name, $"%{searchString}%")) ||
                (c.Status != null && c.Status.Name != null && EF.Functions.Like(c.Status.Name, $"%{searchString}%")) ||
                (c.Priority != null && c.Priority.Name != null && EF.Functions.Like(c.Priority.Name, $"%{searchString}%")) ||
                (c.Bu != null && EF.Functions.Like(c.Bu, $"%{searchString}%")) ||
                (c.Dept != null && EF.Functions.Like(c.Dept, $"%{searchString}%")) ||
                (c.BOwner != null && EF.Functions.Like(c.BOwner, $"%{searchString}%")) ||
                (c.BUser != null && EF.Functions.Like(c.BUser, $"%{searchString}%")) ||
                (c.ITBP != null && EF.Functions.Like(c.ITBP, $"%{searchString}%")) ||
                (c.ReleasedMark != null && EF.Functions.Like(c.ReleasedMark, $"%{searchString}%")) ||
                (c.Location != null && EF.Functions.Like(c.Location, $"%{searchString}%")) ||
                (c.SURL != null && EF.Functions.Like(c.SURL, $"%{searchString}%")) ||
                (c.Ip != null && EF.Functions.Like(c.Ip, $"%{searchString}%")) ||
                (c.DEndpoint != null && EF.Functions.Like(c.DEndpoint, $"%{searchString}%")) ||
                // ค้นหาในอีเมลของ Functional/SA และ Vendor/Developer
                c.Assignments!.Any(a => a.Role == AssignmentRole.FunctionalSA &&
                                        a.Developer != null &&
                                        a.Developer.Email != null &&
                                        EF.Functions.Like(a.Developer.Email, $"%{searchString}%")) ||
                c.Assignments!.Any(a => a.Role == AssignmentRole.VendorDeveloper &&
                                        a.Developer != null &&
                                        a.Developer.Email != null &&
                                        EF.Functions.Like(a.Developer.Email, $"%{searchString}%"))
            );
        }

        var catalogs = await q.ToListAsync();

        // Map email lists
        ViewBag.FunctionalMap = catalogs.ToDictionary(
            c => c.Id,
            c => string.Join(",", c.Assignments!
                                    .Where(a => a.Role == AssignmentRole.FunctionalSA)
                                    .Select(a => a.Developer!.Email))
        );
        ViewBag.VendorMap = catalogs.ToDictionary(
            c => c.Id,
            c => string.Join(",", c.Assignments!
                                    .Where(a => a.Role == AssignmentRole.VendorDeveloper)
                                    .Select(a => a.Developer!.Email))
        );

        ViewBag.CurrentFilter = searchString;
        return View(catalogs);
    }

    // =========================
    // Details
    // =========================
    //[Authorize(Policy = AppPolicies.CatalogRead)]
    //public async Task<IActionResult> Details(int? id)
    //{
    //    if (id == null) return NotFound();

    //    var catalog = await _context.Catalogs
    //        .AsNoTracking()
    //        .Include(c => c.Type)
    //        .Include(c => c.Category)
    //        .Include(c => c.Status)
    //        .Include(c => c.Priority)
    //        .Include(c => c.DeploymentType)
    //        .Include(c => c.Assignments!)
    //            .ThenInclude(a => a.Developer)
    //        .FirstOrDefaultAsync(m => m.Id == id);

    //    if (catalog == null || catalog.Deleted) return NotFound();

    //    ViewBag.FunctionalList = catalog.Assignments!
    //        .Where(a => a.Role == AssignmentRole.FunctionalSA)
    //        .Select(a => a.Developer!.Email)
    //        .ToList();

    //    ViewBag.VendorList = catalog.Assignments!
    //        .Where(a => a.Role == AssignmentRole.VendorDeveloper)
    //        .Select(a => a.Developer!.Email)
    //        .ToList();

    //    return View(catalog);
    //}

    // =========================
    // Create (GET)
    // =========================
    [Authorize(Policy = AppPolicies.CatalogWrite)]
    public async Task<IActionResult> Create()
    {
        var vm = new CatalogEditVM();
        await LoadDropdownsAsync(vm);
        await LoadDevelopersAsync(vm);
        return View(vm);
    }

    // =========================
    // Create (POST)
    // =========================
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Policy = AppPolicies.CatalogWrite)]
    public async Task<IActionResult> Create(CatalogEditVM vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(vm);
            await LoadDevelopersAsync(vm);
            return View(vm);
        }

        var entity = new Catalog
        {
            Name = vm.SystemName,
            Description = vm.Description,
            ServiceTypeId = vm.TypeId,
            CategoryId = vm.CategoryId,
            StatusId = vm.StatusId,
            PriorityId = vm.PriorityId,
            DeploymentTypeId = vm.DeploymentTypeId,
            ReleasedDate = vm.ReleasedDate,
            ReleasedMark = vm.ReleasedMark,
            LastUpdate = vm.LastUpdate,
            Location = vm.Location,
            SURL = vm.SURL,
            Ip = vm.Ip,
            DEndpoint = vm.DEndpoint,
            Bu = vm.Bu,
            Dept = vm.Dept,
            BOwner = vm.BOwner,
            BUser = vm.BUser,
            ITBP = vm.ITBP
        };

        _context.Catalogs.Add(entity);
        await _context.SaveChangesAsync();
        TempData["ok"] = "เพิ่ม Service สำเร็จ";


        await SaveAssignmentsAsync(entity.Id, vm.FunctionalSaIds, vm.VendorDevIds);

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // Edit (GET)
    // =========================
    [HttpGet]
    [Authorize(Policy = AppPolicies.CatalogMeta)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var c = await _context.Catalogs
            .Include(x => x.Assignments!)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c == null) return NotFound();

        var vm = new CatalogEditVM
        {
            Id = c.Id,
            SystemName = c.Name,
            Description = c.Description,
            TypeId = c.ServiceTypeId,
            CategoryId = c.CategoryId,
            StatusId = c.StatusId,
            PriorityId = c.PriorityId,
            DeploymentTypeId = c.DeploymentTypeId,
            ReleasedDate = c.ReleasedDate,
            ReleasedMark = c.ReleasedMark,
            LastUpdate = c.LastUpdate,
            Location = c.Location,
            SURL = c.SURL,
            Ip = c.Ip,
            DEndpoint = c.DEndpoint,
            Bu = c.Bu,
            Dept = c.Dept,
            BOwner = c.BOwner,
            BUser = c.BUser,
            ITBP = c.ITBP,

            FunctionalSaIds = c.Assignments!
                .Where(a => a.Role == AssignmentRole.FunctionalSA)
                .Select(a => a.DeveloperId).ToList(),

            VendorDevIds = c.Assignments!
                .Where(a => a.Role == AssignmentRole.VendorDeveloper)
                .Select(a => a.DeveloperId).ToList()
        };

        await LoadDropdownsAsync(vm);
        await LoadDevelopersAsync(vm);
        return View(vm);
    }

    // =========================
    // Edit (POST)
    // =========================
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Policy = AppPolicies.CatalogMeta)]
    public async Task<IActionResult> Edit(int id, CatalogEditVM vm)
    {
        if (id != vm.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(vm);
            await LoadDevelopersAsync(vm);
            return View(vm);
        }

        var c = await _context.Catalogs.FirstOrDefaultAsync(x => x.Id == id);
        if (c == null) return NotFound();

        c.Name = vm.SystemName;
        c.Description = vm.Description;
        c.ServiceTypeId = vm.TypeId;
        c.CategoryId = vm.CategoryId;
        c.StatusId = vm.StatusId;
        c.PriorityId = vm.PriorityId;
        c.DeploymentTypeId = vm.DeploymentTypeId;
        c.ReleasedDate = vm.ReleasedDate;
        c.ReleasedMark = vm.ReleasedMark;
        c.LastUpdate = vm.LastUpdate;
        c.Location = vm.Location;
        c.SURL = vm.SURL;
        c.Ip = vm.Ip;
        c.DEndpoint = vm.DEndpoint;
        c.Bu = vm.Bu;
        c.Dept = vm.Dept;
        c.BOwner = vm.BOwner;
        c.BUser = vm.BUser;
        c.ITBP = vm.ITBP;

        await _context.SaveChangesAsync();
        TempData["ok"] = "บันทึกการแก้ไขสำเร็จ";

        await SaveAssignmentsAsync(id, vm.FunctionalSaIds, vm.VendorDevIds);

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // Delete (Soft Delete)
    // =========================
    [HttpPost]
    [Authorize(Policy = AppPolicies.CatalogMeta)]
    public async Task<IActionResult> Delete(int id)
    {
        var catalog = await _context.Catalogs.FindAsync(id);
        if (catalog != null)
        {
            catalog.Deleted = true;
            await _context.SaveChangesAsync();
            TempData["ok"] = "ลบสำเร็จ";
        }
        return RedirectToAction(nameof(Index));
    }

    // =========================
    // Helpers
    // =========================
    private async Task LoadDropdownsAsync(CatalogEditVM vm)
    {
        ViewBag.ServiceTypeId = new SelectList(
            await _context.ServiceType
                .AsNoTracking()
                .Where(x => !x.Deleted)
                .OrderBy(x => x.Name)
                .ToListAsync(),
            "Id",
            "Name",
            vm.TypeId
        );

        ViewBag.CategoryId = new SelectList(
            await _context.Categories.AsNoTracking().Where(x => !x.Deleted).OrderBy(x => x.Name).ToListAsync(),
            "Id",
            "Name",
            vm.CategoryId
        );

        ViewBag.StatusId = new SelectList(
            await _context.Statuses.AsNoTracking().Where(x => !x.Deleted).OrderBy(x => x.Name).ToListAsync(),
            "Id",
            "Name",
            vm.StatusId
        );

        ViewBag.PriorityId = new SelectList(
            await _context.Priorities.AsNoTracking().Where(x => !x.Deleted).OrderBy(x => x.Name).ToListAsync(),
            "Id",
            "Name",
            vm.PriorityId
        );

        ViewBag.DeploymentTypeId = new SelectList(
            await _context.DeploymentTypes.AsNoTracking().Where(x => !x.Deleted).OrderBy(x => x.Name).ToListAsync(),
            "Id",
            "Name",
            vm.DeploymentTypeId
        );
    }

    private async Task LoadDevelopersAsync(CatalogEditVM vm)
    {
        var items = await _context.Developers
            .AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.Email)
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Email })
            .ToListAsync();

        vm.FunctionalSaOptions = items;
        vm.VendorDevOptions = new List<SelectListItem>(items);
    }

    private async Task SaveAssignmentsAsync(
        int catalogId,
        IEnumerable<int>? functionalIds,
        IEnumerable<int>? vendorIds)
    {
        var old = await _context.CatalogAssignments
            .Where(a => a.CatalogId == catalogId)
            .ToListAsync();

        _context.CatalogAssignments.RemoveRange(old);
        await _context.SaveChangesAsync();

        var func = (functionalIds ?? Enumerable.Empty<int>()).Distinct();
        var vend = (vendorIds ?? Enumerable.Empty<int>()).Distinct();

        var newOnes = new List<CatalogAssignment>();
        newOnes.AddRange(func.Select(id => new CatalogAssignment
        {
            CatalogId = catalogId,
            DeveloperId = id,
            Role = AssignmentRole.FunctionalSA
        }));
        newOnes.AddRange(vend.Select(id => new CatalogAssignment
        {
            CatalogId = catalogId,
            DeveloperId = id,
            Role = AssignmentRole.VendorDeveloper
        }));

        if (newOnes.Count > 0)
        {
            _context.CatalogAssignments.AddRange(newOnes);
            await _context.SaveChangesAsync();
        }
    }

    private bool CatalogExists(int id) => _context.Catalogs.Any(e => e.Id == id);


}
