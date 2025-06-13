using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MarketingIT.Areas.Identity.Data;    // For ApplicationUser
using MarketingIT.Data;                   // For MarketingITDbContext (your EF context)
using MarketingIT.Models;                 // For ProfileViewModel, PostViewModel, EditProfileViewModel
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MarketingIT.Controllers    // Note: "MarketingIT" (capital I and T)
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MarketingITDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            MarketingITDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Profile/{username}
        [HttpGet("/Profile/{username}")]
        public async Task<IActionResult> Index(string username)
        {
            if (string.IsNullOrEmpty(username))
                return NotFound();

            // 1) Find the user by UserName
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            // 2) Load this user’s posts from the correct DbContext
            var userPosts = await _context.Posts
        .Where(p => p.UserId == user.Id)
        .Include(p => p.Images)            // ← include Images
        .Include(p => p.Likes)             // ← if you also need Likes count
        .Include(p => p.Comments)          // ← if you show Comments here
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();


            // 3) Build a ProfileViewModel
            var model = new ProfileViewModel
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AvatarPath = user.AvatarPath,
                Posts = userPosts
            };

            return View(model);
        }

        // GET: /Profile/Edit
        [HttpGet("/Profile/Edit")]
        public async Task<IActionResult> Edit()

        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ExistingAvatarPath = user.AvatarPath
            };

            return View(model);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            if (user.Email != model.Email)
            {
                var emailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!emailResult.Succeeded)
                {
                    foreach (var error in emailResult.Errors)
                        ModelState.AddModelError("Email", error.Description);
                    return View(model);
                }
            }

            if (model.Avatar != null && model.Avatar.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "avatars");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Avatar.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await model.Avatar.CopyToAsync(fs);
                }

                // Delete old avatar if it exists
                if (!string.IsNullOrEmpty(user.AvatarPath))
                {
                    var oldAvatarFullPath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        user.AvatarPath.TrimStart('/'));

                    if (System.IO.File.Exists(oldAvatarFullPath))
                    {
                        System.IO.File.Delete(oldAvatarFullPath);
                    }
                }

                user.AvatarPath = $"/avatars/{uniqueFileName}";
                model.ExistingAvatarPath = user.AvatarPath; // ✅ Reflect change in ViewModel
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                return RedirectToAction("Index", new { username = user.UserName });
            }

            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

    }
}



