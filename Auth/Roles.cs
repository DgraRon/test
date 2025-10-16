// Auth/Roles.cs
namespace AuthSystem
{
    /// <summary>
    /// ชื่อบทบาทที่ระบบรองรับ
    /// </summary>
    public static class AppRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string User = "User";

        // ใช้ตอน seed / ตรวจสอบค่า
        public static readonly string[] All = { Admin, User };

        public static bool IsValid(string role) =>
            All.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// ชื่อ Authorization Policies ที่ใช้ใน [Authorize(Policy = ...)]
    /// </summary>
    public static class AppPolicies
    {
        // อ่าน/ดูรายการ (Index/Details) — Admin, User
        public const string CatalogRead = "CatalogRead";
        // เพิ่ม/แก้ไขรายการ (Create/Edit) — Admin, User
        public const string CatalogWrite = "CatalogWrite";
        // จัดการเมทาดาต้า (Type/Category/Status/Priority/DeploymentType) — Admin เท่านั้น
        public const string CatalogMeta = "CatalogMeta";
    }

    /// <summary>
    /// (ถ้าจะไปทาง Claim-based) กำหนดชื่อ claim/permission กลางไว้ที่นี่
    /// </summary>
    public static class AppClaims
    {
        public const string Permission = "perm";
        public const string CatalogRead = "catalog.read";
        public const string CatalogWrite = "catalog.write";
        public const string CatalogMeta = "catalog.meta";
    }
}
