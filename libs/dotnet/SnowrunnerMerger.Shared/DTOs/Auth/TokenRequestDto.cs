namespace SnowrunnerMerger.Shared.DTOs.Auth;

public record TokenRequestDto(
    string ClientId,
    string GrantType,
    string Code,
    string CodeVerifier
);