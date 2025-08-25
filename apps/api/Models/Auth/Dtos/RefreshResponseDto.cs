namespace SnowrunnerMerger.Api.Models.Auth.Dtos;

public record RefreshResponseDto : LoginResponseDto
{
    public string? RefreshToken { get; init; }
};