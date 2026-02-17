namespace SnowrunnerMerger.Api.Models.Auth.Dtos;

public record TokensDto(
    string AccessToken,
    string RefreshToken,
    int AccessTokenExpiresIn,
    int RefreshTokenExpiresIn
);