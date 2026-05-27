namespace Gym_Management_System.Business.DTOs.ReviewDTOs
{
    public class CreateReviewDto
    {
        public Guid? GymClassId { get; set; }
        public Guid TrainerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
