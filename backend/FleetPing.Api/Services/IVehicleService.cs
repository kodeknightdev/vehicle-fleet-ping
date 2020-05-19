using System.Collections.Generic;
using System.Threading.Tasks;
using FleetPing.Api.Dtos;

namespace FleetPing.Api.Services
{
    public interface IVehicleService
    {
        Task<IList<VehicleDto>> ListAsync();

        Task<VehicleDto> GetAsync(int id);

        Task<VehicleDto> CreateAsync(VehicleInput input);

        Task<bool> UpdateAsync(int id, VehicleInput input);

        Task<bool> DeleteAsync(int id);

        Task<bool> PlateInUseAsync(string plate, int? exceptId = null);
    }
}
