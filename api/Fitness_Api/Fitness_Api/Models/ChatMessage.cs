namespace Fitness_Api.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int TrainerId { get; set; }
    public int ClientId { get; set; }
    public string SenderRole { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}
