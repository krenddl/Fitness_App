namespace Fitness_Api.Models;

public class Visit
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DateTime EnteredAt { get; set; }
    public DateTime? ExitedAt { get; set; }
    public string AccessType { get; set; } = "QR";
}
