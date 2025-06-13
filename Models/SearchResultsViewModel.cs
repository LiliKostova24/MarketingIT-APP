using System.Collections.Generic;
using MarketingIT.Areas.Identity.Data;
using MarketingIT.Models;

namespace MarketingIT.Models
{
    public class SearchResultsViewModel
    {
        public string Query { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<Post> Posts { get; set; }
    }
}
