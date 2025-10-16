// ViewModels/Catalogs/CatalogEditVM.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ServiceCatalogBGC_V2.ViewModels.Catalogs
{
    public class CatalogEditVM
    {
        public int Id { get; set; }

        // ชื่อบริการ
        [Required(ErrorMessage = "กรุณากรอกชื่อระบบ")]
        [StringLength(150, ErrorMessage = "ชื่อระบบต้องไม่เกิน 150 ตัวอักษร")]
        [Display(Name = "Name")]
        public string? SystemName { get; set; }

        // FK dropdowns
        [Required(ErrorMessage = "กรุณาเลือก Type")]
        public int? TypeId { get; set; }
        [Required(ErrorMessage = "กรุณาเลือก Category")]
        public int? CategoryId { get; set; }
        [Required(ErrorMessage = "กรุณาเลือก Status")]
        public int? StatusId { get; set; }
        [Required(ErrorMessage = "กรุณาเลือก Priority")]
        public int? PriorityId { get; set; }
        [Required(ErrorMessage = "กรุณาเลือก Deployment Type")]
        public int? DeploymentTypeId { get; set; }

        // รายละเอียด
        [StringLength(1000, ErrorMessage = "Description ต้องไม่เกิน 1000 ตัวอักษร")]
        public string? Description { get; set; }

        // วันที่/Mark
        [DataType(DataType.Date)]
        public DateTime? ReleasedDate { get; set; }
        [StringLength(50)]
        public string? ReleasedMark { get; set; }
        [DataType(DataType.Date)]
        public DateTime? LastUpdate { get; set; }

        // ข้อมูลอื่น ๆ
        [StringLength(50)]
        public string? Location { get; set; }
        [Url(ErrorMessage = "รูปแบบ URL ไม่ถูกต้อง")]
        [Display(Name = "System URL")]
        public string? SURL { get; set; }
        [StringLength(100)]
        public string? Ip { get; set; }
        [StringLength(100)]
        public string? DEndpoint { get; set; }
        [StringLength(100)]
        public string? Bu { get; set; }
        [StringLength(100)]
        public string? Dept { get; set; }
        [StringLength(100)]
        public string? BOwner { get; set; }
        [StringLength(100)]
        public string? BUser { get; set; }
        [StringLength(100)]
        public string? ITBP { get; set; }

        // ---------- Select2: People ----------
        // ค่าที่ผู้ใช้เลือก (โพสต์กลับมา)
        [Display(Name = "Functional/SA")]
        public List<int>? FunctionalSaIds { get; set; } = new();

        [Display(Name = "Vendor/Developer")]
        public List<int>? VendorDevIds { get; set; } = new();

        // รายการตัวเลือกสำหรับแต่ละช่อง (Text = email, Value = Developer.Id)
        public IEnumerable<SelectListItem> FunctionalSaOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> VendorDevOptions { get; set; } = Enumerable.Empty<SelectListItem>();

        // (ออปชัน) ถ้าจะจำกัดจำนวนที่เลือก ใช้ bind ไป data-max ใน view ได้
        public int? FunctionalMax { get; set; }
        public int? VendorMax { get; set; }
    }
}
