using System;
using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUAD.Models
{
    public class Policy
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; }

        public DateTime LastUpdated { get; set; }

    }

}
