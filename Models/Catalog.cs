using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceCatalogBGC_V2.Models
{
    public class Catalog : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ServiceTypeId { get; set; }
        public ServiceType? Type { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? StatusId { get; set; }
        public new Status? Status { get; set; }
        public int? PriorityId { get; set; }
        public Priority? Priority { get; set; }
        [DisplayName("BU")]
        public string? Bu { get; set; }
        [DisplayName("Dept.")]
        public string? Dept { get; set; }
        [DisplayName("Business owner")]
        public string? BOwner { get; set; }
        [DisplayName("Business user")]
        public string? BUser { get; set; }
        [DisplayName("IT BP/IT PM")]
        public string? ITBP { get; set; }


        [Column("Updated")]                 // map ไปคอลัมน์เดิมชื่อ Updated
        [Display(Name = "Released mark")]   // ป้ายชื่อในฟอร์ม/วิว
        public string? ReleasedMark { get; set; }   // << เดิมเป็น Updated(string)

        [DataType(DataType.Date)]
        [Display(Name = "Released date")]
        public DateTime? ReleasedDate { get; set; } // << คอลัมน์ใหม่


        public DateTime? LastUpdate { get; set; }
        //[DisplayName("Functional/SA")]
        //public string? Functional { get; set; }
        //[DisplayName("Vendor/Developer")]
        //public string? Dev { get; set; }
        public int? DeploymentTypeId { get; set; }
        public DeploymentType? DeploymentType { get; set; }
        [DisplayName("Plant/Location")]
        public string? Location { get; set; }
        [DisplayName("System URL")]
        public string? SURL { get; set; }
        [DisplayName("IP/HostName")]
        public string? Ip { get; set; }
        [DisplayName("Database Endpoint")]
        public string? DEndpoint { get; set; }

        public ICollection<CatalogAssignment>? Assignments { get; set; }





    }

}
