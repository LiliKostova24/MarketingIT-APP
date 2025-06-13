using MarketingIT.Areas.Identity.Data;

namespace MarketingIT.Models
{
    public class EventSubscription
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
