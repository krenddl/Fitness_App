namespace Fitness_Api.Models;

public class WorkoutSession
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TrainerId { get; set; }
    public DateTime StartAt { get; set; }
    public int Capacity { get; set; }
    public List<int> ClientIds { get; set; } = new();
}
