using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MyNursery.Areas.Welcome.Models;

namespace MyNursery.Views.Identity.Pages.Account
{
    public class VerifyOTPModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyOTPModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string? UserId { get; set; }

        [BindProperty]
        [Required]
        public string? OTP { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId ?? "");

            if (user == null)
            {
                ErrorMessage = "Invalid user.";
                return Page();
            }

            if (user.EmailOTP != OTP || DateTime.UtcNow > user.EmailOTPExpiry)
            {
                ErrorMessage = "Invalid or expired OTP.";
                return Page();
            }

            user.EmailConfirmed = true;
            user.EmailOTP = null;
            await _userManager.UpdateAsync(user);

            return RedirectToPage("Login");
        }
    }
}
