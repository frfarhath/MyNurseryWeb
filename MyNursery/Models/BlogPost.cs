using MyNursery.Areas.Welcome.Models;
using MyNursery.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNursery.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public required string Category { get; set; }

        // These can be removed later if you fully switch to BlogImage
        public string? CoverImagePath { get; set; }
        public string? OptionalImage1Path { get; set; }
        public string? OptionalImage2Path { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public required string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PublishDate { get; set; }

        public string? Status { get; set; } = SD.Status_Pending;

        public string? CreatedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public ApplicationUser? CreatedByUser { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public List<BlogImage>? BlogImages { get; set; }
    }
}
