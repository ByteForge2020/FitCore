using Authentication.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Data
{
    public class AuthenticationDbContext : IdentityDbContext
    {
        public AuthenticationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<FitCoreUser> FitCoreUsers  { get; set; }
    }
}