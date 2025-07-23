using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNursery.Models
{
    public class BlogImage
    {
        public int Id { get; set; }

        [Required]
        public string ImagePath { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!; // e.g. "Cover", "Optional1", "Optional2"

        public int BlogPostId { get; set; }

        [ForeignKey("BlogPostId")]
        public BlogPost? BlogPost { get; set; }
    }
}
