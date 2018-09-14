using Microsoft.EntityFrameworkCore;
namespace server.Models
{
    public class DataContext : DbContext
    {
        
        public DataContext(DbContextOptions<DataContext> ConnectionStrings)
            : base(ConnectionStrings)
        {
        }

        public DbSet<User> AllUsers { get; set; }
    }
}