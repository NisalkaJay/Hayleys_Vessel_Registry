using Microsoft.AspNetCore.Mvc;
using VesselRegistry.Api.Dtos;
using VesselRegistry.Api.Services;

namespace VesselRegistry.Api.Controllers
{
    [ApiController]
    [Route("api/vessels")]
    public class VesselsController : ControllerBase
    {
        private readonly IVesselService _vesselService;

        public VesselsController(IVesselService vesselService)
        {
            _vesselService = vesselService;
        }

        private int CompanyId => (int)HttpContext.Items["CompanyId"]!;
        private int UserId => (int)HttpContext.Items["UserId"]!;

        [HttpGet]
        public async Task<IActionResult> GetVessels(
            [FromQuery] string? search, [FromQuery] int? vesselTypeId, [FromQuery] bool? isActive, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ApiResponse<object>.Error(
                    "Validation",
                    "Page must be at least 1 and pageSize must be between 1 and 100."));
            }

            var result = await _vesselService.GetPagedVesselsAsync(CompanyId, search, vesselTypeId, isActive, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVessel(int id)
        {
            var result = await _vesselService.GetByIdAsync(id, CompanyId);
            
            if (result.ErrorCode == "NotFound")
                return NotFound(result); // 404 if not found or wrong company[cite: 4]

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVessel([FromBody] VesselDto dto)
        {
            // Handle Data Annotations validation errors manually to match required envelope
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1), // camelCase keys
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return BadRequest(ApiResponse<object>.Error("Validation", "Validation failed.", errors));
            }

            var result = await _vesselService.CreateAsync(dto, CompanyId, UserId);

            if (result.ErrorCode == "Duplicate")
                return Conflict(result); // 409 for duplicate IMO[cite: 3]

            if (result.ErrorCode == "InvalidVesselType")
                return BadRequest(result);

            return CreatedAtAction(nameof(GetVessel), new { id = result.Data!.VesselId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVessel(int id, [FromBody] VesselDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1),
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return BadRequest(ApiResponse<object>.Error("Validation", "Validation failed.", errors));
            }

            var result = await _vesselService.UpdateAsync(id, dto, CompanyId, UserId);

            if (result.ErrorCode == "NotFound")
                return NotFound(result); // 404[cite: 4]

            if (result.ErrorCode == "Duplicate")
                return Conflict(result); // 409[cite: 3]

            if (result.ErrorCode == "InvalidVesselType")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateVessel(int id)
        {
            var result = await _vesselService.DeactivateAsync(id, CompanyId, UserId);

            if (result.ErrorCode == "NotFound")
                return NotFound(result); // 404[cite: 4]

            return Ok(result);
        }
    }
}