using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ServiceCatalogBGC_V2.Areas.Identity.Data;

namespace ServiceCatalogBGC_V2.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser_> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser_> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // ค่า default หลังล็อกอิน → ไปหน้า Catalog/Index
            var defaultUrl = Url.Action("Index", "Catalog") ?? Url.Content("~/");

            ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? defaultUrl : returnUrl;

            // ให้แน่ใจว่าคุกกี้ external เคลียร์ก่อน
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager
                                .GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var defaultUrl = Url.Action("Index", "Catalog") ?? Url.Content("~/");
            returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? defaultUrl : returnUrl;

            ExternalLogins = (await _signInManager
                                .GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // หมายเหตุ: โดยดีฟอลต์โค้ดนี้จะใช้ Email เป็น Username ด้วย
            var result = await _signInManager.PasswordSignInAsync(
                Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                // ป้องกัน open redirect – อนุญาตเฉพาะ local url
                return Url.IsLocalUrl(returnUrl)
                    ? LocalRedirect(returnUrl)
                    : Redirect(defaultUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa",
                    new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
