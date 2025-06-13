using MarketingIT.Areas.Identity.Data;

namespace MarketingIT.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public PostType Type { get; set; } = PostType.Post;

        
        public string? SurveyQuestion { get; set; }
        public string? SurveyOptions { get; set; }

        
        public string? ImageUrl { get; set; }   
        public List<Image> Images { get; set; } = new();

       
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public List<Comment> Comments { get; set; } = new();
    }
}

