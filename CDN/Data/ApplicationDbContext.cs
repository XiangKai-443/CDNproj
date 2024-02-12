namespace CDN.Data
{
    using CDN.Model;
    using Microsoft.EntityFrameworkCore;
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
    }
}
