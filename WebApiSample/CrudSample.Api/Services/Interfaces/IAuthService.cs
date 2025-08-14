using CrudSample.Api.DTOs;

namespace CrudSample.Api.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequestDto dto);
    Task<(string UserId, string UserName, string Role)> ValidateLoginAsync(LoginRequestDto dto);
}
