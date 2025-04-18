namespace Talc.Models.DTOs.Args;

public class RefreshTokenArgs {
  public Guid UserId { get; set; }
  public string RefreshToken { get; set; }
}