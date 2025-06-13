// File: Services/NoOpEmailSender.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MarketingIT.Services
{
    /// <summary>
    /// A do-nothing email sender for development/testing.
    /// </summary>
    public class NoOpEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Intentionally do nothing
            return Task.CompletedTask;
        }
    }
}
