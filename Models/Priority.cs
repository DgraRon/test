using System.ComponentModel.DataAnnotations;

namespace ServiceCatalogBGC_V2.Models
{
    public class Priority : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public ICollection<Catalog>? Catalogs { get; set; }
    }

}
