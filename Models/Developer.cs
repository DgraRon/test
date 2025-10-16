using System.ComponentModel.DataAnnotations;

namespace ServiceCatalogBGC_V2.Models
{
    public class Developer
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Name { get; set; }  // ถ้ายังไม่ใช้ จะละไว้ก็ได้

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<CatalogAssignment>? Assignments { get; set; }
    }
}
