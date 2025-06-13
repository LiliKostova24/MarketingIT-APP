// Models/EditPostViewModel.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MarketingIT.Models
{
    public class EditPostViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        // show the existing URL
        public string ExistingImageUrl { get; set; }

        // allow a new upload
        public IFormFile Photo { get; set; }
    }
}
