using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MarketingIT.Areas.Identity.Data;

namespace MarketingIT.Helpers
{
    public class UserCreationExample
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserCreationExample(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(string userName, string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);
            return result;
        }
    }
}
