using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talc.Models.DTOs;
using Talc.Models.DTOs.Args;
using Talc.Models.Entities;
using Talc.Services;

namespace Talc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller 
{
  private readonly IAuthService _authService;

  public AuthController(
    IAuthService authService
  ) {
    _authService = authService;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(UserArgs args) {
    User? user = await _authService.RegisterAsync(args);
    if (user == null) 
    {
      return BadRequest("User already exists");
    }
    return Ok(user);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(UserArgs args) {
    TokenResponse? tokenResponse = await _authService.LoginAsync(args);
    
    if (tokenResponse == null) 
      return BadRequest("Invalid email or password");

    return Ok(tokenResponse);
  }

  [HttpPost("refresh-token")]
  public async Task<IActionResult> RefreshToken(RefreshTokenArgs args)
  {
    TokenResponse? result = await _authService.RefreshTokensAsync(args);
    if (result == null || result.AccessToken == null || result.RefreshToken == null)
      return Unauthorized("Invalid refresh token");

    return Ok(result);
  }

  [Authorize]
  [HttpGet("IsAlive")]
  public IActionResult IsAliveAuthenticated() 
  {
    return Ok("You are authenticated");
  }
}