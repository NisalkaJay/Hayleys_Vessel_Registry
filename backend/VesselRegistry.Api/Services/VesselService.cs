using Microsoft.EntityFrameworkCore;
using VesselRegistry.Api.Data;
using VesselRegistry.Api.Dtos;
using VesselRegistry.Api.Entities;

namespace VesselRegistry.Api.Services
{
    public class VesselService : IVesselService
    {
        private readonly AppDbContext _context;

        public VesselService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PagedResultDto<VesselDto>>> GetPagedVesselsAsync(
            int companyId, string? search, int? vesselTypeId, bool? isActive, int page, int pageSize)
        {
            var query = _context.Vessels.Include(v => v.VesselType).AsQueryable();

            // Filter by the caller's company ID[cite: 3]
            query = query.Where(v => v.CompanyId == companyId);

            if (isActive.HasValue)
            {
                query = query.Where(v => v.IsActive == isActive.Value);
            }

            if (vesselTypeId.HasValue)
            {
                query = query.Where(v => v.VesselTypeId == vesselTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Search matches vessel name or IMO number[cite: 3]
                query = query.Where(v => v.VesselName.Contains(search) || v.ImoNumber.Contains(search));
            }

            // Do filtering and paging in the database query[cite: 3]
            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(v => v.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new VesselDto
                {
                    VesselId = v.VesselId,
                    VesselName = v.VesselName,
                    ImoNumber = v.ImoNumber,
                    VesselTypeId = v.VesselTypeId,
                    VesselTypeName = v.VesselType.Name,
                    FlagCountry = v.FlagCountry,
                    GrossTonnage = v.GrossTonnage,
                    YearBuilt = v.YearBuilt,
                    IsActive = v.IsActive
                })
                .ToListAsync();

            var result = new PagedResultDto<VesselDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PagedResultDto<VesselDto>>.Ok(result);
        }

        public async Task<ApiResponse<VesselDto>> GetByIdAsync(int id, int companyId)
        {
            var vessel = await _context.Vessels
                .Include(v => v.VesselType)
                .FirstOrDefaultAsync(v => v.VesselId == id);

            // Check ownership. If it belongs to another company or doesn't exist, return 404[cite: 2, 4]
            if (vessel == null || vessel.CompanyId != companyId)
            {
                return ApiResponse<VesselDto>.Error("NotFound", "Vessel not found.");
            }

            var dto = new VesselDto
            {
                VesselId = vessel.VesselId,
                VesselName = vessel.VesselName,
                ImoNumber = vessel.ImoNumber,
                VesselTypeId = vessel.VesselTypeId,
                VesselTypeName = vessel.VesselType.Name,
                FlagCountry = vessel.FlagCountry,
                GrossTonnage = vessel.GrossTonnage,
                YearBuilt = vessel.YearBuilt,
                IsActive = vessel.IsActive
            };

            return ApiResponse<VesselDto>.Ok(dto);
        }

        public async Task<ApiResponse<VesselDto>> CreateAsync(VesselDto dto, int companyId, int userId)
        {
            // If IMO number exists for that company, return Duplicate error[cite: 3]
            bool imoExists = await _context.Vessels.AnyAsync(v => v.CompanyId == companyId && v.ImoNumber == dto.ImoNumber);
            if (imoExists)
            {
                return ApiResponse<VesselDto>.Error("Duplicate", "IMO number already exists for this company.");
            }

            var vessel = new Vessel
            {
                CompanyId = companyId,
                VesselName = dto.VesselName,
                ImoNumber = dto.ImoNumber,
                VesselTypeId = dto.VesselTypeId,
                FlagCountry = dto.FlagCountry,
                GrossTonnage = dto.GrossTonnage,
                YearBuilt = dto.YearBuilt,
                IsActive = dto.IsActive,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Vessels.Add(vessel);
            await _context.SaveChangesAsync();

            dto.VesselId = vessel.VesselId;
            return ApiResponse<VesselDto>.Ok(dto);
        }

        public async Task<ApiResponse<VesselDto>> UpdateAsync(int id, VesselDto dto, int companyId, int userId)
        {
            var vessel = await _context.Vessels.FirstOrDefaultAsync(v => v.VesselId == id);

            // 404 check[cite: 4]
            if (vessel == null || vessel.CompanyId != companyId)
            {
                return ApiResponse<VesselDto>.Error("NotFound", "Vessel not found.");
            }

            // Check IMO collision with a different vessel
            bool imoExists = await _context.Vessels.AnyAsync(v => v.CompanyId == companyId && v.ImoNumber == dto.ImoNumber && v.VesselId != id);
            if (imoExists)
            {
                return ApiResponse<VesselDto>.Error("Duplicate", "IMO number already exists for this company.");
            }

            vessel.VesselName = dto.VesselName;
            vessel.ImoNumber = dto.ImoNumber;
            vessel.VesselTypeId = dto.VesselTypeId;
            vessel.FlagCountry = dto.FlagCountry;
            vessel.GrossTonnage = dto.GrossTonnage;
            vessel.YearBuilt = dto.YearBuilt;
            vessel.IsActive = dto.IsActive;
            vessel.ModifiedBy = userId;
            vessel.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse<VesselDto>.Ok(dto);
        }

        public async Task<ApiResponse<bool>> DeactivateAsync(int id, int companyId, int userId)
        {
            var vessel = await _context.Vessels.FirstOrDefaultAsync(v => v.VesselId == id);

            // 404 check[cite: 4]
            if (vessel == null || vessel.CompanyId != companyId)
            {
                return ApiResponse<bool>.Error("NotFound", "Vessel not found.");
            }

            vessel.IsActive = false; // Deactivate sets IsActive to 0[cite: 3]
            vessel.ModifiedBy = userId;
            vessel.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true);
        }
    }
}