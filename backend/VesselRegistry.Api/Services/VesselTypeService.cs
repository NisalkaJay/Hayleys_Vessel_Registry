using Microsoft.EntityFrameworkCore;
using VesselRegistry.Api.Data;
using VesselRegistry.Api.Dtos;

namespace VesselRegistry.Api.Services
{
    public class VesselTypeService : IVesselTypeService
    {
        private readonly AppDbContext _context;

        public VesselTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<VesselTypeDto>>> GetAllAsync()
        {
            var types = await _context.VesselTypes
                .AsNoTracking()
                .Select(vt => new VesselTypeDto
                {
                    VesselTypeId = vt.VesselTypeId,
                    Name = vt.Name
                })
                .ToListAsync(); // Use async/await for database calls[cite: 2]

            return ApiResponse<IEnumerable<VesselTypeDto>>.Ok(types);
        }
    }
}