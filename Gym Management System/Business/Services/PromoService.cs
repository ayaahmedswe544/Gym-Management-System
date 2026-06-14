using Gym_Management_System.Business.DTOs.PromoDTOs;
using Gym_Management_System.Business.GeneralResponse;
using Gym_Management_System.Business.IService;
using Gym_Management_System.Data;
using Gym_Management_System.Data.Models;
using Gym_Management_System.Data_Access.Data.Models;
using Gym_Management_System.Data_Access.IRepository;
using Microsoft.EntityFrameworkCore;
namespace Gym_Management_System.Business.Services
{
    public class PromoService:IPromoService
    {

        private readonly IRepository<PromoCode> _promoRepository;
        private readonly ApplicationDbContext _db;

        public PromoService(IRepository<PromoCode> promoRepository, ApplicationDbContext db)
        {
            _promoRepository = promoRepository;
            _db = db;
        }

        public async Task<GeneralResponse<PromoCodeDto>> CreatePromoAsync(CreatePromoCodeDto promoCodeDto)
        {
            var promoCode = new PromoCode
            {
                Code = promoCodeDto.Code,
                DiscountPercentage = promoCodeDto.DiscountPercentage,
                ValidFrom = promoCodeDto.ValidFrom,
                ValidUntil = promoCodeDto.ValidUntil,
                MaxUses = promoCodeDto.MaxUses
            };
            await _promoRepository.AddAsync(promoCode);
            await _promoRepository.SaveChangesAsync();

            return GeneralResponse<PromoCodeDto>.Ok(MapToDto(promoCode), "Promo code created.");
        }

        public async Task<GeneralResponse<PromoCodeDto>> UpdatePromoAsync(Guid id, UpdatePromoCodeDto updatedPromoDto)
        {
            var promo = await _promoRepository.GetByIdAsync(id);
            if (promo == null)
                return GeneralResponse<PromoCodeDto>.Failure("Promo code not found.");

            promo.Code = updatedPromoDto.Code;
            promo.DiscountPercentage = updatedPromoDto.DiscountPercentage;
            promo.ValidFrom = updatedPromoDto.ValidFrom;
            promo.ValidUntil = updatedPromoDto.ValidUntil;
            promo.MaxUses = updatedPromoDto.MaxUses;

            _promoRepository.Update(promo);
            await _promoRepository.SaveChangesAsync();

            return GeneralResponse<PromoCodeDto>.Ok(MapToDto(promo), "Promo code updated.");
        }
        public async Task<GeneralResponse<PromoCodeDto>> ValidatePromoAsync(string code)
        {
            var (promo, error) = await GetValidPromoAsync(code);
            if (promo == null)
                return GeneralResponse<PromoCodeDto>.Failure(error!);

            return GeneralResponse<PromoCodeDto>.Ok(MapToDto(promo), "Promo code is valid.");
        }
        public async Task<GeneralResponse<PromoCodeDto>> ApplyPromoAsync(string code, Guid userId)
        {
            var (promo, error) = await GetValidPromoAsync(code);
            if (promo == null)
                return GeneralResponse<PromoCodeDto>.Failure(error!);
            var alreadyUsed = await _db.PromoCodeUsages
                .AnyAsync(u => u.PromoCodeId == promo.Id && u.UserId == userId);

            if (alreadyUsed)
                return GeneralResponse<PromoCodeDto>.Failure("You have already used this promo code.");
            var usage = new PromoCodeUsage
            {
                PromoCodeId = promo.Id,
                UserId = userId,
                UsedAt = DateTime.UtcNow
            };
            await _db.PromoCodeUsages.AddAsync(usage);
            return GeneralResponse<PromoCodeDto>.Ok(MapToDto(promo), "Promo code applied.");
        }

        private async Task<(PromoCode? promo, string? error)> GetValidPromoAsync(string code)
        {
            var promos = await _promoRepository.FindAsync(p => p.Code == code);
            var promo = promos.FirstOrDefault();

            if (promo == null)
                return (null, "Invalid promo code.");

            var now = DateTime.UtcNow;
            if (now < promo.ValidFrom || now > promo.ValidUntil)
                return (null, "Promo code is expired or not yet valid.");
            var usageCount = await _db.PromoCodeUsages
                .CountAsync(u => u.PromoCodeId == promo.Id);

            if (usageCount >= promo.MaxUses)
                return (null, "Promo code has reached its maximum number of uses.");

            return (promo, null);
        }
        private static PromoCodeDto MapToDto(PromoCode p) => new()
        {
            Id = p.Id,
            Code = p.Code,
            DiscountPercentage = p.DiscountPercentage,
            ValidFrom = p.ValidFrom,
            ValidUntil = p.ValidUntil,
            MaxUses = p.MaxUses
        };
    }

}
