namespace SnowrunnerMerger.Api.Models.Auth.Dtos;

public record TokenRequestDto(
    string ClientId,
    string GrantType,
    string Code,
    string CodeVerifier
);