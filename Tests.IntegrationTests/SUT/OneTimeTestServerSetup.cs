#nullable disable
using Hangfire;
using Hangfire.MemoryStorage;
using Libraries.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog;
using Tests.IntegrationTests;
using Unleash;

[SetUpFixture]
#pragma warning disable CA1050 // Declare types in namespaces
public class OneTimeTestServerSetup
#pragma warning restore CA1050 // Declare types in namespaces
{
    private static TestServer _testServer;
    private static Mock<IUnleash> _unleashMock;
    internal static HttpClient Client = new();
    internal static DatabaseContext Database = new(new DbContextOptionsBuilder<DatabaseContext>().Options);

    [OneTimeSetUp]
    public async Task Before()
    {
        System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        _unleashMock = new();
        _unleashMock.Setup(m => m.IsEnabled(It.IsAny<string>())).Returns(true);
        _unleashMock.Setup(m => m.IsEnabled(It.IsAny<string>(), true)).Returns(true);
        _unleashMock.Setup(m => m.IsEnabled(It.IsAny<string>(), false)).Returns(true);

        _testServer = new(TestServerBuilder);
        await _testServer.Host.StartAsync();
        Client = _testServer.CreateClient();
        Database = _testServer.Host.Services.GetRequiredService<DatabaseContext>();
    }

    [OneTimeTearDown]
    public async Task After()
    {
        await _testServer.Host.StopAsync();
        _testServer.Dispose();
        Client.Dispose();
        Database.Dispose();
    }

    private static IWebHostBuilder TestServerBuilder => new WebHostBuilder()
        .UseTestServer()
        .UseConfiguration(new ConfigurationBuilder()
            .AddInMemoryCollection(ConfigurationValues)
            .Build()
        )
        .UseSerilog(new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger()
        )
        .UseStartup<Host.Startup>()
        .ConfigureTestServices(services =>
        {
            services.AddTransient(_ => new Mock<AzureResourceHandler>().Object);
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("Database");
            });
            services.AddAuthentication("BasicAuthentication")
                    .AddScheme<AuthenticationSchemeOptions, MockAuthenticatedUser>("BasicAuthentication", null);
            services.AddSingleton(_unleashMock.Object);
            services.AddHangfire(configuration => configuration
                .UseSerilogLogProvider()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage()
            );
        });

    private static Dictionary<string, string> ConfigurationValues => new()
    {
        {"oidc:roles:user", "user"},
        {"oidc:roles:admin", "admin"},
    };
}
