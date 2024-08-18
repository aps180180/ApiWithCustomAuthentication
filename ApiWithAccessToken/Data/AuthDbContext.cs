using ApiWithAccessToken.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiWithAccessToken.Data
{
    public class AuthDbContext :DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options):base(options)
        {
                
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
