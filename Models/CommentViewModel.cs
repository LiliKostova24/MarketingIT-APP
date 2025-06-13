using System.ComponentModel.DataAnnotations;

namespace MarketingIT.Models
{
    public class CommentViewModel
    {
        [Required]
        public string Text { get; set; }

        public int PostId { get; set; }
    }
}
