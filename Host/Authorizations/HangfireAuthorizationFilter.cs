using Hangfire.Dashboard;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Host.Authorizations;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private const string HangFireCookieName = ".hangfire";

    private readonly JwtSecurityTokenHandler _tokenHandler;

    public HangfireAuthorizationFilter()
    {
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();

        var token = string.Empty;
        var setCookie = false;

        if (httpContext.Request.Query.ContainsKey("token"))
        {
            token = httpContext.Request.Query["token"].FirstOrDefault();
            setCookie = true;
        }
        else
        {
            token = httpContext.Request.Cookies[HangFireCookieName];
        }

        if (string.IsNullOrEmpty(token)) return false;

        try
        {
            var tokenValidationParameters = TokenValidationParameters(httpContext, configuration).GetAwaiter().GetResult();
            var claims = _tokenHandler.ValidateToken(token, tokenValidationParameters, out var _);

            var adminRole = claims.FindFirst(c =>
                c.Type == ClaimTypes.Role &&
                c.Value == configuration.GetValue<string>("oidc:roles:admin")
            );

            if (adminRole == null) return false;
        }
        catch
        {
            return false;
        }

        if (setCookie)
        {
            httpContext.Response.Cookies.Append(
                key: HangFireCookieName,
                value: token,
                options: new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30)
                }
            );
        }

        return true;
    }

    private static async Task<TokenValidationParameters> TokenValidationParameters(HttpContext httpContext, IConfiguration configuration)
    {
        var clientFactory = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>();

        var client = clientFactory.CreateClient();
        var authorityUrl = configuration.GetValue<string>("oidc:authorityUri");

        var discoveryDocument = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = authorityUrl,
            Policy =
                {
                    ValidateIssuerName = true,
                    ValidateEndpoints = true,
                    AdditionalEndpointBaseAddresses = !authorityUrl.StartsWith("https://login.microsoftonline.com") ? new HashSet<string>() : new HashSet<string>
                    {
                        authorityUrl[..authorityUrl.LastIndexOf('/')],
                        "https://graph.microsoft.com/oidc/userinfo"
                    }
                }
        });

        var keys = new List<SecurityKey>();
        foreach (var key in discoveryDocument.KeySet.Keys)
        {
            var e = Base64UrlEncoder.DecodeBytes(key.E);
            var n = Base64UrlEncoder.DecodeBytes(key.N);

            var rsa = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
            {
                KeyId = key.Kid
            };

            keys.Add(rsa);
        }

        return new TokenValidationParameters
        {
            ValidIssuer = discoveryDocument.Issuer,
            ValidAudience = configuration.GetValue<string>("oidc:audience"),
            IssuerSigningKeys = keys,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            RequireSignedTokens = true,
            NameClaimType = "name",
            RoleClaimType = "roles"
        };
    }
}
