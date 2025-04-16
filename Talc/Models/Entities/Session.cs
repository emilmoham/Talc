using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Talc.Models.Entities;

public class Session {
  public int Id { get; set; }
  public Guid UserId { get; set; }
  public string Token { get; set; }
  public string RefreshToken { get; set; }
  public DateTimeOffset Expires { get;  set; }
}

public class SessionTypeConfiguration : IEntityTypeConfiguration<Session>
{
  public void Configure(EntityTypeBuilder<Session> entity)
  {
    entity.HasKey(t => t.Id);
    entity.Property(t => t.Id)
      .ValueGeneratedOnAdd();
  }
}