using System.ComponentModel.DataAnnotations;

namespace Fitness_Api.Requests;

public class CreateNewUser
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int Role_Id { get; set; }

    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Specialization { get; set; }
}
