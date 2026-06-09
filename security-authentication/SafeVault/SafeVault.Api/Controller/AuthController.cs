using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using SafeVault.Api.Data;
using SafeVault.Api.Models;

namespace SafeVault.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new User
        {
            Username = request.Username,
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        _db.Users.Add(user);

        _db.SaveChanges();

        return Ok();
    }

    [HttpGet("user/{username}")]
    public IActionResult GetUser(string username)
    {
        var user = _db.Users
            .FirstOrDefault(u => u.Username == username);

        if (user == null)
            return NotFound();

        return Ok(user.Username);
    }
}