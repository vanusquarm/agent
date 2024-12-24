using AuthSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Datasource;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    
}