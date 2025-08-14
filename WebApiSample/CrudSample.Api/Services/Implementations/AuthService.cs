using BCrypt.Net;
using CrudSample.Api.DTOs;
using CrudSample.Api.Models;
using CrudSample.Api.Repositories.Interfaces;
using CrudSample.Api.Repositories.Implementations;
using CrudSample.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrudSample.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IRepository<AuthUser> _users;
    private readonly IUnitOfWork _uow;

    public AuthService(IRepository<AuthUser> users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task RegisterAsync(RegisterRequestDto dto)
    {
        var uname = dto.Username.Trim();
        var exists = await _users.Query().AnyAsync(u => u.UserName.ToLower() == uname.ToLower());
        if (exists) throw new InvalidOperationException("Username already exists.");

        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new AuthUser { UserName = uname, PasswordHash = hash, Role = dto.Role?.Trim() ?? "User" };

        await _users.AddAsync(user);
        await _uow.SaveChangesAsync();
    }

    public async Task<(string UserId, string UserName, string Role)> ValidateLoginAsync(LoginRequestDto dto)
    {
        var uname = dto.Username.Trim();
        var user = await _users.Query().FirstOrDefaultAsync(u => u.UserName.ToLower() == uname.ToLower());
        if (user is null) throw new UnauthorizedAccessException("Invalid credentials.");

        var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!ok) throw new UnauthorizedAccessException("Invalid credentials.");

        return (user.Id.ToString(), user.UserName, user.Role);
    }
}
