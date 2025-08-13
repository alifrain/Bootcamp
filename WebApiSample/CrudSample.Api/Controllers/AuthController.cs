using Microsoft.AspNetCore.Mvc;
using CrudSample.Api.Auth;
using CrudSample.Api.DTOs;

namespace CrudSample.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokens;

    // Demo user store (jangan dipakai di production)
    private static readonly Dictionary<string,(string pwd, string role, string id)> _users = new()
    {
        // username -> (password, role, userId)
        ["admin"] = ("admin123", "Admin", "1"),
        ["user"]  = ("user123",  "User",  "2")
    };

    public AuthController(IJwtTokenService tokens) => _tokens = tokens;

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public ActionResult<AuthResponseDto> Login(LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return Unauthorized();

        if (!_users.TryGetValue(dto.Username, out var info))
            return Unauthorized();

        if (info.pwd != dto.Password) // ganti dengan hashing kalau serius
            return Unauthorized();

        var result = _tokens.CreateToken(info.id, dto.Username, info.role);
        return Ok(new AuthResponseDto(result.AccessToken, "Bearer", result.ExpiresAtUtc));
    }
}
