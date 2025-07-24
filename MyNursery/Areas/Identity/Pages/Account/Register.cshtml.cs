using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MyNursery.Areas.Welcome.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using MyNursery.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNursery.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = default!;

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = default!;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = default!;

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = default!;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = default!;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.Email,
                Email = Input.Email,
                NUUSDashboardId = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow,
                UserType = SD.UserType_Registered // Default user type for registered users
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Role assignment logic
                if (user.Email.Equals("frfarhath21@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_Admin); // Optional if you want this admin account
                    user.UserType = SD.UserType_AdminAdded;
                    user.Area = "NUUS"; // ✅ Admin-added user → NUUS
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_User); // Role_User = NuOUS
                    user.UserType = SD.UserType_Registered;
                    user.Area = "NUOUS"; // ✅ Registered user → NUOUS
                }


                await _userManager.UpdateAsync(user);

                // Generate OTP for email verification
                var otp = new Random().Next(100000, 999999).ToString();
                user.EmailOTP = otp;
                user.EmailOTPExpiry = DateTime.UtcNow.AddMinutes(5);
                await _userManager.UpdateAsync(user);

                // Send OTP email
                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Your OTP Code for Little Sprouts Nursery",
                    $@"<html>
  <body style='font-family: Arial, sans-serif; color: #333;'>
    <h2>Welcome to Little Sprouts Nursery!</h2>
    <p>Thank you for registering. To verify your email, use the OTP below:</p>
    <p style='font-size: 24px; font-weight: bold; color: #007bff;'>{otp}</p>
    <p>This OTP is valid for <strong>5 minutes</strong>. Please do not share it with anyone.</p>
    <hr />
    <p style='font-size: 12px; color: #999;'>If you didn’t request this, you can safely ignore this email.</p>
    <p style='font-size: 12px;'>— Little Sprouts Nursery Team</p>
  </body>
</html>");

                return RedirectToPage("VerifyOTP", new { userId = user.Id });
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Something failed, redisplay form
            return Page();
        }
    }
}
