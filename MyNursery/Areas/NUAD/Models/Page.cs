using System;
using System.ComponentModel.DataAnnotations;
using MyNursery.Areas.Welcome.Models;  // <-- Add this for ApplicationUser

namespace MyNursery.Areas.NUAD.Models
{
    public class Page
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Slug { get; set; }  // Add Slug property

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation property
        public ApplicationUser? LastUpdatedByUser { get; set; }  // nullable

        public string? LastUpdatedByUserId { get; set; }  // FK, nullable if optional
    }
}
