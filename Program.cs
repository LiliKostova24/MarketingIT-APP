using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketingIT.Data;
using MarketingIT.Areas.Identity.Data;
using Microsoft.Extensions.FileProviders;
using MarketingIT.Models;
using Microsoft.AspNetCore.Identity.UI.Services;                              
using MarketingIT.Services;



public class Program
{
    public static async Task Main(string[] args)
    {


        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<EmailSettings>(
        builder.Configuration.GetSection("EmailSettings"));

        var connectionString = builder.Configuration.GetConnectionString("MarketingITDbContextConnection") ?? throw new InvalidOperationException("Connection string 'MarketingITDbContextConnection' not found.");

        builder.Services.AddDbContext<MarketingITDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MarketingITDbContext>();

        // Add services to the container.
        // Add services to the container and disable auto-[Required] on non-nullable refs
        builder.Services
                       .AddControllersWithViews(options =>
                       {
                               // Don’t treat non-nullable reference types as if they had [Required]
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                           })
             .AddRazorRuntimeCompilation();

        // builder.Services.AddTransient < Microsoft.AspNetCore.Identity.UI.Services.IEmailSender,
        // MarketingIT.Services.SmtpEmailSender > ();

        //── Email sender registration ─────────────────────────────────────────────
        if (builder.Environment.IsDevelopment())
        {
            // In Development: use a no-op email sender so SMTP lookup never runs
            builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();
        }
        else
        {
            // In non-Development (e.g. Production): use the real SMTP sender
            builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
        }
        

        builder.Services.AddRazorPages();
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireUppercase = false;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        
        app.UseRouting();
        app.UseAuthentication(); ;

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "User", "ITCompany" };

            foreach (var role in roles)
           {
                if (!await roleManager.RoleExistsAsync(role))
                   await roleManager.CreateAsync(new IdentityRole(role));
           }
        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminUser = await userManager.FindByEmailAsync("petrova.01@gmail.com");

            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }


        app.Run();
    }
}

