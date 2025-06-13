using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingIT.Areas.Identity.Data;
using MarketingIT.Data;
using MarketingIT.Models;
using MarketingIT.Models.ViewModels;

namespace MarketingIT.Controllers
{
    [Authorize(Roles = "ITCompany")]
    public class CompanyController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MarketingITDbContext _context;
       // private readonly IWebHostEnvironment _env;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(
            UserManager<ApplicationUser> userManager,
            MarketingITDbContext context,
             IWebHostEnvironment webHostEnvironment)
        //IWebHostEnvironment env)
        {
            _userManager = userManager;
            _context = context;
            //_env = env;
            _webHostEnvironment = webHostEnvironment;
        }

        // ───────────────────────────────────────────────
        // CREATE NEW POST (type-aware)
        // ───────────────────────────────────────────────

        // GET: /Company/CreatePost
        [HttpGet]
        public IActionResult CreatePost()
        {
            return View(new PostFormModel());
        }

        // POST: /Company/CreatePost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(PostFormModel form)
        {

            Console.WriteLine($"🔔 Entered CreatePost: Type={form.Type}, Text={form.Text?.Length}, Photo={(form.Photo == null ? "<null>" : form.Photo.FileName)}, Q={form.SurveyQuestion}, Opts={form.SurveyOptions}");
            var user = await _userManager.GetUserAsync(User);

             // 1) Type-specific validation
            if (form.Type == PostType.Post || form.Type == PostType.Article)
            {
                if (string.IsNullOrWhiteSpace(form.Text))
                    ModelState.AddModelError(nameof(form.Text), "Text is required.");
            }

            if (form.Type == PostType.Post && form.Photo == null)
            {
                ModelState.AddModelError(nameof(form.Photo), "An image is required.");
            }

            if (form.Type == PostType.Survey)
            {
                
                if (string.IsNullOrWhiteSpace(form.SurveyQuestion))
                    ModelState.AddModelError(nameof(form.SurveyQuestion), "Question is required.");

                bool anyOption =
                !string.IsNullOrWhiteSpace(form.SurveyOption1) ||
                !string.IsNullOrWhiteSpace(form.SurveyOption2) ||
                !string.IsNullOrWhiteSpace(form.SurveyOption3);


                
            }

            if (!ModelState.IsValid)
                return View(form);



            var post = new Post
            {
                UserId = user.Id,
                CreatedAt = DateTime.Now,
                Type = form.Type,
                Content = form.Text ?? "",       // for Post & Article
                SurveyQuestion = form.Type == PostType.Survey
                   ? form.SurveyQuestion : null,
                SurveyOptions = form.Type == PostType.Survey
                   ? string.Join("||", new[] { form.SurveyOption1, form.SurveyOption2, form.SurveyOption3 })
                   : null
            };

            if (form.Type == PostType.Post && form.Photo != null)
            {
                // ... save file, set post.ImageUrl ...
            }

            switch (form.Type)
            {
                case PostType.Post:
                    post.Content = form.Text;
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(form.Photo.FileName)}";
                    var savePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                    using (var stream = System.IO.File.Create(savePath))
                    {
                        await form.Photo.CopyToAsync(stream);
                    }
                    post.ImageUrl = "/uploads/" + fileName;
                    break;

                case PostType.Article:
                    post.Content = form.Text;
                    break;

                case PostType.Survey:
                    post.SurveyQuestion = form.SurveyQuestion;
                    var opts = new[]
                     {
                       form.SurveyOption1?.Trim(),
                       form.SurveyOption2?.Trim(),
                       form.SurveyOption3?.Trim()
                    }
                    .Where(o => !string.IsNullOrWhiteSpace(o));

                    post.SurveyOptions = string.Join("||", opts);
                    break;
            }

            // 3) Persist and redirect
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        // ───────────────────────────────────────────────
        // EDIT EXISTING POST
        // ───────────────────────────────────────────────


        // GET: /Company/EditPost/5

        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            var vm = new EditPostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                ExistingImageUrl = post.Images.FirstOrDefault()?.Url
            };
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(EditPostViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var post = await _context.Posts
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == vm.Id);
            if (post == null) return NotFound();

            // 1) always update text
            post.Content = vm.Content;

            // 2) if they uploaded a new photo, save it
            if (vm.Photo != null && vm.Photo.Length > 0)
            {
                // delete old file if you want…

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + "_" + Path.GetFileName(vm.Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await vm.Photo.CopyToAsync(stream);

                var newUrl = "/uploads/" + fileName;

                // either update existing image record or add a new one:
                var image = post.Images.FirstOrDefault();
                if (image != null)
                {
                    image.Url = newUrl;
                }
                else
                {
                    post.Images.Add(new Image { Url = newUrl, Post = post });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }


        // ───────────────────────────────────────────────
        // DELETE POST
        // ───────────────────────────────────────────────

        // POST: /Company/DeletePost/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            // 1) Load the post along with its comments *and* likes
            var post = await _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (post.UserId == user.Id)
                {
                    // 2) Remove any comments first
                    if (post.Comments?.Any() == true)
                        _context.Comments.RemoveRange(post.Comments);

                    // 3) Then remove any likes
                    if (post.Likes?.Any() == true)
                        _context.Likes.RemoveRange(post.Likes);

                    // 4) Finally delete the post
                    _context.Posts.Remove(post);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Dashboard));
        }



        // ───────────────────────────────────────────────
        // DASHBOARD
        // ───────────────────────────────────────────────

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var posts = await _context.Posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p => p.UserId == user.Id)
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }
    }
}

