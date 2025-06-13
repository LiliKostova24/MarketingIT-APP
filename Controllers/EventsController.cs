using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MarketingIT.Areas.Identity.Data;
using MarketingIT.Data;
using Microsoft.EntityFrameworkCore;
using MarketingIT.Models;




public class EventsController : Controller
{
    private readonly MarketingITDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EventsController(MarketingITDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "ITCompany")]
    public async Task<IActionResult> MyEvents()
    {
        var user = await _userManager.GetUserAsync(User);

        var events = await _context.Events
            .Where(e => e.CompanyId == user.Id)
            .Include(e => e.Subscriptions)
            .ToListAsync();

        return View(events);
    }


    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ITCompany")]
    public async Task<IActionResult> Create(Event model)
    {
        // 1) Set the CompanyId from the logged-in user
        var user = await _userManager.GetUserAsync(User);
        model.CompanyId = user.Id;

        // 2) Remove the stale validation error for CompanyId
        ModelState.Remove(nameof(model.CompanyId));

        Console.WriteLine("➡️ POST /Events/Create hit");
        Console.WriteLine($"ModelState.IsValid = {ModelState.IsValid}");

        // 3) Now ModelState.IsValid will include only real form errors
        if (!ModelState.IsValid)
        {
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"❌ {entry.Key}: {error.ErrorMessage}");
                }
            }
            return View(model);
        }

        // 4) Save and redirect
        _context.Events.Add(model);
        await _context.SaveChangesAsync();
        Console.WriteLine($"✅ Saved Event ID = {model.Id}, Title = {model.Title}");
        return RedirectToAction("MyEvents");
    }



    [Authorize(Roles = "ITCompany")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var ev = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == user.Id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    [Authorize(Roles = "ITCompany")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Event model)
    {
        if (id != model.Id) return BadRequest();

        var user = await _userManager.GetUserAsync(User);
        var ev = await _context.Events.FindAsync(id);
        if (ev == null || ev.CompanyId != user.Id)
            return Forbid();

        // drop the required‐field error again since CompanyId wasn't in the form
        ModelState.Remove(nameof(model.CompanyId));
        if (!ModelState.IsValid) return View(ev);

        // _context.Events.Update(model);


        ev.Title = model.Title;
        ev.Date = model.Date;
        ev.Location = model.Location;
        ev.Description = model.Description;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MyEvents));
    }

    // ──────────────────────────────────────────────────
    // DELETE
    // ──────────────────────────────────────────────────

    [Authorize(Roles = "ITCompany")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var ev = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == user.Id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    [Authorize(Roles = "ITCompany")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var ev = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == user.Id);
        if (ev == null) return NotFound();

        var subs = _context.EventSubscriptions
        .Where(s => s.EventId == ev.Id);
        _context.EventSubscriptions.RemoveRange(subs);

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MyEvents));
    }

    // ──────────────────────────────────────────────────
    // Public Browse & Subscriptions
    // ──────────────────────────────────────────────────



    [AllowAnonymous] // or [Authorize] if only for logged-in users
    public async Task<IActionResult> Browse()
    {
        var events = await _context.Events
            .Include(e => e.Company)
            .Include(e => e.Subscriptions)
            .OrderBy(e => e.Date)
            .ToListAsync();

        return View(events);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ToggleSubscription(int eventId)
    {
        var user = await _userManager.GetUserAsync(User);

        var existing = await _context.EventSubscriptions
            .FirstOrDefaultAsync(s => s.EventId == eventId && s.UserId == user.Id);

        if (existing != null)
        {
            _context.EventSubscriptions.Remove(existing);
        }
        else
        {
            _context.EventSubscriptions.Add(new EventSubscription
            {
                EventId = eventId,
                UserId = user.Id
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Browse");
    }



}