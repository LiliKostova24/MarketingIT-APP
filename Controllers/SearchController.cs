using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingIT.Data;
using MarketingIT.Models;
using System.Linq;
using System.Threading.Tasks;

public class SearchController : Controller
{
    private readonly MarketingITDbContext _context;

    public SearchController(MarketingITDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string query)
    {
        var model = new SearchResultsViewModel
        {
            Query = query,
            Users = new List<MarketingIT.Areas.Identity.Data.ApplicationUser>(),
            Posts = new List<Post>()
        };

        if (!string.IsNullOrWhiteSpace(query))
        {
            model.Users = await _context.Users
                .Where(u =>
                    u.FirstName.Contains(query) ||
                    u.LastName.Contains(query) ||
                    u.UserName.Contains(query))
                .ToListAsync();

            model.Posts = await _context.Posts
                .Include(p => p.User)
                .Where(p =>
                    p.Content.Contains(query) ||
                    p.User.FirstName.Contains(query) ||
                    p.User.LastName.Contains(query))
                .ToListAsync();
        }

        return View(model);
    }
}
