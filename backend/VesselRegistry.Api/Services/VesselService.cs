using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VesselRegistry.Api.Data;
using VesselRegistry.Api.Dtos;
using VesselRegistry.Api.Entities;

namespace VesselRegistry.Api.Services
{
    public class VesselService : IVesselService
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;

        public VesselService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        }

        public async Task<ApiResponse<PagedResultDto<VesselDto>>> GetPagedVesselsAsync(
            int companyId, string? search, int? vesselTypeId, bool? isActive, int page, int pageSize,
            string? sortBy, string? sortDirection)
        {
            var orderColumn = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["vesselName"] = "v.VesselName",
                ["imoNumber"] = "v.ImoNumber",
                ["vesselTypeName"] = "vt.Name",
                ["flagCountry"] = "v.FlagCountry",
                ["grossTonnage"] = "v.GrossTonnage",
                ["yearBuilt"] = "v.YearBuilt",
                ["isActive"] = "v.IsActive"
            };
            var selectedColumn = orderColumn.TryGetValue(sortBy ?? string.Empty, out var column)
                ? column
                : "v.CreatedAt";
            var direction = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";
            var where = new List<string> { "v.CompanyId = @CompanyId" };
            if (isActive.HasValue) where.Add("v.IsActive = @IsActive");
            if (vesselTypeId.HasValue) where.Add("v.VesselTypeId = @VesselTypeId");
            if (!string.IsNullOrWhiteSpace(search)) where.Add("(v.VesselName LIKE @Search OR v.ImoNumber LIKE @Search)");

            var sql = $"""
                SELECT COUNT(1)
                FROM Vessels v
                INNER JOIN VesselTypes vt ON vt.VesselTypeId = v.VesselTypeId
                WHERE {string.Join(" AND ", where)};

                SELECT v.VesselId, v.VesselName, v.ImoNumber, v.VesselTypeId,
                       vt.Name AS VesselTypeName, v.FlagCountry, v.GrossTonnage,
                       v.YearBuilt, v.IsActive
                FROM Vessels v
                INNER JOIN VesselTypes vt ON vt.VesselTypeId = v.VesselTypeId
                WHERE {string.Join(" AND ", where)}
                ORDER BY {selectedColumn} {direction}, v.VesselId
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                """;
            var parameters = new
            {
                CompanyId = companyId,
                IsActive = isActive,
                VesselTypeId = vesselTypeId,
                Search = string.IsNullOrWhiteSpace(search) ? null : $"%{search.Trim()}%",
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            };
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var grid = await connection.QueryMultipleAsync(sql, parameters);
            var totalCount = await grid.ReadSingleAsync<int>();
            var items = (await grid.ReadAsync<VesselDto>()).ToList();

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

            if (!await _context.VesselTypes.AnyAsync(v => v.VesselTypeId == dto.VesselTypeId))
            {
                return ApiResponse<VesselDto>.Error("InvalidVesselType", "Vessel type does not exist.");
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
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                return ApiResponse<VesselDto>.Error("Duplicate", "IMO number already exists for this company.");
            }

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

            if (!await _context.VesselTypes.AnyAsync(v => v.VesselTypeId == dto.VesselTypeId))
            {
                return ApiResponse<VesselDto>.Error("InvalidVesselType", "Vessel type does not exist.");
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                return ApiResponse<VesselDto>.Error("Duplicate", "IMO number already exists for this company.");
            }

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

        private static bool IsUniqueConstraintViolation(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sqlException &&
                (sqlException.Number == 2601 || sqlException.Number == 2627);
        }
    }
}