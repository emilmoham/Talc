using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Talc.Models.DTOs.Args;
using Talc.Models.Entities;
using Talc.Repositories;

namespace Talc.Services;

public interface IAuthService {
  Task<User?> RegisterAsync(UserArgs args);
  Task<string?> LoginAsync(UserArgs args);
}

public class AuthService : IAuthService
{
  private readonly IConfiguration _configuration;
  private readonly PasswordHasher<User> _passwordHasher;
  private readonly UserRepository _userRepository;

  public AuthService(
    IConfiguration configuration,
    UserRepository userRepository
  ) {
    _configuration = configuration;
    _passwordHasher = new PasswordHasher<User>();
    _userRepository = userRepository;
  }

  public async Task<string?> LoginAsync(UserArgs args)
  {
    User? user = await _userRepository.GetUserAsync(args.Email);
    if (user == null) 
    {
      return null;
    }

    if (!VerifyPassword(user, args.Password))
    {
      return null;
    }

    return CreateToken(user);
  }

  public async Task<User?> RegisterAsync(UserArgs args)
  {
    if (await _userRepository.GetUserAsync(args.Email) != null)
      return null;
    
    User user = new User();
    user.Email = args.Email;
    user.HashedPassword = HashPassword(user, args.Password);
    
    user = await _userRepository.AddUserAsync(user);
    await _userRepository.SaveChangesAsync();

    return user;
  }

  private string CreateToken(User user)
  {
    List<Claim> claims = new()
    {
      new Claim(ClaimTypes.Name, user.Email),
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
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

  private string HashPassword(User user, string password) 
  {
    return _passwordHasher.HashPassword(user, password);
  }

  private bool VerifyPassword(User user, string password)
  {
    return _passwordHasher
      .VerifyHashedPassword(
        user, 
        user.HashedPassword, 
        password) == PasswordVerificationResult.Success;
  }
}