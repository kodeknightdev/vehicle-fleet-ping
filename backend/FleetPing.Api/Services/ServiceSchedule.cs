using FleetPing.Api.Models;

namespace FleetPing.Api.Services
{
    public static class ServiceSchedule
    {
        /// <summary>
        /// Log types that reset the service interval.
        /// </summary>
        public static readonly MaintenanceType[] ResetTypes =
        {
            MaintenanceType.OilChange,
            MaintenanceType.Inspection
        };

        /// <summary>
        /// Flag a vehicle as due when it is this close to the next service.
        /// </summary>
        public const int DueWarningKm = 1000;
    }
}
