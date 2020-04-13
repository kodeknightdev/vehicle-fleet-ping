using System.Collections.Generic;
using System.Threading.Tasks;
using FleetPing.Api.Dtos;
using FleetPing.Api.Models;

namespace FleetPing.Api.Services
{
    public interface IMaintenanceService
    {
        /// <returns>null when the vehicle does not exist</returns>
        Task<IList<MaintenanceLog>> ListAsync(int vehicleId);

        Task<MaintenanceLog> GetAsync(int vehicleId, int id);

        /// <returns>null when the vehicle does not exist</returns>
        Task<MaintenanceLog> CreateAsync(int vehicleId, MaintenanceLogInput input);

        Task<bool> UpdateAsync(int vehicleId, int id, MaintenanceLogInput input);

        Task<bool> DeleteAsync(int vehicleId, int id);
    }
}
