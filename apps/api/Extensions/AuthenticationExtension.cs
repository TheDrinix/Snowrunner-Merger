using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SnowrunnerMerger.Api.Data;

namespace SnowrunnerMerger.Api.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection SetupAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                var jwtSecret = config.GetSection("Authentication:JwtSecret").Value;
        
                if (jwtSecret is null)
                {
                    throw new ArgumentNullException(nameof(jwtSecret));
                }
        
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                        .GetBytes(jwtSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                opt.IncludeErrorDetails = true;
            });
        
        return services;
    }
}