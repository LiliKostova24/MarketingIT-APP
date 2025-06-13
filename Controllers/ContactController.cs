using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MarketingIT.Models;              // wherever you put ContactViewModel
using Microsoft.AspNetCore.Identity.UI.Services; // for IEmailSender

namespace MarketingIT.Controllers
{
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IEmailSender _emailSender;

        public ContactController(
            ILogger<ContactController> logger,
            IEmailSender emailSender)          // register a concrete IEmailSender in Program.cs
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // If there’s a one-time success message, copy it into ViewBag
            if (TempData.TryGetValue("ContactSuccess", out var msg))
                ViewBag.Success = msg as string;

            return View("Contact", new ContactViewModel());
        }

        // POST /Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Contact", vm);

            // Log it
            _logger.LogInformation(
                "Contact form from {Name} <{Email}>: {Subject}\n{Message}",
                vm.Name, vm.Email, vm.Subject, vm.Message);

            // Send the email
            await _emailSender.SendEmailAsync(vm.Email,
                 $"Contact: {vm.Subject}",
                $"<p><strong>From:</strong> {vm.Name} ({vm.Email})</p><p>{vm.Message}</p>"
            );

            // Store a one-time success message
            TempData["ContactSuccess"] = "Thanks for reaching out! We’ll be in touch shortly.";

            // Redirect to GET so the form clears and the message appears
            return RedirectToAction(nameof(Index));
        }
    }

}
