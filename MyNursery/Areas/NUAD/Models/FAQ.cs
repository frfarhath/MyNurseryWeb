using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUAD.Models
{
    public class FAQ
    {
        public int Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        public string Category { get; set; }

        public int OrderDisplay { get; set; } = 0;
    }
}
