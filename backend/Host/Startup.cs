using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
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
            AddOpenIdConnectAuthentication(services);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy => policy.RequireClaim(""));
            });

            var connectionString = Configuration.GetConnectionString("Database");
            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

                services.AddHangfire(configuration => configuration
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

            services.AddTracing();
            services.AddSwaggerGen();
            services.AddControllers();
            services.AddHangfireServer();

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
        }

        private void AddOpenIdConnectAuthentication(IServiceCollection services)
        {
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
            })
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = authorityUri;
                options.ClientId = Configuration.GetValue<string>("oidc:clientId");
                options.ClientSecret = Configuration.GetValue<string>("oidc:clientSecret");
                options.CallbackPath = "/api/account/signin-oidc";
                options.SignedOutRedirectUri = "/";
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters.ValidateIssuer = true;
                options.RequireHttpsMetadata = !Environment.IsDevelopment();
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
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
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

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always
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
