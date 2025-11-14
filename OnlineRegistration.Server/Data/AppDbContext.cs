
using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Models;

namespace OnlineRegistration.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UsersEmployee> EmployeeUsers { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<BiometricDataEnrollment> BiometricEnrollments { get; set; }
        public DbSet<BypassLog> BypassLogs { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<CitizenVerificationLog>VerificationLogs { get; set; }



    }
}
