using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FleetPing.Api.Data;
using FleetPing.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FleetPing.Api.Services
{
    /// <summary>
    /// Fakes tracker traffic for active vehicles so the map has something to show
    /// without real devices. Each tick moves every active vehicle a little and stores a ping.
    /// </summary>
    public class GpsSimulator : BackgroundService
    {
        private const double EarthRadiusKm = 6371.0;

        private readonly IServiceScopeFactory _scopes;
        private readonly SimulatorOptions _options;
        private readonly ILogger<GpsSimulator> _logger;
        private readonly Random _rng = new Random();

        public GpsSimulator(IServiceScopeFactory scopes, IOptions<SimulatorOptions> options, ILogger<GpsSimulator> logger)
        {
            _scopes = scopes;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                return;
            }

            _logger.LogInformation("GPS simulator running every {Interval}s", _options.IntervalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await TickAsync(stoppingToken);
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    _logger.LogWarning(ex, "Simulator tick failed");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
            }
        }

        private async Task TickAsync(CancellationToken ct)
        {
            using var scope = _scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FleetContext>();

            var vehicleIds = await db.Vehicles
                .Where(v => v.Status == VehicleStatus.Active)
                .Select(v => v.Id)
                .ToListAsync(ct);

            foreach (var id in vehicleIds)
            {
                var last = await db.GpsPings
                    .Where(p => p.VehicleId == id)
                    .OrderByDescending(p => p.RecordedAt)
                    .FirstOrDefaultAsync(ct);

                db.GpsPings.Add(Next(id, last));
            }

            await db.SaveChangesAsync(ct);
        }

        private GpsPing Next(int vehicleId, GpsPing last)
        {
            var lat = last?.Latitude ?? MockPositionService.DepotLat;
            var lng = last?.Longitude ?? MockPositionService.DepotLng;
            var heading = last?.HeadingDeg ?? _rng.Next(0, 360);

            // wander a bit, head home if we strayed too far
            heading = (heading + _rng.Next(-35, 36) + 360) % 360;
            if (DistanceKm(lat, lng, MockPositionService.DepotLat, MockPositionService.DepotLng) > _options.MaxRadiusKm)
            {
                heading = Bearing(lat, lng, MockPositionService.DepotLat, MockPositionService.DepotLng);
            }

            // the odd stop at a delivery
            var speed = _rng.NextDouble() < 0.15 ? 0 : 20 + _rng.NextDouble() * 40;
            var distanceKm = speed * _options.IntervalSeconds / 3600.0;
            var (newLat, newLng) = Move(lat, lng, heading, distanceKm);

            return new GpsPing
            {
                VehicleId = vehicleId,
                Latitude = Math.Round(newLat, 6),
                Longitude = Math.Round(newLng, 6),
                SpeedKph = Math.Round(speed, 1),
                HeadingDeg = Math.Round(heading),
                RecordedAt = DateTime.Now,
                ReceivedAt = DateTime.Now
            };
        }

        private static (double Lat, double Lng) Move(double lat, double lng, double headingDeg, double distanceKm)
        {
            var d = distanceKm / EarthRadiusKm;
            var brng = ToRad(headingDeg);
            var lat1 = ToRad(lat);
            var lng1 = ToRad(lng);

            var lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(d) + Math.Cos(lat1) * Math.Sin(d) * Math.Cos(brng));
            var lng2 = lng1 + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat1), Math.Cos(d) - Math.Sin(lat1) * Math.Sin(lat2));

            return (ToDeg(lat2), ToDeg(lng2));
        }

        private static double DistanceKm(double lat1, double lng1, double lat2, double lng2)
        {
            var dLat = ToRad(lat2 - lat1);
            var dLng = ToRad(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return EarthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double Bearing(double lat1, double lng1, double lat2, double lng2)
        {
            var y = Math.Sin(ToRad(lng2 - lng1)) * Math.Cos(ToRad(lat2));
            var x = Math.Cos(ToRad(lat1)) * Math.Sin(ToRad(lat2)) -
                    Math.Sin(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Cos(ToRad(lng2 - lng1));
            return (ToDeg(Math.Atan2(y, x)) + 360) % 360;
        }

        private static double ToRad(double deg) => deg * Math.PI / 180.0;

        private static double ToDeg(double rad) => rad * 180.0 / Math.PI;
    }
}
