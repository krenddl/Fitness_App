namespace Fitness_Api.Requests;

public class EnterVisitRequest
{
    public int ClientId { get; set; }
    public string AccessType { get; set; } = "QR";
}
