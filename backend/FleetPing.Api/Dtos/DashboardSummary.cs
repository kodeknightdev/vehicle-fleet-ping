using System;
using System.Collections.Generic;

namespace FleetPing.Api.Dtos
{
    public class DashboardSummary
    {
        public int TotalVehicles { get; set; }

        public int Active { get; set; }

        public int InShop { get; set; }

        public int Retired { get; set; }

        /// <summary>
        /// Vehicles with a GPS ping in the last <see cref="ReportingWindowMinutes"/> minutes.
        /// </summary>
        public int Reporting { get; set; }

        public int ReportingWindowMinutes { get; set; }

        public DateTime? LastPingAt { get; set; }

        public decimal MaintenanceCost30d { get; set; }

        public int MaintenanceCount30d { get; set; }

        public IList<VehicleDto> ServiceDue { get; set; } = new List<VehicleDto>();
    }
}
