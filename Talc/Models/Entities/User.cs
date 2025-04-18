using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Talc.Models.Entities;

public class User 
{
  public Guid Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public string HashedPassword { get; set; } = string.Empty;
  public string? RefreshToken { get; set; }
  public DateTimeOffset? RefreshTokenExpiresAt { get ; set; }
  
}

public class UserTypeConfiguration : IEntityTypeConfiguration<User> 
{
  public void Configure(EntityTypeBuilder<User> entity) 
  {
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Email).IsRequired();
    entity.Property(e => e.HashedPassword).IsRequired();
  }
}