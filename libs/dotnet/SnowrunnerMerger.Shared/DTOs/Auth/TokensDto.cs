namespace SnowrunnerMerger.Shared.DTOs.Auth;

public record TokensDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);