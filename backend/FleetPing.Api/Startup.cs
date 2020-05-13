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
using Microsoft.OpenApi.Models;

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

            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddSingleton<MockPositionService>();
            services.AddScoped<IPositionService, TrackedPositionService>();

            services.Configure<SimulatorOptions>(Configuration.GetSection(SimulatorOptions.Section));
            services.AddHostedService<GpsSimulator>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FleetPing API", Version = "v1" });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    Description = "Dispatch / workshop login"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "basic" }
                        },
                        new string[0]
                    }
                });
            });

            services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (Configuration.GetValue("EnableSwagger", env.IsDevelopment()))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetPing API v1"));
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
