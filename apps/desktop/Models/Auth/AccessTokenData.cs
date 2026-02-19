using System;

namespace SnowrunnerMerger.Desktop.Models.Auth;

public record AccessTokenData(
    string Token,
    DateTime ExpiresAt
);