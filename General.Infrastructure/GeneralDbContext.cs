using General.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace General.Infrastructure;
public class GeneralDbContext : DbContext
{
    // public GeneralDbContext() => Database.EnsureCreated();
    public DbSet<Account> Accounts { get; set; }
}