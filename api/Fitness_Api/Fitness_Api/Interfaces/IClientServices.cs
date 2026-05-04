using Microsoft.AspNetCore.Mvc;

namespace Fitness_Api.Interfaces;

public interface IClientServices
{
    Task<IActionResult> GetAllClients();
    Task<IActionResult> GetMyClient(string token);
}
