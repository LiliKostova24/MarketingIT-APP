using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using MarketingIT.Models;

namespace MarketingIT.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser

    {
        [PersonalData]
        [Column(TypeName = "nvarchar(200)")]
        public string? AvatarPath { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}