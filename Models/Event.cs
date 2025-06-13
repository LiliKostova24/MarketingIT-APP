using MarketingIT.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace MarketingIT.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string? Location { get; set; }

        public string CompanyId { get; set; } // ITCompany who created the event

        [ValidateNever]
        public ApplicationUser Company { get; set; }

        public List<EventSubscription>? Subscriptions { get; set; }


    }
}
