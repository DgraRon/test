using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceCatalogBGC_V2.Models;
using System.Security.Claims;

namespace ServiceCatalogBGC_V2.data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _http;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor http) : base(options)
        {
            _http = http;
        }

        // --- DbSets เดิม ---
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<ServiceType> ServiceType { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<DeploymentType> DeploymentTypes { get; set; }

        // --- DbSets ใหม่ ---
        public DbSet<Developer> Developers { get; set; }              // เก็บอีเมล dev
        public DbSet<CatalogAssignment> CatalogAssignments { get; set; } // ตารางเชื่อม + บทบาท

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // Global filters (soft delete)
            // -------------------------
            modelBuilder.Entity<Catalog>().HasQueryFilter(e => !e.Deleted);
            modelBuilder.Entity<ServiceType>().HasQueryFilter(e => !e.Deleted);
            modelBuilder.Entity<Category>().HasQueryFilter(e => !e.Deleted);
            modelBuilder.Entity<Status>().HasQueryFilter(e => !e.Deleted);
            modelBuilder.Entity<Priority>().HasQueryFilter(e => !e.Deleted);
            modelBuilder.Entity<DeploymentType>().HasQueryFilter(e => !e.Deleted);

            // -------------------------
            // Developer
            // -------------------------
            // อีเมลต้องไม่ซ้ำ
            modelBuilder.Entity<Developer>()
                .HasIndex(d => d.Email)
                .IsUnique();

            // -------------------------
            // CatalogAssignment (junction table)
            // -------------------------
            // คีย์ผสม: CatalogId + DeveloperId + Role
            modelBuilder.Entity<CatalogAssignment>()
                .HasKey(a => new { a.CatalogId, a.DeveloperId, a.Role });

            // Catalog 1 - many Assignments
            modelBuilder.Entity<CatalogAssignment>()
                .HasOne(a => a.Catalog)
                .WithMany(c => c.Assignments)      // ถ้าไม่ได้เพิ่ม nav ใน Catalog ให้ใช้ .WithMany()
                .HasForeignKey(a => a.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);

            // Developer 1 - many Assignments
            modelBuilder.Entity<CatalogAssignment>()
                .HasOne(a => a.Developer)
                .WithMany(d => d.Assignments)
                .HasForeignKey(a => a.DeveloperId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        // ---- Audit stamping ----
        public override int SaveChanges()
        {
            StampAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            StampAudit();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void StampAudit()
        {
            var now = DateTime.Now;
            var user = GetCurrentUser();

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = now;
                    entry.Entity.CreatedBy = user;
                    entry.Entity.ModifiedDate = now;
                    entry.Entity.ModifiedBy = user;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    entry.Property(e => e.CreatedDate).IsModified = false;

                    entry.Entity.ModifiedDate = now;
                    entry.Entity.ModifiedBy = user;
                }
            }
        }

        private string GetCurrentUser()
        {
            var principal = _http.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated == true)
            {
                return principal.FindFirstValue(ClaimTypes.Email)
                       ?? principal.FindFirstValue(ClaimTypes.Name)
                       ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? "System";
            }
            return "System";
        }
    }
}
