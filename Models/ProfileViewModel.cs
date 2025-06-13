using MarketingIT.Areas.Identity.Data;

namespace MarketingIT.Models
{
    public class ProfileViewModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IList<Post> Posts { get; set; }
        
        
        public ApplicationUser User { get; set; }

        public string? AvatarPath { get; set; }   




    }
}
