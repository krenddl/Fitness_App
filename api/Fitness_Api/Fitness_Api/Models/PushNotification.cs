namespace Fitness_Api.Models;

public class PushNotification
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
