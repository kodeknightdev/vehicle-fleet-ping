using System.Collections.Generic;
using System.Threading.Tasks;
using FleetPing.Api.Models;

namespace FleetPing.Api.Services
{
    public interface IPositionService
    {
        Task<VehiclePosition> GetLastKnownAsync(Vehicle vehicle);

        Task<IList<VehiclePosition>> GetLastKnownAsync(IList<Vehicle> vehicles);
    }
}
