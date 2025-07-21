using System;
using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUAD.Models
{
    public class CurriculumItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string AgeGroup { get; set; }

        public bool IsSpecialProgram { get; set; } = false;

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
