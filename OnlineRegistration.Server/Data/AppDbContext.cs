using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Models;

namespace OnlineRegistration.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Users> Users { get; set; }
    }
}
