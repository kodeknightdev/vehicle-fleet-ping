using System.Text.Json.Serialization;
using FleetPing.Api.Data;
using FleetPing.Api.Services;
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
