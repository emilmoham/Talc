using Microsoft.AspNetCore.Mvc;

namespace Talc;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller 
{
  public AuthController() {}

  [HttpGet("IsAlive")]
  public IActionResult IsAlive() {
    return Ok("Is Alive");
  }
}