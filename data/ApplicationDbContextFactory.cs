using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Http;     
using ServiceCatalogBGC_V2.data;

namespace ServiceCatalogBGC_V2
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=WF-074-67\\SQLEXPRESS; Database=CatalogDB_V2; Trusted_Connection=True; TrustServerCertificate=True");

            // ตอน design-time ไม่มี HttpContext จริง ใช้ตัวเปล่าได้
            return new ApplicationDbContext(optionsBuilder.Options, new HttpContextAccessor());
        }
    }
}
