using System;
using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUAD.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Display(Name = "Event Time")]
        [DataType(DataType.Time)]
        public TimeSpan EventTime { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Featured")]
        public bool IsFeatured { get; set; }
    }
}
