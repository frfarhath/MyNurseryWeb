using System.ComponentModel.DataAnnotations;

namespace MyNursery.Areas.NUSAD.Models
{
    public class UserDisplayViewModel
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? ContactNumber { get; set; }
        public string UserType { get; set; } = null!;
        [MaxLength(50)]
        public string Area { get; set; } = string.Empty;



    }
}
