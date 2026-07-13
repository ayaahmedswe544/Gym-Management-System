using Gym_Management_System.Business.DTOs.PromoDTOs;
using Gym_Management_System.Business.GeneralResponse;

namespace Gym_Management_System.Business.IService
{
    public interface IPromoService
    {
        Task<GeneralResponse<IEnumerable<PromoCodeDto>>> GetPromosAsync(string? code = null, bool? isActive = null);
        Task<GeneralResponse<PromoCodeDto>> CreatePromoAsync(CreatePromoCodeDto promoCodeDto);
        Task<GeneralResponse<PromoCodeDto>> UpdatePromoAsync(Guid id, UpdatePromoCodeDto updatedPromoDto);
        Task<GeneralResponse<PromoCodeDto>> ValidatePromoAsync(string code);
        Task<GeneralResponse<PromoCodeDto>> ApplyPromoAsync(string code, Guid userId);
        Task<GeneralResponse<bool>> DeletePromoAsync(Guid id);

    }
}
