using MarketingIT.Areas.Identity.Data; // ApplicationUser
using MarketingIT.Models; // Post, Image
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MarketingIT.Data
{
    public class MarketingITDbContext : IdentityDbContext<ApplicationUser>
    {
        private ModelBuilder builder;

        public MarketingITDbContext(DbContextOptions<MarketingITDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventSubscription> EventSubscriptions { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
        .HasMany(p => p.Comments)
        .WithOne(c => c.Post)
        .HasForeignKey(c => c.PostId)
        .OnDelete(DeleteBehavior.Cascade);


            // Configure the FK relationship to Posts, for example:
            modelBuilder.Entity<Like>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the FK to ApplicationUser:
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany()    // or .WithMany(u => u.Likes) if you added that nav prop
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // (Any other configuration you need…)


            modelBuilder.Entity<Comment>()
                    .HasOne(c => c.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Restrict);
            

            // Prevent multiple cascade paths
            modelBuilder.Entity<EventSubscription>()
                .HasOne(es => es.Event)
                .WithMany(e => e.Subscriptions)
                .HasForeignKey(es => es.EventId)
                .OnDelete(DeleteBehavior.Restrict); // or .NoAction()

            modelBuilder.Entity<EventSubscription>()
                .HasOne(es => es.User)
                .WithMany()
                .HasForeignKey(es => es.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}










    


