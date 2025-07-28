using MyNursery.Areas.NUSAD.Models;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MyNursery.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public required string Title { get; set; }

        // Nullable to allow posts without category initially
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public BlogCategory? Category { get; set; }


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
        public List<BlogImage> BlogImages { get; set; } = new List<BlogImage>();

        [NotMapped]
        public string? CoverImagePath => BlogImages.FirstOrDefault(i => i.Type == "Cover")?.ImagePath;

        [NotMapped]
        public string? OptionalImage1Path => BlogImages.FirstOrDefault(i => i.Type == "Optional1")?.ImagePath;

        [NotMapped]
        public string? OptionalImage2Path => BlogImages.FirstOrDefault(i => i.Type == "Optional2")?.ImagePath;
    }
}
