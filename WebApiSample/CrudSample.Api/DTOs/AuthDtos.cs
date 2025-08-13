namespace CrudSample.Api.DTOs;

public record LoginRequestDto(string Username, string Password);
public record AuthResponseDto(string AccessToken, string TokenType, DateTime ExpiresAtUtc);
