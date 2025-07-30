using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MyNursery.Areas.NUUS.Controllers
{
    [Area("NUUS")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        // GET: /NUUS/Account/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData[SD.Error_Msg] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account", new { area = "NUUS" });
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            ViewData["UserRoles"] = roles;

            return View("~/Areas/NUUS/Views/Profile/Profile.cshtml", user);
        }

        // POST: /NUUS/Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Areas/NUUS/Views/Profile/Profile.cshtml", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData[SD.Error_Msg] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account", new { area = "NUUS" });
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("~/Areas/NUUS/Views/Profile/Profile.cshtml", model);
            }

            // ✅ Mark user as no longer needing to change password
            user.MustChangePassword = false;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                TempData[SD.Warning_Msg] = "Password changed, but user state could not be updated.";
            }

            await _signInManager.RefreshSignInAsync(user);

            // 📧 Send confirmation email
            string subject = "Password Changed Successfully";
            string body = $@"
    <div style='font-family:Segoe UI, sans-serif; font-size:15px; line-height:1.6;'>
        <h2 style='color:#2d7d46;'>Hello {user.FirstName},</h2>
        <p>We wanted to let you know that your account password was successfully changed on 
        <strong>{DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC</strong>.</p>
        
        <p>If you did not make this change, please contact our support team immediately to secure your account.</p>

        <p>Thank you for being part of the Little Sprouts Nursery community.</p>

        <p style='margin-top:30px;'>Warm regards,<br>
        <strong>Little Sprouts Nursery Team</strong></p>
    </div>";

            try
            {
                await _emailSender.SendEmailAsync(user.Email, subject, body);
                TempData[SD.Success_Msg] = "Your password has been changed successfully. A confirmation email has been sent.";
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Password changed but email not sent: {ex.Message}");
                TempData[SD.Warning_Msg] = "Password changed, but confirmation email could not be sent.";
            }

            return RedirectToAction("Profile", "Account", new { area = "NUUS" });
        }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}