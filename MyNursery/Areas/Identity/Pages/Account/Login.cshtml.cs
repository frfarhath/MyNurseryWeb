﻿// LOGIN MODEL
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyNursery.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            ReturnUrl = returnUrl ?? Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
                return Page();
            // Check user exists and is active
            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, user == null ? "Invalid login attempt." : "Your account is disabled. Please contact administrator.");
                return Page();
            }

            // ✅ Sign in using Identity's method (it handles cookies & claims automatically)
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // ✅ You can store session values here
                var roles = await _userManager.GetRolesAsync(user);
                HttpContext.Session.SetString("UserFullName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", roles.FirstOrDefault() ?? "Unknown");
                HttpContext.Session.SetString("Area", user.Area ?? "Welcome");

           

            if (user.MustChangePassword)
                {
                    TempData["ForceChangePassword"] = "true";
                    return RedirectToAction("Index", "Home", new { area = "NUUS" });
                }

                if (roles.Contains(SD.Role_Admin))
                {
                    return user.Email.ToLower() switch
                    {
                        "nuad.user@littlesprouts.com" => RedirectToAction("Index", "Home", new { area = "NUAD" }),
                        "nusad.user@littlesprouts.com" => RedirectToAction("Index", "Home", new { area = "NUSAD" }),
                        "csad.user@littlesprouts.com" => RedirectToAction("Index", "Home", new { area = "CSAD" }),
                        _ => RedirectToAction("Index", "Home", new { area = "NUAD" })
                    };
                }

                if (roles.Contains(SD.Role_SuperAdmin))
                    return RedirectToAction("Index", "Home", new { area = "NUSAD" });
                if (roles.Contains(SD.Role_AdminCSAD))
                    return RedirectToAction("Index", "Home", new { area = "CSAD" });
                if (user.UserType == "AdminAdded")
                    return RedirectToAction("Index", "Home", new { area = "NUUS" });
                if (roles.Contains(SD.Role_OtherUser))
                    return RedirectToAction("Index", "Home", new { area = "NUUS" });
                if (roles.Contains(SD.Role_User))
                    return RedirectToAction("Index", "Home", new { area = "NUOUS" });

                return RedirectToAction("Index", "Home", new { area = "Welcome" });
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
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