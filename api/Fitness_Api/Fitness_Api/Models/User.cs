namespace Fitness_Api.Models;

public class User
{
    public int id_User { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Role_Id { get; set; }
    public int? Client_Id { get; set; }
    public int? Trainer_Id { get; set; }
}
