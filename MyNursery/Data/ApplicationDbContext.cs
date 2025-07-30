using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNursery.Areas.NUAD.Models;
using MyNursery.Areas.NUSAD.Models;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Models;
// Aliases to resolve ambiguous models
using NUADModels = MyNursery.Areas.NUAD.Models;
using NUSADModels = MyNursery.Areas.NUSAD.Models;

namespace MyNursery.Data
{
    // Use ApplicationUser and ApplicationRole for Identity
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Custom Users table (non-identity users)
        public new DbSet<User> Users { get; set; }

        // Blog posts
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogImage> BlogImages { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }



        // Contact messages
        public DbSet<NUADModels.ContactMessage> ContactMessages { get; set; }

        // Content management tables
        public DbSet<Page> Pages { get; set; }
        public DbSet<CurriculumItem> CurriculumItems { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles with Ids and optionally Description if your ApplicationRole supports it
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN", Description = "Administrator role" },
                new ApplicationRole { Id = "2", Name = "Staff", NormalizedName = "STAFF", Description = "Staff role" },
                new ApplicationRole { Id = "3", Name = "Parent", NormalizedName = "PARENT", Description = "Parent role" }
            );

            // Configure relationship: Page.LastUpdatedByUser (ApplicationUser)
            builder.Entity<Page>()
                .HasOne(p => p.LastUpdatedByUser)
                .WithMany() // Adjust if ApplicationUser has navigation property for Pages
                .HasForeignKey(p => p.LastUpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
