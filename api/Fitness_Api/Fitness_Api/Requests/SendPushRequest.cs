namespace Fitness_Api.Requests;

public class SendPushRequest
{
    public int ClientId { get; set; }
    public string Text { get; set; } = string.Empty;
}
