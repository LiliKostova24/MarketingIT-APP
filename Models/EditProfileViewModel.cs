using System.ComponentModel.DataAnnotations;

namespace MarketingIT.Models
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Upload New Avatar")]
        public IFormFile? Avatar { get; set; }       // for upload
        public string? ExistingAvatarPath { get; set; } // to show current
        
    }
}
