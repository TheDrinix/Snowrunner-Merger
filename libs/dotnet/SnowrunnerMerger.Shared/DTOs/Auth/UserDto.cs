namespace SnowrunnerMerger.Shared.DTOs.Auth;

public record UserDto(Guid Id, string Username, string Email, DateTime CreatedAt);