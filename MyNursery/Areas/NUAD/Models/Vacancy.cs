using System;
using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUAD.Models
{
    public class Vacancy
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        public string Description { get; set; }

        public string Requirements { get; set; }

        public string ApplicationProcess { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Posted")]
        public DateTime DatePosted { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Closing Date")]
        public DateTime? ClosingDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
