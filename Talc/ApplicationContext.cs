using Microsoft.EntityFrameworkCore;
using Talc.Models.Entities;

namespace Talc;

public class ApplicationContext : DbContext {
  
  public DbSet<Session> Sessions { get; set; }
  public DbSet<User> Users { get; set; }

  public ApplicationContext(DbContextOptions<ApplicationContext> options)
    : base(options) {}

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    new SessionTypeConfiguration().Configure(modelBuilder.Entity<Session>());
    new UserTypeConfiguration().Configure(modelBuilder.Entity<User>());
  }
}