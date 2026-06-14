using Azure;
using Gym_Management_System.Business.DTOs.PromoDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromosController : ControllerBase
    {
        private readonly IPromoService _promoService;

        public PromosController(IPromoService promoService)
        {
            _promoService = promoService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GeneralResponse<PromoCodeDto>>> CreatePromo([FromBody] CreatePromoCodeDto promoCodeDto)
        {
            var response = await _promoService.CreatePromoAsync(promoCodeDto);
            return StatusCode(response.Success ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest, response);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GeneralResponse<PromoCodeDto>>> UpdatePromo(Guid id, [FromBody] UpdatePromoCodeDto updatedPromoDto)
        {
            var response = await _promoService.UpdatePromoAsync(id, updatedPromoDto);     
            return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status304NotModified, response);
        }

        [HttpPost("validate")]
        [Authorize(Roles = "Member,Admin")]
        public async Task<ActionResult<GeneralResponse<object>>> ValidatePromo([FromBody] string code)
        {
            var response = await _promoService.ValidatePromoAsync(code);

           return StatusCode(response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, response);
            
        }
    }
}
