using Microsoft.AspNetCore.Mvc;
using CrudSample.Api.Auth;
using CrudSample.Api.DTOs;
using CrudSample.Api.Services.Interfaces;

namespace CrudSample.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokens;
    private readonly IAuthService _auth;

    public AuthController(IJwtTokenService tokens, IAuthService auth)
    {
        _tokens = tokens;
        _auth = auth;
    }

    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        await _auth.RegisterAsync(dto);
        return StatusCode(201);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
    {
        var (id, name, role) = await _auth.ValidateLoginAsync(dto);
        var result = _tokens.CreateToken(id, name, role);
        return Ok(new AuthResponseDto(result.AccessToken, "Bearer", result.ExpiresAtUtc));
    }
}
