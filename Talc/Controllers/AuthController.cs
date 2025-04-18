using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    string? token = await _authService.LoginAsync(args);
    
    if (token == null) 
      return BadRequest("Invalid email or password");

    return Ok(token);
  }

  [Authorize]
  [HttpGet("IsAlive")]
  public IActionResult IsAliveAuthenticated() 
  {
    return Ok("You are authenticated");
  }
}