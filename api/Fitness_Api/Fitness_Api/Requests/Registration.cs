using System.ComponentModel.DataAnnotations;

namespace Fitness_Api.Requests;

public class Registration
{
    [Required(ErrorMessage = "Имя обязательно")]
    [MinLength(2, ErrorMessage = "Имя должно содержать минимум 2 символа")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон обязателен")]
    public string Phone { get; set; } = string.Empty;

    public string? Description { get; set; }
}
