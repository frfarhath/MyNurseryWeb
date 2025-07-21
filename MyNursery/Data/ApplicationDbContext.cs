using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNursery.Areas.NUAD.Models;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Models;
// Aliases to resolve ambiguous models
using NUADModels = MyNursery.Areas.NUAD.Models;

namespace MyNursery.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Identity Users table handled by base IdentityDbContext<ApplicationUser>

        // Custom Users table (non-identity user)
        public new DbSet<User> Users { get; set; }

        // Blog posts
        public DbSet<BlogPost> BlogPosts { get; set; }

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

            // Seed roles
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().HasData(
                new Microsoft.AspNetCore.Identity.IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new Microsoft.AspNetCore.Identity.IdentityRole { Id = "2", Name = "Staff", NormalizedName = "STAFF" },
                new Microsoft.AspNetCore.Identity.IdentityRole { Id = "3", Name = "Parent", NormalizedName = "PARENT" }
            );

            // Configure relationship: Page.LastUpdatedByUser
            builder.Entity<Page>()
                .HasOne(p => p.LastUpdatedByUser)
                .WithMany() // Assuming ApplicationUser has no Pages collection, adjust if otherwise
                .HasForeignKey(p => p.LastUpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
