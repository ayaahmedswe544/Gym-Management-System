namespace Gym_Management_System.Business.DTOs.PromoDTOs
{
    public class PromoCodeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public int MaxUses { get; set; }
    }
}
