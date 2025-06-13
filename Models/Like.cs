using MarketingIT.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingIT.Models
{
    public class Like
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int PostId { get; set; }

        public Post Post { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
