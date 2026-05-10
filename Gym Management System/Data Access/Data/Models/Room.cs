namespace Gym_Management_System.Data.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Features { get; set; } = string.Empty; 

        public ICollection<GymClass> Classes { get; set; } = new List<GymClass>();
    }
}
