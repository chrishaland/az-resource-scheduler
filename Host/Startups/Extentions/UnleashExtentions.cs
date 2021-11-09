using System.Security.Claims;
using Unleash;

namespace Host;

public static class StartupUnleashExtentions
{
    private static string AppName => "Cloud Resource Scheduler";
    private static string ApiUrl(IConfiguration configuration) => configuration.GetValue<string>("unleash:apiUrl");
    private static string Environment(IConfiguration configuration) => configuration.GetValue<string>("unleash:environment") ?? "Development";
    private static string InstanceTag(IConfiguration configuration) => configuration.GetValue<string>("unleash:instanceTag");

    public static IServiceCollection AddUnleash(this IServiceCollection services, IConfiguration configuration)
    {
        var unleashApiUrl = configuration.GetValue<string>("unleash:apiUrl");
        if (!string.IsNullOrEmpty(unleashApiUrl))
        {
            services.AddHttpContextAccessor();
            services.AddTransient<UnleashContextProvider>();
            services.AddSingleton<IUnleash>(context => new DefaultUnleash(new UnleashSettings
            {
                AppName = AppName,
                Environment = Environment(configuration),
                InstanceTag = InstanceTag(configuration),
                UnleashApi = new Uri(ApiUrl(configuration)),
                UnleashContextProvider = context.GetRequiredService<UnleashContextProvider>()
            }));
        }
        else
        {
            services.AddSingleton<IUnleash>(new DefaultUnleash(new UnleashSettings()));
        }

        return services;
    }

    public static IApplicationBuilder UseUnleash(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            var claims = context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

            context.Items["UnleashContext"] = new UnleashContext
            {
                AppName = AppName,
                Environment = Environment(configuration),
                UserId = context.User?.Identity?.Name,
                RemoteAddress = context.Connection?.RemoteIpAddress?.ToString(),
                Properties = new Dictionary<string, string>()
                {
                        { "roles", claims.Any() ? claims.Aggregate((x,y) => x + "," + y) : string.Empty }
                }
            };
            await next.Invoke();
        });

        return app;
    }
}

public class UnleashContextProvider : IUnleashContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UnleashContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UnleashContext Context => (_httpContextAccessor.HttpContext?.Items["UnleashContext"] as UnleashContext) ?? new UnleashContext();
}
