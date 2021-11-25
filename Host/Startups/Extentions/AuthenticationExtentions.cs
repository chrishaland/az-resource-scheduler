using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Host;

public static class AuthenticationExtentions
{
    public static IServiceCollection AddOpenIdConnectAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authorityUri = configuration.GetValue<string>("oidc:authorityUri");
        if (string.IsNullOrEmpty(authorityUri)) return services;

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Audience = configuration.GetValue<string>("oidc:audience");
            options.Authority = authorityUri;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateLifetime = true,
                RequireSignedTokens = true,
                NameClaimType = configuration.GetValueOrDefault("oidc:claim_types:name", "name"),
                RoleClaimType = configuration.GetValueOrDefault("oidc:claim_types:role", "roles")
            };
        });

        return services;
    }
}
