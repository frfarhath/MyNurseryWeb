using Microsoft.AspNetCore.Identity;
using MyNursery.Utility;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNursery.Areas.Welcome.Models
{
  

    public class ApplicationUser : IdentityUser
    {
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }

        public string? EmailOTP { get; set; }

        public DateTime EmailOTPExpiry { get; set; }

        [Display(Name = "User Type")]
        public string UserType { get; set; } = SD.UserType_Registered;
        public string NUUSDashboardId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Area { get; set; } = string.Empty;

        public bool MustChangePassword { get; set; } = false;




    }

}
