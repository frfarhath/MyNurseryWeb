using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNursery.Areas.NUAD.Models;          
using MyNursery.Areas.Welcome.Models;

namespace MyNursery.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Add your app-specific DbSets here
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<MyNursery.Areas.NUAD.Models.ContactMessage> ContactMessages { get; set; }

        public DbSet<MyNursery.Areas.NUAD.Models.User> Users { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "Parent", NormalizedName = "PARENT" },
                new IdentityRole { Name = "Staff", NormalizedName = "STAFF" }
            );
        }
    }
}
