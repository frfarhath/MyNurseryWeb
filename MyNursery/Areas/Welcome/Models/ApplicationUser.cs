using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNursery.Areas.Welcome.Models
{
    public enum UserType
    {
        Predefined = 0,    // seeded in Program.cs
        Registered = 1,    // registered through system
        AdminAdded = 2     // manually added by admin (e.g., NUAD)
    }

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
        public UserType UserType { get; set; } = UserType.Registered;
        public string NUUSDashboardId { get; set; } = string.Empty;

    }

}
