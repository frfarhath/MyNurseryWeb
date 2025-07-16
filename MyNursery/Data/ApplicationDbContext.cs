using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// Aliases to resolve ambiguous models
using NUADModels = MyNursery.Areas.NUAD.Models;
using MyNursery.Areas.Welcome.Models;
// using WelcomeModels = MyNursery.Areas.Welcome.Models; // Uncomment if you want Welcome ContactMessages too

namespace MyNursery.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// NUAD users table
        /// </summary>
        public new DbSet<NUADModels.User> Users { get; set; }

        /// <summary>
        /// NUAD blog posts
        /// </summary>
        public DbSet<NUADModels.BlogPost> BlogPosts { get; set; }

        /// <summary>
        /// NUAD contact messages
        /// </summary>
        public DbSet<NUADModels.ContactMessage> ContactMessages { get; set; }

        // If you want Welcome ContactMessages as well, uncomment this and add the using alias above
        // public DbSet<WelcomeModels.ContactMessage> WelcomeContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().HasData(
                new Microsoft.AspNetCore.Identity.IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new Microsoft.AspNetCore.Identity.IdentityRole { Id = "2", Name = "Staff", NormalizedName = "STAFF" },
                new Microsoft.AspNetCore.Identity.IdentityRole { Id = "3", Name = "Parent", NormalizedName = "PARENT" }
            );

            // Additional configurations can be added here if needed
        }
    }
}
