using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using MyNursery.Services;
using MyNursery.Areas.Welcome.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyNursery.Data;

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
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.Email,
                Email = Input.Email,
                DateCreated = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Role assignment
                if (user.Email.ToLower() == "frfarhath21@gmail.com")
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, Input.Role);
                }

                // Generate OTP for email verification
                var otp = new Random().Next(100000, 999999).ToString();
                user.EmailOTP = otp;
                user.EmailOTPExpiry = DateTime.UtcNow.AddMinutes(5);
                await _userManager.UpdateAsync(user);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Your OTP Code for Little Sprouts Nursery",
                    $@"
<html>
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
        }

        return Page();
    }
}
