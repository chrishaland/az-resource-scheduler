using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Unleash;

namespace Host
{
    public static class StartupUnleashExtentions
    {
        private static string AppName => "Azure Resource Scheduler";
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

                context.Items["UnleashContext"] = new UnleashContext
                {
                    AppName = AppName,
                    Environment = Environment(configuration),
                    UserId = context.User?.Identity?.Name,
                    RemoteAddress = context.Connection?.RemoteIpAddress?.ToString(),
                    Properties = new Dictionary<string, string>()
                    {
                        { "roles", context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value).Aggregate((x,y) => x + "," + y) }
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

        public UnleashContext Context => _httpContextAccessor.HttpContext.Items["UnleashContext"] as UnleashContext;
    }
}
