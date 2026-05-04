namespace Fitness_Api.Models;

public class PersonalPlan
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int TrainerId { get; set; }
    public string TrainingPlan { get; set; } = string.Empty;
    public string NutritionPlan { get; set; } = string.Empty;
    public int ProgressPercent { get; set; }
}
