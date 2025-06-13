using MarketingIT.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace MarketingIT.Models
{
    public class HomeViewModel
    {
        public PostFormModel PostForm { get; set; }

        [BindNever]
        public List<Post>? Posts { get; set; } = new();

        [BindNever]
        public List<Event>? UpcomingEvents { get; set; } = new();



    }
}
