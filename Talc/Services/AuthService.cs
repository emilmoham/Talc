using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Talc.Models.DTOs.Args;
using Talc.Models.Entities;
using Talc.Repositories;
using System.Security.Cryptography;
using Talc.Models.DTOs;

namespace Talc.Services;

public interface IAuthService {
  Task<TokenResponse?> LoginAsync(UserArgs args);
  Task<TokenResponse?> RefreshTokensAsync(RefreshTokenArgs args);
  Task<User?> RegisterAsync(UserArgs args);
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

  public async Task<TokenResponse?> LoginAsync(UserArgs args)
  {
    User? user = await _userRepository.GetUserByEmailAsync(args.Email);
    if (user == null)
    {
      return null;
    }

    if (!VerifyPassword(user, args.Password))
    {
      return null;
    }

    return await CreateTokenResponse(user);
  }

  public async Task<TokenResponse?> RefreshTokensAsync(RefreshTokenArgs args) 
  {
    User? user = await ValidateRefreshTokenAsync(args.UserId, args.RefreshToken);

    if (user == null) 
      return null;

    return await CreateTokenResponse(user);
  }

  public async Task<User?> RegisterAsync(UserArgs args)
  {
    if (await _userRepository.GetUserByEmailAsync(args.Email) != null)
      return null;
    
    User user = new User();
    user.Email = args.Email;
    user.HashedPassword = HashPassword(user, args.Password);
    
    user = _userRepository.AddUser(user);
    await _userRepository.SaveChangesAsync();

    return user;
  }

  private string CreateJwt(User user)
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

  private async Task<TokenResponse?> CreateTokenResponse(User user)
  {
    string accessToken = CreateJwt(user);
    string refreshToken = await GenerateAndSaveRefreshTokenAsync(user);

    return new()
    {
      AccessToken = accessToken,
      RefreshToken = refreshToken
    };
  }

  private string GenerateRefreshToken()
  {
    byte[] randomNumber = new byte[32];

    using RandomNumberGenerator rng = RandomNumberGenerator.Create();
    
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
  }

  private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
  {
    string refreshToken = GenerateRefreshToken();
    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
    _userRepository.UpdateUser(user);
    await _userRepository.SaveChangesAsync();
    return refreshToken;
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

  private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
  {
    User? user = await _userRepository.GetUserByIdAsync(userId);

    if (user == null 
      || user.RefreshToken != refreshToken 
      || user.RefreshTokenExpiresAt <= DateTimeOffset.UtcNow)
      return null;

    return user;
  }
}