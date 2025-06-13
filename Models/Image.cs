using System.ComponentModel.DataAnnotations;

namespace MarketingIT.Models
{
    public class Image
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }  

        // Foreign key to Post
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
