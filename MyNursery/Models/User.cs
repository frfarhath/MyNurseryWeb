using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [MaxLength(100)]
    [NotMapped]  // Prevent EF from expecting Password column
    public string? Password { get; set; }

    [Required]
    public string Role { get; set; }

    public DateTime AddedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(15)]
    public string ContactNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public string Area { get; set; } = string.Empty;



}
