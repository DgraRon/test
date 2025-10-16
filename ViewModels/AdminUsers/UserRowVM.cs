namespace ServiceCatalogBGC_V2.ViewModels.AdminUsers
{
    public class UserRowVM
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string Roles { get; set; } = "";
        public bool IsDisabled { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
    //public class EditUserVM
    //{
    //    public string Id { get; set; }
    //    public string Email { get; set; }
    //    public bool IsDisabled { get; set; }           // ใช้ Lockout เป็นตัวแทน “Disable”
    //    public List<SelectListItem> AllRoles { get; set; } = new();
    //    public List<string> SelectedRoles { get; set; } = new(); // roles ที่ติ๊กอยู่
    //}

}
