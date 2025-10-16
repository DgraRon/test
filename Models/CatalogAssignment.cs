using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceCatalogBGC_V2.Models
{
    public enum AssignmentRole
    {
        FunctionalSA = 1,
        VendorDeveloper = 2
    }

    public class CatalogAssignment
    {
        // Composite Key: CatalogId + DeveloperId + Role
        public int CatalogId { get; set; }
        public int DeveloperId { get; set; }
        public AssignmentRole Role { get; set; }

        // Navigation
        public Catalog? Catalog { get; set; }
        public Developer? Developer { get; set; }
    }
}
