using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUSAD.Models
{
    public class ApplicationRole : IdentityRole
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
        public override string Name { get; set; }  // Consider overriding Name property from IdentityRole

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string? Description { get; set; }
    }

}
