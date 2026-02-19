namespace SnowrunnerMerger.Shared.DTOs.Auth;

public record RefreshResponseDto : LoginResponseDto
{
    public string? RefreshToken { get; init; }
};