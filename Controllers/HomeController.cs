using MarketingIT.Models;
using MarketingIT.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using MarketingIT.Data;
using MarketingIT.Models.ViewModels;
using System;

namespace MarketingIT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MarketingITDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(
            ILogger<HomeController> logger,
            MarketingITDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.Likes)                   // ← load the Likes
                .ThenInclude(l => l.User)           // ← optionally load who liked
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();


            var events = await _context.Events
                   .Where(e => e.Date >= DateTime.Now)
                   .OrderBy(e => e.Date)
                   .ToListAsync();

            var model = new HomeViewModel
            {
                PostForm = new PostFormModel(),

                Posts = posts,
                UpcomingEvents = events
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostContent(HomeViewModel model)
        {

            if (model.PostForm.Type != PostType.Survey)
            {
                ModelState.Remove("PostForm.SurveyQuestion");
                ModelState.Remove("PostForm.SurveyOption1");
                ModelState.Remove("PostForm.SurveyOption2");
                ModelState.Remove("PostForm.SurveyOption3");
            }

            // validation for the Photo field if they didn’t choose the Post form
            if (model.PostForm.Type != PostType.Post)
            {
                ModelState.Remove("PostForm.Photo");
            }

            if (!ModelState.IsValid)
            {
                model.UpcomingEvents = await _context.Events
                     .Where(e => e.Date > DateTime.UtcNow)
                     .Include(e => e.Company)
                     .OrderBy(e => e.Date)
                     .ToListAsync();

                model.Posts = await _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Images)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index");

            var post = new Post
               {
                        Type = model.PostForm.Type,
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        Images = new List<Image>()
               };
            
              if (post.Type == PostType.Post || post.Type == PostType.Article)
                   {
                      // Posts & Articles both use the Text field
                post.Content = model.PostForm.Text;
                   }
               else if (post.Type == PostType.Survey)
                  {
                       // Surveys get a question + three options
                     post.SurveyQuestion = model.PostForm.SurveyQuestion;
                     post.SurveyOptions = string.Join("||", new[] {
                     model.PostForm.SurveyOption1,
                     model.PostForm.SurveyOption2,
                     model.PostForm.SurveyOption3
                   });
                   }

            if (model.PostForm.Photo != null && model.PostForm.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.PostForm.Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PostForm.Photo.CopyToAsync(stream);
                }

                // ✅ THIS IS CRITICAL
                var image = new Image
                {
                    Url = "/uploads/" + fileName,
                    Post = post // associate the image with the post
                };

                _context.Images.Add(image);    // add image to DB context
                post.Images.Add(image);        // link image to post
            }

            _context.Posts.Add(post);         // add post to DB context
            await _context.SaveChangesAsync(); // 💾 SAVE post + image

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostComment(CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var comment = new Comment
            {
                Text = model.Text,
                PostId = model.PostId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == user.Id);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike); // unlike
            }
            else
            {
                _context.Likes.Add(new Like { PostId = postId, UserId = user.Id });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
