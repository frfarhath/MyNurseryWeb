using MyNursery.Areas.Welcome.Models;
using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUUS.Models
{

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

    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public ChangePasswordViewModel ChangePasswordModel { get; set; } = null!;
    }
}
