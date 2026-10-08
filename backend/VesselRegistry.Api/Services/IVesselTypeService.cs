using VesselRegistry.Api.Dtos;

namespace VesselRegistry.Api.Services
{
    public interface IVesselTypeService
    {
        Task<ApiResponse<IEnumerable<VesselTypeDto>>> GetAllAsync();
    }
}