using System.ComponentModel.DataAnnotations;

namespace MyNursery.Models
{
    public class CompanyInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = null!; // Non-nullable, required

        [Required(ErrorMessage = "Phone Number is required.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!; // Non-nullable, required

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        [Display(Name = "Instagram URL")]
        public string? InstagramUrl { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        [Display(Name = "Facebook URL")]
        public string? FacebookUrl { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        [Display(Name = "Twitter URL")]
        public string? TwitterUrl { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        [Display(Name = "LinkedIn URL")]
        public string? LinkedInUrl { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        [Display(Name = "YouTube URL")]
        public string? YouTubeUrl { get; set; }

        [Display(Name = "Footer Description")]
        public string? FooterDescription { get; set; }
    }
}
