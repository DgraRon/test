namespace ServiceCatalogBGC_V2.Models
{
    public abstract class BaseEntity
    {
        public bool Status { get; set; } = true;
        public bool Deleted { get; set; } = false;

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }

}
