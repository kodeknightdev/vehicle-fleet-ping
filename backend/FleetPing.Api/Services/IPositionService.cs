using System.Collections.Generic;
using FleetPing.Api.Models;

namespace FleetPing.Api.Services
{
    public interface IPositionService
    {
        VehiclePosition GetLastKnown(Vehicle vehicle);

        IEnumerable<VehiclePosition> GetLastKnown(IEnumerable<Vehicle> vehicles);
    }
}
