using System;
using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUAD.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters and spaces are allowed")]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters and spaces are allowed")]
        public string LastName { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string EmailAddress { get; set; }

        [Required]
        public string Role { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(15)]
        public string ContactNumber { get; set; }
    }
}
