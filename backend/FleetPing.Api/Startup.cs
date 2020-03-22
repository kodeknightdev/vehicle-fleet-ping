using System.Text.Json.Serialization;
using FleetPing.Api.Auth;
using FleetPing.Api.Data;
using FleetPing.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FleetPing.Api
{
    public class Startup
    {
        private const string FrontendCors = "frontend";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FleetContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Fleet")));

            services.AddCors(options =>
            {
                options.AddPolicy(FrontendCors, policy =>
                {
                    var origins = Configuration.GetSection("Cors:Origins").Get<string[]>()
                        ?? new[] { "http://localhost:4200" };
                    policy.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.Configure<BasicAuthOptions>(Configuration.GetSection(BasicAuthOptions.Section));
            services.AddAuthentication(BasicAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, null);
            services.AddAuthorization(options =>
            {
                // everything requires a login unless marked [AllowAnonymous]
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddSingleton<IPositionService, MockPositionService>();

            services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(FrontendCors);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
