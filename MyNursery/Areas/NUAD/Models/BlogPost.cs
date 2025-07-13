using System;
using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUAD.Models

{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public required string Category { get; set; }

        public string? CoverImagePath { get; set; }
        public string? OptionalImage1Path { get; set; }
        public string? OptionalImage2Path { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public required string Content { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Optional PublishDate - can be null if not set
        public DateTime? PublishDate { get; set; }
        public string? Status { get; set; }
    }
}
