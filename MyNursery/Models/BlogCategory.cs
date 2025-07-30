using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUSAD.Models
{
    public class BlogCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [Display(Name = "Category Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(500)]
        public string? Description { get; set; }  // made nullable
    }
}
