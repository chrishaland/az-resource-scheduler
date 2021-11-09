using Haland.DotNetTrace;
using Hangfire;
using Libraries.Providers;
using Service.Jobs;
using Service.Providers;
using Service.Resources;

namespace Host;

public partial class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabase(Configuration);
        services.AddOpenApiDocumentation();

        services.AddAuthorizationAndPolicies(Configuration);
        services.AddOpenIdConnectAuthentication(Configuration);

        services.AddUnleash(Configuration);

        services.AddTransient<StopResourceJob>();
        services.AddTransient<StartResourceJob>();
        services.AddTransient<StartEnvironmentJob>();
        services.AddTransient<UpsertAzureProviderHandler>();
        services.AddTransient<UpsertVirtualMachineHandler>();
        services.AddTransient<UpsertVirtualMachineScaleSetHandler>();

        services.AddTracing();
        services.AddControllers().AddNewtonsoftJson();

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
        });

        services.AddTransient<AzureResourceHandler>();

        services.AddSingleton<ProviderResourceHandlerDelegate>(services => key =>
        {
            return key switch
            {
                nameof(AzureResourceHandler) => services.GetRequiredService<AzureResourceHandler>(),
                _ => throw new ArgumentException($"Invalid resource handler '{key}'"),
            };
        });
    }
}
