using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Haland.DotNetTrace;
using Hangfire;
using Hangfire.SqlServer;
using Serilog;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository;
using Unleash;
using Host.Authorizations;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Azure.ResourceManager.Compute;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using System.Linq;
using Service.Resources;

namespace Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddDatabase(services);
            AddFeatureFlags(services);
            AddOpenApiDocumentation(services);
            
            AddAuthorizationAndPolicies(services);
            AddOpenIdConnectAuthentication(services);

            services.AddTransient<UpsertVirtualMachineHandler>();
            services.AddTransient<UpsertVirtualMachineScaleSetHandler>();

            services.AddTracing();
            services.AddControllers();
            services.AddHangfireServer();

            var subscriptionId = Configuration.GetValue<string>("azure:subscriptionId");
            if (!string.IsNullOrEmpty(subscriptionId))
            {
                services.AddTransient(_ => new ComputeManagementClient(subscriptionId, new ClientSecretCredential(
                    tenantId: Configuration.GetValue<string>("azure:tenantId"),
                    clientId: Configuration.GetValue<string>("azure:clientId"),
                    clientSecret: Configuration.GetValue<string>("azure:clientSecret")
                )));
            }
        }

        private void AddDatabase(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Database");
            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

                services.AddHangfire(configuration => configuration
                    .UseSerilogLogProvider()
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));
            }
        }

        private void AddFeatureFlags(IServiceCollection services)
        {
            var unleashApiUrl = Configuration.GetValue<string>("unleash:apiUrl");
            if (!string.IsNullOrEmpty(unleashApiUrl))
            {
                services.AddSingleton<IUnleash>(new DefaultUnleash(new UnleashSettings
                {
                    AppName = "Azure Resource Scheduler",
                    Environment = Configuration.GetValue<string>("unleash:environment") ?? "Development",
                    InstanceTag = Configuration.GetValue<string>("unleash:instanceTag"),
                    UnleashApi = new Uri(unleashApiUrl)
                }));
            }
            else
            {
                services.AddSingleton<IUnleash>(new DefaultUnleash(new UnleashSettings()));
            }
        }

        private void AddAuthorizationAndPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimTypes.Role, "user")
                    .Build();

                options.AddPolicy("admin", policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimTypes.Role, "admin"));

                options.AddPolicy("account", policy => policy.RequireAuthenticatedUser());
            });
        }

        private void AddOpenIdConnectAuthentication(IServiceCollection services)
        {
            var clientId = Configuration.GetValue<string>("oidc:clientId");
            var authorityUri = Configuration.GetValue<string>("oidc:authorityUri");
            if (string.IsNullOrEmpty(authorityUri)) return;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = ".auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = false;
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    // Handle redirects for api controller requests by returning Forbidden (403) instead
                    // https://github.com/dotnet/aspnetcore/issues/9039
                    if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == 200)
                    {
                        context.Response.StatusCode = 403;
                    }

                    return Task.CompletedTask;
                };
            })
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = authorityUri;
                options.ClientId = clientId;
                options.ClientSecret = Configuration.GetValue<string>("oidc:clientSecret");
                options.CallbackPath = "/api/account/signin-oidc";
                options.SignedOutRedirectUri = "/";
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters.ValidateIssuer = true;
                options.RequireHttpsMetadata = true;
                options.ClaimActions.MapJsonKey(
                    claimType: ClaimTypes.Role, 
                    jsonKey: $"{clientId}.roles",
                    valueType: ClaimValueTypes.String);
                
                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    // Handle redirects for api controller requests by returning Unauthorized (401) instead
                    // https://github.com/dotnet/aspnetcore/issues/9039

                    if (!context.Request.Path.StartsWithSegments("/api/account/login") && 
                        context.Request.Path.StartsWithSegments("/api") && 
                        context.Response.StatusCode == 200)
                    {
                        context.Response.StatusCode = 401;
                        context.HandleResponse();
                    }

                    return Task.CompletedTask;
                };
                options.Events.OnUserInformationReceived = context =>
                {
                    // For debugging user info mappings
                    return Task.CompletedTask;
                };
            });
        }

        private void AddOpenApiDocumentation(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Azure Resource Scheduler", Version = "v1" });
                options.TagActionsBy(api =>
                {
                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor == null) throw new InvalidOperationException("Unable to determine tag for endpoint.");

                    var route = controllerActionDescriptor.AttributeRouteInfo?.Template;
                    if (route != null)
                    {
                        var parts = route.Split('/');
                        if (parts.Length > 1) return new[] { String.Join('/', parts.Take(parts.Length - 1)) };
                    }

                    return new[] { controllerActionDescriptor.ControllerName };
                });
                options.DocInclusionPredicate((name, api) => true);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseTracing();
            app.UseSerilogRequestLogging();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseReferrerPolicy(options => options.NoReferrer());

            app.UseRedirectValidation(options =>
            {
                options.AllowSameHostRedirectsToHttps();

                var authorityUri = Configuration.GetValue<string>("oidc:authorityUri");
                if (string.IsNullOrEmpty(authorityUri)) return;
                options.AllowedDestinations(authorityUri);
            });

            app.UseXContentTypeOptions();
            app.UseXRobotsTag(options => options.NoIndex().NoFollow());

            if (!env.IsDevelopment())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseExceptionHandler(app =>
            {
                app.Run(async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
                    var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();

                    await Task.CompletedTask;
                    logger.LogError(exceptionHandler.Error, "Application error:");
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = "docs";
            });

            app.UseCsp(options =>
            {
                options.BlockAllMixedContent();
                options.StyleSources(s => s.Self());
                options.FontSources(s => s.Self());
                options.FormActions(s => s.Self());
                options.FrameAncestors(s => s.Self());
                options.ImageSources(s => s.Self());
                options.ScriptSources(s => s.Self());
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard("/jobs", new DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } });
            });
        }
    }
}
