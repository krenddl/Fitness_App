namespace Fitness_Api.Models;

public class Session
{
    public int id_Session { get; set; }
    public string Token { get; set; } = string.Empty;
    public int User_Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
