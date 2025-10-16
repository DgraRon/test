using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuthSystem;

namespace ServiceCatalogBGC_V2.ViewModels.AdminUsers
{
    public class CreateUserVM
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อ"), Display(Name = "First name")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "กรุณากรอกนามสกุล"), Display(Name = "Last name")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "กรุณากรอกอีเมล"), EmailAddress]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน"), DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; } = "";

        // ⬇️ เพิ่มอันนี้
        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน"), DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        public string Role { get; set; } = AppRoles.User;

        public List<SelectListItem> RoleOptions { get; set; } = new();
    }
}
