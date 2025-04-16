using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Talc.Models.Args;
using Talc.Models.Entities;

namespace Talc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller 
{
  private static User user = new();

  private readonly IConfiguration _configuration;

  public AuthController(IConfiguration configuration) 
  {
    _configuration = configuration;
  }

  [HttpPost("register")]
  public IActionResult Register(UserArgs args) {
    string hashedPassword = new PasswordHasher<User>()
      .HashPassword(user, args.Password);

    user.Email = args.Email;
    user.HashedPassword = hashedPassword;

    return Ok(user);
  }

  [HttpPost("login")]
  public IActionResult Login(UserArgs args) {
    if (user.Email != args.Email) 
    {
      return BadRequest("Login failed");
    }

    if (new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, args.Password)
      == PasswordVerificationResult.Failed)
    {
      return BadRequest("Login failed");
    }

    string token = CreateToken(user);

    return Ok(token);
  }

  private string CreateToken(User user)
  {
    List<Claim> claims = new()
    {
      new Claim(ClaimTypes.Name, user.Email)
    };

    SymmetricSecurityKey key = new (Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

    SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

    JwtSecurityToken tokenDescriptor = new(
      issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
      audience: _configuration.GetValue<string>("AppSettings:Audience"),
      claims: claims,
      expires: DateTime.UtcNow.AddHours(1),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
  }
}