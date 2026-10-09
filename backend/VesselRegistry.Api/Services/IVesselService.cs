using VesselRegistry.Api.Dtos;


namespace VesselRegistry.Api.Services
{
    public interface IVesselService
    {
        Task<ApiResponse<PagedResultDto<VesselDto>>> GetPagedVesselsAsync(int companyId, string? search, int? vesselTypeId, bool? isActive, int page, int pageSize, string? sortBy, string? sortDirection);
        Task<ApiResponse<VesselDto>> GetByIdAsync(int id, int companyId);
        Task<ApiResponse<VesselDto>> CreateAsync(VesselDto vesselDto, int companyId, int userId);
        Task<ApiResponse<VesselDto>> UpdateAsync(int id, VesselDto vesselDto, int companyId, int userId);
        Task<ApiResponse<bool>> DeactivateAsync(int id, int companyId, int userId);
    }
}