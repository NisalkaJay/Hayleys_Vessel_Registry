using Microsoft.AspNetCore.Mvc;
using VesselRegistry.Api.Services;

namespace VesselRegistry.Api.Controllers
{
    [ApiController]
    [Route("api/vessel-types")]
    public class VesselTypesController : ControllerBase
    {
        private readonly IVesselTypeService _vesselTypeService;

        public VesselTypesController(IVesselTypeService vesselTypeService)
        {
            _vesselTypeService = vesselTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVesselTypes()
        {
            var result = await _vesselTypeService.GetAllAsync();
            return Ok(result);
        }
    }
}