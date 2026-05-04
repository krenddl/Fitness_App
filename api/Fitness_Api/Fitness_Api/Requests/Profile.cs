using System.ComponentModel.DataAnnotations;

namespace Fitness_Api.Requests;

public class Profile
{
    [Required(ErrorMessage = "Имя обязательно")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    public string? Password { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
}
