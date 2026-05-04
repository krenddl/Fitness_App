namespace Fitness_Api.Requests;

public class SendChatRequest
{
    public int TrainerId { get; set; }
    public int ClientId { get; set; }
    public string Text { get; set; } = string.Empty;
}
