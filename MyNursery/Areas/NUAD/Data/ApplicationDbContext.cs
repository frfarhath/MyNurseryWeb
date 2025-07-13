using Microsoft.EntityFrameworkCore;
using MyNursery.Areas.NUAD.Models;


namespace MyNursery.Areas.NUAD.Data

{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}
