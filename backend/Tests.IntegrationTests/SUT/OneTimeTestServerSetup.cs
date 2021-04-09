using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.ResourceManager.Compute;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Repository;
using Serilog;
using Tests.IntegrationTests;
using Unleash;

[SetUpFixture]
public class OneTimeTestServerSetup
{
    private static TestServer _testServer;
    private static Mock<IUnleash> _unleashMock;
    internal static HttpClient Client;
    internal static DatabaseContext Database;

    [OneTimeSetUp]
    public async Task Before()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        _unleashMock = new Mock<IUnleash>();
        _unleashMock.Setup(m => m.IsEnabled(It.IsAny<string>())).Returns(false);
        _unleashMock.Setup(m => m.IsEnabled(It.IsAny<string>(), true)).Returns(true);
        _unleashMock.Setup(m => m.IsEnabled(It.IsAny<string>(), false)).Returns(false);

        _testServer = new TestServer(TestServerBuilder);
        await _testServer.Host.StartAsync();
        Client = _testServer.CreateClient();
        Database = _testServer.Host.Services.GetRequiredService<DatabaseContext>();
    }

    [OneTimeTearDown]
    public async Task After()
    {
        await _testServer?.Host?.StopAsync();
        _testServer?.Dispose();
        Client?.Dispose();
    }

    private static IWebHostBuilder TestServerBuilder => new WebHostBuilder()
        .UseTestServer()
        .UseConfiguration(new ConfigurationBuilder()
            .AddInMemoryCollection(ConfigurationValues)
            .Build()
        )
        .UseSerilog(new LoggerConfiguration()
            .WriteTo.NUnitOutput()
            .CreateLogger()
        )
        .UseStartup<Host.Startup>()
        .ConfigureTestServices(services =>
        {
            services.AddTransient(_ => new Mock<ComputeManagementClient>().Object);
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

    private static Dictionary<string, string> ConfigurationValues => new Dictionary<string, string> 
    {
        
    };
}
