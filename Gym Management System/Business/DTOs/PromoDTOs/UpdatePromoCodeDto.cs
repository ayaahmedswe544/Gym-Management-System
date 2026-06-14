namespace Gym_Management_System.Business.DTOs.PromoDTOs
{
    public class UpdatePromoCodeDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public int MaxUses { get; set; }
    }
}
