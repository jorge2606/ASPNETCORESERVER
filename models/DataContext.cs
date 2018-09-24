using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace server.Models
{
    public class DataContext : IdentityDbContext<User>
    {
        
        public DataContext(DbContextOptions<DataContext> ConnectionStrings)
            : base(ConnectionStrings)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<User> AllUsers { get; set; }
    }

    public class AppIdentityRole : IdentityRole
    {

    }
}